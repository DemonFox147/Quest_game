using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steamworks;

public class ScenarioParser : MonoBehaviour
{
    [HideInInspector]
    public ScriptReferencesPool srp;

    private ScreenFader sf;
    private InspectorXML inspectorXML;

    public GameObject OpenWoodenMiniMenu;
    public GameObject WoodenMiniMenu;
    public GameObject DialogPanel;
    public GameObject NameHero;
    public GameObject History;
    public GameObject ExitMenu;

    public Text DialogText;
    public Text HistoryText;

    private Dictionary<string, XmlNode> langScenarioXML;

    private XmlNodeList currentXmlText;

    private List<string> oldText;

    private string currentName;
    private string lastPlaceWithText;
    private string[] currentBlockLines;

    private int currentLineNum;
    private int currentTagNum;
    private int endAtLine;

    private float textSpeed;
    private float autoHopTime;
    private float autoTimeRead;

    private bool isTyping = true;
    private bool cancelTyping = false;
    private bool rewind = false;
    private bool nameState;
    private bool textContinue;
    private bool autoReadText;
    private bool autoReadingMode;

    public bool ExitScene
    {
        get;
        private set;
    }

    public bool DeathMode
    {
        get;
        private set;
    }

    public bool GameEnd
    {
        get;
        private set;
    }

    public bool ChoiceMode
    {
        get;
        private set;
    }

    public bool TravelMode
    {
        get;
        set;
    }

    public bool GameItemsMode
    {
        get;
        set;
    }

    public bool AnimMode
    {
        get;
        set;
    }

    public bool MiniGameMode
    {
        get;
        set;
    }

    void Awake()
    {
        sf = GetComponent<ScreenFader>();
        inspectorXML = GetComponent<InspectorXML>();

        DialogText.fontSize = PlayerPrefs.GetInt("fontSize");
        textSpeed = 0.1f - PlayerPrefs.GetFloat("textSpeed");
        autoHopTime = PlayerPrefs.GetFloat("autoHopTime");

        ExitScene = false;
        UIClickBlocker.allowClick = true;
    }

    void Start()
    {
        DialogPanelChange(Singleton<GameState>.Instance.dialogPanel);

        langScenarioXML = Singleton<ToolBox>.Instance.LangScenarioXML[Singleton<GameState>.Instance.currentDay];

        if (PlayerPrefs.GetString("language") == Singleton<ToolBox>.Instance.GameLanguage)
        {
            PlayerPrefs.SetString("language", Singleton<ToolBox>.Instance.GameLanguage);
            oldText = Singleton<GameState>.Instance.oldText;
        }
        else
        {
            PlayerPrefs.SetString("language", Singleton<ToolBox>.Instance.GameLanguage);
            oldText = new List<string>();
        }

        GoNextChapter(Singleton<GameState>.Instance.nameChapter, false);

        currentTagNum = Singleton<GameState>.Instance.currentTagNum;
        currentLineNum = Singleton<GameState>.Instance.currentLineNum;

        if (Singleton<GameState>.Instance.choiceMode == "True")
        {
            ChoiceMode = true;

            lastPlaceWithText = Singleton<GameState>.Instance.lastPlaceWithText;
            string[] array = Singleton<GameState>.Instance.lastPlaceWithText.Split('^');

            currentXmlText = langScenarioXML[array[0]].ChildNodes;
            currentTagNum = int.Parse(array[1]);
            currentLineNum = int.Parse(array[2]);

            currentBlockLines = currentXmlText.Item(currentTagNum).InnerText.Split('\n');
            DialogText.text = currentBlockLines[currentLineNum];

            currentTagNum++;
            Inspection();
        }
        else
        {
            if (Singleton<GameState>.Instance.travelMode == "False")
            {
                TravelMode = false;

                Inspection();
                currentBlockLines = currentXmlText.Item(currentTagNum).InnerText.Split('\n');

                StartCoroutine(TextScroll(currentBlockLines[currentLineNum]));

                currentLineNum++;

                if (currentLineNum < currentBlockLines.Length)
                {
                    textContinue = true;
                }
                else
                {
                    textContinue = false;
                }
            }
            else
            {
                TravelMode = true;
                Singleton<GameState>.Instance.travelMode = "True";
                srp.SceneTransitionController.StartTransitionMode();
                InterfacePanelSetActive(false);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !ExitMenu.activeSelf && !DeathMode && !GameEnd)
        {
            ExitMenu.SetActive(true);
            ExitScene = true;
            Time.timeScale = 1f;
        }

        if (Input.GetKeyUp(KeyCode.Space) && !ExitScene && !ChoiceMode
            && !TravelMode && !DeathMode && !GameEnd && !MiniGameMode)
        {
            cancelTyping = true;
            autoReadingMode = false;
            InterfacePanelSetActive(!DialogPanel.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Escape) || Input.touchCount > 0)
        {
            autoReadingMode = false;
        }

        if (Time.timeScale != 1)
        {
            rewind = true;
            autoReadingMode = false;
            srp.MusicController.TimeScrolling(rewind);
        }
        else
        {
            rewind = false;
            srp.MusicController.TimeScrolling(rewind);
        }

        if (( (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && UIClickBlocker.allowClick && !ExitScene
            && !AnimMode && !DeathMode && !ChoiceMode && !TravelMode && !MiniGameMode)
            || (rewind && !AnimMode) || autoReadText)
        {
            autoReadText = false;

            if (DialogPanel.activeSelf)
            {
                if (!isTyping && currentTagNum != endAtLine)
                {
                    if (!textContinue)
                    {
                        currentTagNum++;
                        Inspection();
                    }

                    if (!ChoiceMode && !TravelMode)
                    {
                        if (!textContinue)
                        {
                            currentLineNum = 0;
                            currentBlockLines = currentXmlText.Item(currentTagNum).InnerText.Split('\n');
                        }

                        Singleton<GameState>.Instance.currentLineNum = currentLineNum;
                        Singleton<GameState>.Instance.currentTagNum = currentTagNum;

                        StartCoroutine(TextScroll(currentBlockLines[currentLineNum]));
                        autoTimeRead = currentBlockLines[currentLineNum].Length / 10.0f;
                        currentLineNum++;

                        if (currentLineNum < currentBlockLines.Length)
                        {
                            textContinue = true;
                        }
                        else
                        {
                            lastPlaceWithText = Singleton<GameState>.Instance.nameChapter + "^" +
                                Singleton<GameState>.Instance.currentTagNum + "^" +
                                Singleton<GameState>.Instance.currentLineNum;

                            Singleton<GameState>.Instance.lastPlaceWithText = lastPlaceWithText;
                            textContinue = false;
                        }
                    }
                }
                else if (isTyping && !cancelTyping)
                {
                    cancelTyping = true;
                }
            }
            else if (!TravelMode)
            {
                InterfacePanelSetActive(true);
            }
        }
    }

    private void Inspection()
    {
        if (currentXmlText.Item(currentTagNum).Name == "name")
        {
            nameState = true;

            currentName = currentXmlText.Item(currentTagNum).InnerText;
            srp.NamesHeroes.CurSpriteNameHero(currentName);
            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "char")
        {
            inspectorXML.CharapterInspector(currentXmlText.Item(currentTagNum));
            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "bg")
        {
            inspectorXML.BGInspector(currentXmlText.Item(currentTagNum));
            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "fbg")
        {
            string fbg = currentXmlText.Item(currentTagNum).Attributes["name"].Value;
            srp.Backgrounds.ApplyFBG(fbg);
            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "music")
        {
            string name = currentXmlText.Item(currentTagNum).Attributes["name"].Value;
            srp.MusicController.LoadMusic(name);
            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "sound")
        {
            inspectorXML.SoundInspector(currentXmlText.Item(currentTagNum));
            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "choices")
        {
            ChoiceMode = true;

            int count = currentXmlText.Item(currentTagNum).ChildNodes.Count;

            XmlNode[] choices = new XmlNode[count];

            for (int i = 0; i < count; i++)
            {
                choices[i] = currentXmlText.Item(currentTagNum).ChildNodes.Item(i);
            }
            srp.SelectionController.Selection(choices);
        }
        else if (currentXmlText.Item(currentTagNum).Name == "chapter")
        {
            string name = currentXmlText.Item(currentTagNum).Attributes["name"].Value;
            TravelMode = false;
            Singleton<GameState>.Instance.travelMode = "False";
            GoNextChapter(name, false);
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "ReadChapter")
        {
            string name = currentXmlText.Item(currentTagNum).Attributes["name"].Value;
            IsChapterRead(name);
            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "day")
        {
            string day = currentXmlText.Item(currentTagNum).Attributes["name"].Value;
            NextDay(day);
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "animation")
        {
            inspectorXML.AnimationInspector(currentXmlText.Item(currentTagNum));
            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "IsItemInInventory")
        {
            string[] namesItems = currentXmlText.Item(currentTagNum).Attributes["namesItems"].Value.Split(',');
            string chapter = currentXmlText.Item(currentTagNum).Attributes["chapter"].Value;
            string altChapter = currentXmlText.Item(currentTagNum).Attributes["altChapter"].Value;

            bool result = srp.Inventory.IsItemsInInventory(namesItems);

            if (currentXmlText.Item(currentTagNum).Attributes["mode"] != null)
            {
                result = currentXmlText.Item(currentTagNum).Attributes["mode"].Value == "reverse" ? !result : result;
            }

            if (result)
            {
                GoNextChapter(chapter, false);
            }
            else
            {
                if (altChapter != string.Empty)
                {
                    GoNextChapter(altChapter, false);
                }
                else
                {
                    currentTagNum++;
                }
            }

            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "AddItem")
        {
            string item = currentXmlText.Item(currentTagNum).Attributes["name"].Value;
            bool combinedItem = false;
            string removeUseItem = "";
            int taskComplete = -1;

            if (currentXmlText.Item(currentTagNum).Attributes["combinedItem"] != null)
            {
                combinedItem = bool.Parse(currentXmlText.Item(currentTagNum).Attributes["combinedItem"].Value);
                removeUseItem = currentXmlText.Item(currentTagNum).Attributes["removeUseItem"].Value;
                taskComplete = int.Parse(currentXmlText.Item(currentTagNum).Attributes["taskComplete"].Value);
            }

            srp.Inventory.AddItemToInventory(item, combinedItem, removeUseItem, taskComplete);

            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "RemoveItem")
        {
            string item = currentXmlText.Item(currentTagNum).Attributes["name"].Value;

            if (item == "All")
            {
                srp.Inventory.RemoveAllItemToInventory();
            }
            else
            {
                srp.Inventory.RemoveItemToInventory(item);
            }

            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "TravelMode")
        {
            TravelMode = true;
            Singleton<GameState>.Instance.travelMode = "True";
            srp.SceneTransitionController.StartTransitionMode();
            InterfacePanelSetActive(false);
        }
        else if (currentXmlText.Item(currentTagNum).Name == "ModeGameItemsOnScene")
        {
            string state = currentXmlText.Item(currentTagNum).Attributes["state"].Value;
            GameItemsMode = bool.Parse(state);

            if (GameItemsMode)
            {
                srp.SceneGameItemsController.AddGameItemsOnScene();
            }
            else
            {
                srp.SceneGameItemsController.RemoveGameItemsOnScene();
            }

            Singleton<GameState>.Instance.gameItemsMode = GameItemsMode.ToString();

            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "EditGameItemOnScene")
        {
            string scene = currentXmlText.Item(currentTagNum).Attributes["scene"].Value;
            string item = currentXmlText.Item(currentTagNum).Attributes["item"].Value;
            string interaction = currentXmlText.Item(currentTagNum).Attributes["interaction"].Value;
            string visibility = currentXmlText.Item(currentTagNum).Attributes["visibility"].Value;

            srp.SceneGameItemsController.EditGameItemOnScene(scene, item, interaction, visibility);

            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "TaskComplite")
        {
            int numTask = int.Parse(currentXmlText.Item(currentTagNum).Attributes["num"].Value);
            srp.TaskController.TaskComplete(numTask);
            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "EventEntranceOnScene")
        {
            string eventEntranceOnScene = currentXmlText.Item(currentTagNum).Attributes["name"].Value;
            string command = currentXmlText.Item(currentTagNum).Attributes["command"].Value;

            if (command == "Add")
            {
                Singleton<GameState>.Instance.eventEntranceOnScene.Add(eventEntranceOnScene);
            }
            else if (command == "Remove")
            {
                Singleton<GameState>.Instance.eventEntranceOnScene.Remove(eventEntranceOnScene);
            }

            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "RememberEventsToScenes")
        {
            string[] eventsToScenes = currentXmlText.Item(currentTagNum).Attributes["eventsToScenes"].Value.Split(',');
            string command = currentXmlText.Item(currentTagNum).Attributes["command"].Value;

            if (command == "Add")
            {
                for (int i = 0; i < eventsToScenes.Length; i++)
                {
                    if (!Singleton<GameState>.Instance.rememberEventsToScenes.Contains(eventsToScenes[i]))
                    {
                        Singleton<GameState>.Instance.rememberEventsToScenes.Add(eventsToScenes[i]);
                    }
                }

                currentTagNum++;
            }
            else if (command == "Remove")
            {
                for (int i = 0; i < eventsToScenes.Length; i++)
                {
                    if (Singleton<GameState>.Instance.rememberEventsToScenes.Contains(eventsToScenes[i]))
                    {
                        Singleton<GameState>.Instance.rememberEventsToScenes.Remove(eventsToScenes[i]);
                    }
                }

                currentTagNum++;
            }
            else if (command == "IsEvent")
            {
                bool result = IsEventsToScenes(eventsToScenes);

                if (result)
                {
                    string chapter = currentXmlText.Item(currentTagNum).Attributes["chapter"].Value;
                    GoNextChapter(chapter, false);
                }
                else
                {
                    string altChapter = currentXmlText.Item(currentTagNum).Attributes["altChapter"].Value;

                    if (altChapter != string.Empty)
                    {
                        GoNextChapter(altChapter, false);
                    }
                    else
                    {
                        currentTagNum++;
                    }
                }
            }
            else if (command == "EditGameItemOnScene")
            {
                bool result = IsEventsToScenes(eventsToScenes);

                if (result)
                {
                    string scene = currentXmlText.Item(currentTagNum).Attributes["scene"].Value;
                    string item = currentXmlText.Item(currentTagNum).Attributes["item"].Value;
                    string interaction = currentXmlText.Item(currentTagNum).Attributes["interaction"].Value;
                    string visibility = currentXmlText.Item(currentTagNum).Attributes["visibility"].Value;

                    srp.SceneGameItemsController.EditGameItemOnScene(scene, item, interaction, visibility);
                }

                currentTagNum++;
            }

            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "DeathEnd")
        {
            DeathMode = true;
            Time.timeScale = 1f;
            StartCoroutine(srp.DeathEnd.IEDeathScene());
        }
        else if (currentXmlText.Item(currentTagNum).Name == "GameEnd")
        {
            GameEnd = true;
            Time.timeScale = 1f;
            StartCoroutine(srp.GameEnd.IEStartGameEnd());
        }
        else if (currentXmlText.Item(currentTagNum).Name == "Note")
        {
            string name = currentXmlText.Item(currentTagNum).Attributes["name"].Value;

            if (currentXmlText.Item(currentTagNum).Attributes["pressButton"] != null)
            {
                srp.NotesController.ShowNotePressButton(name);
            }
            else
            {
                srp.NotesController.ShowNote(name);
            }

            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "Tutor")
        {
            string name = currentXmlText.Item(currentTagNum).Attributes["name"].Value;
            srp.TutorController.ShowTutor(name);

            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "MiniGame")
        {
            MiniGameMode = true;

            string name = currentXmlText.Item(currentTagNum).Attributes["name"].Value;

            int currentNumberGame = 0;

            if (currentXmlText.Item(currentTagNum).Attributes["game"] != null)
            {
                currentNumberGame = int.Parse(currentXmlText.Item(currentTagNum).Attributes["game"].Value);
            }

            srp.MiniGamesController.StartMiniGame(name, currentNumberGame);
            InterfacePanelSetActive(false);
        }
        else if (currentXmlText.Item(currentTagNum).Name == "DialogPanel")
        {
            string dialogPanel = currentXmlText.Item(currentTagNum).Attributes["name"].Value;
            Singleton<GameState>.Instance.dialogPanel = dialogPanel;
            DialogPanelChange(dialogPanel);

            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "Log")
        {
            string log = currentXmlText.Item(currentTagNum).Attributes["name"].Value;
            Debug.Log("<color=purple>" + log + "</color>");

            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "Lore")
        {
            string name = currentXmlText.Item(currentTagNum).Attributes["name"].Value;
            string category = currentXmlText.Item(currentTagNum).Attributes["category"].Value;
            string unlockLevel = currentXmlText.Item(currentTagNum).Attributes["unlockLevel"].Value;

            Singleton<ToolBox>.Instance.LoreOpen.UnlockLore(name, category, unlockLevel);

            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "SteamAchivka")
        {
            try
            {
                string name = currentXmlText.Item(currentTagNum).Attributes["name"].Value;

                uint achivkaCount = SteamUserStats.GetNumAchievements();

                for (uint i = 0; i < achivkaCount; i++)
                {
                    string achivkaName = SteamUserStats.GetAchievementName(i);

                    if (achivkaName == name)
                    {
                        bool Unlocked;
                        SteamUserStats.GetAchievement(achivkaName, out Unlocked);
                        if (!Unlocked) SteamUserStats.SetAchievement(achivkaName);
                        SteamUserStats.StoreStats();
                    }
                }
            }
            catch { Debug.Log("Нет подключения к Steam"); }

            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "ArrowsSettings")
        {
            string mode = currentXmlText.Item(currentTagNum).Attributes["mode"].Value;

            if (mode == "deactivate")
            {
                string arrows = currentXmlText.Item(currentTagNum).Attributes["arrows"].Value;
                srp.SceneTransitionController.DeactivateArrows(arrows);
            }
            else if (mode == "text")
            {
                string arrow = currentXmlText.Item(currentTagNum).Attributes["arrow"].Value;
                string bg = currentXmlText.Item(currentTagNum).Attributes["bg"].Value;
                string chapter = currentXmlText.Item(currentTagNum).Attributes["chapter"].Value;
                srp.SceneTransitionController.ReWriteChapterArrow(arrow, bg, chapter);
            }

            currentTagNum++;
            Inspection();
        }
        else if (currentXmlText.Item(currentTagNum).Name == "Special")
        {
            string value = currentXmlText.Item(currentTagNum).Attributes["value"].Value;

            if (value == "openGallery")
            {
                PlayerPrefs.SetString("openGallery", "True");
            }
            else if (value == "openAmulet")
            {
                Singleton<GameState>.Instance.openAmulet = true;
            }

            currentTagNum++;
            Inspection();
        }
    }

    private IEnumerator TextScroll(string lineOfText)
    {
        lineOfText = lineOfText.Trim();
        int letter = 0;
        DialogText.text = "";
        isTyping = true;
        cancelTyping = false;

        if (textSpeed > 0.01)
        {
            while (isTyping && !cancelTyping && letter < lineOfText.Length - 1)
            {
                DialogText.text += lineOfText[letter];
                letter++;
                yield return new WaitForSeconds(textSpeed);
            }
        }

        AddToLog(lineOfText);
        DialogText.text = lineOfText;
        isTyping = false;
        cancelTyping = false;
    }

    public void GoNextChapter(string chapter, bool contGame)
    {
        Debug.Log("<color=green>Работает " + chapter + "</color>");

        if (!langScenarioXML.ContainsKey(chapter)) { Debug.Log("Глава не найдена"); return; }

        Singleton<GameState>.Instance.nameChapter = chapter;

        currentXmlText = langScenarioXML[chapter].ChildNodes;
        currentTagNum = 0;
        endAtLine = currentXmlText.Count - 1;

        isTyping = true;
        ChoiceMode = false;

        if (contGame)
        {
            Inspection();

            currentLineNum = 0;
            currentBlockLines = currentXmlText.Item(currentTagNum).InnerText.Split('\n');

            Singleton<GameState>.Instance.currentLineNum = currentLineNum;
            Singleton<GameState>.Instance.currentTagNum = currentTagNum;

            StartCoroutine(TextScroll(currentBlockLines[currentLineNum]));
            Inspection();
            currentLineNum++;

            if (currentLineNum < currentBlockLines.Length)
            {
                textContinue = true;
            }
            else
            {
                lastPlaceWithText = Singleton<GameState>.Instance.nameChapter + "^" +
                    Singleton<GameState>.Instance.currentTagNum + "^" +
                    Singleton<GameState>.Instance.currentLineNum;

                Singleton<GameState>.Instance.lastPlaceWithText = lastPlaceWithText;
                textContinue = false;
            }
        }
    }

    private void AddToLog(string curText)
    {
        if (nameState)
        {
            if (currentName == "Zero")
            {
                currentName = " ";
            }
            oldText.Add("\n" + currentName + "\n");
            oldText.Add(curText);
            nameState = false;
        }
        else if (!oldText.Contains(curText))
        {
            oldText.Add(curText);
        }
        if (oldText.Count > 100)
        {
            oldText.RemoveRange(0, 2);
        }
        HistoryText.text = "";
        foreach (string str in oldText)
        {
            HistoryText.text += str + "\n";
        }
        Singleton<GameState>.Instance.oldText = oldText;
    }

    public void AutoReadText()
    {
        if (autoReadingMode)
        {
            autoReadingMode = false;
        }
        else
        {
            autoReadingMode = true;
            StartCoroutine(IEAutoReading());
        }
    }

    private IEnumerator IEAutoReading()
    {
        while (autoReadingMode)
        {
            if (!AnimMode && !ChoiceMode && !TravelMode && !GameEnd && !DeathMode && !ExitScene)
            {
                autoReadText = true;
                yield return new WaitForSeconds(1.0f);
                yield return new WaitForSeconds(autoHopTime + autoTimeRead - 1.0f);
            }
            else
            {
                autoReadingMode = false;
            } 
        }
    }

    public bool IsChapterRead(string chapter)
    {
        List<string> chaptersRead = Singleton<GameState>.Instance.chaptersRead;

        if (chaptersRead.Contains(chapter)) { return true; }

        chaptersRead.Add(chapter);
        Singleton<GameState>.Instance.chaptersRead = chaptersRead;

        return false;
    }

    public void HistoryActive()
    {
        isTyping = false;
        if (History.activeSelf)
            UIClickBlocker.allowClick = true;
        History.SetActive(!History.activeSelf);
    }

    private void DialogPanelChange(string dialogPanel)
    {
        if (dialogPanel == "Standart")
        {
            DialogPanel.transform.GetChild(0).gameObject.SetActive(true);
            DialogPanel.transform.GetChild(1).gameObject.SetActive(false);
            OpenWoodenMiniMenu.SetActive(false);
            WoodenMiniMenu.SetActive(false);
        }
        else if (dialogPanel == "Wooden")
        {
            DialogPanel.transform.GetChild(0).gameObject.SetActive(false);
            DialogPanel.transform.GetChild(1).gameObject.SetActive(true);
            OpenWoodenMiniMenu.SetActive(true);
            WoodenMiniMenu.SetActive(true);
        }
        else
        {
            Debug.Log("<color=red>Диалоговая панель " + dialogPanel + " не найдена. </color>");
        }
    }

    public void InterfacePanelSetActive(bool active)
    {
        DialogPanel.SetActive(active);
        NameHero.SetActive(active);

        if (Singleton<GameState>.Instance.dialogPanel == "Wooden")
        {
            srp.MiniMenuController.ActivateTypeMiniMenu(false);
            srp.Characters.InterfaceCharacterToggleSet(active);

            if (TravelMode)
            {
                OpenWoodenMiniMenu.SetActive(true);
            }
            else
            {
                OpenWoodenMiniMenu.SetActive(active);
            }
        }
    }

    public void NextDay(string day)
    {
        Singleton<GameState>.Instance.currentDay = day;
        langScenarioXML = Singleton<ToolBox>.Instance.LangScenarioXML[day];

        Singleton<GameState>.Instance.gameItemsOnScenesDict = Singleton<ToolBox>.Instance.GameItemsOnScenesByDaysXML[day].ToDictionary(kv => kv.Key, kv => kv.Value.Clone());
        srp.SceneGameItemsController.DicGameItemsOnScenesByDays = Singleton<ToolBox>.Instance.GameItemsOnScenesByDaysXML[day].ToDictionary(kv => kv.Key, kv => kv.Value.Clone());
        srp.SceneGameItemsController.ReloadGameItemsOnScene(false);

        srp.TaskController.StartTasks();

        Singleton<GameState>.Instance.chaptersRead = new List<string>();
        Singleton<GameState>.Instance.eventEntranceOnScene = new List<string>();

        GoNextChapter("chap_0", false);
    }

    public void SetModeSaveLoad(string mode)
    {
        Singleton<ToolBox>.Instance.ModeSaveLoad = mode;
    }

    public void CloseExitMenu()
    {
        ExitMenu.SetActive(false);
        ExitScene = false;
        UIClickBlocker.allowClick = true;
    }

    public void ResetState()
    {
        Singleton<GameState>.Instance.ResetState();
    }

    public void ExitToMainMenu()
    {
        ExitScene = true;
        ResetState();
        sf.LoadLevel("Menu");
    }

    public bool IsEventToScene(string eventToScene)
    {
        if (eventToScene == string.Empty) { return false; }

        List<string> rememberEventsToScenes = Singleton<GameState>.Instance.rememberEventsToScenes;

        if (rememberEventsToScenes.Contains(eventToScene)) { return true; }

        return false;
    }

    public bool IsEventsToScenes(string[] eventsToScenes)
    {
        if (eventsToScenes.Length == 0) { return false; }

        for (int i = 0; i < eventsToScenes.Length; i++)
        {
            if (!IsEventToScene(eventsToScenes[i]))
            {
                return false;
            }
        }

        return true;
    }
}