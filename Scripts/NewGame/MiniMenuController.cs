using UnityEngine;
using UnityEngine.UI;

public class MiniMenuController : MonoBehaviour
{
    public Image OpenWoodenMiniMenu;

    public void ActivateTypeMiniMenu()
    {
        if (transform.GetChild(0).gameObject.activeSelf)
        {
            DeActivateTreeMiniMenu();
        }
        else
        {
            ActivateTreeMiniMenu();
        }
    }

    public void ActivateTypeMiniMenu(bool active)
    {
        if (active)
        {
            ActivateTreeMiniMenu();
        }
        else
        {
            DeActivateTreeMiniMenu();
        }
    }

    private void ActivateTreeMiniMenu()
    {
        OpenWoodenMiniMenu.sprite = Resources.Load<Sprite>("Images/Interface/MiniMenu/Eye_Open");

        int countT = transform.childCount;

        for (int i = 0; i < countT; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void DeActivateTreeMiniMenu()
    {
        OpenWoodenMiniMenu.sprite = Resources.Load<Sprite>("Images/Interface/MiniMenu/Eye_Close");

        int countT = transform.childCount;

        for (int i = 0; i < countT; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}