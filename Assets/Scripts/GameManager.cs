using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
	public float moveSpeed = 4;

	//0 = happy land, 1 = scary land
	private int currentWorld = 0;

    // Start is called before the first frame update
    void Start()
    {  
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.G))
		{

		}

	}

	public void EndGame()
	{
		SceneManager.LoadScene("Main");
	}
}
