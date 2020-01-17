using UnityEngine;

public class ControllerSoundButton : MonoBehaviour
{
    private AudioSource sourseButton;
    private AudioSource musicSourse;

    void Start()
    {
        sourseButton = gameObject.AddComponent<AudioSource>();
        sourseButton.clip = Resources.Load<AudioClip>("AudioClips/Menu/SelectButton");
        musicSourse = GameObject.Find("(singleton) ToolBox").GetComponent<AudioSource>();
    }

    public void PlaySoundButton()
    {
        sourseButton.volume = PlayerPrefs.GetFloat("soundVolume");
        sourseButton.Play();
    }

    public void PlaySoundButton(string sound)
    {
        sourseButton.volume = PlayerPrefs.GetFloat("soundVolume");
        sourseButton.clip = Resources.Load<AudioClip>("AudioClips/Sounds/" + sound);
        sourseButton.Play();
    }

    public void PlaySoundButtonMiniGames(string sound, string miniGame)
    {
        sourseButton.volume = PlayerPrefs.GetFloat("soundVolume");
        sourseButton.clip = Resources.Load<AudioClip>("AudioClips/MiniGames/" + miniGame + "/" + sound);
        sourseButton.Play();
    }
}