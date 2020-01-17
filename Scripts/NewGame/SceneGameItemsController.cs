using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class SceneGameItemsController : MonoBehaviour
{
    [HideInInspector]
    public ScriptReferencesPool srp;

    private bool tipsActive;
    
    public Inventory Inventory
    {
        get;
        private set;
    }

    public GameObject[] GameItemsOnScene;

    public GameObject[] Tips;

    public Dictionary<string, XmlNode> DicGameItemsOnScenesByDays;

    void Start()
    {
        tipsActive = false;

        Inventory = FindObjectOfType<Inventory>();

        if (Singleton<GameState>.Instance.gameItemsOnScenesDict.Count != 0)
        {
            DicGameItemsOnScenesByDays = 
                Singleton<GameState>.Instance.gameItemsOnScenesDict.ToDictionary(kv => kv.Key, kv => kv.Value.Clone());
        }
        else
        {
            DicGameItemsOnScenesByDays = 
                Singleton<ToolBox>.Instance.GameItemsOnScenesByDaysXML[Singleton<GameState>.Instance.currentDay].ToDictionary(kv => kv.Key, kv => kv.Value.Clone());

            Singleton<GameState>.Instance.gameItemsOnScenesDict =
                Singleton<ToolBox>.Instance.GameItemsOnScenesByDaysXML[Singleton<GameState>.Instance.currentDay].ToDictionary(kv => kv.Key, kv => kv.Value.Clone());
        }

        Transform gameItemsByScene = transform.GetChild(0);
        GameItemsOnScene = new GameObject[gameItemsByScene.childCount];

        for (int i = 0; i < GameItemsOnScene.Length; i++)
        {
            GameItemsOnScene[i] = gameItemsByScene.GetChild(i).gameObject;
            GameItemsOnScene[i].GetComponent<SceneGameItem>().srp = srp;
        }

        Tips = new GameObject[GameItemsOnScene.Length];

        for (int i = 0; i < Tips.Length; i++)
        {
            Tips[i] = GameItemsOnScene[i].transform.GetChild(0).gameObject;
        }

        if (Singleton<GameState>.Instance.gameItemsMode == "True")
        {
            srp.ScenarioParser.GameItemsMode = true;
            AddGameItemsOnScene();
            ReloadGameItemsOnScene(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !tipsActive && srp.ScenarioParser.TravelMode)
        {
            tipsActive = true;

            for (int i = 0; i < Tips.Length; i++)
            {
                if (GameItemsOnScene[i].GetComponent<Button>().enabled)
                {
                    Tips[i].SetActive(true);
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.T))
        {
            tipsActive = false;

            for (int i = 0; i < Tips.Length; i++)
            {
                Tips[i].SetActive(false);
            }
        }
        else if (tipsActive && !srp.ScenarioParser.TravelMode)
        {
            tipsActive = false;

            for (int i = 0; i < Tips.Length; i++)
            {
                Tips[i].SetActive(false);
            }
        }
    }

    public void IsGameItemsOnScene(bool state)
    {
        if (state)
        {
            srp.SceneGameItemsController.AddGameItemsOnScene();
        }
        else
        {
            srp.SceneGameItemsController.RemoveGameItemsOnScene();
        }
    }

    public void AddGameItemsOnScene()
    {
        srp.Backgrounds.BGChange += ReloadGameItemsOnScene;
    }

    public void RemoveGameItemsOnScene()
    {
        srp.Backgrounds.BGChange -= ReloadGameItemsOnScene;

        foreach (GameObject gameItem in GameItemsOnScene)
        {
            gameItem.SetActive(false);
        }
    }

    public void ReloadGameItemsOnScene(bool modeLoad)
    {
        if (modeLoad)
        {
            StartCoroutine(IEReloadGameItems(false));
        }

        string scene = Singleton<GameState>.Instance.nameBackground;

        foreach (GameObject gameItem in GameItemsOnScene)
        {
            gameItem.SetActive(false);
        }

        if (!DicGameItemsOnScenesByDays.ContainsKey(scene))
        {
            Debug.Log("Комната " + scene + " для изменения не найдена");
            return;
        }

        XmlNode xScene = DicGameItemsOnScenesByDays[scene].Clone();

        int gameItemNum = 0;

        foreach (XmlNode xGameItemByScene in xScene)
        {
            GameItemsOnScene[gameItemNum].SetActive(true);

            SceneGameItem gameItem = GameItemsOnScene[gameItemNum].GetComponent<SceneGameItem>();
            gameItem.ResetValues();

            foreach (XmlNode xParamGameItem in xGameItemByScene)
            {
                switch (xParamGameItem.Name)
                {
                    case "Name":
                        {
                            GameItemsOnScene[gameItemNum].GetComponent<Image>().sprite =
                                Resources.Load<Sprite>("Images/ObjectsBG/" + scene + "/" + xParamGameItem.InnerText);
                            break;
                        }
                    case "State":
                        {
                            bool interaction = bool.Parse(xParamGameItem.Attributes["interaction"].Value);
                            bool visibility = bool.Parse(xParamGameItem.Attributes["visibility"].Value);

                            GameItemsOnScene[gameItemNum].GetComponent<Button>().enabled = interaction;
                            GameItemsOnScene[gameItemNum].SetActive(visibility);
                            break;
                        }
                    case "Anchours":
                        {
                            float xMin = float.Parse(xParamGameItem.Attributes["xMin"].Value);
                            float xMax = float.Parse(xParamGameItem.Attributes["xMax"].Value);
                            float yMin = float.Parse(xParamGameItem.Attributes["yMin"].Value);
                            float yMax = float.Parse(xParamGameItem.Attributes["yMax"].Value);

                            GameItemsOnScene[gameItemNum].GetComponent<RectTransform>().anchorMin = new Vector2(xMin, yMin);
                            GameItemsOnScene[gameItemNum].GetComponent<RectTransform>().anchorMax = new Vector2(xMax, yMax);
                            break;
                        }
                    case "Chapter":
                        {
                            string chapter = xParamGameItem.Attributes["name"].Value;
                            gameItem.Chapter = chapter;
                            break;
                        }
                    case "AltChapter":
                        {
                            string altChapter = xParamGameItem.Attributes["name"].Value;
                            gameItem.AltChapter = altChapter;
                            break;
                        }
                    case "ItemInterection":
                        {
                            string itemInterection = xParamGameItem.Attributes["name"].Value;
                            gameItem.ItemInterection = itemInterection;
                            break;
                        }
                    case "ItemIChapter":
                        {
                            string itemIChapter = xParamGameItem.Attributes["name"].Value;
                            gameItem.ItemIChapter = itemIChapter;
                            break;
                        }
                    default:
                        {
                            Debug.Log("Не верное имя параметра кнопки события: " + xParamGameItem.Name);
                            break;
                        }
                }
            }
            gameItemNum++;
        }

        if (modeLoad)
        {
            StartCoroutine(IEReloadGameItems(true));
        }
    }

    private IEnumerator IEReloadGameItems(bool reverse)
    {
        CanvasGroup gi = transform.GetChild(0).GetComponent<CanvasGroup>();

        if (!reverse)
        {
            for (float f = 1f; f > 0f; f -= 0.05f)
            {
                gi.alpha = f;
                yield return new WaitForFixedUpdate();
            }

            gi.alpha = 0f;
        }
        else
        {
            for (float f = 0f; f < 1f; f += 0.05f)
            {
                gi.alpha = f;
                yield return new WaitForFixedUpdate();
            }

            gi.alpha = 1f;
        }
    }

    public void EditGameItemOnScene(string scene, string item, string interaction, string visibility)
    {
        if (!DicGameItemsOnScenesByDays.ContainsKey(scene))
        {
            Debug.Log("Комната " + scene + " с предметом для изменения не найдена");
            return;
        }

        XmlElement xScene = (XmlElement)DicGameItemsOnScenesByDays[scene];

        foreach (XmlElement xGameItemByScene in xScene)
        {
            if (xGameItemByScene.ChildNodes.Item(0).InnerText == item)
            {
                Debug.Log("Предмет " + item + " найден.");

                XmlElement xGameItemOld = xGameItemByScene;
                XmlElement xGameItemNew = xGameItemOld;
                XmlElement xStateOld = (XmlElement)xGameItemNew.ChildNodes.Item(1);
                XmlElement xStateNew = xStateOld;

                xStateNew.SetAttribute("interaction", interaction);
                xStateNew.SetAttribute("visibility", visibility);

                xGameItemNew.ReplaceChild(xStateNew, xStateOld);

                xScene.ReplaceChild(xGameItemNew, xGameItemOld);

                break;
            }
        }

        DicGameItemsOnScenesByDays[scene] = xScene;

        Singleton<GameState>.Instance.gameItemsOnScenesDict = DicGameItemsOnScenesByDays.ToDictionary(kv => kv.Key, kv => kv.Value.Clone());

        if (Singleton<GameState>.Instance.nameBackground == scene)
        {   
            ReloadGameItemsOnScene(false);
            Debug.Log("Комната " + scene + " перезагружена.");
        }
    }

    private bool CheckOnExcludedItem(string item, string[] excludedItems)
    {
        foreach(string exc in excludedItems)
        {
            if (exc == item) { return true; }
        }

        return false;
    }
}