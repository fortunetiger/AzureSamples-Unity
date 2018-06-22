using UnityEngine;

/// <summary>
/// This class follows the Unity tutorial: 
/// https://unity3d.com/learn/tutorials/topics/2d-game-creation/adding-column-obstacles?playlist=17093
/// </summary>
public class Column : MonoBehaviour 
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
       if (collision.GetComponent<Bird>() != null)
        {
            GameControl.Instance.ScoreBird();
        }
    }
}
