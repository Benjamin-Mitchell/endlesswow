using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour
{
	private float moveSpeed;

	public GameObject endPoint;

	private void Awake()
	{
		moveSpeed = GameManager.Instance.moveSpeed;
		endPoint = transform.Find("EndPoint").gameObject;
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
    }
}
