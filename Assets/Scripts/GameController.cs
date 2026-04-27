using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public enum PlayerMark { None = 0, X = 1, O = 2 }

    [Header("Board Settings")]
    public Image[] cellImages; // replace text cells with images
    private Button[] buttons;
    private PlayerMark currentPlayer;
    public Sprite xSprite; // Prevuci X sliku ovde
    public Sprite oSprite; // Prevuci O sliku ovde

    // internal board state: PlayerMark.None, X, or O
    private PlayerMark[] board;

    // win conditions
    private readonly int[][] winConditions = new int[][]
    {
        new[] {0,1,2}, new[] {3,4,5}, new[] {6,7,8},
        new[] {0,3,6}, new[] {1,4,7}, new[] {2,5,8},
        new[] {0,4,8}, new[] {2,4,6}
    };

    [Header("HUD Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI movesText;

    public GameObject gameOverPanel;
    public TextMeshProUGUI winnerText;

    int moveCount = 0;
    int player1Moves = 0;
    int player2Moves = 0;

    float startTime;
    private bool isGameActive = true;

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";

    [Header("SFX")]
    public AudioSource sfxSource;
    public AudioClip placeClip;
    public AudioClip winClip;
    public AudioClip drawClip;

    [Header("BGM")]
    public AudioSource bgmSource; // assign your background music AudioSource here (fallback to MusicS named object)

    [Header("Strike Settings")]
    // Use the 8 pre-made strike line GameObjects in the inspector (must match winConditions order)
    public GameObject[] strikeLines;
    private int winningIndex = -1;

    private void Awake()
    {
        // basic validation
        if (cellImages == null || cellImages.Length != 9)
            Debug.LogWarning("cellImages should contain 9 Image items.");

        if (strikeLines != null && strikeLines.Length != winConditions.Length)
            Debug.LogWarning($"strikeLines length ({strikeLines.Length}) does not match expected win conditions ({winConditions.Length}). Make sure order matches: 0=top row,1=middle row,2=bottom row,3=left col,4=middle col,5=right col,6=diag TL-BR,7=diag TR-BL");

        // initialization
        currentPlayer = PlayerMark.X;
        startTime = Time.time;
        isGameActive = true;
        moveCount = 0;
        player1Moves = 0;
        player2Moves = 0;

        // init board state
        int boardLen = cellImages != null ? cellImages.Length : 9;
        board = new PlayerMark[boardLen];
        for (int i = 0; i < board.Length; i++) board[i] = PlayerMark.None;

        // Cache buttons and prepare images
        int len = cellImages != null ? cellImages.Length : 0;
        buttons = new Button[len];
        for (int i = 0; i < len; i++)
        {
            if (cellImages[i] != null)
            {
                buttons[i] = cellImages[i].GetComponentInParent<Button>();
                cellImages[i].gameObject.SetActive(false);
                cellImages[i].sprite = null;
            }
        }

        // hide pre-made strike lines at start
        HideAllStrikes();
        winningIndex = -1;

        // Apply saved audio preferences via SettingsManager (AudioMixer)
       // var settings = FindObjectOfType<SettingsManager>();
        //if (settings != null)
          //  settings.ApplySettings();

        UpdateHUD();
    }

    private void Update()
    {
        if (isGameActive && timerText != null)
            UpdateTimerUI();
    }

    public void EndTurn(int index)
    {
        if (!isGameActive) return;
        if (cellImages == null) return;
        if (index < 0 || index >= cellImages.Length) return;
        if (buttons == null || buttons.Length <= index || buttons[index] == null) return;
        if (!buttons[index].interactable) return;

        board[index] = currentPlayer;

        if (cellImages[index] != null)
        {
            cellImages[index].sprite = (currentPlayer == PlayerMark.X) ? xSprite : oSprite;
            cellImages[index].gameObject.SetActive(true);
        }

        buttons[index].interactable = false;

        if (sfxSource != null && placeClip != null)
            sfxSource.PlayOneShot(placeClip);

        moveCount++;
        if (currentPlayer == PlayerMark.X) player1Moves++; else player2Moves++;

        UpdateHUD();

        if (CheckForWin())
        {
            float duration = Time.time - startTime;
            Debug.Log($"Win detected. winningIndex={winningIndex} combo=[{string.Join(",", winConditions[winningIndex])}] ");
            EndGame(PlayerToString(currentPlayer), duration);
            return;
        }

        if (moveCount >= 9)
        {
            float duration = Time.time - startTime;
            EndGame("Draw", duration);
            return;
        }

        currentPlayer = (currentPlayer == PlayerMark.X) ? PlayerMark.O : PlayerMark.X;
    }

    void UpdateTimerUI()
    {
        int t = (int)(Time.time - startTime);
        string minutes = (t / 60).ToString("00");
        string seconds = (t % 60).ToString("00");
        if (timerText != null) timerText.text = minutes + ":" + seconds;
    }

    void UpdateHUD()
    {
        if (movesText != null)
            movesText.text = "Moves X: " + player1Moves + "\nMoves O: " + player2Moves;
    }

    bool CheckForWin()
    {
        winningIndex = -1;
        for (int i = 0; i < winConditions.Length; i++)
        {
            int[] combo = winConditions[i];
            if (board[combo[0]] == PlayerMark.None || board[combo[1]] == PlayerMark.None || board[combo[2]] == PlayerMark.None)
                continue;
            if (board[combo[0]] == currentPlayer &&
                board[combo[1]] == currentPlayer &&
                board[combo[2]] == currentPlayer)
            {
                winningIndex = i;
                return true;
            }
        }
        return false;
    }

    void EndGame(string result, float duration)
    {
        isGameActive = false; // stop timer
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (winnerText != null)
        {
            if (result == "Draw")
            {
                winnerText.text = "It's a Draw!";
                if (sfxSource != null && drawClip != null) sfxSource.PlayOneShot(drawClip);
            }
            else
            {
                winnerText.text = "Winner is: " + result + "\n\n Duration: " + duration.ToString("F1") + "s";
                if (sfxSource != null && winClip != null) sfxSource.PlayOneShot(winClip);
            }
        }

        if (winningIndex >= 0 && strikeLines != null && winningIndex < strikeLines.Length)
        {
            var go = strikeLines[winningIndex];
            if (go != null) go.SetActive(true);
        }

        SetBoardInteractable(false);
        StatisticsManager.SaveGame(result, duration);
    }

    void HideAllStrikes()
    {
        if (strikeLines == null) return;
        for (int i = 0; i < strikeLines.Length; i++)
        {
            if (strikeLines[i] != null) strikeLines[i].SetActive(false);
        }
    }

    void SetBoardInteractable(bool state)
    {
        if (buttons != null && buttons.Length == cellImages.Length)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] != null) buttons[i].interactable = state;
            }
        }
        else if (cellImages != null)
        {
            for (int i = 0; i < cellImages.Length; i++)
            {
                var b = cellImages[i]?.GetComponentInParent<Button>();
                if (b != null) b.interactable = state;
            }
        }
    }

    string PlayerToString(PlayerMark p)
    {
        if (p == PlayerMark.X) return "X";
        if (p == PlayerMark.O) return "O";
        return "";
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void RestartGame()
    {
        HideAllStrikes();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMenu()
    {
        HideAllStrikes();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // deprecated helpers kept for compatibility
    public void MuteBGM(bool isOn)
    {
        if (bgmSource == null)
        {
            var go = GameObject.Find("MusicS");
            if (go != null) bgmSource = go.GetComponent<AudioSource>();
        }
        if (bgmSource != null) bgmSource.mute = !isOn;
    }

    public void MuteSFX(bool isOn)
    {
        if (sfxSource != null) sfxSource.mute = !isOn;
    }

    public void PlayClickSound()
    {
        if(sfxSource != null && placeClip != null)
            sfxSource.PlayOneShot(placeClip);
    }
}