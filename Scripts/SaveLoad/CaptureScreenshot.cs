using UnityEngine;

public class CaptureScreenshot : MonoBehaviour
{
    private bool _grab;

    public Texture2D Screenshot
    {
        get;
        private set;
    }

    private void Awake()
    {
        Screenshot = new Texture2D(Screen.width, Screen.height);
    }

    private void OnPostRender()
    {
        if (_grab)
        {
            _grab = false;
            Screenshot.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
            Screenshot.Apply();
            Singleton<GameState>.Instance.ScreenConvert(Screenshot);
        }
    }

    public void Grab()
    {
        _grab = true;
    }
}