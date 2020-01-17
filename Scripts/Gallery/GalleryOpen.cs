using System.Collections.Generic;
using UnityEngine;

public class GalleryOpen : MonoBehaviour
{
    public Dictionary<int, string[]> BGDictionaty;
    public Dictionary<int, string[]> CGDictionaty;

    public Dictionary<int, bool> BGDictionatyActive;
    public Dictionary<int, bool> CGDictionatyActive;

    void Start()
    {
        BGDictionaty = Singleton<ToolBox>.Instance.BGDictionaty;
        CGDictionaty = Singleton<ToolBox>.Instance.CGDictionaty;

        BGDictionatyActive = new Dictionary<int, bool>();
        CGDictionatyActive = new Dictionary<int, bool>();

        if (PlayerPrefs.GetString("saveBW_openedBGsList") == "Zero")
        {
            string text = string.Empty;

            for (int i = 0; i < BGDictionaty.Count; i++)
            {
                text += 'f';
                BGDictionatyActive.Add(i, false);
            }

            PlayerPrefs.SetString("saveBW_openedBGsList", text);
        }
        else
        {
            string ListBG = PlayerPrefs.GetString("saveBW_openedBGsList");

            for (int i = 0; i < ListBG.Length; i++)
            {
                char c = ListBG[i];
                if (c == 't')
                {
                    BGDictionatyActive.Add(i, true);
                }
                else
                {
                    BGDictionatyActive.Add(i, false);
                }
            }
        }

        if (PlayerPrefs.GetString("saveBW_openedCGsList") == "Zero")
        {
            string text = string.Empty;

            for (int i = 0; i < CGDictionaty.Count; i++)
            {
                text += 'f';
                CGDictionatyActive.Add(i, false);
            }

            PlayerPrefs.SetString("saveBW_openedCGsList", text);
        }
        else
        {
            string ListCG = PlayerPrefs.GetString("saveBW_openedCGsList");

            for (int i = 0; i < ListCG.Length; i++)
            {
                char c2 = ListCG[i];
                if (c2 == 't')
                {
                    CGDictionatyActive.Add(i, true);
                }
                else
                {
                    CGDictionatyActive.Add(i, false);
                }
            }
        }
    }

    public void OpenBG(string nameBG)
    {
        for (int i = 0; i < BGDictionatyActive.Count; i++)
        {
            if (BGDictionaty[i][0] == nameBG && !BGDictionatyActive[i])
            {
                BGDictionatyActive[i] = true;
                SaveBGChanges();

                break;
            }
        }
    }

    public void OpenCG(string nameCG)
    {
        for (int i = 0; i < CGDictionatyActive.Count; i++)
        {
            if (CGDictionaty[i][0] == nameCG && !CGDictionatyActive[i])
            {
                CGDictionatyActive[i] = true;
                SaveCGChanges();

                break;
            }
        }
    }

    private void SaveBGChanges()
    {
        string text = string.Empty;

        for (int i = 0; i < BGDictionatyActive.Count; i++)
        {
            if (BGDictionatyActive[i])
            {
                text += 't';
            }
            else
            {
                text += 'f';
            }
        }

        PlayerPrefs.SetString("saveBW_openedBGsList", text);
    }

    private void SaveCGChanges()
    {
        string text = string.Empty;

        for (int i = 0; i < CGDictionatyActive.Count; i++)
        {
            if (CGDictionatyActive[i])
            {
                text += 't';
            }
            else
            {
                text += 'f';
            }
        }

        PlayerPrefs.SetString("saveBW_openedCGsList", text);
    }
}