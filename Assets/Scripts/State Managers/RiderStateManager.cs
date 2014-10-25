using UnityEngine;
using System.Collections;

public class RiderStateManager : PlayerStateManager
{
	public SimpleState rideState, jumpState, deathState, finishedState;

	public float rideSpeed, jumpSpeed;

	private Vector3 jumpDirection;

	public override void Start()
	{
		setupState = new SimpleState(SetupEnter, SetupUpdate, SetupExit, "SETUP");
		rideState = new SimpleState(RideEnter, RideUpdate, RideExit, "RIDE");
		jumpState = new SimpleState(JumpEnter, JumpUpdate, JumpExit, "JUMP");
		deathState = new SimpleState(DeathEnter, DeathUpdate, DeathExit, "DEATH");
		finishedState = new SimpleState(null, null, null, "FINISHED");

		Setup();
	}

	public override void Update ()
	{
		Execute();
	}

	public override void Execute ()
	{
		stateMachine.Execute();
	}

	public override void Setup () 
	{
		stateMachine.SwitchStates(setupState);
	}

	#region SETUP
	public override void SetupEnter ()
	{

	}

	public override void SetupUpdate ()
	{
		stateMachine.SwitchStates(rideState);
	}

	public override void SetupExit ()
	{
		
	}
	#endregion

	#region RIDE
	void RideEnter ()
	{

	}

	void RideUpdate () 
	{
		RideInput();
		Ride();
	}
		void RideInput ()
		{
			if (Input.GetKeyUp("space")) 
			{
				stateMachine.SwitchStates(jumpState);
				jumpDirection = (MainStateManager.instance.CIRCLE_CENTER - this.transform.position).normalized;
			}
		}

		void Ride ()
		{
			this.transform.RotateAround(Vector3.zero, Vector3.forward, -rideSpeed * Time.deltaTime);
		}

	void RideExit ()
	{

	}
	#endregion

	#region JUMP
	void JumpEnter ()
	{

	}

	void JumpUpdate () 
	{
		this.transform.position += jumpSpeed * Time.deltaTime * jumpDirection;
		CircleDetection();
		JumpInput();
	}
		void JumpInput ()
		{
			
		}

		void CircleDetection ()
		{
			Vector3 circleCenter = MainStateManager.instance.CIRCLE_CENTER;
			Vector3 riderToCenter = circleCenter - this.transform.position;
			float distanceFromCenter = riderToCenter.magnitude;
			float circleRadius = MainStateManager.instance.CIRCLE_RADIUS;
			
			if (distanceFromCenter >= circleRadius)
			{
				this.transform.position = circleCenter - riderToCenter.normalized * circleRadius;
				stateMachine.SwitchStates(rideState);
			}
		}

	void JumpExit ()
	{

	}
	#endregion
	
	#region DEATH
	void DeathEnter ()
	{

	}

	void DeathUpdate () 
	{

	}

	void DeathExit ()
	{
		stateMachine.SwitchStates(finishedState);
	}
	#endregion
}
