using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriggerTimer : MonoBehaviour
{
	public float initialTime = 1;
	public float timeRemaining;

	void Start()
	{
		timeRemaining = initialTime;
	}

	void FixedUpdate()
	{
		if (timeRemaining > 0) timeRemaining -= Time.deltaTime;
		else timeRemaining = initialTime;
	}
}
