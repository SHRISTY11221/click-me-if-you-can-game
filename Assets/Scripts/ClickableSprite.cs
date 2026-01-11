using UnityEngine;

public class ClickableSprite : MonoBehaviour
{
    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnMouseDown()
    {
        GameManager.instance.SelectSprite(sr);
    }
}
