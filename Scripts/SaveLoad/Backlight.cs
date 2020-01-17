using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Backlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject TargetImage;

    public Sprite defaultSprite;
    public Sprite lightSprite;

    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;

        if (TargetImage == null)
        {
            TargetImage = gameObject;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TargetImage.GetComponent<Image>().sprite = lightSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TargetImage.GetComponent<Image>().sprite = defaultSprite;
    }
}