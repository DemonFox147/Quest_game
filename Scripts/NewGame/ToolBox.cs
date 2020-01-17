using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml;
using System.Collections.Generic;

public class ToolBox : Singleton<ToolBox>
{
    private AudioSource music;

    private string currLevel;

    public string GameLanguage
    {
        get;
        set;
    }

    public GalleryOpen GalleryOpen
    {
        get;
        private set;
    }

    public LoreOpen LoreOpen
    {
        get;
        private set;
    }

    public GameState GameState
    {
        get;
        private set;
    }

    private ScreenshotController screenshotController;

    public string PrevLevel
    {
        get;
        private set;
    }

    public string ModeSaveLoad
    {
        get;
        set;
    }

    public Dictionary<int, string[]> BGDictionaty
    {
        get;
        private set;
    }

    public Dictionary<int, string[]> CGDictionaty
    {
        get;
        private set;
    }

    public Dictionary<int, bool> BGDictionatyActive
    {
        get;
        private set;
    }

    public Dictionary<int, bool> CGDictionatyActive
    {
        get;
        private set;
    }

    public Dictionary<string, Dictionary<string, XmlNode>> LangScenarioXML
    {
        get;
        private set;
    }

    public Dictionary<string, Dictionary<string, XmlNode>> LoreDictionaryXML
    {
        get;
        private set;
    }

    public Dictionary<string, Dictionary<string, XmlNode>> GameItemsOnScenesByDaysXML
    {
        get;
        private set;
    }

    public Dictionary<string, Dictionary<string, XmlNode>> TasksByDaysXML
    {
        get;
        private set;
    }

    public Dictionary<string, XmlNode> TransitionRoomsXML
    {
        get;
        private set;
    }

    public Dictionary<string, XmlNode> InfoTextXML
    {
        get;
        private set;
    }

    public Dictionary<string, Dictionary<string, XmlNode>> FiltersForCharactersByScenesXML
    {
        get;
        private set;
    }

    void Awake()
    {
        ModeSaveLoad = "Load";
        GameState = gameObject.AddComponent<GameState>();
        screenshotController = gameObject.AddComponent<ScreenshotController>();
        music = gameObject.AddComponent<AudioSource>();
        music.loop = true;

        if (PlayerPrefs.GetString("FirstStart", "Zero") != "FirstStartFullGame")
        {
            PlayerPrefs.SetString("FirstStart", "FirstStartFullGame");
            PlayerPrefs.SetInt("fontSize", 23);
            PlayerPrefs.SetInt("fontValue", 1);
            PlayerPrefs.SetFloat("musicVolume", 0.5f);
            PlayerPrefs.SetFloat("soundVolume", 0.5f);
            PlayerPrefs.SetFloat("textSpeed", 0.089f);
            PlayerPrefs.SetFloat("autoHopTime", 5f);
            PlayerPrefs.SetString("language", "Eng");
            PlayerPrefs.SetString("openGallery", "False");
            PlayerPrefs.SetString("saveBW_openedBGsList", "Zero");
            PlayerPrefs.SetString("saveBW_openedCGsList", "Zero");
            PlayerPrefs.SetString("saveBW_openedLoreCharacters", "Zero");
            PlayerPrefs.SetString("saveBW_openedLoreCreatures", "Zero");
            PlayerPrefs.SetString("saveBW_openedLoreWorld", "Zero");
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.GetString("SecondStart", "Zero") != "SecondStartFullGame")
        {
            PlayerPrefs.SetString("SecondStart", "SecondStartFullGame");
            PlayerPrefs.SetString("saveBW_LoreNewInfo", "");
        }

        music.volume = PlayerPrefs.GetFloat("musicVolume");
        GameLanguage = PlayerPrefs.GetString("language");

        GameState.language = GameLanguage;
        TextLanguage(GameLanguage);
        LoreLanguage(GameLanguage);
        GameItemsOnScenesByDays();
        TransitionBetweenRooms();
        TaskListByDays();
        InfoText();
        FiltersForCharactersByScenes();
    }

    void Start()
    {
        OnLevelWasLoaded();

        BGDictionaty = new Dictionary<int, string[]>();
        CGDictionaty = new Dictionary<int, string[]>();
        
        XmlNodeList allCategoriesBG = InfoTextXML["AllCategoriesBG"].ChildNodes;
        for (int i = 0; i < allCategoriesBG.Count; i++)
        {
            BGDictionaty.Add(i, allCategoriesBG[i].InnerText.Split(','));
        }

        XmlNodeList allCategoriesCG = InfoTextXML["AllCategoriesCG"].ChildNodes;
        for (int i = 0; i < allCategoriesCG.Count; i++)
        {
            CGDictionaty.Add(i, allCategoriesCG[i].InnerText.Split(','));
        }

        GalleryOpen = gameObject.AddComponent<GalleryOpen>();
        LoreOpen = gameObject.AddComponent<LoreOpen>();
    }

    public void StartToolBox() { }

    public void OnLevelWasLoaded()
    {
        if (SceneManager.GetActiveScene().name != "StartVideo")
        {
            if (SceneManager.GetActiveScene().name != currLevel)
            {
                PrevLevel = currLevel;
                currLevel = SceneManager.GetActiveScene().name;
            }
        }
        else
        {
            currLevel = SceneManager.GetActiveScene().name;
            PrevLevel = currLevel;
        }

        if (currLevel == "StartVideo")
        {
            music.Stop();
            music.clip = Resources.Load<AudioClip>("AudioClips/StartVideo/StartMusic");
            music.Play();
        }
        else if (currLevel == "Menu")
        {
            if (music.clip == null || music.clip.name != "MainMusic")
            {
                music.Stop();
                music.clip = Resources.Load<AudioClip>("AudioClips/Menu/MainMusic");
                music.Play();
            }
        }
    }

    public void TextLanguage(string Lang)
    {
        LangScenarioXML = new Dictionary<string, Dictionary<string, XmlNode>>();
        TextAsset langText = Resources.Load<TextAsset>("Text/" + Lang);
        XmlDocument xDoc = new XmlDocument();
        xDoc.LoadXml(langText.text);

        XmlNodeList daysList = xDoc.DocumentElement.ChildNodes;

        foreach (XmlNode day in daysList)
        {
            XmlNodeList chapList = day.ChildNodes;
            Dictionary<string, XmlNode> chapDic = new Dictionary<string, XmlNode>();
            foreach (XmlNode chap in chapList)
            {
                if (!chapDic.ContainsKey(chap.Name))
                {
                    chapDic.Add(chap.Name, chap);
                }
            }
            LangScenarioXML.Add(day.Name, chapDic);
        }
    }

    public void LoreLanguage(string Lang)
    {
        LoreDictionaryXML = new Dictionary<string, Dictionary<string, XmlNode>>();
        TextAsset langLore = Resources.Load<TextAsset>("Text/Lore");
        XmlDocument xDoc = new XmlDocument();
        xDoc.LoadXml(langLore.text);

        XmlElement dataList = xDoc.DocumentElement;
        XmlNodeList categories;

        if (Lang == "Rus")
        {
            categories = dataList.ChildNodes.Item(0).ChildNodes;
        }
        else if (Lang == "Eng")
        {
            categories = dataList.ChildNodes.Item(1).ChildNodes;
        }
        else
        {
            Debug.Log("Ошибка в получении Лора " + GameLanguage);
            return;
        }

        foreach (XmlNode category in categories)
        {
            Dictionary<string, XmlNode> categoryDict = new Dictionary<string, XmlNode>();

            XmlNodeList information = category.ChildNodes;

            foreach (XmlNode info in information)
            {
                if (!categoryDict.ContainsKey(info.Name))
                {
                    categoryDict.Add(info.Name, info);
                }
            }
            LoreDictionaryXML.Add(category.Name, categoryDict);
        }
    }

    public bool IsContainsKeyInTextLanguage(string chapter)
    {
        return LangScenarioXML[Singleton<GameState>.Instance.currentDay].ContainsKey(chapter);
    }

    public void GameItemsOnScenesByDays()
    {
        GameItemsOnScenesByDaysXML = new Dictionary<string, Dictionary<string, XmlNode>>();
        TextAsset GI = Resources.Load<TextAsset>("Text/GameItemsOnScenesByDays");
        XmlDocument xDoc = new XmlDocument();
        xDoc.LoadXml(GI.text);

        XmlNodeList daysList = xDoc.DocumentElement.ChildNodes;

        foreach (XmlNode day in daysList)
        {
            XmlNodeList roomsList = day.ChildNodes;
            Dictionary<string, XmlNode> roomDic = new Dictionary<string, XmlNode>();
            foreach (XmlNode room in roomsList)
            {
                roomDic.Add(room.Name, room);
            }
            GameItemsOnScenesByDaysXML.Add(day.Name, roomDic);
        }
    }

    public void TransitionBetweenRooms()
    {
        TransitionRoomsXML = new Dictionary<string, XmlNode>();
        TextAsset TBR = Resources.Load<TextAsset>("Text/TransitionBetweenRooms");
        XmlDocument xDoc = new XmlDocument();
        xDoc.LoadXml(TBR.text);

        XmlNodeList xListRooms = xDoc.DocumentElement.ChildNodes;

        foreach (XmlNode room in xListRooms)
        {
            TransitionRoomsXML.Add(room.Name, room);
        }
    }

    public void TaskListByDays()
    {
        TasksByDaysXML = new Dictionary<string, Dictionary<string, XmlNode>>();
        TextAsset TaskXML = Resources.Load<TextAsset>("Text/Task");
        XmlDocument xDoc = new XmlDocument();
        xDoc.LoadXml(TaskXML.text);

        XmlNodeList langList = xDoc.DocumentElement.ChildNodes;

        foreach (XmlNode lang in langList)
        {
            XmlNodeList daysList = lang.ChildNodes;
            Dictionary<string, XmlNode> tasks = new Dictionary<string, XmlNode>();

            foreach (XmlNode day in daysList)
            {
                tasks.Add(day.Name, day);
            }

            TasksByDaysXML.Add(lang.Name, tasks);
        }
    }

    public void InfoText()
    {
        InfoTextXML = new Dictionary<string, XmlNode>();
        TextAsset IT = Resources.Load<TextAsset>("Text/InfoText");
        XmlDocument xDoc = new XmlDocument();
        xDoc.LoadXml(IT.text);

        XmlNodeList xNodeList = xDoc.DocumentElement.ChildNodes;
 
        foreach (XmlNode item in xNodeList)
        {
            if (!InfoTextXML.ContainsKey(item.Name))
            {
                InfoTextXML.Add(item.Name, item);
            }
        }
    }

    public void FiltersForCharactersByScenes()
    {
        FiltersForCharactersByScenesXML = new Dictionary<string, Dictionary<string, XmlNode>>();
        TextAsset FCS = Resources.Load<TextAsset>("Text/FiltersForCharactersByScenes");
        XmlDocument xDoc = new XmlDocument();
        xDoc.LoadXml(FCS.text);

        XmlNodeList scenes = xDoc.DocumentElement.FirstChild.ChildNodes;

        foreach (XmlNode scene in scenes)
        {
            XmlNodeList characters = scene.FirstChild.ChildNodes;

            Dictionary<string, XmlNode> charactersDictionary = new Dictionary<string, XmlNode>();

            foreach (XmlNode character in characters)
            {
                charactersDictionary.Add(character.Name, character);
            }

            FiltersForCharactersByScenesXML.Add(scene.Name, charactersDictionary);
        }
    }
}