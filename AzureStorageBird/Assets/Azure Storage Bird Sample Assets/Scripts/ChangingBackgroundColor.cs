using UnityEngine;

/// <summary>
/// Handles changing the background when the player reaches a new level.
/// Intended to give better feedback that a new level has been reached.
/// This script should be on a semi-transparent sprite arranged between the player and the background sprites.
/// </summary>
public class ChangingBackgroundColor : MonoBehaviour 
{
    // Array of colors exposed to the editor to make picking the colors easier.
    // The array is of the length of the MusicFileNamesInLevelOrder array, because this
    // array length matches the number of levels in our game.
    [Tooltip("Do not change the size of this array, " +
        "it matches the number of levels. Each element corresponds to the background color of a level, in successive order.")]
    [SerializeField]
    private Color[] colors = new Color[LevelMusicPlayer.MusicFileNamesInLevelOrder.Length];

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Handler for GameControl.StartedNewLevel event.
    /// Updates the color based on the current level index.
    /// </summary>
    /// <param name="levelIndex">The event will pass us the index of the current level.</param>
    private void OnStartedNewLevel(int levelIndex)
    {
        if (colors.Length > levelIndex)
            spriteRenderer.color = colors[levelIndex];
    }

    #region Event subscription / unsubscription
    private void OnEnable()
    {
        GameControl.StartedNewLevel += OnStartedNewLevel;
    }

    private void OnDisable()
    {
        GameControl.StartedNewLevel -= OnStartedNewLevel;
    }
    #endregion
}
