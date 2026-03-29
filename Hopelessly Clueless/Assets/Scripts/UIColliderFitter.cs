using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(BoxCollider2D))] 
public class UIColliderFitter : MonoBehaviour
{
    private RectTransform rectTransform;
    private BoxCollider2D boxCollider;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        boxCollider = GetComponent<BoxCollider2D>();
        FitCollider();
    }

 
    
    void OnRectTransformDimensionsChange()
    {
        FitCollider();
    }

    void FitCollider()
    {
        if (rectTransform != null && boxCollider != null)
        {
            // Force the physics box to exactly match the width and height of the UI Rect
            boxCollider.size = rectTransform.rect.size;
        }
    }
}