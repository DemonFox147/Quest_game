using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OpenInventoryButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject InventoryMenu;
    public Text TaskTip;

    public Sprite Backpack;
    public Sprite CellForItem;
    public Sprite RemoveItem;

    private Image openInventory;
    private Image item;

    private string itemName;

    public Inventory Inventory;

    void Start()
    {
        itemName = "Zero";
        openInventory = GetComponent<Image>();
        item = transform.GetChild(0).GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemName != "Zero")
        {
            openInventory.sprite = RemoveItem;
            item.gameObject.SetActive(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (itemName != "Zero")
        {
            openInventory.sprite = CellForItem;
            item.gameObject.SetActive(true);
        }
    }

    public void OnClick()
    {
        if (openInventory.sprite.name == "Backpack")
        {
            InventoryMenu.SetActive(true);
        }
        else
        {
            RemoveItemTaken();
            Inventory.RemoveItemTaken();
        }
    }

    public void SetItemTaken(string itemName)
    {
        this.itemName = itemName;
        openInventory.sprite = CellForItem;
        item.sprite = Resources.Load<Sprite>("Images/Inventory/" + itemName);
    }

    public void RemoveItemTaken()
    {
        itemName = "Zero";
        item.gameObject.SetActive(true);
        openInventory.sprite = Backpack;
        item.sprite = Resources.Load<Sprite>("Images/Inventory/Zero");
    }
}
