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

	public Vector2 direction;
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

	// todo: delete - only for debugging
	void Update () {
		if (!isDied) {
			Vector3 scale = transform.localScale;
			if (Input.GetKey (KeyCode.RightArrow))
			{
				SetDirection(Vector2.right);
			}
			else if (Input.GetKey(KeyCode.DownArrow))
			{
				SetDirection(Vector2.down);
			}
			else if (Input.GetKey(KeyCode.LeftArrow))
			{
				SetDirection(Vector2.left);
			}
			else if (Input.GetKey(KeyCode.UpArrow))
			{
				SetDirection(Vector2.up);
			}
		}
	}

	public void DestroyPlayer()
	{
		tailObjects.ForEach((g) => { Destroy(g); });
		tail.Clear();
		Destroy(this.gameObject);
	}

	public void StartNew()
	{
		tailObjects.ForEach((g) => { Destroy(g); });
		tail.Clear();
		transform.position = new Vector3(0, 0, 0);
		direction = new Vector2(0, 0);
		isDied = false;
	}

	void Move() {
		Vector2 v = transform.position;
		transform.Translate(direction);

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

	public void SetDirection(Vector2 direction)
	{
		Vector2 dir = this.direction / PlayerScale;
		Debug.Log(dir);
		if ((dir == Vector2.up && direction == Vector2.down) ||
			(dir == Vector2.down && direction == Vector2.up) ||
			(dir == Vector2.left && direction == Vector2.right) ||
			(dir == Vector2.right && direction == Vector2.left))
		{
			Die();
		}
		else
		{
			StartMoving();
			this.direction = direction * PlayerScale;
		}
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
		direction = new Vector2(0, 0);
		CancelInvoke("Move");
		GameManager.Instance.GameOver();
	}

}
 