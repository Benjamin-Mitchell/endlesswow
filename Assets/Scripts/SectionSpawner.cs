using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionSpawner : MonoBehaviour
{
	private GameObject latestEndPoint;
	private Vector3 spawnPoint;

	public GameObject[] sections; 

	// Start is called before the first frame update
	void Start()
    {
		spawnPoint = GameObject.Find("SectionSpawnPoint").transform.position;
		latestEndPoint = GameObject.Find("EmptySection").GetComponent<Section>().endPoint;
		Spawn();
	}

    // Update is called once per frame
    void Update()
    {
        if(latestEndPoint.transform.position.x < spawnPoint.x)
		{
			Spawn();
		}
    }

	void Spawn()
	{
		int roll = Random.Range(0, 100);
		GameObject temp;
		int sectionIndex = 0;

		if (roll < 34)
			sectionIndex = 1;
		else if (roll < 67)
			sectionIndex = 2;
		else
			sectionIndex = 3;


		temp = GameObject.Instantiate(sections[sectionIndex], latestEndPoint.transform.position, Quaternion.identity);
		latestEndPoint = temp.GetComponent<Section>().endPoint;
		
	}
}
