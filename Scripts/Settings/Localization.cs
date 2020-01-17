using System;
using UnityEngine;
using UnityEngine.UI;

public class Localization : MonoBehaviour
{
    public LocGameImage[] LocGI;

    void Awake()
    {
        if (Singleton<ToolBox>.Instance.GameLanguage == "Rus")
        {
            for (int i = 0; i < LocGI.Length; i++)
            {
                LocGI[i].LocImage.sprite = LocGI[i].RusSprite;
            }
        }
        else if (Singleton<ToolBox>.Instance.GameLanguage == "Eng")
        {
            for (int i = 0; i < LocGI.Length; i++)
            {
                LocGI[i].LocImage.sprite = LocGI[i].EngSprite;
            }
        }
    }

    [Serializable]
    public struct LocGameImage
    {
        public Image LocImage;
        public Sprite RusSprite;
        public Sprite EngSprite;
    }
}