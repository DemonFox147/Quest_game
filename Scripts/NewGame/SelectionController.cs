using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class SelectionController : MonoBehaviour
{
    [HideInInspector]
    public ScriptReferencesPool srp;

    private XmlNode[] NamesSelection;

    private GameObject[] GOSelections;

    void Awake()
    {
        GOSelections = new GameObject[transform.childCount];

        int x = 0;
        foreach (Transform child in transform)
        {
            GOSelections[x++] = child.gameObject;
        }
    }

    public void Selection(XmlNode[] namesSelection)
    {
        NamesSelection = namesSelection;

        List<XmlNode> listNameSel = new List<XmlNode>();
        for (int i = 0; i < namesSelection.Length; i++)
        {
            listNameSel.Add(namesSelection[i]);
        }
        namesSelection = listNameSel.ToArray();

        GOSelections[namesSelection.Length - 2].SetActive(true);

        for (int i = 0; i < namesSelection.Length; i++)
        {
            string nameChapter = namesSelection[i].Attributes["chapter"].Value;
            string nameSelection = namesSelection[i].Attributes["nameSelection"].Value;
            GOSelections[namesSelection.Length - 2].transform.GetChild(i).GetComponent<SelectionButton>().Chapter = nameChapter;
            GOSelections[namesSelection.Length - 2].transform.GetChild(i).GetComponent<Image>().sprite =
                Resources.Load<Sprite>("Images/Interface/Choices/" + Singleton<ToolBox>.Instance.GameLanguage + "/" + nameSelection);
        }

        Singleton<GameState>.Instance.choiceMode = "True";

        string[] choices = new string[namesSelection.Length];
        for (int i = 0; i < namesSelection.Length; i++)
        {
            choices[i] = namesSelection[i].OuterXml;
        }

        NamesSelection = namesSelection;
    }

    public void FindNextChapter(string nameChapter)
    {
        Singleton<GameState>.Instance.choiceMode = "False";

        for (int i = 0; i < GOSelections.Length; i++)
        {
            GOSelections[i].SetActive(false);
        }

        for (int i = 0; i < NamesSelection.Length; i++)
        {
            if (NamesSelection[i].Attributes.GetNamedItem("chapter").Value == nameChapter)
            {
                srp.ScenarioParser.GoNextChapter(nameChapter, true);
                break;
            }
        }
    }
}