using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private Sprite bgImage;  // Background image 
    private Sprite[] currentBackImages;
    public Sprite[] puzzles;  // All sprites in the puzzle folder
    public List<Sprite> gamePuzzles = new List<Sprite>();  // Shuffled puzzle images for the game
    public List<Button> btn = new List<Button>();  // List of puzzle buttons

    private bool firstGuess, secondGuess;
    private int countGuesses;
    private int countCorrectGuesses;
    private int gameGuesses;
    private int firstGuessIndex, secondGuessIndex;
    private Sprite firstGuessPuzzle, secondGuessPuzzle;
    public Button refreshButton, backButton, backButton2;
    private int maxMoves;
    private int remainingMoves;
    public TMP_Text movesText;
    public TMP_Text currentLevelText;
    private bool isCheckingMatch = false;
    public GameEndController gameEndController;  // Reference to GameEndController
    private LevelManager levelManager;
    private bool isGameOver = false;
    private int selectedLevel;
    public CanvasGroup transitionCanvas;

    // Animation Settings
    [Header("Animation Settings")]
    public float animationDuration = 0.5f;
    public float delayBetweenCards = 0.2f;
    public Ease animationEase = Ease.OutBack;

    void Awake()
    {

        levelManager = LevelManager.Instance;

        if (levelManager == null)
        {
            Debug.LogError("LevelManager instance is null! Ensure it is set up in the scene.");
            return;
        }

        GameManager.Instance.LoadCurrentLevel(levelManager.levels);
        Level currentLevel = GameManager.Instance.currentLevel;

        if (currentLevel != null)
        {
            selectedLevel = currentLevel.levelNumber;
            LoadBackImagesForLevel(selectedLevel);
        }
        else
        {
            Debug.LogError("Current level data not found! Make sure GameManager is correctly loading the level.");
        }
    }


    void OnDestroy()
    {
        DOTween.Kill(this);
    }
    private void LoadBackImagesForLevel(int levelNumber)
    {
        string folderPath = "";
        levelManager = LevelManager.Instance;


        if (levelNumber >= 1 && levelNumber <= 5)
        {
            folderPath = "Sprites/Level1to5";
        }
        else if (levelNumber >= 6 && levelNumber <= 10)
        {
            folderPath = "Sprites/Level6to10";
        }
        else if (levelNumber >= 11 && levelNumber <= 15)
        {
            folderPath = "Sprites/Level11to15";
        }


        puzzles = Resources.LoadAll<Sprite>(folderPath);

        if (puzzles.Length > 0)
        {
            Debug.Log($"Loaded {puzzles.Length} back images for level {levelNumber}");
            puzzles = puzzles.OrderBy(x => Random.value).ToArray();
        }
        else
        {
            Debug.LogError("No back images found in the specified folder.");
        }
    }


    void Start()
    {
        try
        {
            if (puzzles == null || puzzles.Length == 0)
            {
                Debug.LogError("Puzzles array is null or empty! Please check your sprite loading logic.");
                return;
            }

            if (MinigameAudio.Instance != null)
            {
                MinigameAudio.Instance.PlayBackgroundMusic();
            }

            levelManager = LevelManager.Instance;
            if (levelManager == null)
            {
                Debug.LogError("LevelManager not found!");
                return;
            }

            GameManager.Instance.LoadCurrentLevel(levelManager.levels);
            Level currentLevel = GameManager.Instance.currentLevel;

            maxMoves = currentLevel.maxMoves;
            remainingMoves = maxMoves;
            UpdateMovesText();

            UpdateCurrentLevelText(currentLevel.levelNumber);

            GetButton(currentLevel.numberOfCards);
            AddListeners();
            SetupGame();
            Shuffle(gamePuzzles);
            gameGuesses = gamePuzzles.Count / 2;  // Total pairs of cards

            refreshButton.onClick.AddListener(RefreshGame);
            backButton.onClick.AddListener(GoBackLevelSelection);
            backButton2.onClick.AddListener(GoBackLevelSelection);
            RefreshGame();

            AnimateCardsEntrance();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("An error occurred in GameController.Start(): " + ex.Message);
        }
    }

    private void UpdateCurrentLevelText(int levelNumber)
    {
        currentLevelText.fontSize = 60;
        currentLevelText.text = "Level " + levelNumber.ToString();
    }

    void GetButton(int numberOfCards)
    {
        Debug.Log("Instantiating " + numberOfCards + " puzzle buttons.");

        try
        {

            foreach (Button button in btn)
            {
                Destroy(button.gameObject);
            }
            btn.Clear();


            Transform puzzleField = GameObject.Find("Puzzle Field")?.transform;
            if (puzzleField == null)
            {
                Debug.LogError("Puzzle Field not found!");
                return;
            }

            GameObject buttonPrefab = Resources.Load<GameObject>("Prefabs/0");
            if (buttonPrefab == null)
            {
                Debug.LogError("PuzzleButton prefab not found in Resources/Prefabs!");
                return;
            }


            for (int i = 0; i < numberOfCards; i++)
            {
                GameObject buttonObj = Instantiate(buttonPrefab);
                buttonObj.name = "Card_" + (i + 1).ToString();
                buttonObj.transform.SetParent(puzzleField, false);
                buttonObj.tag = "PuzzleButton";

                buttonObj.transform.localScale = Vector3.zero;

                Button button = buttonObj.GetComponent<Button>();
                if (button != null)
                {
                    btn.Add(button);
                }
                else
                {
                    Debug.LogError("Button component not found on instantiated puzzle object!");
                }
            }
            AnimateCardsEntrance();

        }
        catch (System.Exception ex)
        {
            Debug.LogError("An error occurred in GetButton(): " + ex.Message);
        }


        Debug.Log("Total buttons instantiated: " + btn.Count);
    }

    void SetupGame()
    {
        Debug.Log("Setting up game with " + btn.Count + " buttons.");
        try
        {

            gamePuzzles.Clear();
            int pairs = btn.Count / 2;

            if (puzzles == null || puzzles.Length == 0)
            {
                Debug.LogError("Puzzles array is null or empty!");
                return;
            }

            for (int i = 0; i < pairs; i++)
            {
                if (i >= puzzles.Length)
                {
                    Debug.LogError("Not enough sprites in the puzzle array to create pairs!");
                    return;
                }

                gamePuzzles.Add(puzzles[i]);
                gamePuzzles.Add(puzzles[i]);
            }

            Shuffle(gamePuzzles);

            for (int i = 0; i < btn.Count; i++)
            {

                CardFlip cardFlip = btn[i].GetComponent<CardFlip>();
                if (cardFlip == null)
                {
                    Debug.LogError("CardFlip component missing on button " + i);
                    return;
                }

                cardFlip.frontSprite = bgImage;
                cardFlip.backSprite = gamePuzzles[i];
                btn[i].image.color = new Color(1, 1, 1, 1);
                btn[i].transform.localScale = Vector3.zero;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("An error occurred in SetupGame(): " + ex.Message);
        }
        Debug.Log("Game setup completed with " + gamePuzzles.Count + " puzzles.");

    }

    void AddListeners()
    {
        foreach (Button buttons in btn)
        {
            buttons.onClick.AddListener(() => PickAPuzzle());
        }
    }

    public void PickAPuzzle()
    {

        if (isCheckingMatch || isGameOver) return;

        Button selectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        int selectedButtonIndex = btn.IndexOf(selectedButton);

        MinigameAudio.Instance.PlayCardClickSound();

        if (selectedButton.interactable == false) return;

        if (!firstGuess)
        {
            firstGuess = true;
            firstGuessIndex = selectedButtonIndex;
            firstGuessPuzzle = gamePuzzles[firstGuessIndex];
            btn[firstGuessIndex].GetComponent<CardFlip>().Flip();  // Flip the first card
        }
        else if (!secondGuess)
        {
            secondGuess = true;
            secondGuessIndex = selectedButtonIndex;

            // Checking state of two cards
            if (firstGuessIndex == secondGuessIndex)
            {
                secondGuess = false; // Reset second guess
                btn[firstGuessIndex].interactable = false; // Make the card non-interactable
                btn[firstGuessIndex].image.color = new Color(0, 0, 0, 0); // Hide the matched card
                countCorrectGuesses++; // Increase the count of correct guesses
                CheckIfGameFinish();
                return;
            }
            secondGuessPuzzle = gamePuzzles[secondGuessIndex];
            btn[secondGuessIndex].GetComponent<CardFlip>().Flip();  // Flip the second card

            countGuesses++;
            isCheckingMatch = true;
            StartCoroutine(CheckIfPuzzleMatch());
        }
    }

    IEnumerator CheckIfPuzzleMatch()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Checking match between " + firstGuessPuzzle + " and " + secondGuessPuzzle);

        if (firstGuessPuzzle == secondGuessPuzzle)
        {
            yield return new WaitForSeconds(.5f);

            btn[firstGuessIndex].interactable = false;
            btn[secondGuessIndex].interactable = false;

            btn[firstGuessIndex].image.color = new Color(0, 0, 0, 0);  // Hide matched card 1
            btn[secondGuessIndex].image.color = new Color(0, 0, 0, 0);  // Hide matched card 2

            countCorrectGuesses++;

            MinigameAudio.Instance.PlayCardMatchSound();

            Debug.Log("Cards matched!");
            CheckIfGameFinish();
        }
        else
        {
            yield return new WaitForSeconds(.5f);

            // Flip the cards back if they don't match
            btn[firstGuessIndex].GetComponent<CardFlip>().Flip();
            btn[secondGuessIndex].GetComponent<CardFlip>().Flip();

            MinigameAudio.Instance.PlayCardMismatchSound();

            Debug.Log("Cards did not match.");
            DecreaseMoves();
        }

        yield return new WaitForSeconds(.5f);

        // Reset guesses
        firstGuess = secondGuess = false;
        isCheckingMatch = false;

        CheckRemainingMoves();
    }

    void CheckRemainingMoves()
    {
        if (remainingMoves <= 0)
        {
            isGameOver = true;
            foreach (Button button in btn)
            {
                button.interactable = false;  // Disable all buttons
            }

            MinigameAudio.Instance.PlayGameOverSound();

            Debug.Log("Game Over");
            gameEndController.ShowGameOver();
        }
    }

    void DecreaseMoves()
    {
        remainingMoves--;
        UpdateMovesText();
        CheckRemainingMoves();
    }

    void UpdateMovesText()
    {
        movesText.text = remainingMoves.ToString();
    }

    void CheckIfGameFinish()
    {
        if (countCorrectGuesses == gameGuesses)
        {
            Debug.Log("Game Finished");
            Debug.Log("It took you " + countGuesses + " guesses");

            MinigameAudio.Instance.PlayVictorySound();

            gameEndController.ShowCongratulations();

            // Unlocking next level if the game finish
            UnlockNextLevel();
        }
    }

    void UnlockNextLevel()
    {
        int currentLevelNumber = GameManager.Instance.currentLevel.levelNumber;
        int highestLevel = PlayerPrefs.GetInt("highestLevel", 1);

        if (currentLevelNumber >= highestLevel)
        {
            PlayerPrefs.SetInt("highestLevel", currentLevelNumber + 1);
            PlayerPrefs.Save();
        }
    }

    void Shuffle(List<Sprite> list)
    {
        // Shuffle the game puzzle list
        for (int i = 0; i < list.Count; i++)
        {
            Sprite temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void GoBackLevelSelection()
    {
        StartCoroutine(TransitionToLevelSelection());
    }

    private IEnumerator TransitionToLevelSelection()
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene("LevelSelectionScene");
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "LevelSelectionScene");

        Button resetButton = GameObject.Find("Reset").GetComponent<Button>();
        if (resetButton != null)
        {
            Debug.Log("Reset Button found, interactable: " + resetButton.interactable);
        }
        else
        {
            Debug.LogError("Reset Button not found!");
        }
        
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.SetupLevelButtons();
        }
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        transitionCanvas.gameObject.SetActive(true);
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            transitionCanvas.alpha = i;
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            transitionCanvas.alpha = i;
            yield return null;
        }
        transitionCanvas.gameObject.SetActive(false);
    }

    public void RefreshGame()
    {
        isGameOver = false;
        firstGuess = secondGuess = false;
        countGuesses = countCorrectGuesses = 0;

        // Reload the current level data
        SetupGame();

        foreach (Button button in btn)
        {
            button.interactable = true;
            button.image.color = new Color(1, 1, 1, 1);

            var cardFlip = button.GetComponent<CardFlip>();
            if (cardFlip != null)
            {
                cardFlip.ResetFlipState();
            }
            button.transform.localScale = Vector3.zero;
        }

        // Reset moves based on current level
        remainingMoves = maxMoves;
        UpdateMovesText();

        Debug.Log("Game Refreshed");
        AnimateCardsEntrance();
    }


    void AnimateCardsEntrance()
    {
        for (int i = 0; i < btn.Count; i++)
        {
            Button button = btn[i];
            button.transform.localScale = Vector3.zero;
            button.transform.DOScale(Vector3.one, animationDuration)
                         .SetEase(animationEase)
                         .SetDelay(i * delayBetweenCards);
        }

        float totalDuration = animationDuration + (btn.Count - 1) * delayBetweenCards;
        DOVirtual.DelayedCall(totalDuration, () =>
        {
            Debug.Log("All cards have been animated in!");
        });
    }
}
