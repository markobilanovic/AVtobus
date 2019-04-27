using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Troll : MonoBehaviour
{
	private bool isStarted= false;
	private bool maxPointReached = false;
	private float startX = 32;
	private float endX = 20;

	private AudioSource[] audioSources;

	private void Start()
	{
		audioSources = GetComponents<AudioSource>();
	}

	public void StartAnimation()
	{
		this.isStarted = true;
		AudioSource randomAudio = audioSources[Random.Range(0, audioSources.Length)];
		randomAudio.PlayDelayed(0.5f);
	}

	// Update is called once per frame
	void Update()
    {
        if (isStarted)
		{
			if (maxPointReached)
			{
				transform.Translate(new Vector3(0.1f, 0));
				if (transform.position.x >= startX)
				{
					maxPointReached = false;
					isStarted = false;
				}
			}
			else
			{
				transform.Translate(new Vector3(-0.1f, 0));
				if (transform.position.x <= endX)
				{
					maxPointReached = true;
				}
			}
		}
    }
}
