using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	CircleCollider2D cc;
	Rigidbody2D rb;
	TriggerTimer timer;

	// if true, the dot will be attracted to the mouse
	private bool isTriggered = false;
	private int id;

	public void create(int id, float size, float speed, float triggerRadius, float escapeRadius, Sprite sprite, Color color)
	{
		this.id = id;
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
		cc = GetComponent<CircleCollider2D>();
		rb = GetComponent<Rigidbody2D>();
		rb.gravityScale = 0;
		timer = GetComponent<TriggerTimer>();
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

		if (timer.timeRemaining == timer.initialTime) CheckNeighborsTriggerStatus();
		
		if (isTriggered)
		{
			// if the cursor is inside the trigger radius, move towards the cursor
			Camera cam = Camera.main;

			Vector3 mousePos = Input.mousePosition;
			mousePos.z = -cam.transform.position.z;
			mousePos = cam.ScreenToWorldPoint(mousePos);
			transform.position = Vector3.MoveTowards(transform.position, mousePos, speed * Time.deltaTime);
		}
		else
		{
			// if it's not trigghered start wandering
			// transform.position += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0) * speed / 2 * Time.deltaTime;
			var xMovement = Mathf.PerlinNoise((Time.time * id / 100), id) * 2 - 1;
			var yMovement = Mathf.PerlinNoise(id, (Time.time * id / 100)) * 2 - 1;
			transform.position += new Vector3(xMovement, yMovement, 0) * speed * Time.deltaTime;
		}

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
		// triggered dots inside the trigger radius
		Dot[] triggeredDots = FindObjectsOfType<Dot>()
						.Where(d => d.id != id)
						.Where(d => Vector3.Distance(transform.position, d.transform.position) < triggerRadius)
						.Where(d => d.isTriggered)
						.ToArray();

		if (triggeredDots.Length == 0) return;

		foreach (Dot dot in triggeredDots) dot.CheckCursorOutsideEscapeRadius();

		isTriggered = true;
	}
}
