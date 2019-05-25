using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour {

	public GameObject tailPrefab;

	bool gameStarted = false;
	bool ate = false;
	bool isDead = false;
	bool wantsToTurn = false;

	AudioSource biteSound;

	Vector3 direction;
	Vector3 nextDirection;
	List<Transform> tail = new List<Transform>();
	List<GameObject> tailObjects = new List<GameObject>();
	float speed = 0.175f;
	float speedFactor = 0.007f;
	float movementSpeed = 5.0f;

	float turnTrashhold;

	void Start () {
		AudioSource[] audioSources = GetComponents<AudioSource>();
		biteSound = audioSources[0];
		turnTrashhold = GameManager.Instance.gridItemSize / 3;
	}

	public float PlayerScale {
		get
		{
			return GameManager.Instance.gridItemSize;
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
		if (!isDead && gameStarted) {
			if (Input.GetKey (KeyCode.RightArrow))
			{
				SetDirection(Vector3.right);
			}
			else if (Input.GetKey(KeyCode.DownArrow))
			{
				SetDirection(Vector3.down);
			}
			else if (Input.GetKey(KeyCode.LeftArrow))
			{
				SetDirection(Vector3.left);
			}
			else if (Input.GetKey(KeyCode.UpArrow))
			{
				SetDirection(Vector3.up);
			}

			if (wantsToTurn)
			{
			}
			else
			{

			}

			transform.position += direction * Time.deltaTime * movementSpeed;
		}
	}

	public void DestroyPlayer()
	{
		tailObjects.ForEach((g) => { Destroy(g); });
		tail.Clear();
		Destroy(this.gameObject);
	}

	private bool DieIfGoBackward(Vector3 newDirection)
	{
		Vector3 dir = this.direction / PlayerScale;
		if ((dir == Vector3.up && newDirection == Vector3.down) ||
			(dir == Vector3.down && newDirection == Vector3.up) ||
			(dir == Vector3.left && newDirection == Vector3.right) ||
			(dir == Vector3.right && newDirection == Vector3.left))
		{
			Die();
			return true;
		} else {
			return false;
		}
	}

	public void SetDirection(Vector3 newDirection)
	{
		if (isDead) {
			return;
		}

		if (DieIfGoBackward(newDirection)) {
			return;
		}

		if (!gameStarted) {
			gameStarted = true;
			GameManager.Instance.GameStarted();
			this.direction = newDirection * PlayerScale;
			return;
		}

		// todo: check if same direction

		//if (wantsToTurn) {
		//	if(canTurn()) {
		//		// this.direction = newDirection * PlayerScale;
		//	}
		//}
		
		// check if within point treshold area for changing direction
		float x = transform.position.x;
		float y = transform.position.y;

		float xx = x % PlayerScale;

		nextDirection = newDirection;
		wantsToTurn = true;

		// todo: delete this when changed to new system
		this.direction = newDirection * PlayerScale;

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
		isDead = true;
		gameStarted = false;
		direction = new Vector2(0, 0);
		CancelInvoke("Move");
		GameManager.Instance.PlayerDied();
	}

}
 