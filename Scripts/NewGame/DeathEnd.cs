using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeathEnd : MonoBehaviour
{
    [HideInInspector]
    public ScriptReferencesPool srp;

    public IEnumerator IEDeathScene()
    {
        srp.MusicController.LoadMusic("Death");

        srp.ScenarioParser.InterfacePanelSetActive(false);

        GameObject BgGO = transform.GetChild(0).gameObject;
        Image BgImage = BgGO.GetComponent<Image>();

        BgImage.sprite = Resources.Load<Sprite>("Images/Death/YouDied" + Singleton<ToolBox>.Instance.GameLanguage);

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

        yield return new WaitForSeconds(13f);

        FindObjectOfType<ScreenFader>().LoadLevel("Menu");
    }
}