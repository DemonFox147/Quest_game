using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    private AudioSource sourseMusic;

    public Slider Music;
    public Slider Sound;

    public Slider AutoHopTime;
    public Slider TextSpeed;

    public Image Windowed;
    public Image FullScreen;

    public Transform Font;

    void Start()
    {
        sourseMusic = GameObject.Find("(singleton) ToolBox").GetComponent<AudioSource>();

        Music.value = PlayerPrefs.GetFloat("musicVolume");
        Sound.value = PlayerPrefs.GetFloat("soundVolume");

        AutoHopTime.value = PlayerPrefs.GetFloat("autoHopTime");
        TextSpeed.value = PlayerPrefs.GetFloat("textSpeed");

        if (Screen.fullScreen)
        {
            FullScreenSelect();
        }
        else
        {
            WindowedSelect();
        }

        SetFontSize(PlayerPrefs.GetInt("fontValue", 1));
    }
    

    public void SetMusicVolume()
    {
        sourseMusic.volume = Music.value;
        PlayerPrefs.SetFloat("musicVolume", Music.value);
    }

    public void SetSoundVolume()
    {
        PlayerPrefs.SetFloat("soundVolume", Sound.value);
    }

    public void SetAutoHopTime()
    {
        PlayerPrefs.SetFloat("autoHopTime", AutoHopTime.value);
    }

    public void SetTextSpeed()
    {
        PlayerPrefs.SetFloat("textSpeed", TextSpeed.value);
    }

    public void FullScreenSelect()
    {
        Screen.SetResolution(Screen.width, Screen.height, true, 60);
        string lang = Singleton<ToolBox>.Instance.GameLanguage;

        FullScreen.sprite = 
            Resources.Load<Sprite>("Images/Settings/" + lang + "/FullScreenGreen");
        Windowed.sprite = 
            Resources.Load<Sprite>("Images/Settings/" + lang + "/WindowedRed");
    }

    public void WindowedSelect()
    {
        Screen.SetResolution(Screen.width, Screen.height, false, 60);
        string lang = Singleton<ToolBox>.Instance.GameLanguage;

        FullScreen.sprite =
                    Resources.Load<Sprite>("Images/Settings/" + lang + "/FullScreenGreen");
        Windowed.sprite =
            Resources.Load<Sprite>("Images/Settings/" + lang + "/WindowedRed");
    }

    public void SetFontSize(int fontSize)
    {
        string lang = Singleton<ToolBox>.Instance.GameLanguage;

        switch (fontSize)
        {
            case 0:
                PlayerPrefs.SetInt("fontSize", 20);
                PlayerPrefs.SetInt("fontValue", 0);

                Font.GetChild(0).GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("Images/Settings/" + lang + "/SmallFont_Light");
                Font.GetChild(1).GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("Images/Settings/" + lang + "/MediumFont_Def");
                Font.GetChild(2).GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("Images/Settings/" + lang + "/LargeFont_Def");

                break;
            case 1:
                PlayerPrefs.SetInt("fontSize", 23);
                PlayerPrefs.SetInt("fontValue", 1);

                Font.GetChild(0).GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("Images/Settings/" + lang + "/SmallFont_Def");
                Font.GetChild(1).GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("Images/Settings/" + lang + "/MediumFont_Light");
                Font.GetChild(2).GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("Images/Settings/" + lang + "/LargeFont_Def");

                break;
            case 2:
                PlayerPrefs.SetInt("fontSize", 27);
                PlayerPrefs.SetInt("fontValue", 2);

                Font.GetChild(0).GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("Images/Settings/" + lang + "/SmallFont_Def");
                Font.GetChild(1).GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("Images/Settings/" + lang + "/MediumFont_Def");
                Font.GetChild(2).GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("Images/Settings/" + lang + "/LargeFont_Light");

                break;
        }
    }

    public void SetLanguage(string Lang)
    {
        switch (Lang)
        {
            case "Rus":
                PlayerPrefs.SetString("language", "Rus");
                Singleton<ToolBox>.Instance.GameLanguage = "Rus";
                Singleton<ToolBox>.Instance.TextLanguage("Rus");
                Singleton<ToolBox>.Instance.LoreLanguage("Rus");
                Singleton<ToolBox>.Instance.LoreOpen.UpdateLoreLang();
                Singleton<GameState>.Instance.language = "Rus";
                break;

            case "Eng":
                PlayerPrefs.SetString("language", "Eng");
                Singleton<ToolBox>.Instance.GameLanguage = "Eng";
                Singleton<ToolBox>.Instance.TextLanguage("Eng");
                Singleton<ToolBox>.Instance.LoreLanguage("Eng");
                Singleton<ToolBox>.Instance.LoreOpen.UpdateLoreLang();
                Singleton<GameState>.Instance.language = "Eng";
                break;
        }

        SceneManager.LoadScene(3);
    }
}