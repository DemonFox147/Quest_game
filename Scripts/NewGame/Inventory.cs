using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject[] InventoryItemsCells;

    public GameObject[] NumberPages;

    public List<string> InventoryList;

    public Image EventObject;

    public Image ItemInfo;

    public Text DescriptionItem;

    public string ItemTaken;

    private Dictionary<string, XmlNode> ItemsXML;

    public OpenInventoryButton OpenInventoryButton;

    public int CurrentPage
    {
        get;
        private set;
    }

    void Start()
    {
        CurrentPage = 1;
        ItemTaken = "Zero";

        Transform t = transform.GetChild(0).GetChild(1);
        InventoryItemsCells = new GameObject[t.childCount];

        for (int i = 0; i < t.childCount; i++)
        {
            InventoryItemsCells[i] = t.GetChild(i).gameObject;
        }

        InventoryList = Singleton<GameState>.Instance.inventory;

        ItemsXML = new Dictionary<string, XmlNode>();
        TextAsset XMLTextAsset = Resources.Load<TextAsset>("Text/Inventory");
        XmlDocument xDoc = new XmlDocument();
        xDoc.LoadXml(XMLTextAsset.text);
        XmlElement dataList = xDoc.DocumentElement;

        if (Singleton<ToolBox>.Instance.GameLanguage == "Rus")
        {
            XmlNode rusLang = dataList.ChildNodes.Item(0);
            XmlNodeList xNodeList = rusLang.ChildNodes.Item(0).ChildNodes;
            foreach (XmlNode item in xNodeList)
            {
                ItemsXML.Add(item.Name, item);
            }
        }
        else if (Singleton<ToolBox>.Instance.GameLanguage == "Eng")
        {
            XmlNode engLang = dataList.ChildNodes.Item(1);
            XmlNodeList xNodeList = engLang.ChildNodes.Item(0).ChildNodes;
            foreach (XmlNode item in xNodeList)
            {
                ItemsXML.Add(item.Name, item);
            }
        }
        else
        {
            Debug.Log("Ошибка в получении языка " + Singleton<ToolBox>.Instance.GameLanguage + " для инвентаря");
            return;
        }

        ChangeCurrentPage(0);
    }

    public void ChangeCurrentPage(int changePage)
    {
        if (CurrentPage + changePage >= 1 && CurrentPage + changePage <= 4)
        {
            NumberPages[CurrentPage - 1].SetActive(false);
            CurrentPage += changePage;
            NumberPages[CurrentPage - 1].SetActive(true);
            UpdateInventory();
        }
    }

    public void ResetCurrentPage(int changePage)
    {
        NumberPages[CurrentPage - 1].SetActive(false);
        CurrentPage = changePage;
        NumberPages[CurrentPage - 1].SetActive(true);
        UpdateInventory();
    }

    public void AddItemToInventory(string item, bool combinedItem = false, string removeUseItem = "", int taskComplete = -1)
    {
        if (!combinedItem)
        {
            if (ItemsXML[item] != null && !InventoryList.Contains(item))
            {
                InventoryList.Add(item);
                UpdateInventory();

                StartCoroutine(IEShowItemAdded(item));
            }
        }
        else
        {
            if (InventoryList.Contains(item + "Two"))
            {
                InventoryList.Remove(item + "Two");
                InventoryList.Add(item + "Three");

                InventoryList.Remove(removeUseItem);
                UpdateInventory();

                TaskController taskController = FindObjectOfType<TaskController>();
                taskController.TaskComplete(taskComplete);

                StartCoroutine(IEShowItemAdded(item + "Three"));
            }
            else if (InventoryList.Contains(item + "One"))
            {
                InventoryList.Remove(item + "One");
                InventoryList.Add(item + "Two");
                UpdateInventory();

                StartCoroutine(IEShowItemAdded(item + "Two"));
            }
            else
            {
                InventoryList.Add(item + "One");
                UpdateInventory();

                StartCoroutine(IEShowItemAdded(item + "One"));
            }
        }

        Singleton<GameState>.Instance.inventory = InventoryList;
    }

    private IEnumerator IEShowItemAdded(string item)
    {
        EventObject.gameObject.SetActive(true);

        EventObject.sprite = Resources.Load<Sprite>("Images/Interface/InventoryEvent/" +
            Singleton<ToolBox>.Instance.GameLanguage + "/Panel");

        Image itemImage = EventObject.transform.GetChild(0).GetComponent<Image>();
        itemImage.sprite = Resources.Load<Sprite>("Images/Inventory/" + item);

        Color color = EventObject.color;
        for (float f = 0f; f < 1f; f += 0.02f)
        {
            color.a = f;
            EventObject.color = color;
            itemImage.color = color;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(2f);
        for (float f = 1f; f > 0f; f -= 0.02f)
        {
            color.a = f;
            EventObject.color = color;
            itemImage.color = color;
            yield return new WaitForFixedUpdate();
        }
        color.a = 0f;
        EventObject.color = color;
        itemImage.color = color;
        EventObject.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
    }

    public void RemoveItemToInventory(string nameItem)
    {
        if (ItemsXML[nameItem] != null && InventoryList.Contains(nameItem))
        {
            InventoryList.Remove(nameItem);
            Singleton<GameState>.Instance.inventory = InventoryList;

            UpdateInventory();
        }
    }

    public void RemoveAllItemToInventory()
    {
        InventoryList = new List<string>();
        Singleton<GameState>.Instance.inventory = InventoryList;

        UpdateInventory();
    }

    public void UpdateInventory()
    {
        for (int i = 0; i < InventoryItemsCells.Length; i++)
        {
            InventoryItemsCells[i].SetActive(false);
        }

        if (InventoryList.Contains(""))
        {
            InventoryList.Remove("");
            Debug.Log("<color=blue>Удален фиг знает от куда взявшийся предмет</color>");
            Singleton<GameState>.Instance.inventory = InventoryList;
        }

        if (InventoryList.Count != 0)
        {
            for (int i = 0; i < InventoryItemsCells.Length; i++)
            {
                int num = (CurrentPage - 1) * InventoryItemsCells.Length + i;

                if (InventoryList.Count > num)
                {
                    InventoryItemsCells[num % InventoryItemsCells.Length].SetActive(true);
                    InventoryItemsCells[num % InventoryItemsCells.Length].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Inventory/" + InventoryList[num]);
                }
            }
        }
    }

    public void InformationAboutItem(Image item)
    {
        ItemInfo.sprite = item.sprite;
        string descriptionItem = ItemsXML[item.sprite.name].InnerText;
        DescriptionItem.text = descriptionItem;
    }

    public void CleanItemInfo()
    {
        ItemInfo.sprite = Resources.Load<Sprite>("Images/Inventory/Zero");
        DescriptionItem.text = "";
    }

    public bool IsItemInInventory(string item)
    {
        if (item == string.Empty) { return false; }

        if (InventoryList.Contains(item)) { return true; }

        return false;
    }

    public bool IsItemsInInventory(string[] items)
    {
        if (items.Length == 0) { return false; }

        for (int i = 0; i < items.Length; i++)
        {
            if (!IsItemInInventory(items[i]))
            {
                return false;
            }
        }

        return true;
    }

    public void ItemTake()
    {
        if (ItemInfo.sprite.name != "Zero")
        {
            ItemTaken = ItemInfo.sprite.name;
            OpenInventoryButton.SetItemTaken(ItemTaken);
            transform.GetChild(0).gameObject.SetActive(false);
            CleanItemInfo();
        }
    }

    public void RemoveItemTaken()
    {
        ItemTaken = "Zero";
        OpenInventoryButton.RemoveItemTaken();
    }

    public bool IsItemTaken(string item)
    {
        if (ItemTaken != "Zero")
        {
            if (ItemTaken == item)
            {
                return true;
            }
        }

        return false;
    }

    public IEnumerator IEShowLore()
    {
        EventObject.gameObject.SetActive(true);

        EventObject.sprite = Resources.Load<Sprite>("Images/Interface/LoreEvent/" +
            Singleton<ToolBox>.Instance.GameLanguage + "/Panel");

        Color color = EventObject.color;
        for (float f = 0f; f < 1f; f += 0.02f)
        {
            color.a = f;
            EventObject.color = color;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(2f);
        for (float f = 1f; f > 0f; f -= 0.02f)
        {
            color.a = f;
            EventObject.color = color;
            yield return new WaitForFixedUpdate();
        }
        color.a = 0f;
        EventObject.color = color;
        EventObject.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
    }
}
