using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardFlip : MonoBehaviour
{
    private bool isAnimating = false; 
    private bool isShowingBack = false; 
    private RectTransform rectTransform;
    private Image image;
    public Sprite frontSprite;  
    public Sprite backSprite;   

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    public void Flip()
    {
        if (isAnimating) return;  
        isAnimating = true;
        // rotating 90 degrees only
        rectTransform.DORotate(new Vector3(0, 90, 0), 0.15f, RotateMode.LocalAxisAdd).OnComplete(() =>
        {
            image.sprite = isShowingBack ? frontSprite : backSprite;
            isShowingBack = !isShowingBack;  

            rectTransform.DORotate(new Vector3(0, 90, 0), 0.15f, RotateMode.LocalAxisAdd).OnComplete(() =>
            {
                isAnimating = false;  
            });
        });
    }

    public void ResetFlipState()
    {
        isShowingBack = false;
        image.sprite = frontSprite;  // Reset to front image
        rectTransform.localRotation = Quaternion.Euler(0, 0, 0);  
    }
}
