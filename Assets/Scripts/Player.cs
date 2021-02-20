using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public float jumpVelocity;
	public float moveVelocity;
	public float rotationVelocity;

	private Rigidbody2D rgdBody2D;
	private BoxCollider2D boxCollider2D;
	private SpriteRenderer spriteRenderer;

	private bool canJump = true;
	private bool sliding = false;
	private float colliderHeight, playerHeight;

	private GameManager.CURRENT_WORLD currentWorld;

	// Start is called before the first frame update
	void Start()
	{
		currentWorld = GameManager.Instance.currentWorld;
		boxCollider2D = GetComponent<BoxCollider2D>();
		colliderHeight = boxCollider2D.size.y;
		playerHeight = transform.localScale.y;

		rgdBody2D = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		Vector3 rotationTargetVector = Vector3.right;

		if (currentWorld == GameManager.CURRENT_WORLD.HAPPY_LAND)
		{
			if (Input.GetKeyDown(KeyCode.Space) && canJump)
			{
				rgdBody2D.velocity = Vector2.up * jumpVelocity;
				canJump = false;
			}

			if (Input.GetKeyDown(KeyCode.X) && !sliding)
			{
				StartCoroutine(Slide());
				sliding = false;
			}
		}
		else if (currentWorld == GameManager.CURRENT_WORLD.SCARY_LAND)
		{
			rotationTargetVector = Vector3.up;
			if (Input.GetKey(KeyCode.X))
			{
				spriteRenderer.flipX = false;
				rgdBody2D.velocity = Vector2.up * moveVelocity;

			}

			if (Input.GetKey(KeyCode.A))
			{
				spriteRenderer.flipX = true;
				rgdBody2D.velocity = Vector2.down * moveVelocity;
			}
		}

		//rotation for falling
		float angle = Mathf.Atan2(rotationTargetVector.y, rotationTargetVector.x) * Mathf.Rad2Deg;
		Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotationVelocity);

	}

	public void SetWorld(GameManager.CURRENT_WORLD world)
	{
		currentWorld = world;
	}

	private IEnumerator Slide()
	{
		sliding = true;
		//boxCollider2D.size = new Vector2( boxCollider2D.size.x, colliderHeight / 2.0f);
		//boxCollider2D.offset = new Vector2(boxCollider2D.offset.x, boxCollider2D.offset.y - colliderHeight / 2.0f);
		transform.localScale = new Vector2(transform.localScale.x, playerHeight / 2.0f);

		yield return new WaitForSeconds(0.5f);

		sliding = false;
		//boxCollider2D.size = new Vector2(boxCollider2D.size.x, colliderHeight);
		//boxCollider2D.offset = new Vector2(boxCollider2D.offset.x, boxCollider2D.offset.y + colliderHeight / 2.0f);
		transform.localScale = new Vector2(transform.localScale.x, playerHeight);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.CompareTag("Floor"))
		{
			canJump = true;
		}

		if(collision.gameObject.CompareTag("Lethal"))
		{
			GameManager.Instance.EndGame();
		}
	}
}
