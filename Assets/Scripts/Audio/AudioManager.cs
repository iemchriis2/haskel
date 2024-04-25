//Thomas Pereira
//Doesn't get used. Can be deleted if needed.
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

#region Custom Sound Class
[System.Serializable]
public class Sound
{
    public string Name;
    
    public AudioClip Clip;

    [Range(0,1)]
    public float Volume;
    
    public float Pitch;
    
    //0 is 2D, 1 is 3D
    [Range(0,1)]
    public float SpacialBlend;
    
    public bool Loop;

    [HideInInspector]
    public AudioSource source;

}
#endregion

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;

    public static AudioManager Singleton;

    #region MonoBehaviour Implementation
    private void Awake()
    {
        if(Singleton == null)
        {
            Singleton = this;   
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in Sounds)
        {
            s.source = gameObject.GetComponent<AudioSource>();
            s.source.clip = s.Clip;
            s.source.loop = s.Loop;
            s.source.pitch = s.Pitch;
            s.source.spatialBlend = s.SpacialBlend;
            s.source.volume = s.Volume;
        }
    }
    #endregion

    public void Play(string _songName)
    {
        Sound _s = Array.Find(Sounds, _sound => _sound.Name == _songName);
        _s.source.Play();
    }

    public void Stop(string _songName)
    {
        Sound _s = Array.Find(Sounds, _sound => _sound.Name == _songName);
        _s.source.Stop();
    }

}
