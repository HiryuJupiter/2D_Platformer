using System.Collections;
using UnityEngine;
using UnityEngine.Events;



[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(Player2DMotor))]
[RequireComponent(typeof(Player2DFeedbacks))]
[RequireComponent(typeof(Player2DHealth))]

public class Player2DController : MonoBehaviour
{
	//Components and classes
	Player2DMotor motor;
	Player2DFeedbacks feedbacks;
	Player2DHealth health;
	#region MonoBehavior
	public void Awake()
	{
		motor = GetComponent<Player2DMotor>();
		feedbacks = GetComponentInChildren<Player2DFeedbacks>();
		health = GetComponent<Player2DHealth>();
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

	public void DamagePlayer(Vector2 enemyPos, int damage)
	{
		motor.DamagePlayer(enemyPos);
	}
}