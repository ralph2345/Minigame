using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public Button[] levelButtons;
    public Sprite lockedSprite;
    public Sprite unlockedSprite;
    private int highestLevel;
    public static LevelManager Instance { get; private set; }

    public Level[] levels;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }
    void Start()
    {
        SetupLevelButtons();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MinigameScene")
        {
            return;
        }
        else if (scene.name == "LevelSelectionScene")
        {
            FindLevelButtons();
            SetupLevelButtons(); 
        }
    }
    public void SetupLevelButtons()
    {
        highestLevel = PlayerPrefs.GetInt("highestLevel", 1);

        if (levelButtons.Length != levels.Length)
        {
            Debug.LogError($"Mismatch: {levelButtons.Length} buttons for {levels.Length} levels!");
            return;
        }

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] == null)
            {
                Debug.LogWarning($"Button at index {i} is missing or destroyed.");
                continue;
            }

            levelButtons[i].onClick.RemoveAllListeners();
            int levelNum = i + 1;

            if (levelNum > highestLevel)
            {
                levelButtons[i].interactable = false;
                levelButtons[i].GetComponent<Image>().sprite = lockedSprite; // Lock the level
                TMP_Text tmpText = levelButtons[i].GetComponentInChildren<TMP_Text>();
                if (tmpText != null)
                {
                    tmpText.text = "";
                }
                else
                {
                    Debug.LogError("No Text Mesh Pro button on " + i);
                }
            }
            else
            {
                levelButtons[i].interactable = true;
                TMP_Text tmpText = levelButtons[i].GetComponentInChildren<TMP_Text>();
                if (tmpText != null)
                {
                    tmpText.text = "" + levelNum; // Setting the level number for unlocked levels
                }
                else
                {
                    Debug.LogError("No Text Mesh Pro button on " + i);
                }
                levelButtons[i].GetComponent<Image>().sprite = unlockedSprite;
            }

            int capturedLevel = levelNum;
            levelButtons[i].onClick.AddListener(() => LoadLevel(capturedLevel));
        }

        Button resetButton = GameObject.Find("Reset").GetComponent<Button>();
        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners(); 
            resetButton.onClick.AddListener(ResetProgress); 
        }
        else
        {
            Debug.LogError("Reset Button not found!");
        }
    }

    private void FindLevelButtons()
    {
        levelButtons = GameObject.FindGameObjectsWithTag("LevelButton")
            .OrderBy(go => ExtractLevelNumber(go.name))
            .Select(go => go.GetComponent<Button>())
            .ToArray();

        if (levelButtons.Length == 0)
        {
            Debug.LogError("No buttons found with the tag 'LevelButton'.");
        }
    }

    private int ExtractLevelNumber(string buttonName)
    {
        string[] parts = buttonName.Split(' ');
        foreach (var part in parts)
        {
            if (int.TryParse(part, out int levelNumber))
            {
                return levelNumber;
            }
        }
        return 0;
    }

    public void LoadLevel(int levelNumber)
    {
        PlayerPrefs.SetInt("selectedLevel", levelNumber);
        PlayerPrefs.Save();

        SceneManager.LoadScene("MinigameScene");
    }

    public void ResetProgress()
    {
        Debug.Log("Reset is pressed");
        PlayerPrefs.SetInt("highestLevel", 1);
        PlayerPrefs.Save();

        FindLevelButtons();
        SetupLevelButtons();

        // Reload the current scene to reflect changes
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

}
