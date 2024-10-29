using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;


public class PauseController : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] RectTransform pausePanelRect;
    [SerializeField] float topPosY, middlePosY;
    [SerializeField] float tweenDuration;
    [SerializeField] CanvasGroup canvasGroupPanel;
    void Start()
    {

    }
    public void PauseGame()
    {
        pausePanelRect.anchoredPosition = new Vector2(pausePanelRect.anchoredPosition.x, topPosY);

        pauseMenu.SetActive(true);

        MinigameAudio.Instance.PlayPauseSound();

        PanelFadeIn();

        Debug.Log("PAUSE");
    }

    public async void ResumeGame()
    {
        await PanelFadeOut();
        pauseMenu.SetActive(false);
    }

    void PanelFadeIn()
    {
        canvasGroupPanel.DOFade(1, tweenDuration).SetUpdate(true);
        pausePanelRect.DOAnchorPosY(middlePosY, tweenDuration).SetUpdate(true);
    }

    async Task PanelFadeOut()
    {
        canvasGroupPanel.DOFade(0, tweenDuration).SetUpdate(true);
        await pausePanelRect.DOAnchorPosY(topPosY, tweenDuration).SetUpdate(true).AsyncWaitForCompletion();
    }

}
