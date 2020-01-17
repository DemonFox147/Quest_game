using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Xml;

public class SaveLoadCell : MonoBehaviour
{
    private SaveLoadController slc;
    private ScreenFader sf;

    private Sprite diskette;
    private Sprite zero;

    public GameObject SaveLoadCells;
    public GameObject InfoData;
    public GameObject InfoDelete;

    public GameObject[] saveLoadCells;
    public GameObject[] infoData;
    public GameObject[] infoDelete;

    void Awake()
    {
        slc = FindObjectOfType<SaveLoadController>();
        sf = FindObjectOfType<ScreenFader>();

        diskette = Resources.Load<Sprite>("Images/SaveLoad/Diskette");
        zero = Resources.Load<Sprite>("Images/SaveLoad/Zero");

        if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves");
        }

        if (!File.Exists(Application.persistentDataPath + "/Saves/saveMeta.bwPC"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fStream = File.Create(Application.persistentDataPath + "/Saves/saveMeta.bwPC");
            Dictionary<int, string[]> dict = new Dictionary<int, string[]>();
            bf.Serialize(fStream, dict);
            fStream.Close();
        }

        saveLoadCells = new GameObject[SaveLoadCells.transform.childCount];
        for (int i = 0; i < SaveLoadCells.transform.childCount; i++)
        {
            saveLoadCells[i] = SaveLoadCells.transform.GetChild(i).gameObject;
        }

        infoData = new GameObject[InfoData.transform.childCount];
        for (int i = 0; i < InfoData.transform.childCount; i++)
        {
            infoData[i] = InfoData.transform.GetChild(i).gameObject;
        }

        infoDelete = new GameObject[InfoDelete.transform.childCount];
        for (int i = 0; i < InfoDelete.transform.childCount; i++)
        {
            infoDelete[i] = InfoDelete.transform.GetChild(i).gameObject;
        }
    }

    public void SelectMode(int selfNumber)
    {
        IsActiveCell(selfNumber);

        if (slc.ModeSaveLoad == "Save" && infoData[selfNumber].GetComponent<Text>().text != "")
        {
            slc.ReWriteMenu.SetActive(true);
        }
        if (slc.ModeSaveLoad == "Save" && infoData[selfNumber].GetComponent<Text>().text == "")
        {
            Save(selfNumber);
        }
        if (slc.ModeSaveLoad == "Load")
        {
            Load(selfNumber);
        }
    }

    public void Save(int selfNumber)
    {
        //Сохранение состояния
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fStream = File.Create(Application.persistentDataPath + "/Saves/"
            + slc.SavesType + ((slc.CurrentPage - 1) * saveLoadCells.Length + selfNumber).ToString() + ".bwPC");

        bf.Serialize(fStream, Singleton<GameState>.Instance.GetState());
        fStream.Close();

        //Сохранение данных
        string[] array = Singleton<GameState>.Instance.GetInfoState();

        fStream = File.OpenRead(Application.persistentDataPath + "/Saves/saveMeta.bwPC");
        Dictionary<int, string[]> dict = bf.Deserialize(fStream) as Dictionary<int, string[]>;
        fStream.Close();
        if (dict.ContainsKey((slc.CurrentPage - 1) * saveLoadCells.Length + selfNumber))
        {
            dict.Remove((slc.CurrentPage - 1) * saveLoadCells.Length + selfNumber);
        }
        dict.Add((slc.CurrentPage - 1) * saveLoadCells.Length + selfNumber, array);
        fStream = File.OpenWrite(Application.persistentDataPath + "/Saves/saveMeta.bwPC");
        bf.Serialize(fStream, dict);
        fStream.Close();

        Texture2D tex = new Texture2D(Screen.width, Screen.height);
        tex.LoadImage(Singleton<GameState>.Instance.Screenshot);
        saveLoadCells[selfNumber].GetComponent<Image>().sprite =
            Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 0f), 32);

        infoData[selfNumber].GetComponent<Text>().text = Singleton<GameState>.Instance.DateTime.ToString("dd.MM.yyy HH:mm");

        saveLoadCells[selfNumber].GetComponent<Image>().raycastTarget = true;
        infoDelete[selfNumber].SetActive(true);
    }

    public void Load(int selfNumber)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fStream = new FileStream(Application.persistentDataPath + "/Saves/"
            + slc.SavesType + ((slc.CurrentPage - 1) * saveLoadCells.Length + selfNumber).ToString() + ".bwPC", FileMode.Open);

        string[] array = bf.Deserialize(fStream) as string[];
        fStream.Close();

        Singleton<GameState>.Instance.currentDay = array[1];
        Singleton<GameState>.Instance.currentTagNum = int.Parse(array[2]);
        Singleton<GameState>.Instance.currentLineNum = int.Parse(array[3]);
        Singleton<GameState>.Instance.lastPlaceWithText = array[4];
        Singleton<GameState>.Instance.dialogPanel = array[5];
        Singleton<GameState>.Instance.nameBackground = array[6];
        Singleton<GameState>.Instance.nameSaveBackground = array[7];
        Singleton<GameState>.Instance.nameFirstBackground = array[8];
        Singleton<GameState>.Instance.nameCurMusic = array[9];
        Singleton<GameState>.Instance.namePastMusic = array[10];
        Singleton<GameState>.Instance.timePastMusic = float.Parse(array[11]);
        Singleton<GameState>.Instance.stepAnimation = array[12];
        Singleton<GameState>.Instance.loopAnimation = array[13];
        Singleton<GameState>.Instance.combinAnimation = array[14];
        Singleton<GameState>.Instance.nameHero = array[15];
        Singleton<GameState>.Instance.nameChapter = array[16];
        Singleton<GameState>.Instance.choiceMode = array[17];
        Singleton<GameState>.Instance.gameItemsMode = array[18];
        Singleton<GameState>.Instance.travelMode = array[19];
        Singleton<GameState>.Instance.notesMode = array[20];
        Singleton<GameState>.Instance.nameNote = array[21];
        Singleton<GameState>.Instance.tutorMode = array[22];
        Singleton<GameState>.Instance.nameTutor = array[23];
        Singleton<GameState>.Instance.miniGame = array[24];
        Singleton<GameState>.Instance.openAmulet = bool.Parse(array[25]);
        
        Singleton<GameState>.Instance.charTypesDict = new Dictionary<string, string[]>
        {
            {"N",   array[26].Split('^') },
            {"B",   array[27].Split('^') },
            {"IF",  array[28].Split('^') },
            {"NH",  array[29].Split('^') },
            {"NSh", array[30].Split('^') }
        };

        Singleton<GameState>.Instance.taskComplete = array[31].Split('^');
        Singleton<GameState>.Instance.oldText = array[32].Length != 0 ? array[32].Split('^').ToList() : new List<string>();
        Singleton<GameState>.Instance.inventory = array[33].Length != 0 ? array[33].Split('^').ToList() : new List<string>();
        Singleton<GameState>.Instance.chaptersRead = array[34].Length != 0 ? array[34].Split('^').ToList() : new List<string>();
        Singleton<GameState>.Instance.deactivateArrows = array[35].Length != 0 ? array[35].Split('^').ToList() : new List<string>();
        Singleton<GameState>.Instance.rewriteChapterArrow = array[36].Length != 0 ? array[36].Split('^').ToList() : new List<string>();
        Singleton<GameState>.Instance.eventEntranceOnScene = array[37].Length != 0 ? array[37].Split('^').ToList() : new List<string>();
        Singleton<GameState>.Instance.rememberEventsToScenes = array[38].Length != 0 ? array[38].Split('^').ToList() : new List<string>();

        string[] keyDictGameItems = array[39].Split('^');
        string[] valDictGameItems = array[40].Split('^');

        Singleton<GameState>.Instance.gameItemsOnScenesDict = new Dictionary<string, XmlNode>();
        
        XmlDocument xmlDocument = new XmlDocument();

        for (int i = 0; i < keyDictGameItems.Length; i++)
        {
            xmlDocument.LoadXml(valDictGameItems[i]);
            Singleton<GameState>.Instance.gameItemsOnScenesDict.Add(keyDictGameItems[i], xmlDocument.DocumentElement);
        }

        sf.LoadLevel("NewGame");
    }

    public void Reload()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fStream = File.Open(Application.persistentDataPath + "/Saves/saveMeta.bwPC", FileMode.Open, FileAccess.Read);

        Dictionary<int, string[]> dict = bf.Deserialize(fStream) as Dictionary<int, string[]>;
        fStream.Close();

        for (int i = 0; i < saveLoadCells.Length; i++)
        {
            int numDict = (slc.CurrentPage - 1) * saveLoadCells.Length + i;

            if (dict.ContainsKey(numDict))
            {
                string[] infoDict = dict[numDict];
                Texture2D tex = new Texture2D(Screen.width, Screen.height);
                tex.LoadImage(Convert.FromBase64String(infoDict[1]));
                saveLoadCells[numDict % saveLoadCells.Length].GetComponent<Image>().sprite =
                    Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0f, 0f), 32);

                saveLoadCells[numDict % saveLoadCells.Length].GetComponent<Image>().raycastTarget = true;
                infoData[numDict % saveLoadCells.Length].GetComponent<Text>().text = infoDict[0];
                infoDelete[numDict % saveLoadCells.Length].SetActive(true);
            }
            else if (slc.ModeSaveLoad == "Save")
            {
                saveLoadCells[i].GetComponent<Image>().sprite = diskette;
                saveLoadCells[i].GetComponent<Image>().raycastTarget = true;
                infoData[i].GetComponent<Text>().text = "";
                infoDelete[i].SetActive(false);
            }
            else if (slc.ModeSaveLoad == "Load")
            {
                saveLoadCells[i].GetComponent<Image>().sprite = zero;
                saveLoadCells[i].GetComponent<Image>().raycastTarget = false;
                infoData[i].GetComponent<Text>().text = "";
                infoDelete[i].SetActive(false);
            }
        }
    }

    public void IsActiveCell(int selfNumber)
    {
        slc.ActiveCell = selfNumber;
    }

    public void Delete(int selfNumber)
    {
        File.Delete(Application.persistentDataPath + "/Saves/"
            + slc.SavesType + ((slc.CurrentPage - 1) * saveLoadCells.Length + selfNumber).ToString() + ".bwPC");

        BinaryFormatter bf = new BinaryFormatter();
        FileStream fStream = File.OpenRead(Application.persistentDataPath + "/Saves/saveMeta.bwPC");
        Dictionary<int, string[]> dict = bf.Deserialize(fStream) as Dictionary<int, string[]>;
        fStream.Close();
        if (dict.ContainsKey((slc.CurrentPage - 1) * saveLoadCells.Length + selfNumber))
        {
            dict.Remove((slc.CurrentPage - 1) * saveLoadCells.Length + selfNumber);
        }
        fStream = File.OpenWrite(Application.persistentDataPath + "/Saves/saveMeta.bwPC");
        bf.Serialize(fStream, dict);
        fStream.Close();

        if (slc.ModeSaveLoad == "Save")
        {
            saveLoadCells[selfNumber].GetComponent<Image>().sprite = diskette;
            saveLoadCells[selfNumber].GetComponent<Image>().raycastTarget = true;
            infoData[selfNumber].GetComponent<Text>().text = "";
            infoDelete[selfNumber].SetActive(false);
        }
        else if (slc.ModeSaveLoad == "Load")
        {
            saveLoadCells[selfNumber].GetComponent<Image>().sprite = zero;
            saveLoadCells[selfNumber].GetComponent<Image>().raycastTarget = false;
            infoData[selfNumber].GetComponent<Text>().text = "";
            infoDelete[selfNumber].SetActive(false);
        }
    }
}