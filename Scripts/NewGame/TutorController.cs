using UnityEngine;
using UnityEngine.UI;

public class TutorController : MonoBehaviour
{
    private GameObject hint;

    void Start()
    {
        hint = transform.GetChild(0).gameObject;

        if (Singleton<GameState>.Instance.tutorMode == "True")
        {
            ShowTutor(Singleton<GameState>.Instance.nameTutor);
        }
    }

    public void ShowTutor(string nameTutor)
    {
        if (nameTutor != "Zero")
        {
            hint.SetActive(true);
            hint.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Tutor/" + Singleton<ToolBox>.Instance.GameLanguage + "/" + nameTutor);

            Singleton<GameState>.Instance.tutorMode = "True";
            Singleton<GameState>.Instance.nameTutor = nameTutor;
        }
        else
        {
            Singleton<GameState>.Instance.tutorMode = "False";
            Singleton<GameState>.Instance.nameTutor = "Zero";
            hint.SetActive(false);
        }
    }
}
