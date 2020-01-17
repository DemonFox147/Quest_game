using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
    public Transform BGPupil;

    void Update()
    {
        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(BGPupil.position);
        transform.position = BGPupil.position + dir.normalized * 0.4f;
    }
}