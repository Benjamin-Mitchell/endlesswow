using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float moveSpeed = 10.0f;

	//Vector2 direction = Vector2.right;
	Rigidbody2D rgdBody2D;

    // Start is called before the first frame update
    void Awake()
    {
		rgdBody2D = GetComponent<Rigidbody2D>();
    }

	public void SetDirection(Vector2 dir)
	{
		//direction = dir;
		rgdBody2D.velocity = dir * moveSpeed;
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.CompareTag("EvilMob"))
		{
			collision.gameObject.GetComponent<EvilMob>().Die();
		}

		if (collision.gameObject.CompareTag("GoodMob"))
		{
			collision.gameObject.GetComponent<GoodMob>().Convert();
		}

		if (collision.gameObject.CompareTag("Damaging"))
		{
			Destroy(collision.gameObject);
		}

		Destroy(gameObject);
	}
}
