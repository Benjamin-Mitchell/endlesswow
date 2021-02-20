using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour
{
	private float moveSpeed;

	public GameObject endPoint;
	private Vector3 deathPoint;

	private void Awake()
	{
		moveSpeed = GameManager.Instance.moveSpeed;
		endPoint = transform.Find("EndPoint").gameObject;
		deathPoint = GameObject.Find("SectionDeathPoint").transform.position;
	}

	// Start is called before the first frame update
	void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
		float moveDelta = moveSpeed * Time.deltaTime;
		transform.position = new Vector3(transform.position.x - moveDelta, transform.position.y, transform.position.z);

		if (transform.position.x < deathPoint.x)
			Destroy(gameObject);
    }
}
