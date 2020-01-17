using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public GameObject blink;

    private Image sBlink;

    void Awake()
    {
        sBlink = blink.GetComponent<Image>();
        blink.SetActive(true);
    }

    void Start()
    {
        StartCoroutine(OffBlink());
    }

    public IEnumerator OffBlink()
    {
        Color color = sBlink.color;
        for (float f = 1f; f >= 0f; f -= 0.02f)
        {
            color.a = f;
            sBlink.color = color;
            yield return new WaitForSeconds(0.01f);
        }
        color.a = 0;
        sBlink.color = color;
        blink.SetActive(false);
    }

    public IEnumerator OnBlink(string nameScene)
    {
        blink.SetActive(true);
        Color color = sBlink.color;
        for (float f = 0f; f <= 1f; f += 0.02f)
        {
            color.a = f;
            sBlink.color = color;
            yield return new WaitForSeconds(0.01f);
        }
        color.a = 1;
        sBlink.color = color;

        Time.timeScale = 1f;
        SceneManager.LoadScene(nameScene);
        Singleton<ToolBox>.Instance.OnLevelWasLoaded();
    }

    public void LoadLevel(string nameScene)
    {
        StartCoroutine(OnBlink(nameScene));
    }

    public void PrevLevel()
    {
        StartCoroutine(OnBlink(Singleton<ToolBox>.Instance.PrevLevel));
    }

    public void Exit()
    {
        try
        {
            SteamManager steamManager = FindObjectOfType<SteamManager>();
            steamManager.OnDestroy();
        }
        catch { }

        Application.Quit();
    }
}