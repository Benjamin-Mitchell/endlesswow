using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour
{
	private float moveSpeed;

	public GameObject endPoint;
	private Vector3 deathPoint;

	GameManager.CURRENT_WORLD currentWorld;
	GameManager gameManager;
	public GameObject[] HappyMaps;
	public GameObject[] EvilMaps;


	private void Awake()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		moveSpeed = gameManager.moveSpeed;
		endPoint = transform.Find("EndPoint").gameObject;
		deathPoint = GameObject.Find("SectionDeathPoint").transform.position;
	}

	// Start is called before the first frame update
	void Start()
    {
		currentWorld = gameManager.currentWorld;
		UpdateWorld();
    }

    // Update is called once per frame
    void Update()
    {
		float moveDelta = moveSpeed * Time.deltaTime;
		transform.position = new Vector3(transform.position.x - moveDelta, transform.position.y, transform.position.z);

		if(gameManager.currentWorld != currentWorld)
		{
			currentWorld = gameManager.currentWorld;

			UpdateWorld();
		}

		moveSpeed = gameManager.moveSpeed;

		if (transform.position.x < deathPoint.x)
			Destroy(gameObject);
    }

	void UpdateWorld()
	{
		for (int i = 0; i < HappyMaps.Length; i++)
		{
			if (currentWorld == GameManager.CURRENT_WORLD.HAPPY_LAND)
				HappyMaps[i].SetActive(true);
			else if (currentWorld == GameManager.CURRENT_WORLD.SCARY_LAND)
				HappyMaps[i].SetActive(false);
		}

		for (int i = 0; i < EvilMaps.Length; i++)
		{
			if (currentWorld == GameManager.CURRENT_WORLD.SCARY_LAND)
				EvilMaps[i].SetActive(true);
			else if (currentWorld == GameManager.CURRENT_WORLD.HAPPY_LAND)
				EvilMaps[i].SetActive(false);
		}
	}
}
