using UnityEngine;
using UnityEngine.UI;

public class LoreInfoButton : MonoBehaviour
{
    [HideInInspector]
    public LoreController LoreController;

    [HideInInspector]
    public string LoreObject;

    [HideInInspector]
    public string LoreCategory;

    public void OnClick()
    {
        LoreController.ShowInformationAboutObject(LoreObject, LoreCategory, GetComponent<Text>());
    }
}
