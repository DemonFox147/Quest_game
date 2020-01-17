using UnityEngine;
using UnityEngine.UI;

public class AlphaSprite : MonoBehaviour
{
    private const float alphaHit = 0.1f;

    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = alphaHit;
    }
}