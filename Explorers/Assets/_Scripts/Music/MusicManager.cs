using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicManager : Singleton<MusicManager>
{

    public AudioSource musicAudio;//背景音乐组件

    public List<AudioSource> soundAudioList = new List<AudioSource>();//音效组件列表

    public float musicVolumn = 1;

    public float soundVolumn = 1;


    public void PlayBackMusic(string musicName)
    {
        StartCoroutine(FadeMusic(0.1f, musicName));
    }

    IEnumerator FadeMusic(float fadeSpeed, string musicName)
    {
        if (musicAudio != null)
        {
            while (musicAudio.volume > 0)
            {
                musicAudio.volume -= fadeSpeed;
                yield return new WaitForSecondsRealtime(.1f);
            }
            Destroy(musicAudio.gameObject);
            GameObject musicObj = new GameObject("MusicObj");
            musicAudio = musicObj.AddComponent<AudioSource>();
            musicAudio.clip = Resources.Load<AudioClip>("Audio/Music/" + musicName);
            musicAudio.loop = true;
            musicAudio.volume = 0;
            MusicLoadIn(.1f);
        }
        else
        {
            GameObject musicObj = new GameObject("MusicObj");
            musicAudio = musicObj.AddComponent<AudioSource>();
            musicAudio.clip = Resources.Load<AudioClip>("Audio/Music/" + musicName);
            musicAudio.loop = true;
            musicAudio.volume = 0;
            MusicLoadIn(.1f);
        }
    }
    public void MusicLoadIn(float fadeSpeed)
    {
        StartCoroutine(LoadMusic(.1f));
    }
    IEnumerator LoadMusic(float fadeSpeed)
    {
        musicAudio.Play();
        while (musicAudio.volume < musicVolumn)
        {
            musicAudio.volume += fadeSpeed;
            yield return new WaitForSecondsRealtime(.1f);
        }
    }




    public void PlaySound(string soundName)
    {
        StartCoroutine(IEPlaySound(soundName));
    }
    IEnumerator IEPlaySound(string soundName)
    {
        GameObject soundObj = new GameObject();
        soundObj.name = "SoundObj_" + soundName;
        AudioSource soundAudio = soundObj.AddComponent<AudioSource>();
        soundAudioList.Add(soundAudio);
        soundAudio.clip = Resources.Load<AudioClip>("Audio/Sound/" + soundName);
        soundAudio.loop = false;
        soundAudio.volume = soundVolumn;
        soundAudio.Play();
        while (soundAudio.isPlaying)
        {
            yield return new WaitForSecondsRealtime(0.1f);
        }
        soundAudioList.Remove(soundAudio);
        Destroy(soundObj);

    }




}
