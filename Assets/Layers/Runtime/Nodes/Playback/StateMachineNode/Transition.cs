using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Runtime.Nodes.Playback
{
    [System.Serializable]
    public class Transition
    {
        public string guid = System.Guid.NewGuid().ToString();
        public string targetStateGUID;
        public float fadeTime;
        public SoundGraph transitionGraph;

        private SoundGraph _transitionGraphPlaybackGraph;
        public SoundGraph transitionGraphPlaybackGraph
        {
            get
            {
                if (_transitionGraphPlaybackGraph == null || transitionGraph == null || transitionGraph.graphID != _transitionGraphPlaybackGraph.graphID)
                    MakeRuntimeCopy();
                return _transitionGraphPlaybackGraph;
            }
            private set
            {
                _transitionGraphPlaybackGraph = value;
            }
        }
        public string startEventID;
        public string endEventID;
        public string delayEventID;
        public bool expanded = false;
        public bool isInTransition = false;
        public double timeOfLastActivation = 0;

        public void MakeRuntimeCopy()
        {
            if (transitionGraph != null)
                _transitionGraphPlaybackGraph = (SoundGraph)(Application.isPlaying ? transitionGraph.RuntimeCopy() : transitionGraph.Copy());
            else
                _transitionGraphPlaybackGraph = null;
        }
    }
}