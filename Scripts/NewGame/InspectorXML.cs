using System.Xml;
using UnityEngine;

public class InspectorXML : MonoBehaviour
{
    delegate int[] MinusOne(string x);

    private ScriptReferencesPool srp;

    void Start()
    {
        srp = FindObjectOfType<ScriptReferencesPool>();
    }

    public void AnimationInspector(XmlNode animXml)
    {
        string name = animXml.Attributes["name"].Value;
        string type = animXml.Attributes["type"].Value;
        string lang = "";

        Debug.Log("Имя " + name + " тип " + type + " анимации");

        switch (type)
        {
            case "norm":
                {
                    if (animXml.Attributes["stop"] != null)
                    {
                        srp.AnimatorController.StopAnimation();
                        break;
                    }

                    bool freeAnimation = bool.Parse(animXml.Attributes["freeAnimation"].Value);
                    if (freeAnimation)
                    {
                        srp.ScenarioParser.AnimMode = false;
                    }
                    else
                    {
                        srp.ScenarioParser.InterfacePanelSetActive(false);
                        srp.ScenarioParser.AnimMode = true;
                    }

                    if (animXml.Attributes["lang"] != null)
                    {
                        lang = animXml.Attributes["lang"].Value;
                    }

                    if (animXml.Attributes["sound"] != null)
                    {
                        string nameSound = animXml.Attributes["sound"].Value;
                        float timeStart = 0f;
                        if (animXml.Attributes["timeStart"] != null)
                        {
                            timeStart = float.Parse(animXml.Attributes["timeStart"].Value);
                        }
                        srp.AnimatorController.StartAnimator(name, nameSound, timeStart, lang);
                    }
                    else
                    {
                        srp.AnimatorController.StartAnimator(name, lang);
                    }
                    break;
                }
            case "step":
                {
                    if (animXml.Attributes["stop"] != null)
                    {
                        srp.AnimatorController.StopStepAnimation();
                        break;
                    }

                    if (animXml.Attributes["nameAnimLayer_1"] == null)
                    {
                        string nameAnimLayer_0 = animXml.Attributes["nameAnimLayer_0"].Value;
                        srp.AnimatorController.StartStepAnimator(name, nameAnimLayer_0);
                    }
                    else 
                    {
                        string nameAnimLayer_0 = animXml.Attributes["nameAnimLayer_0"].Value;
                        string nameAnimLayer_1 = animXml.Attributes["nameAnimLayer_1"].Value;
                        srp.AnimatorController.StartStepAnimator(name, nameAnimLayer_0, nameAnimLayer_1);
                    }
                    break;
                }
            case "loop":
                {
                    if (animXml.Attributes["stop"] != null)
                    {
                        srp.AnimatorController.StopLoopedAnimation();
                    }
                    else
                    {
                        srp.AnimatorController.StartLoopAnimation(name);
                    }
                    break;
                }
            case "combin":
                {
                    if (animXml.Attributes["stop"] != null)
                    {
                        srp.AnimatorController.StopCombinedAnimation();
                    }
                    else
                    {
                        srp.AnimatorController.StartCombinAnimation(name);
                    }
                    break;
                }
            default:
                {
                    Debug.Log("Что-то пошло не так...");
                    break;
                }
        }
    }

    public void BGInspector(XmlNode bgXml)
    {
        if (bgXml.Attributes["mode"] != null)
        {
            string mode = bgXml.Attributes["mode"].Value;

            switch (mode)
            {
                case "save":
                    {
                        string saveBG = Singleton<GameState>.Instance.nameBackground;
                        Singleton<GameState>.Instance.nameSaveBackground = saveBG;
                        break;
                    }

                case "load":
                    {
                        string loadBG = Singleton<GameState>.Instance.nameSaveBackground;
                        srp.Backgrounds.LoadBG(loadBG);
                        break;
                    }
                case "apply":
                    {
                        string nameBG = bgXml.Attributes["name"].Value;
                        srp.Backgrounds.ApplyBG(nameBG);

                        Singleton<ToolBox>.Instance.GalleryOpen.OpenBG(nameBG);
                        Singleton<ToolBox>.Instance.GalleryOpen.OpenCG(nameBG);
                        break;
                    }
            }
        }
        else
        {
            string nameBG = bgXml.Attributes["name"].Value;
            srp.Backgrounds.LoadBG(nameBG);

            Singleton<ToolBox>.Instance.GalleryOpen.OpenBG(nameBG);
            Singleton<ToolBox>.Instance.GalleryOpen.OpenCG(nameBG);
        }
    }

    public void CharapterInspector(XmlNode charXml)
    {
        MinusOne minusOne = (string x) =>
        {
            string[] y = x.Split(',');
            int[] z = new int[y.Length];
            for (int i = 0; i < y.Length; i++)
            {
                z[i] = int.Parse(y[i]) - 1;
            }
            return z;
        };

        string mode = charXml.Attributes["mode"].Value;
        string type = charXml.Attributes["type"].Value;
        
        switch (mode)
        {
            case "add":
                {
                    string name = charXml.Attributes["name"].Value;
                    int pos = int.Parse(charXml.Attributes["pos"].Value) - 1;
                    srp.Characters.AddCharacterToTheScene(type, name, pos);
                    break;
                }
            case "apply":
                {
                    string name = charXml.Attributes["name"].Value;
                    int pos = int.Parse(charXml.Attributes["pos"].Value) - 1;
                    srp.Characters.ApplyCharacterToTheScene(type, name, pos);
                    break;
                }
            case "rem":
                {
                    string[] types = type.Split(',');
                    string[] dir = charXml.Attributes["dir"].Value.Split(',');
                    int[] pos = minusOne(charXml.Attributes["pos"].Value);
                    srp.Characters.RemoveTheCharacterFromTheScene(types, dir, pos);
                    break;
                }
            case "move":
                {
                    string dir = charXml.Attributes["dir"].Value;
                    int pos = int.Parse(charXml.Attributes["pos"].Value) - 1;
                    srp.Characters.MoveCharacterToTheScene(type, dir, pos);
                    break;
                }
            case "flinch":
                {
                    string[] dir = charXml.Attributes["dir"].Value.Split(',');
                    int[] pos = minusOne(charXml.Attributes["pos"].Value);
                    srp.Characters.FlinchCharacterToTheScene(type, dir, pos);
                    break;
                }
        }
    }

    public void SoundInspector(XmlNode soundXml)
    {
        string name = soundXml.Attributes["name"].Value.Trim();

        if (soundXml.Attributes["stop"] != null)
        {
            srp.MusicController.StopSound();
            return;
        }

        srp.MusicController.LoadSound(name);
    }
}