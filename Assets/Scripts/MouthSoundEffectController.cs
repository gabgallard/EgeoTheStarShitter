using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthSoundEffectController : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip chewClip;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayOpenMouthSound()
    {
        Debug.Log("PlayOpenMouthSound()");
        audioSource.PlayOneShot(chewClip);
    }
}
