using UnityEngine;
using UnityEngine.UI;

public class NotesController : MonoBehaviour
{
    private GameObject note;

    private bool pressButton;

    void Start()
    {
        note = transform.GetChild(0).gameObject;

        pressButton = false;

        if (Singleton<GameState>.Instance.notesMode == "True")
        {
            ShowNote(Singleton<GameState>.Instance.nameNote);
        }
    }

    void Update()
    {
        if (pressButton)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                note.SetActive(false);

                pressButton = false;

                note.GetComponent<Button>().enabled = true;
            }
        }
    }

    public void ShowNotePressButton(string nameNote)
    {
        pressButton = true;

        note.SetActive(true);
        note.GetComponent<Image>().sprite =
            Resources.Load<Sprite>("Images/Notes/" + Singleton<ToolBox>.Instance.GameLanguage + "/" + nameNote);

        note.GetComponent<Button>().enabled = false;
    }

    public void ShowNote(string nameNote)
    {
        if (nameNote != "Zero")
        {
            note.SetActive(true);
            note.GetComponent<Image>().sprite =
                Resources.Load<Sprite>("Images/Notes/" + Singleton<ToolBox>.Instance.GameLanguage + "/" + nameNote);

            Singleton<GameState>.Instance.notesMode = "True";
            Singleton<GameState>.Instance.nameNote = nameNote;
        }
        else
        {
            Singleton<GameState>.Instance.notesMode = "False";
            Singleton<GameState>.Instance.nameNote = "Zero";
            note.SetActive(false);
        }
    }

    public void OnClick()
    {
        Singleton<GameState>.Instance.notesMode = "False";
        Singleton<GameState>.Instance.nameNote = "Zero";
        note.SetActive(false);
        pressButton = false;
    }
}