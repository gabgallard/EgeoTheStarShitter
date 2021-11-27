using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthSoundEffectController : MonoBehaviour
{
    private FMOD.Studio.EventInstance chewingSound;

    void Awake()
    {
        
    }

    public void PlayOpenMouthSound()
    {

        chewingSound = FMODUnity.RuntimeManager.CreateInstance("event:/EgeoChewing");
        chewingSound.start();

        Debug.Log("PlayOpenMouthSound()");

        chewingSound.release();
    }
}
