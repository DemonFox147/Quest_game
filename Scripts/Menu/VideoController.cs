using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoController : MonoBehaviour
{
    // private VideoPlayer VideoPlayer;

    // private Image Logo;

    void Awake()
    {
        Singleton<ToolBox>.Instance.StartToolBox();
    }

    void Start()
    {
        StartCoroutine(StartAnimation());
    }

    /*private IEnumerator VideoStart()
    {
        yield return new WaitForSeconds(0.5f);
        VideoPlayer.Play();
        yield return new WaitForSeconds((float)VideoPlayer.clip.length - 1);
        SceneManager.LoadScene(1);
    }

    private IEnumerator ShowLogo()
    {
        Color _color = Logo.color;
        for (float f = 0; f <= 1; f += 0.1f)
        {
            _color.a = f;
            Logo.color = _color;
            yield return new WaitForSeconds(0.1f);
        }
        _color.a = 1;
        Logo.color = _color;

        yield return new WaitForSeconds(3f);

        for (float f = 1; f >= 0; f -= 0.1f)
        {
            _color.a = f;
            Logo.color = _color;
            yield return new WaitForSeconds(0.1f);
        }
        _color.a = 0;
        Logo.color = _color;
        SceneManager.LoadScene(1);
    }*/

    private IEnumerator StartAnimation()
    {
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene(1);
    }
}