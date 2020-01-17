using UnityEngine;

public class SceneTransitionButton : MonoBehaviour
{
    [HideInInspector]
    public SceneTransitionController STC;

    [HideInInspector]
    public string NextRoom;
    [HideInInspector]
    public string ArrowDirection;

    public void OnClick()
    {
        STC.NextScene(NextRoom, ArrowDirection);
    }
}