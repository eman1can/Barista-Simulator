using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioPlaylist : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] songs;
    public int songIndex = 0;

    public bool next = false;

    private void Start()
    {
        if (songs.Length == 0)
        {
            return;
        }
        source.clip = songs[0];
        source.Play();
    }

    private void Update()
    {
        if (next)
        {
            next = false;
            source.Stop();
        }

        if (!source.isPlaying && songs.Length != 0)
        {
            songIndex++;
            if (songIndex >= songs.Length)
            {
                songIndex = 0;
            }
            source.clip = songs[songIndex];
            source.Play();
        }
    }
}
