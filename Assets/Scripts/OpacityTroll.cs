using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpacityTroll : MonoBehaviour
{
	SpriteRenderer spriteRenderer;
	private AudioSource[] audioSources;
	private bool animationStarted;

	// Start is called before the first frame update
	void Start()
    {
		spriteRenderer = GetComponent<SpriteRenderer>();
		audioSources = GetComponents<AudioSource>();
	}

    void StartAnimation()
	{
		if (!animationStarted)
		{
			animationStarted = true;
			InvokeRepeating("IncreaseOpacity", 0, 0.3f);
		}
		AudioSource randomAudio = audioSources[Random.Range(0, audioSources.Length)];
		randomAudio.PlayDelayed(0.5f);
	}

	void IncreaseOpacity()
	{
		Color tempColor = spriteRenderer.color;
		tempColor.a += 0.1f;
		spriteRenderer.color = tempColor;

		
		if (spriteRenderer.color.a >= 1)
		{
			CancelInvoke("IncreaseOpacity");
			InvokeRepeating("DecreaseOpacity", 1, 0.3f);
		}
	}

	void DecreaseOpacity()
	{
		Color tempColor = spriteRenderer.color;
		tempColor.a -= 0.1f;
		spriteRenderer.color = tempColor;

		if (spriteRenderer.color.a <= 0)
		{
			CancelInvoke("DecreaseOpacity");
			animationStarted = false;
		}
	}

}
