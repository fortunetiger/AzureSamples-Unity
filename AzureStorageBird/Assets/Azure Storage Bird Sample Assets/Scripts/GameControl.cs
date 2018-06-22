using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// This class is based on the Unity tutorial:
/// https://unity3d.com/learn/tutorials/topics/2d-game-creation/adding-game-controller?playlist=17093
/// It has been modified to include:
///     * Sound effects for game over and scoring.
///     * A level progression system with progressively faster background scrolling speed.
///     * Events for when new levels are started and when game over occurs.
/// </summary>
public class GameControl : MonoBehaviour
{
    #region Editor variables
    [Tooltip("Player must get this many new points to reach the next level")]
    [SerializeField]
    private int scoreForNextLevel = 3;

    [Tooltip("How fast ScrollingObjects move at the start of the game.")]
    [SerializeField]
    private float scrollSpeed = -1.5f;

    [Tooltip("Each time the player progresses levels, the scrollSpeed increases by a constant amount" +
        "until reaching this maximum.")]
    [SerializeField]
    private float maxScrollSpeed = -3f;

    [SerializeField]
    private AudioClip gameOverClip, scoreClip;

    [SerializeField]
    private GameObject gameOverText;

    [SerializeField]
    private Text scoreText;
    #endregion

    #region Events
    public static event Action<int> StartedNewLevel;
    public static event Action GameOver;
    #endregion

    #region Properties
    public static GameControl Instance { get; private set; }
    public float ScrollSpeed { get { return scrollSpeed; } }
    public bool IsGameOver { get; private set; }

    private int RequiredTotalScoreForNextLevel
    {
        get
        {
            return currentLevelIndex * scoreForNextLevel + scoreForNextLevel;
        }
    }
    #endregion

    #region Private fields
    // The MusicFileNamesInLevelOrder array contains a filename for each level in the game.
    // Therefore the finalLevelIndex should be equal to it's lenght minus 1.
    private readonly int finalLevelIndex = LevelMusicPlayer.MusicFileNamesInLevelOrder.Length - 1;

    // Represents which level the player is currently on.
    private int currentLevelIndex = 0;

    // Amount the scroll speed increases by each level, 
    // based on maxScrollSpeed and intialized in Start.
    private float scrollSpeedIncreaseAmount;

    private AudioSource audioSource;
    private int score = 0;
    #endregion

    #region Monobehaviour functions
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        scrollSpeedIncreaseAmount = (maxScrollSpeed - scrollSpeed) / LevelMusicPlayer.MusicFileNamesInLevelOrder.Length;        
        StartFirstLevel();
    }

    private void Update()
    {
        if (IsGameOver)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
    #endregion

    #region Public functions
    /// <summary>
    /// Changed from tutorial version to include logic for progressing through levels
    /// based on score.
    /// Called from Column.cs OnTriggerEnter2D
    /// </summary>
    public void ScoreBird()
    {
        if (!IsGameOver)
        {
            score++;
            scoreText.text = "Score: " + score;

            audioSource.clip = scoreClip;
            audioSource.Play();

            if (score >= RequiredTotalScoreForNextLevel)
            {
                StartNewLevel();
            }
        }
    }

    /// <summary>
    /// Raises GameOver event, handles game over behavior.
    /// Called from Bird.cs OnCollisionEnter2D.
    /// </summary>
    public void BirdDied()
    {
        GameOver?.Invoke();
        gameOverText.SetActive(true);
        IsGameOver = true;
        audioSource.clip = gameOverClip;
        audioSource.Play();
    }
    #endregion

    #region Private functions
    /// <summary>
    /// Raises the StartedNewLevel event without incrementing the level index or
    /// increasing the background scrolling speed.
    /// Intended to be called only when starting the first level.
    /// </summary>
    private void StartFirstLevel()
    {
        StartedNewLevel?.Invoke(currentLevelIndex);
    }

    /// <summary>
    /// Raises the StartedNewLevel event as well as incrementing the level index and
    /// increasing the background scrolling speed.
    /// Intended to be called each time a new level is started beyond the first level.
    /// </summary>
    private void StartNewLevel()
    {
        if (currentLevelIndex < finalLevelIndex)
        {
            currentLevelIndex++;
            IncreaseScrollSpeed();
            StartedNewLevel?.Invoke(currentLevelIndex);
        }
    }

    /// <summary>
    /// Increases the speed of the background and columns (any instance of the ScrollingObject).
    /// The scrollSpeedIncreaseAmount is set in Start.
    /// </summary>
    private void IncreaseScrollSpeed()
    {
        scrollSpeed += scrollSpeedIncreaseAmount;
    }
    #endregion
}