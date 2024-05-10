using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicManager : Singleton<MusicManager>
{

    public AudioSource musicAudio;//背景音乐组件

    public List<AudioSource> soundAudioList = new List<AudioSource>();//音效组件列表

    public float musicVolumnMultiplier = 1;//音乐音量要乘以的倍率（主要用于UI面板调整）

    public float soundVolumnMultiplier = 1;//音效音量要乘以的倍率（主要用于UI面板调整）


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
        StartCoroutine(LoadMusic(.05f));
    }
    IEnumerator LoadMusic(float fadeSpeed)
    {
        musicAudio.Play();

        float defaultVolumn = 0.3f;
        if(musicAudio.clip.name=="Boss"|| musicAudio.clip.name == "Boss_2")
        {
            defaultVolumn = 0.6f;
        }
        while (musicAudio.volume < defaultVolumn * musicVolumnMultiplier)
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

        //如果初始音量不合适 可以在这里调整
        switch(soundName)
        {
            case "射击":
                soundAudio.volume = 0.5f;
                break;
            case "手枪射击":
                soundAudio.volume = 0.6f;
                break;
            case "导弹爆炸":
                soundAudio.volume = 0.4f;
                break;
            case "水下正常移动":
                soundAudio.volume = 0.4f;
                break;
            case "水下快速移动":
                soundAudio.volume = 0.6f;
                break;
            case "角色界面发射":
                soundAudio.volume = 0.6f;
                break;
            case "角色界面切换功能":
                soundAudio.volume = 0.5f;
                break;
            case "充能":
                soundAudio.volume = 0.5f;
                break;
            case "收集":
                soundAudio.volume = 0.7f;
                break;
            case "玩家受伤":
                soundAudio.volume = 0.7f;
                break;
            case "Boss吐酸":
                soundAudio.volume = 0.7f;
                break;
            case "Boss苏醒":
                soundAudio.volume = 0.6f;
                break;
            case "潜艇爆炸":
                soundAudio.volume = 0.5f;
                break;
            case "潜艇警报":
                soundAudio.volume = 0.6f;
                break;
            case "震爆":
                soundAudio.volume = 0.6f;
                break;
            case "挖掘机":
                soundAudio.volume = 0.3f;
                break;
            case "教程按钮":
                soundAudio.volume = 0.2f;
                break;
            default:
                soundAudio.volume = 1;
                break;
        }
        soundAudio.volume *= soundVolumnMultiplier;
        soundAudio.Play();
        while (soundAudio.isPlaying)
        {
            yield return new WaitForSecondsRealtime(0.1f);
        }
        soundAudioList.Remove(soundAudio);
        Destroy(soundObj);

    }




}
