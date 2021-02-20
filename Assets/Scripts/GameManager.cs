using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
	public float moveSpeed = 4;

	//0 = happy land, 1 = scary land
	public enum CURRENT_WORLD { HAPPY_LAND, SCARY_LAND};
	public CURRENT_WORLD currentWorld = CURRENT_WORLD.HAPPY_LAND;

	Player player;

	// Start is called before the first frame update
	void Start()
	{
		player = GameObject.Find("Player").GetComponent<Player>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.G))
		{
			SwitchWorld();
		}
	}

	public void SwitchWorld()
	{
		currentWorld = (CURRENT_WORLD)(((int)currentWorld + 1) % 2);

		if(currentWorld == CURRENT_WORLD.SCARY_LAND)
		{
			Physics2D.gravity = new Vector2(0.0f, 0.0f);
			Camera.main.transform.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
		}
		else if(currentWorld == CURRENT_WORLD.HAPPY_LAND)
		{
			Physics2D.gravity = new Vector2(0.0f, -9.8f);
			Camera.main.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
		}

		player.SetWorld(currentWorld);
	}

	public void EndGame()
	{
		currentWorld = CURRENT_WORLD.HAPPY_LAND;
		Physics2D.gravity = new Vector2(0.0f, -9.8f);
		Camera.main.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
		SceneManager.LoadScene("Main");
	}
}
