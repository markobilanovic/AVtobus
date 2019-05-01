using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
	public void OnButtonUp()
	{
		GameManager.Instance.player.SendMessage("SetDirection", Vector2.up);
	}

	public void OnButtonDown()
	{
		GameManager.Instance.player.SendMessage("SetDirection", Vector2.down);
	}

	public void OnButtonLeft()
	{
		GameManager.Instance.player.SendMessage("SetDirection", Vector2.left);
	}

	public void OnButtonRight()
	{
		GameManager.Instance.player.SendMessage("SetDirection", Vector2.right);
	}

	public void OnPlayAgain()
	{
		GameManager.Instance.Reset();
	}

	public void OnTriggerSound()
	{
		GameManager.Instance.OnTriggerSound();
	}

	public void Exit()
	{
		Application.Quit();
	}
}
