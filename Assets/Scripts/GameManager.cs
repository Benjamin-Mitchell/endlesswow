using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public float moveSpeed = 4;
	private float fixedSpeed;

	//0 = happy land, 1 = scary land
	public enum CURRENT_WORLD { HAPPY_LAND, SCARY_LAND};
	public CURRENT_WORLD currentWorld = CURRENT_WORLD.HAPPY_LAND;

	Player player;

	public GameObject goodBackground, badBackground;

	public Transform evilHeartTransform;
	public Transform goodHeartTransform;

	public GameObject hearts;


	// Start is called before the first frame update
	void Start()
	{
		fixedSpeed = moveSpeed;
		player = GameObject.Find("Player").GetComponent<Player>();
	}

	// Update is called once per frame
	void Update()
	{

		if (Input.GetKeyDown(KeyCode.G))
		{
			StartWorldSwitch();
			
		}
	}

	private void StartWorldSwitch()
	{
		//can't update actual currentworld yet - sections rely on that info.
		CURRENT_WORLD  nextWorld = (CURRENT_WORLD)(((int)currentWorld + 1) % 2);
		moveSpeed = 0;
		Physics2D.gravity = new Vector2(0.0f, 0.0f);
		player.StartWorldSwitch(nextWorld);

		StartCoroutine(PerformWorldSwitchOne(nextWorld));
	}

	private IEnumerator PerformWorldSwitchOne(CURRENT_WORLD nextWorld)
	{
		yield return new WaitForSeconds(0.5f);
		
		//can update currentWorld now.
		currentWorld = nextWorld;

		if (currentWorld == CURRENT_WORLD.SCARY_LAND)
		{
			Physics2D.gravity = new Vector2(0.0f, 0.0f);
			Camera.main.transform.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
			Camera.main.transform.position = new Vector3(-4.0f, 0.0f, -10.0f);
			badBackground.SetActive(true);
			goodBackground.SetActive(false);
		}
		else if (currentWorld == CURRENT_WORLD.HAPPY_LAND)
		{
			Physics2D.gravity = new Vector2(0.0f, -9.8f);
			Camera.main.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
			Camera.main.transform.position = new Vector3(0.0f, 0.0f, -10.0f);
			badBackground.SetActive(false);
			goodBackground.SetActive(true);
		}

		player.SetWorld(currentWorld);
		hearts.transform.position = currentWorld == CURRENT_WORLD.SCARY_LAND ? evilHeartTransform.position : goodHeartTransform.position;
		hearts.transform.rotation = currentWorld == CURRENT_WORLD.SCARY_LAND ? evilHeartTransform.rotation : goodHeartTransform.rotation;
		StartCoroutine(PerformWorldSwitchTwo());
	}

	private IEnumerator PerformWorldSwitchTwo()
	{
		yield return new WaitForSeconds(0.5f);
		player.ResumePhysics();
		moveSpeed = fixedSpeed;
	}

	public void EndGame()
	{
		currentWorld = CURRENT_WORLD.HAPPY_LAND;
		Physics2D.gravity = new Vector2(0.0f, -9.8f);
		Camera.main.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
		SceneManager.LoadScene("Main");
	}
}
