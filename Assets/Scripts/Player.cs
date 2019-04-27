using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour {

	bool gameStarted = false;

	bool ate = false;

	bool isDead = false;

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
			GameManager.Instance.GameStarted();
			InvokeRepeating("Move", 0, 0.2f);
		}
	}

	public void ChangeSpeed(float speed)
	{
		CancelInvoke("Move");

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
			CancelInvoke("Move");
			InvokeRepeating("Move", 0, 0.1f);
		}
		else if (coll.name.StartsWith("Jace"))
		{

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


	void Die() {
		crashSound.Play();
		isDead = true;
		gameStarted = false;
		direction = new Vector2(0, 0);
		CancelInvoke("Move");
		GameManager.Instance.PlayerDied();
	}

}
 