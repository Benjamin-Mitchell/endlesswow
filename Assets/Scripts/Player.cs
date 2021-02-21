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
	private Animator animator;

	private bool canJump = true;
	private bool grounded = false;
	private float colliderHeight, playerHeight;
	private float gameMoveSpeed;

	private Vector2 cachedVelocity;

	private LayerMask layerMask;

	private GameManager.CURRENT_WORLD currentWorld;

	private int health = 8;
	private int maxHealth = 8;

	// Start is called before the first frame update
	void Start()
	{
		currentWorld = GameManager.Instance.currentWorld;
		boxCollider2D = GetComponent<BoxCollider2D>();
		colliderHeight = boxCollider2D.size.y;
		playerHeight = transform.localScale.y;

		rgdBody2D = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();

		gameMoveSpeed = GameManager.Instance.moveSpeed;
		layerMask = LayerMask.GetMask("Floor");
	}

	// Update is called once per frame
	void Update()
	{
		animator.SetFloat("VertVelocity", rgdBody2D.velocity.y);
		animator.SetFloat("HorizVelocity", rgdBody2D.velocity.x);
		Vector3 rotationTargetVector = Vector3.right;

		if (currentWorld == GameManager.CURRENT_WORLD.HAPPY_LAND)
		{
			spriteRenderer.flipX = false;
			if (Input.GetKeyDown(KeyCode.Space) && canJump)
			{
				rgdBody2D.velocity = Vector2.up * jumpVelocity;
				canJump = false;
			}

			if (Input.GetKeyDown(KeyCode.X) /*&& !sliding*/)
			{
				animator.SetTrigger("StartSlide");
				//transform.localScale = new Vector2(transform.localScale.x, playerHeight / 2.0f);
				//StartCoroutine(Slide());
				//sliding = false;
			}

			if (Input.GetKeyUp(KeyCode.X))
			{
				animator.SetTrigger("StopSlide");
				//transform.localScale = new Vector2(transform.localScale.x, playerHeight);
			}
		}
		else if (currentWorld == GameManager.CURRENT_WORLD.SCARY_LAND)
		{
			rotationTargetVector = Vector3.up;
			if (Input.GetKey(KeyCode.X))
			{
				spriteRenderer.flipX = false;
				rgdBody2D.velocity = new Vector2(rgdBody2D.velocity.x , moveVelocity);
			}

			if (Input.GetKey(KeyCode.A))
			{
				spriteRenderer.flipX = true;
				rgdBody2D.velocity = new Vector2(rgdBody2D.velocity.x, -moveVelocity);
			}

			grounded = false;
			
			//raycast for moving floors. Stops the character from getting stuck in them.
			RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 0.9f, layerMask);
			if (hit.collider != null)
			{
				if (hit.collider.gameObject.CompareTag("Floor"))
				{
					grounded = true;
					//do something to stay up....
					float moveDelta = gameMoveSpeed * Time.deltaTime;
					transform.position = new Vector3(transform.position.x - moveDelta, transform.position.y, transform.position.z);
				}
			}

			animator.SetBool("Grounded", grounded);

			//Always start moving towards center of the screen
			float stopPos = -2.0f;
			if (transform.position.x < stopPos && !grounded)
			{
				rgdBody2D.velocity = new Vector2(Mathf.Abs(transform.position.x + stopPos), rgdBody2D.velocity.y);
			}
			//Physics2D.gravity = new Vector2(Mathf.Abs(transform.position.x + stopPos), 0.0f);
			//else
			//{
			//	Physics2D.gravity = new Vector2(0.0f, 0.0f);
			//	rgdBody2D.velocity = new Vector2(0.0f, rgdBody2D.velocity.y);
			//}
		}

		//rotation for falling
		float angle = Mathf.Atan2(rotationTargetVector.y, rotationTargetVector.x) * Mathf.Rad2Deg;
		Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotationVelocity);

		if(health < 1)
			GameManager.Instance.EndGame();
	}

	public void SetWorld(GameManager.CURRENT_WORLD world)
	{
		currentWorld = world;
	}

	public void StartWorldSwitch(GameManager.CURRENT_WORLD world)
	{
		//pause Phyiscs
		cachedVelocity = rgdBody2D.velocity;
		rgdBody2D.velocity = Vector2.zero;

		animator.SetBool("BadLand", world == GameManager.CURRENT_WORLD.SCARY_LAND ? true : false);
	}

	public void ResumePhysics()
	{
		rgdBody2D.velocity = cachedVelocity;
	}

	//private IEnumerator Slide()
	//{
	//	sliding = true;
	//	//boxCollider2D.size = new Vector2( boxCollider2D.size.x, colliderHeight / 2.0f);
	//	//boxCollider2D.offset = new Vector2(boxCollider2D.offset.x, boxCollider2D.offset.y - colliderHeight / 2.0f);
	//	//transform.localScale = new Vector2(transform.localScale.x, playerHeight / 2.0f);
	//
	//	yield return new WaitForSeconds(0.50f);
	//
	//	sliding = false;
	//	//boxCollider2D.size = new Vector2(boxCollider2D.size.x, colliderHeight);
	//	//boxCollider2D.offset = new Vector2(boxCollider2D.offset.x, boxCollider2D.offset.y + colliderHeight / 2.0f);
	//	//transform.localScale = new Vector2(transform.localScale.x, playerHeight);
	//}

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

		if (collision.gameObject.CompareTag("Damaging"))
		{
			health--;
			if (health < 1)
				GameManager.Instance.EndGame();
			Destroy(collision.gameObject);
		}
	}
}
