using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIClickable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static Texture2D ActiveTexture;
    public static Texture2D DefaultTexture;
    public static CursorMode CursorMode = CursorMode.Auto;
    public static Vector2 HotSpot = new Vector2(2f, 2f);

    void Start()
    {
        ActiveTexture = Resources.Load<Texture2D>("Images/Cursors/ActiveCursor");
        DefaultTexture = Resources.Load<Texture2D>("Images/Cursors/DefaultCursor");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<Button>().enabled)
        {
            Cursor.SetCursor(ActiveTexture, HotSpot, CursorMode);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GetComponent<Button>().enabled)
        {
            Cursor.SetCursor(DefaultTexture, HotSpot, CursorMode);
        }
    }
}