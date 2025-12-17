using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager_Khoi : MonoBehaviour
{
    public static AudioManager_Khoi Instance { get; private set; }
    public AudioClip shotClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayShotSound(AudioClip clip, Vector3 position, float volume)
    {
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }
}
