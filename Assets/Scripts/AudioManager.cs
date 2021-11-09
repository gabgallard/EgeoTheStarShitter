using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;
    public GameObject CollisionSounds;
    public GameObject SingleObjectSounds;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
