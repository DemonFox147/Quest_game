using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public DateTime DateTime
    {
        get;
        private set;
    }

    public byte[] Screenshot
    {
        get;
        private set;
    }
    
    public string language;

    public string currentDay;

    public int currentTagNum;
    public int currentLineNum;

    public string lastPlaceWithText;

    public string dialogPanel;

    public string nameBackground;
    public string nameSaveBackground;
    public string nameFirstBackground;

    public string nameCurMusic;
    public string namePastMusic;

    public float timePastMusic;

    public string stepAnimation;
    public string loopAnimation;
    public string combinAnimation;

    public string nameHero;
    public string nameChapter;

    public string choiceMode;

    public string gameItemsMode;

    public string travelMode;

    public string notesMode;
    public string nameNote;

    public string tutorMode;
    public string nameTutor;

    public string miniGame;

    public bool openAmulet;

    public string[] taskComplete;

    public List<string> oldText;

    public List<string> inventory;

    public List<string> chaptersRead;

    public List<string> deactivateArrows;

    public List<string> rewriteChapterArrow;

    public List<string> eventEntranceOnScene;

    public List<string> rememberEventsToScenes;

    public Dictionary<string, string[]> charTypesDict;

    public Dictionary<string, XmlNode> gameItemsOnScenesDict;

    public void Awake()
    {
        ResetState();
    }

    public string[] GetState()
    {
        string NormCharPos = string.Join("^", charTypesDict["N"]);
        string BigCharPos = string.Join("^", charTypesDict["B"]);
        string IFCharPos = string.Join("^", charTypesDict["IF"]);
        string NormHatCharPos = string.Join("^", charTypesDict["NH"]);
        string NormShadowCharPos = string.Join("^", charTypesDict["NSh"]);

        string TaskComplete = string.Join("^", taskComplete);
        string OldText = string.Join("^", oldText.ToArray());
        string Inventory = string.Join("^", inventory.ToArray());
        string ChaptersRead = string.Join("^", chaptersRead.ToArray());
        string DeactivateArrows = string.Join("^", deactivateArrows.ToArray());
        string ReWriteChapterArrow = string.Join("^", rewriteChapterArrow.ToArray());
        string EventEntranceToScene = string.Join("^", eventEntranceOnScene.ToArray());
        string RememberEventsToScenes = string.Join("^", rememberEventsToScenes.ToArray());
        string keyDictGameItems = string.Join("^", gameItemsOnScenesDict.Select(kv => kv.Key).ToArray());
        string valDictGameItems = string.Join("^", gameItemsOnScenesDict.Select(kv => kv.Value.OuterXml).ToArray());

        return new string[]
        {
            language,
            currentDay,
            currentTagNum.ToString(),
            currentLineNum.ToString(),
            lastPlaceWithText,
            dialogPanel,
            nameBackground,
            nameSaveBackground,
            nameFirstBackground,
            nameCurMusic,
            namePastMusic,
            timePastMusic.ToString(),
            stepAnimation,
            loopAnimation,
            combinAnimation,
            nameHero,
            nameChapter,
            choiceMode,
            gameItemsMode,
            travelMode,
            notesMode,
            nameNote,
            tutorMode,
            nameTutor,
            miniGame,
            openAmulet.ToString(),
            NormCharPos,
            BigCharPos,
            IFCharPos,
            NormHatCharPos,
            NormShadowCharPos,
            TaskComplete,
            OldText,
            Inventory,
            ChaptersRead,
            DeactivateArrows,
            ReWriteChapterArrow,
            EventEntranceToScene,
            RememberEventsToScenes,
            keyDictGameItems,
            valDictGameItems
        };
    }

    public string[] GetInfoState()
    {
        DateTime = DateTime.Now;

        return new string[]
        {
            DateTime.ToString("dd.MM.yyy HH:mm"),
            Convert.ToBase64String(Screenshot)
        };
    }

    public void ScreenConvert(Texture2D tex)
    {
        Screenshot = tex.EncodeToJPG();
    }

    public void ResetState()
    {
        language = "Eng";
        currentDay = "Day_0";
        currentTagNum = 0;
        currentLineNum = 0;
        lastPlaceWithText = "";
        dialogPanel = "Standart";
        nameBackground = "";
        nameSaveBackground = "";
        nameFirstBackground = "";
        nameCurMusic = "";
        namePastMusic = "";
        timePastMusic = 0f;
        nameHero = "Zero";
        nameChapter = "chap_0";
        choiceMode = "False";
        stepAnimation = "";
        loopAnimation = "";
        combinAnimation = "";
        gameItemsMode = "False";
        travelMode = "False";
        notesMode = "False";
        nameNote = "Zero";
        tutorMode = "False";
        nameTutor = "Zero";
        miniGame = "";
        openAmulet = false;
        taskComplete = new string[0];
        oldText = new List<string>();
        inventory = new List<string>();
        chaptersRead = new List<string>();
        deactivateArrows = new List<string>();
        rewriteChapterArrow = new List<string>();
        eventEntranceOnScene = new List<string>();
        rememberEventsToScenes = new List<string>();

        charTypesDict = new Dictionary<string, string[]>
        {
            {"N",   new string[5] },
            {"B",   new string[3] },
            {"IF",  new string[1] },
            {"NH",  new string[5] },
            {"NSh", new string[3] }
        };

        gameItemsOnScenesDict = new Dictionary<string, XmlNode>();
    }
}