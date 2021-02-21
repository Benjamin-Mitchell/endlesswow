using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public float jumpVelocity;
	public float moveVelocity;
	public float rotationVelocity;

	public SpriteRenderer[] hearts = new SpriteRenderer[8];
	public SpriteRenderer[] heartBackgrounds = new SpriteRenderer[8];

	public Sprite heartFull, heartBroken;
	public Sprite goodHeartBackground, badHeartBackground;

	public GameObject raySource;

	public GameObject goodProjectile, badProjectile;

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

	private GameManager gameManager;

	private int health = 8;
	private int maxHealth = 8;

	private bool canShoot = true;
	private float shootCooldown = 0.5f;

	// Start is called before the first frame update
	void Start()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		currentWorld = gameManager.currentWorld;
		boxCollider2D = GetComponent<BoxCollider2D>();
		colliderHeight = boxCollider2D.size.y;
		playerHeight = transform.localScale.y;

		rgdBody2D = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();

		gameMoveSpeed = gameManager.moveSpeed;
		layerMask = LayerMask.GetMask("Floor");
	}

	// Update is called once per frame
	void Update()
	{
		animator.SetFloat("VertVelocity", rgdBody2D.velocity.y);
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
				animator.SetBool("Slide", true);
				boxCollider2D.size = new Vector2(boxCollider2D.size.x, 0.07f);
				boxCollider2D.offset = new Vector2(boxCollider2D.offset.x, -0.125f);
				//StartCoroutine(Slide());
				//sliding = false;
			}

			if (Input.GetKeyUp(KeyCode.X))
			{
				animator.SetBool("Slide", false);
				boxCollider2D.size = new Vector2(boxCollider2D.size.x, 0.16f);
				boxCollider2D.offset = new Vector2(boxCollider2D.offset.x, -0.08f);
				//transform.localScale = new Vector2(transform.localScale.x, playerHeight);
			}

			if(Input.GetKeyDown(KeyCode.P) && canShoot)
			{
				StartCoroutine(ShootCooldown());
				animator.SetTrigger("Attack");
				Vector2 spawnPos = boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.size.x / 2.0f, 0.0f, 0.0f);
				GameObject proj = GameObject.Instantiate(goodProjectile, spawnPos, Quaternion.identity);
				Projectile proje = proj.GetComponentInChildren<Projectile>();
				proje.SetDirection(Vector2.right);
			}
		}
		else if (currentWorld == GameManager.CURRENT_WORLD.SCARY_LAND)
		{
			rotationTargetVector = Vector3.up;
			if (Input.GetKey(KeyCode.X))
			{
				spriteRenderer.flipX = false;
				rgdBody2D.velocity = new Vector2(rgdBody2D.velocity.x , moveVelocity);

				if (Input.GetKeyDown(KeyCode.P) && canShoot)
				{
					StartCoroutine(ShootCooldown());
					animator.SetTrigger("Attack");
					Vector2 spawnPos = boxCollider2D.bounds.center + new Vector3(0.0f, boxCollider2D.bounds.size.x / 2.0f, 0.0f);
					GameObject proj = GameObject.Instantiate(badProjectile, spawnPos, Quaternion.identity);
					Projectile proje = proj.GetComponentInChildren<Projectile>();
					proje.SetDirection(Vector3.up);
				}
			}

			if (Input.GetKey(KeyCode.A))
			{
				spriteRenderer.flipX = true;
				rgdBody2D.velocity = new Vector2(rgdBody2D.velocity.x, -moveVelocity);

				if (Input.GetKeyDown(KeyCode.P) && canShoot)
				{
					StartCoroutine(ShootCooldown());
					animator.SetTrigger("Attack");
					Vector2 spawnPos = boxCollider2D.bounds.center - new Vector3(0.0f, boxCollider2D.bounds.size.x / 2.0f, 0.0f);
					GameObject proj = GameObject.Instantiate(badProjectile, spawnPos, Quaternion.identity);
					Projectile proje = proj.GetComponentInChildren<Projectile>();
					proje.SetDirection(Vector3.down);
				}
			}

			if (Input.GetKeyDown(KeyCode.P) && canShoot)
			{
				StartCoroutine(ShootCooldown());
				animator.SetTrigger("DownAttack");
				Vector2 spawnPos = boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.size.y / 2.0f, 0.0f, 0.0f);
				GameObject proj = GameObject.Instantiate(badProjectile, spawnPos, Quaternion.identity);
				Projectile proje = proj.GetComponentInChildren<Projectile>();
				proje.SetDirection(Vector3.right);
			}

			grounded = false;
			
			//raycast for moving floors. Stops the character from getting stuck in them.
			//upgrade this to a better box cast.
			RaycastHit2D hit = Physics2D.BoxCast(raySource.gameObject.transform.position, boxCollider2D.bounds.size, 0f, Vector2.right, 0.1f, layerMask);
			if (hit.collider != null)
			{
				Debug.Log(hit.collider.name);
				if (hit.collider.gameObject.CompareTag("Floor"))
				{
					grounded = true;

					float hitpos = hit.point.x;
					//do something to stay up....
					//float moveDelta = gameMoveSpeed * Time.deltaTime;
					//transform.position = new Vector3(transform.position.x - moveDelta, transform.position.y, transform.position.z);
					transform.position = new Vector3(hit.point.x - boxCollider2D.bounds.size.y - /* magic offset number alert*/ 0.35f, transform.position.y, transform.position.z);
				}
			}

			animator.SetBool("Grounded", grounded);

			//Always start moving towards center of the screen
			float stopPos = -4.0f;
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
			gameManager.EndGame();
	}

	private IEnumerator ShootCooldown()
	{
		canShoot = false;
		yield return new WaitForSeconds(shootCooldown);
		canShoot = true;
	}

	public void SetWorld(GameManager.CURRENT_WORLD world)
	{
		currentWorld = world;
		
		foreach(SpriteRenderer background in heartBackgrounds)
		{
			if (currentWorld == GameManager.CURRENT_WORLD.SCARY_LAND)
			{
				background.sprite = badHeartBackground;
			}
			else
			{
				background.sprite = goodHeartBackground;
			}
		}
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
			gameManager.EndGame();
		}

		if (collision.gameObject.CompareTag("GoodMob"))
		{
			hearts[health - 1].sprite = heartBroken;
			health--;
			if (health < 1)
				gameManager.EndGame();

			Destroy(collision.gameObject);
		}

		if (collision.gameObject.CompareTag("EvilMob"))
		{
			hearts[health - 1].sprite = heartBroken;
			health--;
			if (health < 1)
				gameManager.EndGame();
		
			collision.gameObject.GetComponent<EvilMob>().Die();
		}

		if (collision.gameObject.CompareTag("Healing"))
		{
			if(health < maxHealth)
				health++;
			hearts[health - 1].sprite = heartFull;
			Destroy(collision.gameObject);
		}
	}
}
