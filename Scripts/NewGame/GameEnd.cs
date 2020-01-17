using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameEnd : MonoBehaviour
{
    [HideInInspector]
    public ScriptReferencesPool srp;

    public IEnumerator IEStartGameEnd()
    {
        srp.MusicController.LoadMusic("DemoEnd");

        srp.ScenarioParser.InterfacePanelSetActive(false);

        GameObject BgGO = transform.GetChild(0).gameObject;
        Image BgImage = BgGO.GetComponent<Image>();

        BgImage.sprite = Resources.Load<Sprite>("Images/GameEnd/DemoEnd" + Singleton<ToolBox>.Instance.GameLanguage);

        BgGO.SetActive(true);

        Color color = BgImage.color;

        for (float f = 0f; f < 1f; f += 0.05f)
        {
            color.a = f;
            BgImage.color = color;
            yield return new WaitForFixedUpdate();
        }
        color.a = 1;
        BgImage.color = color;

        Singleton<GameState>.Instance.ResetState();

        yield return new WaitForSeconds(10f);

        FindObjectOfType<ScreenFader>().LoadLevel("Menu");
    }
}