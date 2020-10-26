using System.Collections;
using UnityEngine;
using UnityEngine.Events;

//This is the highest level player script that overseas player motor (physics) and feedbacks

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(Player2DMotor))]
[RequireComponent(typeof(Player2DFeedbacks))]

public class Player2DController : MonoBehaviour
{
	//Components and classes
	Player2DMotor motor;
	Player2DFeedbacks feedbacks;

	#region MonoBehavior
	public void Awake()
	{
		motor = GetComponent<Player2DMotor>();
		feedbacks = GetComponentInChildren<Player2DFeedbacks>();
	}

	public void Update()
    {
		//motor.OnUpdate();
		//graphics.OnUpdate();

		if (Input.GetKeyDown(KeyCode.H))
			DamagePlayer(Vector2.zero, 1);
	}
	public void FixedUpdate()
	{
		//motor.OnFixedUpdate();
	}
	#endregion

	
	//When player takes damage, tell the motor this has occured.
	public void DamagePlayer(Vector2 enemyPos, int damage)
	{
		motor.DamagePlayer(enemyPos);
	}
}