using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodMob : MonoBehaviour
{
	BoxCollider2D boxCollider2D;
	public GameObject good, bad, heart;
	// Start is called before the first frame update
	void Start()
    {
		boxCollider2D = GetComponent<BoxCollider2D>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Convert()
	{
		boxCollider2D.enabled = false;
		heart.SetActive(true);
		good.SetActive(true);
		bad.SetActive(false);
	}
}
