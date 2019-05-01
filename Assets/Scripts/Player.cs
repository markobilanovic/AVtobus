using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour {

	public GameObject tailPrefab;

	bool gameStarted = false;
	bool ate = false;
	bool isDead = false;

	AudioSource crashSound;
	AudioSource biteSound;

	Vector2 direction;
	List<Transform> tail = new List<Transform>();
	List<GameObject> tailObjects = new List<GameObject>();
	float speed = 0.175f;
	float speedFactor = 0.007f;
	float playerScale = 1;

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
			GameManager.Instance.GameStarted();
			InvokeRepeating("Move", 0, speed);
		}
	}

	public void SpeedUp()
	{
		CancelInvoke("Move");
		speed -= speedFactor;
		InvokeRepeating("Move", speed, speed);
	}

	// todo: delete - only for debugging
	void Update () {
		if (!isDead) {
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

	void Move() {
		Vector2 v = transform.position;
		transform.Translate(direction);

		if (ate)
		{
			GameObject g = (GameObject)Instantiate(tailPrefab, v, Quaternion.identity);
			tail.Insert(0, g.transform);
			tailObjects.Insert(0, g);
			ate = false;

			// speed up
			if (tail.Count % 3 == 0)
			{
				SpeedUp();
			}
		}
		else if (tail.Count > 0)
		{
			tail.Last().position = v;
			tail.Insert(0, tail.Last());
			tail.RemoveAt(tail.Count - 1);
		}
	}

	public void SetDirection(Vector2 newDirection)
	{
		if (!isDead)
		{
			Vector2 dir = this.direction / PlayerScale;
			if ((dir == Vector2.up && newDirection == Vector2.down) ||
				(dir == Vector2.down && newDirection == Vector2.up) ||
				(dir == Vector2.left && newDirection == Vector2.right) ||
				(dir == Vector2.right && newDirection == Vector2.left))
			{
				Die();
			}
			else
			{
				StartMoving();
				this.direction = newDirection * PlayerScale;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.name.StartsWith("Food"))
		{
			biteSound.Play();
			ate = true;
			Destroy(coll.gameObject);
			GameManager.Instance.PlayerAteFood();
		}
		else if (coll.name.StartsWith("Brze"))
		{
			Destroy(coll.gameObject);
			//CancelInvoke("Move");
			//InvokeRepeating("Move", 0, speed - 0.03f);
			//Invoke("StopSpeedPowerUp", 4);
			GameManager.Instance.ShowTroll();
		}
		else if (coll.name.StartsWith("Jace"))
		{
			Destroy(coll.gameObject);
			GameManager.Instance.ShowTroll();
		}
		else if (coll.name.StartsWith("Bolje"))
		{
			Destroy(coll.gameObject);
			GameManager.Instance.ShowTroll();
		}
		else
		{
			Die();
		}
	}

	void StopSpeedPowerUp()
	{
		CancelInvoke("Move");
		InvokeRepeating("Move", speed, speed);
	}


	void Die() {
		crashSound.Play();
		isDead = true;
		gameStarted = false;
		direction = new Vector2(0, 0);
		CancelInvoke("Move");
		GameManager.Instance.PlayerDied();
	}

}
 