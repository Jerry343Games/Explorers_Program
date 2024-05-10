using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicManager : Singleton<MusicManager>
{

    public AudioSource musicAudio;//�����������

    public List<AudioSource> soundAudioList = new List<AudioSource>();//��Ч����б�

    public float musicVolumnMultiplier = 1;//��������Ҫ���Եı��ʣ���Ҫ����UI��������

    public float soundVolumnMultiplier = 1;//��Ч����Ҫ���Եı��ʣ���Ҫ����UI��������


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

        //�����ʼ���������� �������������
        switch(soundName)
        {
            case "���":
                soundAudio.volume = 0.5f;
                break;
            case "��ǹ���":
                soundAudio.volume = 0.6f;
                break;
            case "������ը":
                soundAudio.volume = 0.4f;
                break;
            case "ˮ�������ƶ�":
                soundAudio.volume = 0.4f;
                break;
            case "ˮ�¿����ƶ�":
                soundAudio.volume = 0.6f;
                break;
            case "��ɫ���淢��":
                soundAudio.volume = 0.6f;
                break;
            case "��ɫ�����л�����":
                soundAudio.volume = 0.5f;
                break;
            case "����":
                soundAudio.volume = 0.5f;
                break;
            case "�ռ�":
                soundAudio.volume = 0.7f;
                break;
            case "�������":
                soundAudio.volume = 0.7f;
                break;
            case "Boss����":
                soundAudio.volume = 0.7f;
                break;
            case "Boss����":
                soundAudio.volume = 0.6f;
                break;
            case "Ǳͧ��ը":
                soundAudio.volume = 0.5f;
                break;
            case "Ǳͧ����":
                soundAudio.volume = 0.6f;
                break;
            case "��":
                soundAudio.volume = 0.6f;
                break;
            case "�ھ��":
                soundAudio.volume = 0.3f;
                break;
            case "�̳̰�ť":
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
