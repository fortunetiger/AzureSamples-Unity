using UnityEngine;

/// <summary>
/// Based on the Unity tutorial:
/// https://unity3d.com/learn/tutorials/topics/2d-game-creation/scrolling-repeating-backgrounds?playlist=17093
/// </summary>
public class RepeatingBackground : MonoBehaviour
{
    private BoxCollider2D groundCollider;
    private float groundHorizontalLength;

	// Use this for initialization
	void Start ()
    {
        groundCollider = GetComponent<BoxCollider2D>();
        groundHorizontalLength = groundCollider.size.x;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (transform.position.x < -groundHorizontalLength)
        {
            RepositionBackground();
        }
	}

    private void RepositionBackground()
    {
        Vector2 groundOffset = new Vector2(groundHorizontalLength * 2, 0);
        transform.position = (Vector2)transform.position + groundOffset;
    }
}
