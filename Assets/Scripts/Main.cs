using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
	[Header("Dots")]
	public int count = 500;

	public float size = 0.2f;
	public float speed = 1;

	[Tooltip("Radius of the circle where dots will be attracted to the mouse")]
	public float triggerRadius = 2f;

	[Tooltip("Radius of the circle where dots will stop following the mouse")]
	public float escapeRadius = 4f;

	[Range(0.0f, 100.0f)]
	[Tooltip("Percentage of randomness")]
	public float randomness = 10f;

	public Sprite sprite = null;
	public Color color = Color.red;

	GameObject[] dots;
	Camera cam;

	void Start()
	{
		dots = new GameObject[count];

		for (int i = 0; i < count; i++)
		{
			dots[i] = new GameObject("Dot " + i);
			dots[i].AddComponent<SpriteRenderer>();
			Dot dot = dots[i].AddComponent<Dot>();

			(float min, float max) sizeRandomness = GetRandomnessMinAndMax(size, randomness);
			(float min, float max) speedRandomness = GetRandomnessMinAndMax(speed, randomness);
			(float min, float max) triggerRadiusRandomness = GetRandomnessMinAndMax(triggerRadius, randomness);
			(float min, float max) escapeRadiusRandomness = GetRandomnessMinAndMax(escapeRadius, randomness);

			// TODO: smaller is faster, bigger is slower
			// TODO: less smaller and less bigger, we need a random that is not linear // https://docs.unity3d.com/ScriptReference/Mathf.SmoothStep.html
			dot.create(
				i,
				Mathf.Lerp(sizeRandomness.min, sizeRandomness.max, Random.Range(0f, 1f)),
				Mathf.Lerp(speedRandomness.min, speedRandomness.max, Random.Range(0f, 1f)),
				Mathf.Lerp(triggerRadiusRandomness.min, triggerRadiusRandomness.max, Random.Range(0f, 1f)),
				Mathf.Lerp(escapeRadiusRandomness.min, escapeRadiusRandomness.max, Random.Range(0f, 1f)),
				sprite,
				color
			);
		}
	}

	(float min, float max) GetRandomnessMinAndMax(float baseNumber, float percentage)
	{
		float min = baseNumber - baseNumber*percentage/100;
		float max = baseNumber + baseNumber*percentage/100;

		return (min, max);
	}
}
