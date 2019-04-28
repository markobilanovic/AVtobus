using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public Camera m_OrthographicCamera;

	public GameObject leftWall;
	public GameObject rightWall;
	public GameObject upWall;
	public GameObject downWall;

	public GameObject brickPrefab;
	public GameObject playerPrefab;
	public GameObject tailPrefab;
	public GameObject foodPrefab;
	public GameObject brzePrefab;
	public GameObject jacePrefab;
	public GameObject boljePrefab;
	public GameObject trollPrefab;

	public float gridItemSize;

	private int gameSize = 13;
	private int marginLeftSize = 1;
	private int marginTopSize = 1;
	private float screenWidth;
	private float screenHeight;
	private float halfBrickSize;

	private float mostLeft;
	private float mostRight;
	private float mostTop;
	private float mostBottom;

	public GameObject player;
	private GameObject food;

	private GameObject powerUp = null;

	private Sprite[] sprites;
	public GameObject optionsMenu;

	public Text scoreLabel;
	private int score = 1;

	private AudioSource themeMusic;

	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
		InitGame();
		sprites = Resources.LoadAll<Sprite>("PlayerSprites");


	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}

		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.DownArrow) ||
			Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.UpArrow))
		{
			player.SendMessage("StartMoving");
		}
	}

	public void Reset()
	{
		score = 1;
		scoreLabel.text = "" + score;
		optionsMenu.SetActive(false);
		DestroyPowerUp();
		player.SendMessage("DestroyPlayer");
		Destroy(food);

		CreatePlayer();
		SpawnFood();
	}

	// triggers when player makes first move
	public void GameStarted()
	{
		PlayMusic();
		Invoke("SpawnPowerUp", Random.Range(5f, 10f));
	}

	public void SpawnFood()
	{
		float x = (int)Random.Range(1, gameSize);
		float y = (int)Random.Range(1, gameSize);

		x = mostLeft + x * gridItemSize;
		y = mostBottom + y * gridItemSize;

		food = Instantiate(foodPrefab, new Vector2(x, y), Quaternion.identity);
	}

	public void PlayerAteFood()
	{
		scoreLabel.text = "" + (++score);
		SpawnFood();
	}

	public void ShowOptionsMenu()
	{
		optionsMenu.SetActive(true);
	}

	void InitGame()
	{
		LoadAudio();
		CalculateSizes();
		ScalePrefabs();
		CreateWalls();
		CreatePlayer();
		SpawnFood();
	}

	void LoadAudio()
	{
		AudioSource[] audioSources = GetComponents<AudioSource>();
		themeMusic = audioSources[0];
	}

	void SpawnPowerUp()
	{
		if (powerUp == null)
		{
			float x = (int)Random.Range(1, gameSize);
			float y = (int)Random.Range(1, gameSize);

			x = mostLeft + x * gridItemSize;
			y = mostBottom + y * gridItemSize;

			int random = Random.Range(0, 3) % 3;

			GameObject powerUp;
			if (random == 0) {
				powerUp = brzePrefab;
			} else if (random == 1) {
				powerUp = jacePrefab;
			} else {
				powerUp = boljePrefab;
			}
			 

			powerUp = Instantiate(powerUp, new Vector2(x, y), Quaternion.identity);
			Invoke("DestroyPowerUp", 4);
		}
	}

	void DestroyPowerUp()
	{
		if (powerUp != null)
		{
			Destroy(powerUp);
		}
		Invoke("SpawnPowerUp", Random.Range(5f, 10f));
	}

	void CalculateSizes() {
		if (m_OrthographicCamera)
		{
			Vector2 lDCorner = m_OrthographicCamera.ViewportToWorldPoint(new Vector3(0, 0f, m_OrthographicCamera.nearClipPlane));
			Vector2 rUCorner = m_OrthographicCamera.ViewportToWorldPoint(new Vector3(1f, 1f, m_OrthographicCamera.nearClipPlane));

			screenWidth = rUCorner.x * 2;
			screenHeight = rUCorner.y * 2;

			gridItemSize = (float)System.Math.Floor(screenWidth / gameSize);
			halfBrickSize = gridItemSize / 2;

			mostRight = (gameSize / 2 + 1) * gridItemSize;
			mostLeft = -mostRight;

			mostTop = rUCorner.y - halfBrickSize - gridItemSize * marginTopSize;
			mostBottom = mostTop - gridItemSize * (gameSize + 1);
		}
	}

	void ScalePrefabs() {
		brickPrefab.transform.localScale = new Vector3(gridItemSize, gridItemSize, 0);
		upWall.transform.localScale = new Vector3(gridItemSize, gridItemSize, 0);
		downWall.transform.localScale = new Vector3(gridItemSize, gridItemSize, 0);
		leftWall.transform.localScale = new Vector3(gridItemSize, gridItemSize, 0);
		rightWall.transform.localScale = new Vector3(gridItemSize, gridItemSize, 0);
		playerPrefab.transform.localScale = new Vector3(gridItemSize, gridItemSize, 0);
		foodPrefab.transform.localScale = new Vector3(gridItemSize, gridItemSize, 0);
		tailPrefab.transform.localScale = new Vector3(gridItemSize * 0.7f, gridItemSize * 0.7f, 0);
		brzePrefab.transform.localScale = new Vector3(gridItemSize * 0.7f, gridItemSize * 0.7f, 0);
		boljePrefab.transform.localScale = new Vector3(gridItemSize * 0.7f, gridItemSize * 0.7f, 0);
		jacePrefab.transform.localScale = new Vector3(gridItemSize * 0.7f, gridItemSize * 0.7f, 0);
	}

	void CreateWalls()
	{
		for (float x = 0; x < mostRight; x += gridItemSize)
		{
			Debug.Log("x: " + x);
			Instantiate(upWall, new Vector2(x, mostTop), Quaternion.identity);
			Instantiate(downWall, new Vector2(x, mostBottom), Quaternion.identity);

			Instantiate(upWall, new Vector2(-x, mostTop), Quaternion.identity);
			Instantiate(downWall, new Vector2(-x, mostBottom), Quaternion.identity);
		}

		for (float y = mostTop - gridItemSize; y >= mostTop - (gridItemSize * gameSize); y -= gridItemSize)
		{
			Instantiate(leftWall, new Vector2(mostLeft, y), Quaternion.identity);
			Instantiate(rightWall, new Vector2(mostRight, y), Quaternion.identity);
		}
	}

	void CreatePlayer() {
		player = Instantiate(playerPrefab, new Vector2((mostRight + mostLeft) / 2, (mostTop + mostBottom) / 2), Quaternion.identity);
	}

	public void PlayerDied()
	{
		StopMusic();
		ShowOptionsMenu();
		CancelInvoke();
	}

	public void PlayMusic()
	{
		themeMusic.Play();
	}

	public void StopMusic()
	{
		themeMusic.Stop();
	}

	public void ShowTroll()
	{
		trollPrefab.SendMessage("StartAnimation");
	}
}
