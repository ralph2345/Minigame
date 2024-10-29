using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Level currentLevel;
    public Level[] levels;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadCurrentLevel(Level[] allLevels)
    {
        int selectedLevelNumber = PlayerPrefs.GetInt("selectedLevel", 1);
        
        selectedLevelNumber = Mathf.Clamp(selectedLevelNumber, 1, allLevels.Length);
        currentLevel = allLevels[selectedLevelNumber - 1];
    }
}
