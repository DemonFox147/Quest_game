using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationHighlightedButton : MonoBehaviour
{
    public LocalizationSprites[] localizationSprites;

    [Serializable]
    public struct LocalizationSprites
    {
        public string Lang;
        public Sprite SpriteDef;
        public Sprite SpriteLight;
    }

    void Start()
    {
        string lang = Singleton<ToolBox>.Instance.GameLanguage;

        int numLocalization;

        if (localizationSprites[0].Lang == lang)
        {
            numLocalization = 0;
        }
        else
        {
            numLocalization = 1;
        }

        GetComponent<Image>().sprite = localizationSprites[numLocalization].SpriteDef;

        SpriteState ss = new SpriteState
        {
            highlightedSprite = localizationSprites[numLocalization].SpriteLight
        };

        GetComponent<Button>().spriteState = ss;
    }

}