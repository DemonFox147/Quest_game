using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Backgrounds : MonoBehaviour
{
    private Image BGSec;
    private Image BGMain;

    public Image FBG;

    public delegate void BackgroundChange(bool state);

    public event BackgroundChange BGChange;

    void Awake()
    {
        BGSec = transform.GetChild(0).GetComponent<Image>();
        BGMain = transform.GetChild(1).GetComponent<Image>();
    }

    void Start()
    {
        if (Singleton<GameState>.Instance.nameBackground == string.Empty)
        {
            Singleton<GameState>.Instance.nameBackground = BGMain.sprite.name ?? "";
        }
        else
        {
            ApplyBG(Singleton<GameState>.Instance.nameBackground);
        }

        if (Singleton<GameState>.Instance.nameFirstBackground == string.Empty)
        {
            Singleton<GameState>.Instance.nameFirstBackground = FBG.sprite.name ?? "";
        }
        else
        {
            ApplyFBG(Singleton<GameState>.Instance.nameFirstBackground);
        }
    }

    public void ApplyBG(string nameBG)
    {
        Sprite BGSprite = Resources.Load<Sprite>("Images/BG_CG/" + nameBG);

        if (BGSprite != null)
        {
            BGMain.sprite = BGSprite;

            Singleton<GameState>.Instance.nameBackground = nameBG;

            if (BGChange != null)
            {
                BGChange(false);
            }
        }
        else
        {
            Debug.Log("<color=red>Фон " + nameBG + " не найден. </color>");
        }
    }

    public void ApplyFBG(string nameFBG)
    {
        Sprite FBGSprite = Resources.Load<Sprite>("Images/FBG/" + nameFBG);

        if (FBGSprite != null)
        {
            FBG.sprite = FBGSprite;

            Singleton<GameState>.Instance.nameFirstBackground = nameFBG;
        }
        else
        {
            Debug.Log("<color=red>Фон " + nameFBG + " не найден.</color>");
        }
    }

    public void LoadBG(string nameBG)
    {
        Sprite BGSprite = Resources.Load<Sprite>("Images/BG_CG/" + nameBG); 

        if (BGSprite != null)
        {
            BGSec.sprite = BGSprite;

            Singleton<GameState>.Instance.nameBackground = nameBG;
            StartCoroutine(IETransition());
        }
        else
        {
            Debug.Log("<color=red>Фон " + nameBG + " не найден.</color>");
        }
    }

    public void LoadEndImage(string nameBG)
    {
        try
        {
            if (Singleton<ToolBox>.Instance.GameLanguage == "Rus")
            {
                BGSec.sprite = Resources.Load<Sprite>("Images/Ends/" + nameBG + "_RUS");
            }
            else
            {
                BGSec.sprite = Resources.Load<Sprite>("Images/Ends/" + nameBG + "_ENG");
            }
        }
        catch
        {
            Debug.Log("<color=red>Фон " + nameBG + " не найден.</color>");
        }

        StartCoroutine(IETransition());
    }

    public IEnumerator IETransition()
    {
        if (BGChange != null)
        {
            BGChange(true);
        }

        Color color = BGMain.color;
        for (float f = 1; f > 0; f -= 0.04f)
        {
            color.a = f;
            BGMain.color = color;
            yield return new WaitForFixedUpdate();
        }
        BGMain.sprite = BGSec.sprite;
        color.a = 1;
        BGMain.color = color;
    }
}