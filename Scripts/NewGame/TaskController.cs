using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class TaskController : MonoBehaviour
{
    [HideInInspector]
    public ScriptReferencesPool srp;

    public GameObject BgTaskMenu;
    public GameObject TaskMenu;

    public Text TaskTip;

    public Image CollectedItem;

    public Transform[] TaskList;

    public bool[] TaskStatusList;

    private Dictionary<string, XmlNode> tasksLangXML;
    
    void Start()
    {
        tasksLangXML = Singleton<ToolBox>.Instance.TasksByDaysXML[Singleton<ToolBox>.Instance.GameLanguage];

        BgTaskMenu = transform.GetChild(0).gameObject;
        TaskMenu = transform.GetChild(1).gameObject;
        Transform taskList = TaskMenu.transform.GetChild(1);
        TaskList = new Transform[taskList.childCount];

        for (int i = 0; i < TaskList.Length; i++)
        {
            TaskList[i] = taskList.GetChild(i);
        }

        TaskTip = TaskMenu.transform.GetChild(2).GetComponent<Text>();

        StartTasks();
    }

    public void StartTasks()
    {
        string collectedItemName = tasksLangXML[Singleton<GameState>.Instance.currentDay].Attributes["name"].Value;

        if (collectedItemName != "")
        {
            CollectedItem.sprite = Resources.Load<Sprite>("Images/Interface/Tasks/" + collectedItemName + Singleton<ToolBox>.Instance.GameLanguage);
        }
        else
        {
            CollectedItem.sprite = Resources.Load<Sprite>("Images/Interface/Tasks/Zero");
        }

        XmlNode xTaskList = tasksLangXML[Singleton<GameState>.Instance.currentDay].FirstChild;
        TaskStatusList = new bool[xTaskList.ChildNodes.Count];

        int numTask = 0;

        foreach (Transform t in TaskList)
        {
            t.gameObject.SetActive(false);
        }

        if (Singleton<GameState>.Instance.taskComplete.Length != TaskStatusList.Length || TaskStatusList.Length == 0)
        {
            Singleton<GameState>.Instance.taskComplete = new string[TaskStatusList.Length];

            foreach (XmlNode xTask in xTaskList)
            {
                TaskList[numTask].gameObject.SetActive(true);
                TaskList[numTask].GetChild(0).GetComponent<Text>().text = xTask.InnerText;
                TaskList[numTask].GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Interface/Tasks/CheckboxWithoutCheckMark");
                TaskStatusList[numTask] = false;
                Singleton<GameState>.Instance.taskComplete[numTask] = "False";
                numTask++;
            }
        }
        else
        {
            foreach (XmlNode xTask in xTaskList)
            {
                TaskStatusList[numTask] = bool.Parse(Singleton<GameState>.Instance.taskComplete[numTask]);

                TaskList[numTask].gameObject.SetActive(true);
                TaskList[numTask].GetChild(0).GetComponent<Text>().text = xTask.InnerText;

                if (TaskStatusList[numTask])
                {
                    TaskList[numTask].GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Interface/Tasks/CheckboxWithCheckMark");
                }
                else
                {
                    TaskList[numTask].GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Interface/Tasks/CheckboxWithoutCheckMark");
                }
                numTask++;
            }
        }
    }

    public void ShowTaskTip(int numTask)
    {
        XmlNode xListOfTips = tasksLangXML[Singleton<GameState>.Instance.currentDay].LastChild;
        TaskTip.text = xListOfTips.ChildNodes.Item(numTask).InnerText;
    }

    public void TaskComplete(int numTask)
    {
        TaskStatusList[numTask] = true;
        Singleton<GameState>.Instance.taskComplete[numTask] = "True";
        TaskList[numTask].GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Interface/Tasks/CheckboxWithCheckMark");

        for (int i = 0; i < TaskStatusList.Length; i++)
        {
            if (!TaskStatusList[i])
            {
                return;
            }
        }

        StartCoroutine(IEAllTaskComplete());
    }

    private IEnumerator IEAllTaskComplete()
    {
        Singleton<GameState>.Instance.eventEntranceOnScene.Add("chap_LastVisit");

        while (true)
        {
            yield return new WaitForSecondsRealtime(0.5f);

            if (srp.ScenarioParser.TravelMode)
            {
                srp.ScenarioParser.TravelMode = false;
                srp.SceneTransitionController.StopTransitionMode();
                srp.ScenarioParser.InterfacePanelSetActive(true);
                srp.ScenarioParser.GoNextChapter("chap_AllTaskComplete", true);
                break;
            }
        }
    }

    public void TaskMenuToggle()
    {
        TaskMenu.SetActive(!TaskMenu.activeSelf);
        BgTaskMenu.SetActive(!BgTaskMenu.activeSelf);

        if (TaskMenu.activeSelf == false)
        {
            TaskTip.text = "";
        }
    }
}