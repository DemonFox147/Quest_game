using System.Collections;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioSource SoundSourse;
    private AudioSource MusicSourse;

    private string curMusic;
    private string pastMusic;
    private float timePastMusic;
    
    void Awake()
    {
        SoundSourse = gameObject.AddComponent<AudioSource>();
        MusicSourse = GameObject.Find("(singleton) ToolBox").GetComponent<AudioSource>();
    }

    void Start()
    {
        curMusic = Singleton<GameState>.Instance.nameCurMusic;
        pastMusic = Singleton<GameState>.Instance.namePastMusic;
        timePastMusic = Singleton<GameState>.Instance.timePastMusic;

        if (Singleton<ToolBox>.Instance.PrevLevel == "Menu" ||
            MusicSourse.clip.name != Singleton<GameState>.Instance.nameCurMusic)
        {
            LoadMusic(Singleton<GameState>.Instance.nameCurMusic);
        }

        SoundSourse.volume = PlayerPrefs.GetFloat("soundVolume");
    }

    void Update()
    {
        // ПЕРЕДЕЛЫВАТЬ
        timePastMusic = MusicSourse.time;
    }

    public void TimeScrolling(bool scroll)
    {
        if (scroll)
        {
            MusicSourse.volume = 0f;
            SoundSourse.volume = 0f;
        }
        else
        {
            MusicSourse.volume = PlayerPrefs.GetFloat("musicVolume");
            SoundSourse.volume = PlayerPrefs.GetFloat("soundVolume");
        }
    }

    public void LoadMusic(string nameMusic, float timeStart = 0f)
    {
        AudioClip music = Resources.Load<AudioClip>("AudioClips/Music/" + nameMusic);

        if (music != null)
        {
            pastMusic = curMusic == nameMusic ? pastMusic : curMusic;
            curMusic = nameMusic;
            Singleton<GameState>.Instance.nameCurMusic = curMusic;
            Singleton<GameState>.Instance.namePastMusic = pastMusic;
            Singleton<GameState>.Instance.timePastMusic = timePastMusic;
            StartCoroutine(IELoadMusic(music, timeStart));
        }
        else
        {
            Debug.Log("<color=red>Музыка " + nameMusic + " не найдена</color>");
        }
    }

    private IEnumerator IELoadMusic(AudioClip music, float timeStart)
    {
        float volMusic = MusicSourse.volume;

        for (float f = MusicSourse.volume; 0 < f; f -= 0.04f)
        {
            MusicSourse.volume = f;
            yield return new WaitForSeconds(0.01f);
        }

        MusicSourse.Stop();
        MusicSourse.clip = music;
        MusicSourse.time = timeStart;
        MusicSourse.Play();

        for (float f = MusicSourse.volume; f < volMusic; f += 0.04f)
        {
            MusicSourse.volume = f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void LoadPastMusic()
    {
        LoadMusic(pastMusic, Singleton<GameState>.Instance.timePastMusic);
    }

    public void LoadSound(string nameSound)
    {
        if (Time.timeScale == 1f)
        {
            AudioClip sound = Resources.Load<AudioClip>("AudioClips/Sounds/" + nameSound);
            if (sound != null)
            {
                SoundSourse.Stop();
                SoundSourse.clip = sound;
                SoundSourse.volume = PlayerPrefs.GetFloat("soundVolume");
                SoundSourse.Play();
            }
            else
            {
                Debug.Log("<color=red>Звук " + nameSound + " не найден</color>");
            }
        }
    }

    public void StopSound()
    {
        StartCoroutine(IEStopSound());
    }

    private IEnumerator IEStopSound()
    {
        for (float f = SoundSourse.volume; 0 < f; f -= 0.04f)
        {
            SoundSourse.volume = f;
            yield return new WaitForSeconds(0.01f);
        }
        SoundSourse.Stop();
    }
}