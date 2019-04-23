using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public Camera m_OrthographicCamera;

	public GameObject brickPrefab;
	public GameObject playerPrefab;
	public GameObject tailPrefab;
	public GameObject foodPrefab;

	public float gridItemSize;

	private int gameSize = 17;
	private int marginLeftSize = 1;
	private int marginTopSize = 2;
	private float screenWidth;
	private float screenHeight;
	private float halfBrickSize;

	private float mostLeft;
	private float mostRight;
	private float mostTop;
	private float mostBottom;

	public GameObject player;

	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
		InitGame();
	}

	void InitGame() {
		CalculateSizes();
		ScalePrefabs();
		CreateWalls();
		CreatePlayer();
		SpawnFood();
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

	void CalculateSizes() {
		if (m_OrthographicCamera)
		{
			Vector2 lDCorner = m_OrthographicCamera.ViewportToWorldPoint(new Vector3(0, 0f, m_OrthographicCamera.nearClipPlane));
			Vector2 rUCorner = m_OrthographicCamera.ViewportToWorldPoint(new Vector3(1f, 1f, m_OrthographicCamera.nearClipPlane));

			screenWidth = rUCorner.x * 2;
			screenHeight = rUCorner.y * 2;

			gridItemSize = screenWidth / (gameSize + marginLeftSize * 2);
			halfBrickSize = gridItemSize / 2;

			mostRight = (gameSize + marginLeftSize) / 2 * gridItemSize;
			mostLeft = -mostRight;

			mostTop = rUCorner.y - halfBrickSize - gridItemSize * marginTopSize;
			mostBottom = mostTop - gridItemSize * (gameSize + 1);
		}
	}

	void ScalePrefabs() {
		brickPrefab.transform.localScale = new Vector3(gridItemSize, gridItemSize, 0);
		playerPrefab.transform.localScale = new Vector3(gridItemSize, gridItemSize, 0);
		foodPrefab.transform.localScale = new Vector3(gridItemSize, gridItemSize, 0);
		tailPrefab.transform.localScale = new Vector3(gridItemSize * 0.7f, gridItemSize * 0.7f, 0);
	}

	void CreateWalls()
	{
		for (float x = mostLeft; x <= mostRight + 1; x += gridItemSize) // +1 -> weird bug where == is not good
		{
			Instantiate(brickPrefab, new Vector2(x, mostTop), Quaternion.identity);
			Instantiate(brickPrefab, new Vector2(x, mostBottom), Quaternion.identity);
		}

		for (float y = mostTop; y >= mostBottom; y -= gridItemSize)
		{
			Instantiate(brickPrefab, new Vector2(mostLeft, y), Quaternion.identity);
			Instantiate(brickPrefab, new Vector2(mostRight, y), Quaternion.identity);
		}
	}

	void CreatePlayer() {
		player = Instantiate(playerPrefab, new Vector2((mostRight + mostLeft) / 2, (mostTop + mostBottom) / 2), Quaternion.identity);
	}

	public void SpawnFood() {
		float x = (int)Random.Range(1, gameSize);
		float y = (int)Random.Range(1, gameSize);

		x = mostLeft + x * gridItemSize;
		y = mostBottom + y * gridItemSize;

		Instantiate(foodPrefab, new Vector2(x, y), Quaternion.identity);
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
