using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Runtime.Midi;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Timeline;
using ABXY.Layers.Runtime.Timeline.Playnode;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Playback
{
    public class PlaybackSystem
    {
        public enum states { Playing, Paused, Stopped }

        //public states currentState = states.Stopped;

        private List<PlayToken> playTokens = new List<PlayToken>();

        private System.Func<List<PlaynodeDataItem>> PlaynodeDataItems;
        private System.Func<IEnumerator,FlowNode.LayersCoroutine> StartCoroutine;
        private System.Func<float> GetCombinedVolume;
        private System.Func<float> GetCombinedPan;
        private System.Func<double> GetEndTime;
        private System.Func<string> GetName;
        private System.Action<string, double, Dictionary<string, object>,int> CallFunctionOnOutputNodes;
        private System.Func<AudioOutSource[]> GetMainBusOuts;
        private System.Func<PlaynodeDataItem, AudioOutSource[]> GetTrackOuts;
        private System.Func<List<TimeLineRowDataItem>> GetTracks;

        public double currentTime { get; private set; }
        public System.Action<double> OnTimeChange;

        private System.Guid lastPlaybackID;

        private FlowNode targetFlowNode = null;

        public bool runningSounds { get { return playTokens.Count != 0; } }

        public PlaybackSystem(Func<List<PlaynodeDataItem>> playnodeDataItems, System.Func<IEnumerator,FlowNode.LayersCoroutine> startCoroutine, 
            System.Func<float> getCombinedVolume, System.Func<float> getCombinedPan,
            Func<double> getEndTime, Func<string> getName, Action<string, double, Dictionary<string, object>,int> callFunctionOnOutputNodes, Func<AudioOutSource[]> getMainBusOuts, Func<PlaynodeDataItem, 
                AudioOutSource[]> getTrackOuts, Func<List<TimeLineRowDataItem>> getTracks, FlowNode targetFlowNode)
        {
            PlaynodeDataItems = playnodeDataItems;
            StartCoroutine = startCoroutine;
            GetEndTime = getEndTime;
            GetName = getName;
            CallFunctionOnOutputNodes = callFunctionOnOutputNodes;
            GetMainBusOuts = getMainBusOuts;
            GetTrackOuts = getTrackOuts;
            GetTracks = getTracks;
            GetCombinedPan = getCombinedPan;
            GetCombinedVolume = getCombinedVolume;
            this.targetFlowNode = targetFlowNode;
        }

        public void QueueMIDIFile(double time, double timeOffset, Dictionary<string, object> parameters, MidiFileAsset midiFile, TempoMap tempoMap, List<Note> notes,int nodesCalledThisFrame)
        {
            PlaynodeDataItem dataItem = new PlaynodeDataItem(0, midiFile, null); ;

            //currentState = states.Playing;
            long endTimeTicks = 0;
            foreach(Note note in notes)
            {
                if (note.Length + note.Time > endTimeTicks)
                    endTimeTicks = note.Length + note.Time;
            }

            double endTime = TimeConverter.ConvertTo<MetricTimeSpan>(endTimeTicks, tempoMap).TotalMicroseconds / 1000000.0;
            lastPlaybackID = System.Guid.NewGuid();

            StartCoroutine(PlayMidiCoroutine(dataItem, tempoMap, notes, time  - timeOffset, time + endTime, parameters, lastPlaybackID, nodesCalledThisFrame));
        
            StartCoroutine(DoTime(time - timeOffset, time + endTime - timeOffset, lastPlaybackID));
        }

        public void QueueAudio(double time, double timeOffset, Dictionary<string, object> parameters,int nodesCalledThisFrame)
        {
            lastPlaybackID = System.Guid.NewGuid();

            QueueLoops(time, timeOffset, parameters, lastPlaybackID, nodesCalledThisFrame);

            //currentState = states.Playing;
            foreach (PlaynodeDataItem change in PlaynodeDataItems())
            {
                if (change == null || change.GetBackingObject() == null)
                    continue;

                if (change.playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.Audio)
                {
                    AudioClip asset = change.GetBackingObject() as AudioClip;
                    StartCoroutine(DoAudioClipPlayback(change, asset,time, timeOffset, time + (GetEndTime()), lastPlaybackID, parameters, lastPlaybackID));
                }
                else if (change.playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.MIDI || change.playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.Event)
                {
                    MidiFileAsset midiAsset = change.GetBackingObject() as MidiFileAsset;
                    if (midiAsset != null)
                    {
                        StartCoroutine(PlayMidiCoroutine(change, midiAsset.GetTempoMap(), midiAsset.GetNotes(), time + (change.startTime) - timeOffset, time + (GetEndTime()), parameters, lastPlaybackID, nodesCalledThisFrame));

                    }
                    //StartCoroutine(DoMIDIEndTime(asset,time + (change.TimeSeconds)));
                }
            }
            StartCoroutine(DoEndTime((float)(time + (GetEndTime()) - timeOffset), parameters, lastPlaybackID, nodesCalledThisFrame));

            double endTime = 0;
            foreach (PlaynodeDataItem change in PlaynodeDataItems())
            {
                if (time + change.startTime + change.length > endTime)
                    endTime = time + change.startTime + change.length;
            }

            StartCoroutine(DoTime(time - timeOffset, endTime, lastPlaybackID));
        }

        private void QueueLoops(double time, double timeOffset,  Dictionary<string, object> parameters, System.Guid session,int nodesCalledThisFrame)
        {
            if (targetFlowNode == null) //Then this isn't running in a flow node. How did you get here? :-)
                return;


            foreach (PlaynodeDataItem change in PlaynodeDataItems().Where(x=>x.playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.Loop))
            {
                StartCoroutine(DoLoop(change, time + change.startTime - timeOffset, time + (GetEndTime()), session, parameters, nodesCalledThisFrame));
            }
        }

        private IEnumerator DoLoop(PlaynodeDataItem timeLineChange, double startTime, double nodeEndTime, System.Guid session, Dictionary<string, object> parameters, int nodesCalledThisFrame)
        {

            if (startTime + timeLineChange.length < AudioSettings.dspTime)
                yield break;

            double targetTrailTime = 0.4;
            // Doing wait for Loop End
            PlayToken playbackToken = new PlayToken(startTime + timeLineChange.length, nodeEndTime, targetTrailTime, this, session, "[DoLoop] Waiting for loop end");
            playTokens.Add(playbackToken);

            while (playbackToken.shouldYield)
            {
                if (playbackToken.currentState == PlayToken.states.StoppedEarly || playbackToken.reachedEndTime)
                {
                    //Debug.Log("[" + GetName() + "]" + "[PlayNode][Loop] playback ended before looping point could be reached");
                    playTokens.Remove(playbackToken);
                    yield break;
                }
                //Debug.Log("Waiting");
                yield return null;
            }

            double pauseDelay = playbackToken.CalculatePauseDelay();
            double nextStartTime = startTime + timeLineChange.length + pauseDelay;

            playTokens.Remove(playbackToken);

            if (targetFlowNode.GetInputValue<bool>(timeLineChange.itemID, false))
                yield break;

            StartCoroutine(SymphonyUtils.WaitForDSPTime(nextStartTime, () => {
                foreach (PlayToken token in playTokens)
                {
                    if (token.executionContext == session)
                    {
                        token.Stop();
                    }
                }
            }));

            QueueAudio(nextStartTime, timeLineChange.startTime, parameters, nodesCalledThisFrame);
        

        }

        private IEnumerator DoTime(double time, double endTime, System.Guid executionContext)
        {
        


            PlayToken playbackToken = new PlayToken(endTime, endTime+2, 0.2f, this, executionContext, "[DoTime] Waiting for time line end");
            playTokens.Add(playbackToken);

            while (playbackToken.shouldYield)
            {
                currentTime = (AudioSettings.dspTime - time);
                OnTimeChange?.Invoke(currentTime);
                if (playbackToken.reachedEndTime || playbackToken.currentState == PlayToken.states.StoppedEarly || executionContext != lastPlaybackID)
                {
                    playbackToken.Stop();
                    playTokens.Remove(playbackToken);
                    //if (playbackToken.reachedEndTime)
                    //currentTime = endTime-time;
                    OnTimeChange?.Invoke(currentTime);
                    yield break;
                }
                yield return null;
            }
            playTokens.Remove(playbackToken);
            currentTime = endTime - time;
            OnTimeChange?.Invoke(currentTime);
        }

        private IEnumerator DoEndTime(float finishTime, Dictionary<string, object> parameters, System.Guid session,int nodesCalledThisFrame)
        {
            // adding 10 to node end time because it doesn't matter in this case
            PlayToken playbackToken = new PlayToken(finishTime, finishTime + 10, 0.2f, this, session, "[DoEndTime] Waiting for end time");
            playTokens.Add(playbackToken);

            while (playbackToken.shouldYield)
            {
                if ( playbackToken.reachedEndTime || playbackToken.currentState == PlayToken.states.StoppedEarly)
                {
                    playbackToken.Stop();
                    playTokens.Remove(playbackToken);
                    //CallFunctionOnOutputNodes("playFinished", playbackToken.CalculatePauseDelay() + finishTime, parameters, audioSettings, midiData);
                    yield break;
                }
                yield return null;
            }
            playTokens.Remove(playbackToken);

            //preventing race conditions between the end of loops and calling end events
            if (targetFlowNode != null)
            {
                double endTimeInSource = GetEndTime();
                PlaynodeDataItem matchingLoopNode = PlaynodeDataItems()
                    .Where(x => x.playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.Loop && x.startTime + x.length == endTimeInSource && !targetFlowNode.GetInputValue<bool>(x.itemID, false)).FirstOrDefault();
                if (matchingLoopNode != null)
                    yield break;
            }

            CallFunctionOnOutputNodes("playFinished", playbackToken.CalculatePauseDelay() + finishTime, parameters, nodesCalledThisFrame);
        }


        #region Audio playback

        private IEnumerator DoAudioClipPlayback(PlaynodeDataItem timeLineChange, AudioClip clip, double nodeStartTime, double timeOffset, double nodeEndTime, System.Guid playbackID, Dictionary<string, object> parameters,System.Guid session)
        {
            if (clip == null)
                yield break;

            System.Guid clipGUID = System.Guid.NewGuid();

            double timelineSpaceStartTime = timeLineChange.startTime - timeOffset;
            double audioTimeShift = 0;

            if (timelineSpaceStartTime < 0)
                audioTimeShift = -timelineSpaceStartTime;

            if (audioTimeShift > timeLineChange.length)
                yield break;
        
            timelineSpaceStartTime = Mathf.Clamp((float)timelineSpaceStartTime, 0f, float.MaxValue);

            double startTime = nodeStartTime + timelineSpaceStartTime;


            bool looping = (timeLineChange.numberRepetitions != 1);

            //Debug.Log("[" + GetName() + "]" + "[PlayNode][PlayAtDSPTime] playback scheduled for " + startTime + ", current time is " + AudioSettings.dspTime + ". Clip is looping: " + looping + ". Clip length: " + timeLineChange.length + ". Delta: " + (startTime - AudioSettings.dspTime));
            
            
            

            double targetTrailTime = 0.4;
            float timeLength = (float)timeLineChange.length;

            // Doing wait for audioclip start
            PlayToken playbackToken = new PlayToken(startTime, nodeEndTime, targetTrailTime, this, session, "[DoAudioClipPlayback] Waiting for audio clip start time");
            playTokens.Add(playbackToken);

            while (playbackToken.shouldYield)
            {
                if (playbackToken.currentState == PlayToken.states.StoppedEarly || playbackToken.reachedEndTime)
                {
                    //Debug.Log("[" + GetName() + "]" + "[PlayNode][PlayAtDSPTime] playback ended before clip could start");
                    playTokens.Remove(playbackToken);
                    yield break;
                }
                //Debug.Log("Waiting");
                yield return null;
            }

            double pauseDelay = playbackToken.CalculatePauseDelay();
            double newStartTime = startTime + pauseDelay;

            playTokens.Remove(playbackToken);


            // Loading audiosources
            AudioOutSource[] mainBusOuts = GetMainBusOuts();
            AudioOutSource[] trackOuts = GetTrackOuts(timeLineChange);
            //AudioSource defaultAudioSource = mainBusOuts.Length > 0 ?  null: AudioPool.audioPoolInstance.Checkout(name);
            Dictionary<AudioOutSource, AudioSource> out2Source = new Dictionary<AudioOutSource, AudioSource>();

            // Adding main bus sources
            foreach (AudioOutSource audioOut in mainBusOuts)
                if (!out2Source.ContainsKey(audioOut))
                {
                    AudioSource source = AudioPool.audioPoolInstance.Checkout(GetName());
                    audioOut.GetAudioSettings(playbackID, parameters).ApplyToAudioSource(source, (PlaynodeTrackItem)GetTracks()[(int)timeLineChange.rowNumber], GetCombinedVolume(), GetCombinedPan(), targetFlowNode);
                    source.clip = clip;
                    out2Source.Add(audioOut, source);
                }

            // Adding track sources
            foreach (AudioOutSource audioOut in trackOuts)
                if (!out2Source.ContainsKey(audioOut))
                {
                    AudioSource source = AudioPool.audioPoolInstance.Checkout(GetName());
                    audioOut.GetAudioSettings(playbackID, parameters).ApplyToAudioSource(source, (PlaynodeTrackItem)GetTracks()[(int)timeLineChange.rowNumber], GetCombinedVolume(), GetCombinedPan(), targetFlowNode);
                    source.clip = clip;
                    source.loop = looping;
                    out2Source.Add(audioOut, source);
                }

            //if (defaultAudioSource != null)
            //defaultAudioSource.clip = clip;

            bool wasLate = false;

            if (AudioSettings.dspTime > newStartTime)
            {
                //Debug.LogWarning(clip.name + " was " + (AudioSettings.dspTime - newStartTime) + " seconds late");
                wasLate = true;
            }

            // Starting play
            foreach (KeyValuePair<AudioOutSource, AudioSource> kv in out2Source)
            {
                //Debug.Log("[" + GetName() + "]" + "[PlayNode][PlayAtDSPTime] Assigned to audiosource");
                kv.Value.PlayScheduled(newStartTime);
                LayersEventBus.RaiseEvent(GetEvent(LayersAnalyzerEvent.LayersEventTypes.AudioScheduled, System.Guid.NewGuid(), AudioSettings.dspTime, null, clip.name, wasLate));
    

                float finalTimeOffset = Mathf.Repeat(
                (float)timeLineChange.interiorStartTime + Mathf.Clamp( (float)(AudioSettings.dspTime - newStartTime), 0f, float.MaxValue) + (float)audioTimeShift, clip.length);
            

                kv.Value.time = finalTimeOffset;
            }
            //defaultAudioSource?.PlayScheduled(newStartTime);

            //Debug.Log("[" + GetName() + "]" + "[PlayNode][PlayAtDSPTime] playback about to start. Fast forwarding " + timeLineChange.interiorStartTime + "In Clip");
            LayersEventBus.RaiseEvent(GetEvent(LayersAnalyzerEvent.LayersEventTypes.AudioStarted, clipGUID, newStartTime, null, clip.name, wasLate));

            // Playback token setup
            playbackToken = new PlayToken(0.5f + newStartTime + timeLength, newStartTime + nodeEndTime, targetTrailTime, this, session, "[DoAudioClipPlayback] Waiting for audio clip to end");
            playbackToken.audioSources.AddRange(out2Source.Values);
            //playbackToken.audioSources.Add(defaultAudioSource);

            playTokens.Add(playbackToken);

            // running this while playing
            while (playbackToken.shouldYield)
            {
                // updating playback values
                foreach (KeyValuePair<AudioOutSource, AudioSource> kv in out2Source)
                {
                    if (kv.Key != null)
                    {
                        kv.Key.GetAudioSettings(playbackID, parameters).ApplyToAudioSource(kv.Value, (PlaynodeTrackItem)GetTracks()[(int)timeLineChange.rowNumber], GetCombinedVolume(), GetCombinedPan(), targetFlowNode);
                        kv.Value.loop = looping;
                    }
                }


                if (playbackToken.reachedEndTime || playbackToken.currentState == PlayToken.states.StoppedEarly)
                {

                    //Debug.Log("[" + GetName() + "]" + "[PlayNode][PlayAtDSPTime] playback stopped. The current time is  " + AudioSettings.dspTime);
                    LayersEventBus.RaiseEvent(GetEvent(LayersAnalyzerEvent.LayersEventTypes.AudioFinished, clipGUID, AudioSettings.dspTime, null, clip.name));
                    playbackToken.Stop();
                    // returning audiosources to pool on stop
                    foreach (AudioSource audioSource in out2Source.Values)
                        AudioPool.audioPoolInstance.Return(audioSource);
                    //if (defaultAudioSource != null)
                    //AudioPool.audioPoolInstance.Return(defaultAudioSource);

                    foreach (AudioOutSource mainbusOut in mainBusOuts)
                        mainbusOut.ReturnAudioSettings(playbackID);

                    foreach (AudioOutSource trackout in trackOuts)
                        trackout.ReturnAudioSettings(playbackID);

                    playTokens.Remove(playbackToken);

                    yield break;
                }
                yield return null;
            }
            LayersEventBus.RaiseEvent(GetEvent(LayersAnalyzerEvent.LayersEventTypes.AudioFinished, clipGUID, newStartTime + timeLineChange.length, null, clip.name, wasLate));

            // returning audiosources to pool
            playTokens.Remove(playbackToken);
            foreach (AudioSource audioSource in out2Source.Values)
                AudioPool.audioPoolInstance.Return(audioSource);
            //if (defaultAudioSource != null)
            //AudioPool.audioPoolInstance.Return(defaultAudioSource);

            //Starting clip

        }


        #endregion



        #region MIDI Playback



        private IEnumerator PlayMidiCoroutine(TimelineDataItem timeLineChange, TempoMap tempoMap, List<Note> allNotes, double startTime, double nodeEndTime, Dictionary<string, object> data, System.Guid session,int nodesCalledThisFrame)
        {
        



            //while (AudioSettings.dspTime + 0.1 < startTime)
            //yield return null;

            // calculating when node end time occurs in the midifile's time space
            double localEndTime = nodeEndTime - startTime;
            if (localEndTime < 0)
                yield break;

            int numberRepetitions = 1;
            double dataSourceLength = timeLineChange.length;
            if (timeLineChange is PlaynodeDataItem)
            {
                numberRepetitions = (timeLineChange as PlaynodeDataItem).numberRepetitions;
                dataSourceLength = (timeLineChange as PlaynodeDataItem).DataSourceLength;
            }



            List<Note> notes = new List<Note>();

            long searchRangeTicks = TimeConverter.ConvertFrom(new MetricTimeSpan((long)((timeLineChange.interiorStartTime + timeLineChange.length) * 1000000)), tempoMap);
            long timelineItemStartInTicks = TimeConverter.ConvertFrom(new MetricTimeSpan((long)(timeLineChange.interiorStartTime * 1000000)), tempoMap);
            //notes.AddRange(midiAsset.GetNotes().Where(x => x.Time >= timelineItemStartInTicks && x.Time <= searchRangeTicks));

            long lengthInTicks = TimeConverter.ConvertFrom(new MetricTimeSpan((long)(dataSourceLength * 1000000)), tempoMap);

            for (int index = 0; index < numberRepetitions; index++)
            {
                long startOffsetinTicks = lengthInTicks * index;
                notes.AddRange(allNotes.Where(x => x.Time + startOffsetinTicks >= timelineItemStartInTicks && x.Time + startOffsetinTicks <= searchRangeTicks).Select(x => new Note(x.NoteNumber, x.Length, x.Time + startOffsetinTicks)));
            }

            double targetTrailTime = 0.4;
            double trailTime = targetTrailTime;

            PlayToken playbackToken = new PlayToken(startTime, nodeEndTime, targetTrailTime, this, session, "[Play MIDI Coroutine] Waiting for MIDI file to start");
            playTokens.Add(playbackToken);

            while (playbackToken.shouldYield)
            {
                if ( playbackToken.currentState == PlayToken.states.StoppedEarly)
                    yield break;
                yield return null;
            }

            double pauseDelay = playbackToken.CalculatePauseDelay();
            startTime = startTime + pauseDelay;

            playTokens.Remove(playbackToken);

            Dictionary<string, object> midiFileParams = new Dictionary<string, object>();

            List<GraphVariable> variables = (timeLineChange as PlaynodeDataItem).eventParameters;
            foreach (GraphVariable var in variables)
                midiFileParams.Add(var.name, var.Value());

            for (int index = 0; index < notes.Count; index++)
            {
                Note currentNote = notes[index];

                double actualNoteTime = currentNote.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000000f;



                PlayToken myPauseToken = new PlayToken(startTime + actualNoteTime, pauseDelay + nodeEndTime, trailTime, this, session, "[Play MIDI Coroutine] Waiting for MIDI note to start");
                playTokens.Add(myPauseToken);

                // lighting up node
                //StartCoroutine(SymphonyUtils.WaitForDSPTime(startTime + actualNoteTime, () => { lastAccessDSPTime = AudioSettings.dspTime; }));


                //yield return myPauseToken.DoPause();
                bool endedEarly = false;
                while (myPauseToken.shouldYield)
                {
                    if (myPauseToken.currentState == PlayToken.states.StoppedEarly)
                    {
                        endedEarly = true;
                        break;
                    }
                    yield return null;
                }


                double timeInaccuracy = (startTime + actualNoteTime - AudioSettings.dspTime);
                if (timeInaccuracy - 0.02 < 0)
                    trailTime += Mathf.Clamp(-(float)(startTime + actualNoteTime - AudioSettings.dspTime), 0, float.MaxValue);
                else
                    trailTime -= (trailTime - targetTrailTime) / 2;

                startTime += myPauseToken.CalculatePauseDelay();


                if (endedEarly)
                {
                    playTokens.Remove(myPauseToken);
                    yield break;
                }

                //queuing up next
                MidiData midiInfo = new MidiData(currentNote.NoteNumber, (MidiData.MidiChannel)((int)currentNote.Channel), currentNote.Velocity / 128f);
                Dictionary<string, object> noteData = new Dictionary<string, object>(midiFileParams);
                noteData.Add("NoteInfo", midiInfo);
                CallFunctionOnOutputNodes("midiOut", myPauseToken.startTime, noteData, nodesCalledThisFrame);

                List<TimeLineRowDataItem> tracks = GetTracks();

                //Adding an escape in case things get screwy
                if (tracks.Count < (int)timeLineChange.rowNumber + 1)
                {
                    Debug.LogError("Attempted to play an event that was not associated with a track");
                    continue;
                }

                string nodeName = GetTracks()[(int)timeLineChange.rowNumber] is PlaynodeTrackItem ? (GetTracks()[(int)timeLineChange.rowNumber] as PlaynodeTrackItem).midiOutNodeName : currentNote.NoteNumber.ToString();

                CallFunctionOnOutputNodes(nodeName, myPauseToken.startTime, noteData, nodesCalledThisFrame);


                // removing pause token
                StartCoroutine(SymphonyUtils.WaitForDSPTime(startTime + actualNoteTime, () =>
                {
                    playTokens.Remove(myPauseToken);
                }));
            }
        }

        #endregion


        public void Pause(double time)
        {
        
            foreach (PlayToken token in playTokens)
                if (!token.paused)
                    token.Pause(time);
        }

        public void ResumePlay(double time)
        {
        
            foreach (PlayToken token in playTokens)
                if (token.paused)
                    token.Resume(time);
        }

        public void Stop(double time)
        {
        
            StartCoroutine(SymphonyUtils. WaitForDSPTime(time, () => {
                foreach (PlayToken token in playTokens)
                    token.Stop();
            }));
            //CallFunctionOnOutputNodes("playFinished", time, data, audioSettings, midiData);
        
        }

        private LayersAnalyzerEvent GetEvent(LayersAnalyzerEvent.LayersEventTypes eventType, System.Guid playbackID, double time, AudioOut audioOutNode, string prettyname, bool wasLate =false)
        {
            string targetNodeName = targetFlowNode != null ? targetFlowNode.name : "";
            System.Guid nodeID = targetFlowNode != null ? targetFlowNode.nodeID : System.Guid.Empty;

            SoundGraph targetSoundGraph = targetFlowNode != null ? targetFlowNode.soundGraph: null;
            string soundGraphName = targetSoundGraph != null ? targetSoundGraph.name : "";
            string soundGraphId = targetSoundGraph != null ? targetSoundGraph.graphID : System.Guid.Empty.ToString();

            SoundGraphPlayer player = targetSoundGraph != null ? targetSoundGraph.owningMono : null;
            string playerName = player != null ? player.name : "Playing in preview";
            System.Guid playerGUID = player != null ? player.playerID : System.Guid.Empty;

            return new LayersAnalyzerEvent(playbackID, targetSoundGraph, targetFlowNode, audioOutNode, time, eventType, prettyname, wasLate);
        }

        private class PlayToken
        {
            public enum states { Active, Complete, StoppedEarly }

            public states currentState { get; private set; }

            public bool paused { get; private set; }

            private double originalStartTime;
            public double startTime { get; private set; }

            private double lastPauseTime = 0;

            private double trailTime = 0.1;

            private double nodeEndTime = 0;

            private PlaybackSystem player;

            //private bool wasStopped;

            public List<AudioSource> audioSources = new List<AudioSource>();

            //private bool forceStopped = false;

            public System.Guid executionContext { get; private set; }

            private string debugLabel;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expectedStartTime">The expected start time for the time segment this token represents</param>
            /// <param name="nodeEndTime">The expected end time for the ENTIRE node</param>
            /// <param name="trailTime">The amount of buffer time used to schedule events in advance. Keep this larger than the deltatime</param>
            /// <param name="player">Reference to the owning node</param>
            public PlayToken(double expectedStartTime, double nodeEndTime, double trailTime, PlaybackSystem player, System.Guid executionContext, string debugLabel)
            {
                this.originalStartTime = expectedStartTime;
                this.startTime = expectedStartTime;
                this.trailTime = trailTime;
                this.player = player;
                this.nodeEndTime = nodeEndTime;
                this.executionContext = executionContext;
                this.debugLabel = debugLabel;
            }


            public void Pause(double time)
            {
                if (lastPauseTime == 0)
                    lastPauseTime = time;


                player.StartCoroutine(FlowNode.WaitForDSPTime(time, () => {
                    paused = true;
                    foreach (AudioSource audioSource in audioSources)
                        audioSource?.Pause();
                }));
            }

            public void Resume(double time)
            {
                if (lastPauseTime != 0)
                {
                    startTime += time - lastPauseTime;
                    lastPauseTime = 0;
                }

                player.StartCoroutine(FlowNode.WaitForDSPTime(time, () => {
                    paused = false;
                    foreach (AudioSource audioSource in audioSources)
                        audioSource?.UnPause();
                }));
            }

            /*
        public IEnumerator DoPause()
        {
            while (shouldYield)
            {
                if (player.currentState == states.Stopped)
                    wasStopped = true;
                yield return null;
            }
        }*/

            public void Stop()
            {
                foreach (AudioSource audioSource in audioSources)
                    if (audioSource != null)
                        audioSource?.Stop();
                currentState = states.StoppedEarly;
            }

            public bool shouldYield
            {
                get
                {
                
                    return AudioSettings.dspTime + trailTime + Time.deltaTime < startTime || paused;
                }
            }


            /// <summary>
            /// True if playback is past the end time for the play node
            /// </summary>
            public bool reachedEndTime
            {
                get
                {
                    return AudioSettings.dspTime > nodeEndTime + CalculatePauseDelay();
                }
            }

            public double CalculatePauseDelay()
            {
                return startTime - originalStartTime;
            }

            public override string ToString()
            {
                return base.ToString() + " | " + debugLabel ;
            }
        }
    }
}
