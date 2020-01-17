using UnityEngine;

public class BackdropText : MonoBehaviour
{
    private ScriptReferencesPool srp;

    void Start()
    {
        srp = FindObjectOfType<ScriptReferencesPool>();
    }

    void Update()
    {
        if (!srp.ScenarioParser.GameEnd && !srp.ScenarioParser.ExitScene 
            && !srp.ScenarioParser.TravelMode && !srp.ScenarioParser.MiniGameMode)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || srp.ScenarioParser.ChoiceMode || srp.ScenarioParser.DeathMode )
            {
                Time.timeScale = 1f;
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                SetSkipMode();
            }
            else if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                SetSkipMode();
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                SetSkipMode();
            }
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void SetSkipMode()
    {
        if (Time.timeScale == 1f)
        {
            Time.timeScale = 10f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void SetSkipMode(bool state)
    {
        if (state)
        {
            Time.timeScale = 10f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}