using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RadioPlaylist : MonoBehaviour {
    public TextMeshPro titleLabel;
    public AudioSource source;
    public AudioClip[] songs;
    
    private int songIndex = 0;
    private bool paused = false;

    public void AdjustVolume(float percent) {
        source.volume = percent;
    }
    
    private void Start() {
        if (songs.Length == 0) {
            return;
        }

        songIndex = Random.Range(0, songs.Length);
        PlaySong(songs[songIndex]);
    }

    private void Update() {
        if (!paused) {
            if (!source.isPlaying && songs.Length != 0) {
                PlayNextSong();
            }
        }
    }
    public void SkipSong() {
        if (source.isPlaying)
            source.Stop();
        PlayNextSong();
    }

    public void PauseSong() {
        if (source.isPlaying) {
            source.Pause();
            paused = true;
            titleLabel.text = "";
        }
    }

    public void ResumeSong() {
        source.Play();
        paused = false;
        titleLabel.text = source.clip.name;
    }

    public void PlayNextSong() {
        songIndex++;
        if (songIndex >= songs.Length) {
            songIndex = 0;
        }
        PlaySong(songs[songIndex]);
    }
    private void PlaySong(AudioClip song) {
        source.clip = song;
        source.Play();
        titleLabel.text = song.name;
    }

    
}
