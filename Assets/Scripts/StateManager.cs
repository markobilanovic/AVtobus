using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateManager : MonoBehaviour
{
	public void OnButtonUp()
	{
		GameManager.Instance.player.SendMessage("GoUp");
	}

	public void OnButtonDown()
	{
		GameManager.Instance.player.SendMessage("GoDown");
	}

	public void OnButtonLeft()
	{
		GameManager.Instance.player.SendMessage("GoLeft");
	}

	public void OnButtonRight()
	{
		GameManager.Instance.player.SendMessage("GoRight");
	}
}
