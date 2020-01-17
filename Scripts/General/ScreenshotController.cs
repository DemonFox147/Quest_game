using System.Collections;
using System.IO;
using UnityEngine;

public class ScreenshotController : MonoBehaviour
{
    private int Count;

    void Start()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Screenshots"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Screenshots");
            Count = 0;
        }
        else
        {
            string[] dirs = Directory.GetFiles(@"C:/Users/DemonFox147/AppData/LocalLow/GravenVisualNovels/Bewitched/Screenshots/");
            Count = dirs.Length;
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F12))
        {
           StartCoroutine(Screenshot());
        }
    }

    private IEnumerator Screenshot()
    {
        Count++;
        FindObjectOfType<CaptureScreenshot>().Grab();

        yield return new WaitForSeconds(0.1f);

        Texture2D tex = new Texture2D(Screen.width, Screen.height);
        tex.LoadImage(Singleton<GameState>.Instance.Screenshot);
        tex.Apply();
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(Application.persistentDataPath + "/Screenshots/GameScreenshot_" + Count + ".png", bytes);
    }
}
