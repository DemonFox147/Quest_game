using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private GameObject GalleryLore;

    void Awake()
    {
        Singleton<ToolBox>.Instance.StartToolBox();
    }

    void Start()
    {
        GalleryLore = GameObject.Find("GalleryLore");

        if (PlayerPrefs.GetString("openGallery") == "True")
        {
            GalleryLore.GetComponent<Button>().enabled = true;
            GalleryLore.GetComponent<Image>().sprite = 
                Resources.Load<Sprite>("Images/Interface/Menu/" + Singleton<ToolBox>.Instance.GameLanguage + "/Gallery");
        }
    }

    public void LoadMode()
    {
        Singleton<ToolBox>.Instance.ModeSaveLoad = "Load";
    }

    public void ResetState()
    {
        Singleton<GameState>.Instance.ResetState();
    }
}