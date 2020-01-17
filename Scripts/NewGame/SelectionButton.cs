using UnityEngine;

public class SelectionButton : MonoBehaviour
{
    public string Chapter;

    public void OnClick()
    {
        FindObjectOfType<ScriptReferencesPool>().SelectionController.FindNextChapter(Chapter);
        Chapter = "";
    }
}