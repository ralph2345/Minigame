using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.SceneManagement;


public class GameEndController : MonoBehaviour
{
    public GameObject congratulationsPanelObject;  
    public GameObject gameOverPanelObject;  
    public GameObject confettiEffect; 
    public CanvasGroup congratulationsCanvasGroup;  
    public RectTransform congratulationsMessage;  
    public CanvasGroup gameOverCanvasGroup;  
    public RectTransform gameOverMessage;  
    public Button gameOverCloseButton;
    public Button nextLevelButton;
    private bool isGameOverVisible = false;
    void OnDestroy()
    {

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveListener(LoadNextLevel);
        }

        if (gameOverCloseButton != null)
        {
            gameOverCloseButton.onClick.RemoveListener(HideGameOver);
        }

        DOTween.Kill(congratulationsCanvasGroup);
        DOTween.Kill(gameOverCanvasGroup);

        DOTween.Kill(congratulationsMessage);
        DOTween.Kill(gameOverMessage);

    }

    void Start()
    {
        
        congratulationsPanelObject.SetActive(false);
        gameOverPanelObject.SetActive(false);
        nextLevelButton.onClick.AddListener(LoadNextLevel);

        SetCanvasGroupState(congratulationsCanvasGroup, 0, false);
        SetCanvasGroupState(gameOverCanvasGroup, 0, false);

        confettiEffect.SetActive(false);
        gameOverCloseButton.onClick.AddListener(HideGameOver);
    }

    private void SetCanvasGroupState(CanvasGroup canvasGroup, float alpha, bool interactable)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
            canvasGroup.interactable = interactable;
            canvasGroup.blocksRaycasts = interactable;
        }
    }
    
    public void ShowCongratulations()
    {
        // Activate the Congratulations panel
        congratulationsPanelObject.SetActive(true);

        // Activate confetti
        confettiEffect.SetActive(true);

        SetCanvasGroupState(congratulationsCanvasGroup, 0, true); 

        if (congratulationsCanvasGroup != null)
        {
            congratulationsCanvasGroup.DOFade(1, 0.5f);  
        }

        if (congratulationsMessage != null)
        {
            congratulationsMessage.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBack);
        }

        nextLevelButton.gameObject.SetActive(true);
    }

    private void HideCongratulations()
    {
        
        confettiEffect.SetActive(false);
        nextLevelButton.gameObject.SetActive(false);

        if (congratulationsCanvasGroup == null)
        {
            Debug.LogWarning("Congratulations CanvasGroup is null");
            return; 
        }

        if (congratulationsMessage != null)
        {
            congratulationsMessage.DOScale(Vector3.zero, 0.7f).SetEase(Ease.InBack);
        }

        congratulationsCanvasGroup.DOFade(0, 0.5f).OnComplete(() =>
        {
            SetCanvasGroupState(congratulationsCanvasGroup, 0, false);
        });

    }

    public void ShowGameOver()
    {
        Debug.Log("Showing Game Over Panel");
        if (isGameOverVisible) return;
        isGameOverVisible = true;
        // Activate the Game Over panel
        gameOverPanelObject.SetActive(true);
        SetCanvasGroupState(gameOverCanvasGroup, 0, true);

        gameOverCanvasGroup.DOFade(1, 0.5f); 
        gameOverMessage.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBack);  

    }

    private void HideGameOver()
    {
        if (!isGameOverVisible) return;
        isGameOverVisible = false;
     
        gameOverCanvasGroup.DOFade(0, 0.5f).OnComplete(() =>
        {
            SetCanvasGroupState(gameOverCanvasGroup, 0, false);  
        });

        gameOverMessage.DOScale(Vector3.zero, 0.7f).SetEase(Ease.InBack);

    }

    private void LoadNextLevel()
    {

        HideCongratulations();
        DOVirtual.DelayedCall(0.5f, () =>
        {
            int currentLevelNumber = GameManager.Instance.currentLevel.levelNumber;
            PlayerPrefs.SetInt("selectedLevel", currentLevelNumber + 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("MinigameScene");
        });

    }
}
