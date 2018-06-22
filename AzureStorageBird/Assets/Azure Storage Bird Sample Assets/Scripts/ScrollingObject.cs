using UnityEngine;

/// <summary>
/// Based on the Unity tutorial:
/// https://unity3d.com/learn/tutorials/topics/2d-game-creation/scrolling-repeating-backgrounds?playlist=17093
/// Modified to adjust the scroll speed each time the player moves to a new level.
/// </summary>
public class ScrollingObject : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;

	// Use this for initialization
	private void Awake ()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	private void Update ()
    {
        // We could use the GameOver event in GameControl for this,
        // But I'm leaving it to keep a closer resemblance to the original Unity tutorial.
		if (GameControl.Instance.IsGameOver)
        {
            rigidbody2D.velocity = Vector2.zero;
        }
	}

    /// <summary>
    /// The original tutorial just set the velocity in Start, but we need to set it at the start of each level.
    /// Logic is unchanged, just refactored into its own function to make repeat calls easier.
    /// </summary>
    private void AdjustVelocityBasedOnScrollSpeed()
    {
        rigidbody2D.velocity = new Vector2(GameControl.Instance.ScrollSpeed, 0);
    }

    /// <summary>
    /// GameControl.StartedNewLevel event handler.
    /// </summary>
    /// <param name="levelIndex">Event passes us the index of the current level, but it's unused here.
    /// </param>
    private void OnStartedNewLevel(int levelIndex)
    {
        AdjustVelocityBasedOnScrollSpeed();
    }


    #region Event subscription / unsubscription.
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
