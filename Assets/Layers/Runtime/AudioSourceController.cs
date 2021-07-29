using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Runtime
{
    public class AudioSourceController
    {
        Dictionary<AudioSourceUsage, AudioSource> audioSourcesInUse = new Dictionary<AudioSourceUsage, AudioSource>();
        List<AudioSource> availableAudioSources = new List<AudioSource>();

        MonoBehaviour owner = null;

        public AudioSourceController(MonoBehaviour owner)
        {
            this.owner = owner;
        }

        private void AddAudioSource(MonoBehaviour owner)
        {
            AudioSource source = owner.gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            this.owner = owner;
            availableAudioSources.Add(source);
        }

        private AudioSourceUsage ClaimUnusedAudiosource()
        {
            if (availableAudioSources.Count == 0)
                AddAudioSource(owner);
            AudioSource source = availableAudioSources[0];
            availableAudioSources.RemoveAt(0);
            AudioSourceUsage usage = new AudioSourceUsage(this, source);
            audioSourcesInUse.Add(usage, source);
            return usage;
        }

        public AudioSourceUsage ScheduleClip(double dspStartTime, AudioClip clip)
        {
            AudioSourceUsage usage = ClaimUnusedAudiosource();
            AudioSource source = audioSourcesInUse[usage];
            source.volume = 1f;
            source.clip = clip;
            source.PlayScheduled(dspStartTime);
            return usage;
        }

        private void ReturnAudiosource(AudioSourceUsage usage)
        {
            AudioSource source;
            if (audioSourcesInUse.TryGetValue(usage, out source))
            {
                audioSourcesInUse.Remove(usage);
                availableAudioSources.Add(source);
            }
        }

        /*
    public void SetVolume (float volume)
    {
        if (currentlyPlayingAudioSource != null)
            currentlyPlayingAudioSource.volume = volume;
    }

    private IEnumerator PlayNewCoroutine(SoundAsset soundAsset, double startDSPTime, bool immediate)
    {
        AudioClip clip = soundAsset.audioClip;
        if (currentlyPlayingAudioSource != null && currentlyPlayingAudioSource.clip == clip)
        {
            currentlyPlayingAudioSource.Play();
            yield break;
        }

        AudioSource nextAudioSource = availableAudioSources[0];
        nextAudioSource.clip = clip;
        nextAudioSource.time = soundAsset.GetStartTime();
        nextAudioSource.PlayScheduled(startDSPTime);


        if (startDSPTime < AudioSettings.dspTime || immediate) // if past, don't wait 
            yield return new WaitForSeconds((float)(startDSPTime - AudioSettings.dspTime));

        if (currentlyPlayingAudioSource != null)
        {
            currentlyPlayingAudioSource.Stop();
            currentlyPlayingAudioSource.clip = null;
            availableAudioSources.Add(currentlyPlayingAudioSource);
        }

        currentlyPlayingAudioSource = nextAudioSource;
        availableAudioSources.Remove(nextAudioSource);
        
    }

    public void PlayNewDelayed(SoundAsset soundAsset, double startDSPTime)
    {
        owner.StartCoroutine(PlayNewCoroutine(soundAsset, startDSPTime, false));
    }

    public void PlayNew(SoundAsset soundAsset)
    {
        owner.StartCoroutine(PlayNewCoroutine(soundAsset, AudioSettings.dspTime, true));
    }

    public void Stop()
    {
        if (currentlyPlayingAudioSource != null)
        {
            currentlyPlayingAudioSource.Stop();
        }
    }

    public void Destroy()
    {
        foreach (AudioSource audiosource in availableAudioSources)
            GameObject.Destroy(audiosource);
        availableAudioSources.Clear();
        GameObject.Destroy(currentlyPlayingAudioSource);
    }*/

        public class AudioSourceUsage
        {
            private AudioSourceController parentController;

            private AudioSource audioSource;

            public AudioSourceUsage(AudioSourceController controller, AudioSource source)
            {
                parentController = controller;
                this.audioSource = source;
            }

            public void Dispose()
            {
                parentController.ReturnAudiosource(this);
            }
        }
    }
}
