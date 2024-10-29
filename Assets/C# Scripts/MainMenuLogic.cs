using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuLogic : MonoBehaviour
{
    public Button playButton;
    public Button exitButton;
    public Button newGameButton;
    public Button loadGameButton;
    public Button backButton;
    public Button miniGameButton;
    public GameObject mainMenu;
    void Start()
    {
        if (playButton == null || exitButton == null || newGameButton == null ||
        loadGameButton == null || backButton == null || miniGameButton == null)
        {
            Debug.LogError("One or more button references are not assigned in the Inspector.");
            return; 
        }
    
        playButton.onClick.AddListener(OnPlayButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
        newGameButton.onClick.AddListener(OnNewGameButtonClick);
        loadGameButton.onClick.AddListener(OnLoadGameButtonClick);
        miniGameButton.onClick.AddListener(OnMiniGameButtonClick);
    }

    public void OnPlayButtonClick()
    {
        Debug.Log("Play");
    }
    
    public void OnExitButtonClick()
    {
        Application.Quit();
    }

    public void OnNewGameButtonClick()
    {
        Debug.Log("New Game");
        // Load a new game scene
    }

    public void OnLoadGameButtonClick()
    {
        Debug.Log("Load Game");
        // Load a saved game code
    }

    public void OnMiniGameButtonClick()
    {
        SceneManager.LoadScene("MiniGameScene");// calling minigame 
    }

    public void BackButtonCLicked()
    {

    }

    

    public void CreditsButtonClicked()
    {
        Debug.Log("Credits");
    }
    
    
}
