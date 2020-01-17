using UnityEngine;
using UnityEngine.EventSystems;

//Кинь на любой УИ объект который должен будет блокировать клик.

public class UIClickBlocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool allowClick = true;

    public void SetStateAllowClick(bool state)
    {
        allowClick = state;
    }

    //Метод наведение на УИ элемент
    public void OnPointerEnter(PointerEventData eventData)
    {
        allowClick = false;
    }

    //Метод выхода из УИ элемента
    public void OnPointerExit(PointerEventData eventData)
    {
        allowClick = true;
    }
}