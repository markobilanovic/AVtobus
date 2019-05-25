using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Point
{
	public float X;
	public float Y;

	public Point(float X, float Y)
	{
		this.X = X;
		this.Y = Y;
	}
}

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	public Camera m_OrthographicCamera;

	public Point[,] gameMatrix;
	public float gridItemSize;

	private int gameSize = 11;
	public List<float> xPoints;
	public List<float> yPoints;


	public GameObject wallPrefab;
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

	private int marginLeftSize = 1;
	private int marginTopSize = 1;
	private float halfBrickSize;

	private float mostLeft;
	private float mostRight;
	private float mostTop;
	private float mostBottom;
	private float centerX;
	private float centerY;

	public GameObject player;
	private GameObject food;

	private GameObject powerUp = null;

	private Sprite[] playerSprites;
	public GameObject optionsMenu;

	public Text scoreLabel;
	public Text finalScoreLabel;
	private int score = 1;

	private AudioSource themeMusic;
	public GameObject volumeIcon;
	private Sprite[] soundSprites;

	private AudioSource[] audioSources;

	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
		InitGame();
		playerSprites = Resources.LoadAll<Sprite>("PlayerSprites");
		soundSprites = Resources.LoadAll<Sprite>("HUD");


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

			player.SendMessage("SetDirection",
				Input.GetKey(KeyCode.RightArrow) ? Vector3.right :
				Input.GetKey(KeyCode.DownArrow) ? Vector3.down :
				Input.GetKey(KeyCode.LeftArrow) ? Vector3.left :
				Input.GetKey(KeyCode.UpArrow) ? Vector3.up : Vector3.zero);
		}
	}

	public void Reset()
	{
		score = 1;
		scoreLabel.text = "" + score;
		finalScoreLabel.text = "" + score;
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
		finalScoreLabel.text = "" + (score);
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
		audioSources = GetComponents<AudioSource>();
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
			powerUp = null;
		}
		Invoke("SpawnPowerUp", Random.Range(5f, 10f));
	}

	void CalculateSizes() {
		if (m_OrthographicCamera)
		{
			Vector2 lDCorner = m_OrthographicCamera.ViewportToWorldPoint(new Vector3(0, 0f, m_OrthographicCamera.nearClipPlane));
			Vector2 rUCorner = m_OrthographicCamera.ViewportToWorldPoint(new Vector3(1f, 1f, m_OrthographicCamera.nearClipPlane));

			float screenWidth = rUCorner.x * 2;
			float screenHeight = rUCorner.y * 2;
			float gameScreenHeight = screenHeight * 0.6f;

			centerX = 0;
			centerY = screenHeight * 0.15f;

			float shorterSide = screenWidth < gameScreenHeight ? screenWidth : gameScreenHeight;
			gridItemSize = (float)System.Math.Floor(shorterSide / gameSize);
			halfBrickSize = gridItemSize / 2;

			float halfGameSize = (gridItemSize / 2 +
								(gridItemSize * ((gameSize - 1) / 2)));
			float leftUpCornerX = centerX - halfGameSize + gridItemSize / 2;
			float leftUpCornerY = centerY - halfGameSize + gridItemSize / 2;


			// creating matrix with field center coordinates
			gameMatrix = new Point[gameSize, gameSize];
			for (int y = 0; y < gameSize; y++) {
				for (int x = 0; x < gameSize; x++)
				{
					gameMatrix[x, y] = new Point(leftUpCornerX + gridItemSize * x,
											 leftUpCornerY + gridItemSize * y);
				}
			}

			mostRight = ((gameSize - 1) / 2) * gridItemSize;
			mostLeft = -mostRight;
			mostTop = centerY + ((gameSize - 1) / 2) * gridItemSize;
			mostBottom = centerY - ((gameSize - 1) / 2) * gridItemSize;
		}
	}

	void ScalePrefabs() {
		brickPrefab.transform.localScale = new Vector3(gridItemSize, gridItemSize, 0);
		upWall.transform.localScale = new Vector3(gridItemSize, gridItemSize, 0);
		downWall.transform.localScale = new Vector3(gridItemSize, gridItemSize, 0);
		wallPrefab.transform.localScale = new Vector3(gridItemSize, gridItemSize, 0);
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
		for (int x = 0; x < gameSize; x++)
		{
			Instantiate(wallPrefab, new Vector2(gameMatrix[x, 0].X, gameMatrix[x, 0].Y), Quaternion.identity);
			Instantiate(wallPrefab, new Vector2(gameMatrix[x, gameSize-1].X, gameMatrix[x, gameSize-1].Y), Quaternion.identity);
		}
		for (int y = 1; y < gameSize - 1; y++)
		{
			Instantiate(wallPrefab, new Vector2(gameMatrix[0, y].X, gameMatrix[0, y].Y), Quaternion.identity);
			Instantiate(wallPrefab, new Vector2(gameMatrix[gameSize - 1, y].X, gameMatrix[gameSize - 1, y].Y), Quaternion.identity);
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
		AudioSource randomAudio = audioSources[Random.Range(1, audioSources.Length)];
		randomAudio.Play();
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

	public void OnTriggerSound()
	{
		// volumeIcon.GetComponent<Image>().sprite = ;
		if (themeMusic.isPlaying)
		{
			themeMusic.Stop();
			volumeIcon.GetComponent<Image>().sprite = soundSprites[1];
		} else {
			themeMusic.Play();
			volumeIcon.GetComponent<Image>().sprite = soundSprites[0];
		}
	}
}
