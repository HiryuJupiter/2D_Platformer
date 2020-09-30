using System.Collections;
using UnityEngine;
using UnityEngine.Events;


[DefaultExecutionOrder(-2)]
[RequireComponent(typeof(Player2DController_Graphics))]
[RequireComponent(typeof(Player2DController_Motor))]
public class Player2DController : MonoBehaviour
{
	//Components and classes
	Player2DController_Motor motor;
	Player2DController_Graphics graphics;


	#region MonoBehavior
	void Awake()
	{
		motor = GetComponent<Player2DController_Motor>();
		graphics = GetComponent<Player2DController_Graphics>();
	}

    void Update()
    {
		
	}
    void FixedUpdate()
	{
		
		
	}
	#endregion
}