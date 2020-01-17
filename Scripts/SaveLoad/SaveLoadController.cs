using UnityEngine;
using UnityEngine.UI;

public class SaveLoadController : MonoBehaviour
{
    private SaveLoadCell SaveLoadCell;

    private GameObject arrowLeft;
    private GameObject arrowRight;
    private Image pageNumber;

    public GameObject DeleteMenu;
    public GameObject ReWriteMenu;

    public int ActiveCell
    {
        private get;
        set;
    }

    public string ModeSaveLoad
    {
        get;
        private set;
    }

    public int CurrentPage
    {
        get;
        private set;
    }

    public string SavesType
    {
        get;
        private set;
    }

    void Start()
    {
        CurrentPage = 1;
        ModeSaveLoad = Singleton<ToolBox>.Instance.ModeSaveLoad;
        SaveLoadCell = GetComponent<SaveLoadCell>();
        SavesType = "saveFile_";

        GameObject pagesController = GameObject.Find("PagesController");
        arrowLeft = pagesController.transform.GetChild(0).gameObject;
        arrowRight = pagesController.transform.GetChild(1).gameObject;
        pageNumber = pagesController.transform.GetChild(2).GetComponent<Image>();

        ChangeCurrentPage(0);
    }

    public void ChangeCurrentPage(int changePage)
    {
        if (CurrentPage + changePage >= 1 && CurrentPage + changePage <= 3)
        {
            CurrentPage += changePage;

            SaveLoadCell.Reload();
            pageNumber.sprite = Resources.Load<Sprite>("Images/SaveLoad/" + CurrentPage);
            arrowLeft.SetActive(true);
            arrowRight.SetActive(true);

            if (CurrentPage == 1)
            {
                arrowLeft.SetActive(false);
            }
            else if (CurrentPage == 3)
            {
                arrowRight.SetActive(false);
            }
        }
    }

    public void YesDelete()
    {
        SaveLoadCell.Delete(ActiveCell);
        DeleteMenu.SetActive(false);
    }

    public void YesReWrite()
    {
        SaveLoadCell.Save(ActiveCell);
        ReWriteMenu.SetActive(false);
    }
}