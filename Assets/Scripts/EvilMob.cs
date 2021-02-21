using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilMob : MonoBehaviour
{
	Animator animator;
	BoxCollider2D boxCollider2D;
    // Start is called before the first frame update
    void Start()
    {
		animator = GetComponent<Animator>();
		boxCollider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Die()
	{
		boxCollider2D.enabled = false;
		animator.SetTrigger("Die");
		StartCoroutine(DelayedDestroy());
	}

	IEnumerator DelayedDestroy()
	{
		yield return new WaitForSeconds(0.5f);
		Destroy(gameObject);
	}
}
