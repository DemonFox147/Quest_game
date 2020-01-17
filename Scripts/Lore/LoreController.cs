using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class LoreController : MonoBehaviour
{
    public Transform InfoList;

    public Image InfoImage;

    public Text InfoText;

    private Text OldTextLoreObject;

    public Scrollbar ScrollbarTextBlock;

    public Dictionary<string, Dictionary<string, XmlNode>> LoreDictionaryXML;

    public Dictionary<string, List<string>> CharactersDictionatyActive;
    public Dictionary<string, List<string>> CreaturesDictionatyActive;
    public Dictionary<string, List<string>> WorldDictionatyActive;

    private Dictionary<string, Dictionary<string, List<string>>> categoryDictionaryActive;

    void Start()
    {
        LoreDictionaryXML = Singleton<ToolBox>.Instance.LoreOpen.LoreDictionaryXML;
        CharactersDictionatyActive = Singleton<ToolBox>.Instance.LoreOpen.CharactersDictionatyActive;
        CreaturesDictionatyActive = Singleton<ToolBox>.Instance.LoreOpen.CreaturesDictionatyActive;
        WorldDictionatyActive = Singleton<ToolBox>.Instance.LoreOpen.WorldDictionatyActive;

        categoryDictionaryActive = new Dictionary<string, Dictionary<string, List<string>>>
        {
            { "Characters", CharactersDictionatyActive },
            { "Creatures", CreaturesDictionatyActive },
            { "World", WorldDictionatyActive }
        };

        string[] categoryKeys = LoreDictionaryXML.Select(kv => kv.Key).ToArray();

        for (int i = 0; i < categoryKeys.Length; i++)
        {
            GameObject categoryPrefab = Resources.Load<GameObject>("LorePrefabs/Category");

            if (categoryPrefab != null)
            {
                GameObject categoryGO = Instantiate(categoryPrefab, InfoList);

                if (categoryKeys[i] == "Characters")
                {
                    if (Singleton<ToolBox>.Instance.GameLanguage == "Rus")
                    {
                        categoryGO.GetComponent<Text>().text = "Персонажи";
                    }
                    else
                    {
                        categoryGO.GetComponent<Text>().text = "Characters";
                    }
                }
                else if (categoryKeys[i] == "Creatures")
                {
                    if (Singleton<ToolBox>.Instance.GameLanguage == "Rus")
                    {
                        categoryGO.GetComponent<Text>().text = "Существа";
                    }
                    else
                    {
                        categoryGO.GetComponent<Text>().text = "Creatures";
                    }
                }
                else if(categoryKeys[i] == "World")
                {
                    if (Singleton<ToolBox>.Instance.GameLanguage == "Rus")
                    {
                        categoryGO.GetComponent<Text>().text = "Мир";
                    }
                    else
                    {
                        categoryGO.GetComponent<Text>().text = "World";
                    }
                }

                categoryGO.name = categoryKeys[i];
            }

            string[] infoKeys = LoreDictionaryXML[categoryKeys[i]].Select(kv => kv.Key).ToArray();

            string[] loreNewInfo = PlayerPrefs.GetString("saveBW_LoreNewInfo").Split(',');

            for (int j = 0; j < infoKeys.Length; j++)
            {
                GameObject infoPrefab = Resources.Load<GameObject>("LorePrefabs/InfoObject");
                if (infoPrefab != null)
                {
                    GameObject categoryGO = Instantiate(infoPrefab, InfoList);
                    categoryGO.name = infoKeys[j];

                    if (!categoryDictionaryActive[categoryKeys[i]][infoKeys[j]].Contains<string>("-1"))
                    {
                        if (loreNewInfo.Contains(infoKeys[j]))
                        {
                            categoryGO.GetComponent<Text>().text = LoreDictionaryXML[categoryKeys[i]][infoKeys[j]].Attributes["name"].Value + " !";
                            categoryGO.GetComponent<Text>().color = new Color(0.7921569f, 0.3686275f, 0.2745098f);
                        }
                        else
                        {
                            categoryGO.GetComponent<Text>().text = LoreDictionaryXML[categoryKeys[i]][infoKeys[j]].Attributes["name"].Value;
                        }

                        categoryGO.GetComponent<LoreInfoButton>().LoreController = this;
                        categoryGO.GetComponent<LoreInfoButton>().LoreObject = infoKeys[j];
                        categoryGO.GetComponent<LoreInfoButton>().LoreCategory = categoryKeys[i];
                    }
                    else
                    {
                        categoryGO.GetComponent<Text>().text = "???";
                        categoryGO.GetComponent<LoreInfoButton>().LoreController = this;
                        categoryGO.GetComponent<LoreInfoButton>().LoreObject = "???";
                        categoryGO.GetComponent<LoreInfoButton>().LoreCategory = "???";
                    }
                }
            }
        }

        if (InfoList.childCount > 9)
        {
            int childCount = InfoList.childCount - 9;

            RectTransform rectTransform = InfoList.GetComponent<RectTransform>();

            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, -75 * childCount);
        }
    }

    public void ShowInformationAboutObject(string name, string category, Text loreTextObject)
    {
        if (name == "???") { return; }

        List<string> OpenInfo = categoryDictionaryActive[category][name];

        InfoImage.sprite = Resources.Load<Sprite>("Images/Lore/" + category + "/" + name);

        string infoText = string.Empty;

        for (int i = 0; i < OpenInfo.Count; i++)
        {
            if (loreTextObject.text.Contains(" !") && i == OpenInfo.Count - 1)
            {
                infoText += "<color=#FCD001>" + LoreDictionaryXML[category][name].ChildNodes.Item(int.Parse(OpenInfo[i])).InnerText + "</color>\n";
            }
            else
            {
                infoText += LoreDictionaryXML[category][name].ChildNodes.Item(int.Parse(OpenInfo[i])).InnerText + "\n";
            }
        }

        InfoText.text = infoText;

        if (OldTextLoreObject == null)
        {
            OldTextLoreObject = loreTextObject;

            loreTextObject.color = new Color(0.7921569f, 0.3686275f, 0.2745098f);
        }
        else
        {
            OldTextLoreObject.color = new Color(1f, 1f, 1f);

            OldTextLoreObject = loreTextObject;

            loreTextObject.color = new Color(0.7921569f, 0.3686275f, 0.2745098f);
        }

        if (loreTextObject.text.Contains(" !"))
        {
            loreTextObject.text = loreTextObject.text.Trim(new char[] { ' ', '!' });

            List<string> loreNewInfo = PlayerPrefs.GetString("saveBW_LoreNewInfo").Split(',').ToList();

            loreNewInfo.Remove(name);

            PlayerPrefs.SetString("saveBW_LoreNewInfo", string.Join(",", loreNewInfo.ToArray()));
        }

        StartCoroutine(IEActivedScrollbarTextBlock());
    }

    private IEnumerator IEActivedScrollbarTextBlock()
    {
        yield return new WaitForSeconds(0.02f);
        ScrollbarTextBlock.value = 1f;
    }

    public void ResetInformationAboutObject()
    {
        InfoImage.sprite = Resources.Load<Sprite>("Images/Lore/Zero");

        InfoText.text = string.Empty;
    }
}