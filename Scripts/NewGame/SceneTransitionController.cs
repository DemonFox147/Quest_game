using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class SceneTransitionController : MonoBehaviour
{
    [HideInInspector]
    public ScriptReferencesPool srp;

    public GameObject ProtectiveLayer;

    public GameObject OpenInventory;
    public GameObject OpenTaskList;
    public GameObject OpenAmulet;

    private GameObject[] arrowsGO;

    public List<string> deactivateArrows;

    public List<string> rewriteChapterArrow;

    private Dictionary<string, XmlNode> transitionRoomsDict;

    void Start()
    {
        deactivateArrows = Singleton<GameState>.Instance.deactivateArrows;

        rewriteChapterArrow = Singleton<GameState>.Instance.rewriteChapterArrow;
        
        transitionRoomsDict = Singleton<ToolBox>.Instance.TransitionRoomsXML;

        Transform arrows = transform.GetChild(0);
        arrowsGO = new GameObject[arrows.childCount];

        for (int i = 0; i < arrowsGO.Length; i++)
        {
            arrowsGO[i] = arrows.GetChild(i).gameObject;
            arrowsGO[i].transform.GetChild(0).GetComponent<SceneTransitionButton>().STC = this;
        }
    }

    public void StartTransitionMode()
    {
        ReloadArrows();
    }

    private void ReloadArrows()
    {
        string room = Singleton<GameState>.Instance.nameBackground;

        XmlNode transitionArrows = transitionRoomsDict[room].ChildNodes.Item(0);

        StopTransitionMode();

        foreach (XmlNode arrow in transitionArrows)
        {
            switch (arrow.Name)
            {
                case "LeftArrow":
                    {
                        arrowsGO[0].SetActive(true);
                        arrowsGO[0].transform.GetChild(0).GetComponent<SceneTransitionButton>().NextRoom = arrow.Attributes["name"].Value;
                        arrowsGO[0].transform.GetChild(0).GetComponent<SceneTransitionButton>().ArrowDirection = arrow.Name;
                        float xMin = float.Parse(arrow.Attributes["xMin"].Value);
                        float xMax = float.Parse(arrow.Attributes["xMax"].Value);
                        float yMin = float.Parse(arrow.Attributes["yMin"].Value);
                        float yMax = float.Parse(arrow.Attributes["yMax"].Value);
                        arrowsGO[0].GetComponent<RectTransform>().anchorMin = new Vector2(xMin, yMin);
                        arrowsGO[0].GetComponent<RectTransform>().anchorMax = new Vector2(xMax, yMax);
                        break;
                    }
                case "RightArrow":
                    {
                        arrowsGO[1].SetActive(true);
                        arrowsGO[1].transform.GetChild(0).GetComponent<SceneTransitionButton>().NextRoom = arrow.Attributes["name"].Value;
                        arrowsGO[1].transform.GetChild(0).GetComponent<SceneTransitionButton>().ArrowDirection = arrow.Name;
                        float xMin = float.Parse(arrow.Attributes["xMin"].Value);
                        float xMax = float.Parse(arrow.Attributes["xMax"].Value);
                        float yMin = float.Parse(arrow.Attributes["yMin"].Value);
                        float yMax = float.Parse(arrow.Attributes["yMax"].Value);
                        arrowsGO[1].GetComponent<RectTransform>().anchorMin = new Vector2(xMin, yMin);
                        arrowsGO[1].GetComponent<RectTransform>().anchorMax = new Vector2(xMax, yMax);
                        break;
                    }
                case "UpArrow":
                    {
                        arrowsGO[2].SetActive(true);
                        arrowsGO[2].transform.GetChild(0).GetComponent<SceneTransitionButton>().NextRoom = arrow.Attributes["name"].Value;
                        arrowsGO[2].transform.GetChild(0).GetComponent<SceneTransitionButton>().ArrowDirection = arrow.Name;
                        float xMin = float.Parse(arrow.Attributes["xMin"].Value);
                        float xMax = float.Parse(arrow.Attributes["xMax"].Value);
                        float yMin = float.Parse(arrow.Attributes["yMin"].Value);
                        float yMax = float.Parse(arrow.Attributes["yMax"].Value);
                        arrowsGO[2].GetComponent<RectTransform>().anchorMin = new Vector2(xMin, yMin);
                        arrowsGO[2].GetComponent<RectTransform>().anchorMax = new Vector2(xMax, yMax);
                        break;
                    }
                case "DownArrow":
                    {
                        arrowsGO[3].SetActive(true);
                        arrowsGO[3].transform.GetChild(0).GetComponent<SceneTransitionButton>().NextRoom = arrow.Attributes["name"].Value;
                        arrowsGO[3].transform.GetChild(0).GetComponent<SceneTransitionButton>().ArrowDirection = arrow.Name;
                        float xMin = float.Parse(arrow.Attributes["xMin"].Value);
                        float xMax = float.Parse(arrow.Attributes["xMax"].Value);
                        float yMin = float.Parse(arrow.Attributes["yMin"].Value);
                        float yMax = float.Parse(arrow.Attributes["yMax"].Value);
                        arrowsGO[3].GetComponent<RectTransform>().anchorMin = new Vector2(xMin, yMin);
                        arrowsGO[3].GetComponent<RectTransform>().anchorMax = new Vector2(xMax, yMax);
                        break;
                    }
                default:
                    {
                        Debug.Log("<color=red>Не верно указанное название параметра " + arrow.Name + "</color>");
                        break;
                    }
            }
        }

        if (deactivateArrows.Count != 0)
        {
            for (int i = 0; i < deactivateArrows.Count; i++)
            {
                switch (deactivateArrows[i])
                {
                    case "LeftArrow":
                        {
                            arrowsGO[0].SetActive(false);
                            break;
                        }
                    case "RightArrow":
                        {
                            arrowsGO[1].SetActive(false);
                            break;
                        }
                    case "UpArrow":
                        {
                            arrowsGO[2].SetActive(false);
                            break;
                        }
                    case "DownArrow":
                        {
                            arrowsGO[3].SetActive(false);
                            break;
                        }
                    default:
                        {
                            Debug.Log("<color=red>Не верно указанное название стрелки " + deactivateArrows[i] + "</color>");
                            break;
                        }
                }
            }
        }

        ProtectiveLayer.SetActive(false);

        OpenTaskList.SetActive(true);
        OpenInventory.SetActive(true);

        if (Singleton<GameState>.Instance.openAmulet)
        {
            OpenAmulet.SetActive(true);
        }
    }

    public void NextScene(string nextRoom, string arrowDirection)
    {
        string arrowDirection_CurrentBG = arrowDirection + "|" + Singleton<GameState>.Instance.nameBackground;
        string temp = rewriteChapterArrow.Find(x => x.Contains(arrowDirection_CurrentBG));

        if (temp != null)
        {
            string[] info = temp.Split('|');
            string chapter = info[2];
            srp.ScenarioParser.TravelMode = false;
            Singleton<GameState>.Instance.travelMode = "False";
            srp.SceneTransitionController.StopTransitionMode();
            srp.ScenarioParser.InterfacePanelSetActive(true);
            srp.ScenarioParser.GoNextChapter(chapter, true);

            return;
        }

        srp.Backgrounds.LoadBG(nextRoom);
        Singleton<ToolBox>.Instance.GalleryOpen.OpenBG(nextRoom);
        ReloadArrows();

        List<string> eventEntranceToScene = Singleton<GameState>.Instance.eventEntranceOnScene;

        if (eventEntranceToScene.Count != 0)
        {
            foreach (string sceneEvent in eventEntranceToScene)
            {
                if (Singleton<ToolBox>.Instance.IsContainsKeyInTextLanguage(sceneEvent + "_" + nextRoom))
                {
                    if (!srp.ScenarioParser.IsChapterRead(sceneEvent + "_" + nextRoom))
                    {
                        srp.ScenarioParser.TravelMode = false;
                        Singleton<GameState>.Instance.travelMode = "False";
                        srp.SceneTransitionController.StopTransitionMode();
                        srp.ScenarioParser.InterfacePanelSetActive(true);
                        srp.ScenarioParser.GoNextChapter(sceneEvent + "_" + nextRoom, true);
                        break;
                    }
                }
            }
        }
    }

    public void StopTransitionMode()
    {
        for (int i = 0; i < arrowsGO.Length; i++)
        {
            arrowsGO[i].SetActive(false);
        }

        ProtectiveLayer.SetActive(true);

        OpenTaskList.SetActive(false);
        OpenInventory.SetActive(false);
        OpenAmulet.SetActive(false);
    }

    public void DeactivateArrows(string listArrows)
    {
        if (listArrows == "Zero")
        {
            deactivateArrows = new List<string>();
        }
        else
        {
            deactivateArrows = listArrows.Split(',').ToList();
        }

        Singleton<GameState>.Instance.deactivateArrows = deactivateArrows;
    }

    public void ReWriteChapterArrow(string arrow, string bg, string chapter)
    {
        if (chapter == "Zero")
        {
            string arrow_BG = arrow + "|" + bg;
            rewriteChapterArrow.Remove(rewriteChapterArrow.Find(x => x.Contains(arrow_BG)));
        }
        else
        {
            rewriteChapterArrow.Add(arrow + "|" + bg + "|" + chapter);
        }

        Singleton<GameState>.Instance.rewriteChapterArrow = rewriteChapterArrow;
    }
}