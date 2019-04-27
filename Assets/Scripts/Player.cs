using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour {

	bool gameStarted = false;

	bool ate = false;

	bool isDied = false;

	public GameObject tailPrefab;

	AudioSource crashSound;
	AudioSource biteSound;

	Vector2 dir;
	float speed = 1;
	float playerScale = 1;

	List<Transform> tail = new List<Transform>();
	List<GameObject> tailObjects = new List<GameObject>();

	void Start () {
		AudioSource[] audioSources = GetComponents<AudioSource>();
		crashSound = audioSources[0];
		biteSound = audioSources[1];
	}

	public float PlayerScale {
		get
		{
			return GameManager.Instance.gridItemSize;
		}
	}

	public void StartMoving() {
		if(!gameStarted)
		{
			gameStarted = true;
			InvokeRepeating("Move", 0, 0.2f);
		}
	}

	public void ChangeSpeed(float speed)
	{
		CancelInvoke("Move");

	}

	public void StopMoving()
	{
		if (gameStarted)
		{
			gameStarted = false;
		}
	}

	void Update () {
		if (!isDied) {

			Vector3 scale = transform.localScale;
			if (Input.GetKey (KeyCode.RightArrow))
				dir = Vector2.right * scale;
			else if (Input.GetKey (KeyCode.DownArrow))
				dir = -Vector2.up * scale;
			else if (Input.GetKey (KeyCode.LeftArrow))
				dir = -Vector2.right * scale;
			else if (Input.GetKey (KeyCode.UpArrow))
				dir = Vector2.up * scale;

		} else {
			if (Input.GetKey(KeyCode.R)){
				tailObjects.ForEach((g) => { Destroy(g); });
				tail.Clear();
				transform.position = new Vector3(0, 0, 0);
				isDied = false;
			}
		}
	}

	public void DestroyPlayer()
	{
		tailObjects.ForEach((g) => { Destroy(g); });
		tail.Clear();
	}

	public void StartNew()
	{
		tailObjects.ForEach((g) => { Destroy(g); });
		tail.Clear();
		transform.position = new Vector3(0, 0, 0);
		dir = new Vector2(0, 0);
		isDied = false;
	}

	void Move() {
		if (!isDied && dir != null) {
			Vector2 v = transform.position;
			transform.Translate (dir);

			if (ate) {
				GameObject g = (GameObject)Instantiate (tailPrefab, v, Quaternion.identity);

				tail.Insert (0, g.transform);
				tailObjects.Insert(0, g);

				ate = false;
			} else if (tail.Count > 0) {
				tail.Last().position = v;
				tail.Insert(0, tail.Last());
				tail.RemoveAt(tail.Count - 1);
			}
		}
	}

	public void SetDirection(Vector2 direction)
	{
		StartMoving();
		dir = direction * PlayerScale;
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.name.StartsWith("Food")) {
			biteSound.Play();
			ate = true;
			Destroy(coll.gameObject);
			GameManager.Instance.SpawnFood();
		} else {
			Die();
		}
	}


	void Die() {
		crashSound.Play();
		isDied = true;
		gameStarted = false;
		dir = new Vector2(0, 0);
		GameManager.Instance.GameOver();
	}

}
 