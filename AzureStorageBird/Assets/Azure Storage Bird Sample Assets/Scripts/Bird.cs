using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class follows this Unity tutorial:
/// https://unity3d.com/learn/tutorials/topics/2d-game-creation/bird-script?playlist=17093
/// It has been modified to use the input manager for better multi-platform support.
/// </summary>
public class Bird : MonoBehaviour
{
    [SerializeField]
    private float upForce = 200f;

    private bool isDead = false;
    private Rigidbody2D rigidbody2D;
    private Animator animator;

	// Use this for initialization
	private void Start ()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	private void Update ()
    {
		if (!isDead)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.AddForce(new Vector2(0, upForce));
                animator.SetTrigger("Flap");
            }
        }		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDead)
        {
            isDead = true;
            animator.SetTrigger("Die");
            GameControl.Instance.BirdDied();
            rigidbody2D.velocity = Vector2.zero;
        }
    }
}
