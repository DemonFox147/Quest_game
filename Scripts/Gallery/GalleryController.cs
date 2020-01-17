using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryController : MonoBehaviour
{
    public GameObject FullScreenImage;

    public GameObject FullScreenSecretCG;

    [Space]
    public Image[] BGCGCells;

    private GameObject arrowLeft;
    private GameObject arrowRight;

    private int CurrentPage;

    private string modeGallery;

    public string[] CurrentCategory;

    public Dictionary<int, string[]> BGDictionaty;
    public Dictionary<int, string[]> CGDictionaty;

    public Dictionary<int, bool> BGDictionatyActive;
    public Dictionary<int, bool> CGDictionatyActive;

    void Start()
    {
        BGDictionaty = Singleton<ToolBox>.Instance.GalleryOpen.BGDictionaty;
        CGDictionaty = Singleton<ToolBox>.Instance.GalleryOpen.CGDictionaty;

        BGDictionatyActive = Singleton<ToolBox>.Instance.GalleryOpen.BGDictionatyActive;
        CGDictionatyActive = Singleton<ToolBox>.Instance.GalleryOpen.CGDictionatyActive;

        GameObject pagesController = GameObject.Find("PagesController");
        arrowLeft = pagesController.transform.GetChild(0).gameObject;
        arrowRight = pagesController.transform.GetChild(1).gameObject;

        Transform BGCGCellsImages = GameObject.Find("BG_CG_Cells").transform;
        BGCGCells = new Image[BGCGCellsImages.childCount];

        for (int i = 0; i < BGCGCells.Length; i++)
        {
            BGCGCells[i] = BGCGCellsImages.GetChild(i).GetComponent<Image>();
        }

        CurrentPage = 1;
        modeGallery = "BG";

        ChangeCurrentPage(0);      
    }

    public void ModeGallery(string mode)
    {
        if (modeGallery == mode) { return; }

        modeGallery = mode;
        CurrentPage = 1;
        ChangeCurrentPage(0);
    }

    public void ChangeCurrentPage(int page)
    {
        CurrentPage += page;

        ReloadPage();

        arrowLeft.GetComponent<Button>().enabled = true;
        arrowRight.GetComponent<Button>().enabled = true;

        arrowLeft.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Gallery/Interface/ArrowLeftLight");
        arrowRight.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Gallery/Interface/ArrowRightLight");

        if (CurrentPage == 1)
        {
            arrowLeft.GetComponent<Button>().enabled = false;
            arrowLeft.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Gallery/Interface/ArrowLeftDef");
        }
        else if ((CurrentPage == Math.Ceiling(BGDictionatyActive.Count / 8f) && modeGallery == "BG") || (CurrentPage == Math.Ceiling(CGDictionatyActive.Count / 8f) && modeGallery == "CG"))
        {
            arrowRight.GetComponent<Button>().enabled = false;
            arrowRight.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Gallery/Interface/ArrowRightDef");
        }
    }

    public void ReloadPage()
    {
        for(int i = 0; i < BGCGCells.Length; i++)
        {
            BGCGCells[i].gameObject.SetActive(false);
        }

        if (modeGallery == "BG")
        {
            int cellsMin = BGCGCells.Length * (CurrentPage - 1);
            int cellsMax = BGCGCells.Length * CurrentPage;

            for (int i = cellsMin; i < cellsMax; i++)
            {
                if (i < BGDictionatyActive.Count)
                {
                    if (BGDictionatyActive[i])
                    {
                        BGCGCells[i % BGCGCells.Length].sprite = Resources.Load<Sprite>("Images/BG_CG/" + BGDictionaty[i][0]);
                        BGCGCells[i % BGCGCells.Length].gameObject.SetActive(true);
                    }
                }
            }
        }

        if (modeGallery == "CG")
        {
            int cellsMin = BGCGCells.Length * (CurrentPage - 1);
            int cellsMax = BGCGCells.Length * CurrentPage;

            for (int i = cellsMin; i < cellsMax; i++)
            {
                if (i < CGDictionatyActive.Count)
                {
                    if (CGDictionatyActive[i])
                    {
                        BGCGCells[i % BGCGCells.Length].sprite = Resources.Load<Sprite>("Images/BG_CG/" + CGDictionaty[i][0]);
                        BGCGCells[i % BGCGCells.Length].gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    public void OpenFullScreenImage(Image image)
    {
        FullScreenImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/BG_CG/" + image.sprite.name);
        FullScreenImage.SetActive(true);

        string nameFullImage = image.sprite.name;

        if (modeGallery == "BG")
        {
            for (int i = 0; i < BGDictionaty.Count; i++)
            {
                if (BGDictionaty[i][0] == nameFullImage)
                {
                    CurrentCategory = BGDictionaty[i];
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < CGDictionaty.Count; i++)
            {
                if (CGDictionaty[i][0] == nameFullImage)
                {
                    CurrentCategory = CGDictionaty[i];
                    break;
                }
            }
        }
    }

    public void NextFullScreenImage()
    {
        string nameFullImage = FullScreenImage.GetComponent<Image>().sprite.name;

        int indexNum = Array.IndexOf(CurrentCategory, nameFullImage);

        if (indexNum != CurrentCategory.Length - 1)
        {
            FullScreenImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/BG_CG/" + CurrentCategory[indexNum + 1]);
        }
        else
        {
            CloseFullScreenImage();
        }  
    }

    public void CloseFullScreenImage()
    {
        FullScreenImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Gallery/Zero");
        FullScreenImage.SetActive(false);
    }

    public void OpenFullScreenSecretCG(Sprite secretCG)
    {
        FullScreenSecretCG.GetComponent<Image>().sprite = secretCG;
        FullScreenSecretCG.SetActive(true);
    }

    public void CloseFullScreenSecretCG()
    {
        FullScreenSecretCG.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Gallery/Zero");
        FullScreenSecretCG.SetActive(false);
    }
}