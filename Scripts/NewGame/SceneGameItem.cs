using UnityEngine;

public class SceneGameItem : UIClickable
{
    [HideInInspector]
    public ScriptReferencesPool srp;

    public string Chapter;

    public string AltChapter;

    public string ItemInterection;

    public string ItemIChapter;

    void Awake()
    {
        ResetValues();
    }

    public void ResetValues()
    {
        Chapter = "";
        AltChapter = "";
        ItemInterection = "";
        ItemIChapter = "";
    }

    public void OnClick()
    {
        srp.ScenarioParser.TravelMode = false;
        Singleton<GameState>.Instance.travelMode = "False";
        srp.SceneTransitionController.StopTransitionMode();
        srp.ScenarioParser.InterfacePanelSetActive(true);

        if (srp.Inventory.IsItemTaken(ItemInterection))
        {
            srp.ScenarioParser.GoNextChapter(ItemIChapter, true);
            srp.Inventory.RemoveItemTaken();
        }
        else if (AltChapter != "" && srp.ScenarioParser.IsChapterRead(Chapter))
        {
            srp.ScenarioParser.GoNextChapter(AltChapter, true);
        }
        else
        {
            srp.ScenarioParser.GoNextChapter(Chapter, true);
        }
    }
}