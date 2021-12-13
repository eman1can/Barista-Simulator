using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RadioPlaylist : MonoBehaviour {
    public TextMeshPro titleLabel;
    public AudioSource source;
    public AudioClip[] songs;
    
    public int songIndex = 0;

    private void Start() {
        if (songs.Length == 0) {
            return;
        }

        songIndex = Random.Range(0, songs.Length);
        PlaySong(songs[songIndex]);
    }

    private void Update() {
        if (!source.isPlaying && songs.Length != 0) {
            songIndex++;
            if (songIndex >= songs.Length) {
                songIndex = 0;
            }
            PlaySong(songs[songIndex]);
        }
    }

    private void PlaySong(AudioClip song) {
        source.clip = song;
        source.Play();
        titleLabel.text = song.name;
    }
}
