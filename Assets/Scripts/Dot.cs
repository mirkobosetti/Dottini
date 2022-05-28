using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
	float size = 0.2f;
	float speed = 1;
	float triggerRadius = 0.5f;
	float escapeRadius = 0.5f;
	Sprite sprite = null;
	Color color = Color.red;
	SpriteRenderer sr;

	// if true, the dot will be attracted to the mouse
	private bool isTriggered = false;

	public void create(float size, float speed, float triggerRadius, float escapeRadius, Sprite sprite, Color color)
	{
		this.size = size;
		this.speed = speed;
		this.triggerRadius = triggerRadius;
		this.escapeRadius = escapeRadius;
		this.sprite = sprite;
		this.color = color;
	}


	void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	void Start()
	{
		Camera cam = Camera.main;
		float cameraHeight = 2f * cam.orthographicSize;
		float cameraWidth = cameraHeight * cam.aspect;

		transform.position = new Vector3(Random.Range(0, cameraWidth) - cameraWidth / 2, Random.Range(0, cameraHeight) - cameraHeight / 2, 0);
		transform.localScale = new Vector3(size, size, 1);

		sr.sprite = sprite ?? Resources.Load<Sprite>("Assets/Sprites/Dot.png"); // TODO: it doesn't retrieve the sprite from the resources folder
		sr.color = color;
	}

	void FixedUpdate()
	{
		CheckCursorInsideTriggerRadius();
		CheckCursorOutsideEscapeRadius();
		CheckNeighborsTriggerStatus();

		// if the cursor is inside the trigger radius, move towards the cursor
		if (isTriggered)
		{
			Camera cam = Camera.main;

			Vector3 mousePos = Input.mousePosition;
			mousePos.z = -cam.transform.position.z;
			mousePos = cam.ScreenToWorldPoint(mousePos);
			transform.position = Vector3.MoveTowards(transform.position, mousePos, speed * Time.deltaTime);
		}

		// if the cursor is outside the escape radius, start wandering
	}

	void CheckCursorInsideTriggerRadius()
	{
		Camera cam = Camera.main;
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = -cam.transform.position.z;
		mousePos = cam.ScreenToWorldPoint(mousePos);

		if (Vector3.Distance(transform.position, mousePos) < triggerRadius) isTriggered = true;
	}

	void CheckCursorOutsideEscapeRadius()
	{
		Camera cam = Camera.main;
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = -cam.transform.position.z;
		mousePos = cam.ScreenToWorldPoint(mousePos);

		if (Vector3.Distance(transform.position, mousePos) > escapeRadius) isTriggered = false;
	}

	/// <summary>
	/// If any dots inside the trigger radius is triggered, this dot will also be triggered.
	/// </summary>
	void CheckNeighborsTriggerStatus()
	{
		foreach (Dot dot in FindObjectsOfType<Dot>())
		{
			if (dot == this) continue;
			if (Vector3.Distance(transform.position, dot.transform.position) <= triggerRadius)
			{
				// ex: 6,8% to be triggered each frame (TODO: maybe it's too much)
				float distancePercentage = Vector3.Distance(transform.position, dot.transform.position) / triggerRadius * 100;
				
				if (Random.Range(0, 100) < distancePercentage)
				{
					dot.CheckCursorOutsideEscapeRadius();
					if (dot.isTriggered)
					{
						isTriggered = true;
						break;
					}
				}
			}
		}
	}
}