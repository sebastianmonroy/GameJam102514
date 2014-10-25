using UnityEngine;
using System.Collections;

public class RiderStateManager : PlayerStateManager
{
	public SimpleState rideState, jumpState, deathState, finishedState;

	public float rideSpeed, jumpSpeed;

	private Vector3 jumpDirection, circleCenter, riderToCenter;

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
		circleCenter = MainStateManager.instance.CIRCLE_CENTER;
		riderToCenter = circleCenter - this.transform.position;

		Execute();
	}

	public override void Execute ()
	{
		stateMachine.Execute();
	}

	public override void Setup () 
	{
		AssignPlayer();
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
			Aim();
		}

		void Ride ()
		{
			this.transform.RotateAround(Vector3.zero, Vector3.forward, -rideSpeed * Time.deltaTime);
		}

		void Aim ()
		{
			Vector3 aimVector = new Vector3(input.LeftStick.x, input.LeftStick.y, 0f);
			if (aimVector.magnitude >= 0.3f && Vector3.Dot(aimVector.normalized, riderToCenter.normalized) > 0.4f)
			{
				Debug.DrawLine(this.transform.position, this.transform.position +  aimVector * 5f, Color.red);
				if (input.AButton) 
				{
					jumpDirection = aimVector.normalized;
					stateMachine.SwitchStates(jumpState);
				}
			}
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
			float distanceFromCenter = riderToCenter.magnitude;
			float circleRadius = MainStateManager.instance.CIRCLE_RADIUS;
			Debug.Log(distanceFromCenter);
			if (distanceFromCenter >= circleRadius)
			{
				this.transform.position = circleCenter - riderToCenter.normalized * circleRadius;
				stateMachine.SwitchStates(rideState);
			}
		}

	void JumpExit ()
	{
		Debug.Log(GetJumpAngle());
	}
		public float GetJumpAngle() {
			Vector3 a = riderToCenter.normalized;
			Vector3 b = jumpDirection;
			Vector3 n = Vector3.forward;
			float sign = (Vector3.Dot(Vector3.Cross(a,b), n) > 0) ? -1 : 1;
			float angle = Vector3.Angle(a,b);

			return sign * angle;
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
