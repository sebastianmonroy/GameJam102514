using UnityEngine;
using System.Collections;

public class RiderStateManager : PlayerStateManager
{
	public SimpleState rideState, jumpState, deathState, finishedState;

	public float rideSpeed, jumpSpeed;
	public float defaultRideSpeed, defaultJumpSpeed;

	private Vector3 jumpDirection, circleCenter, riderToCenter, normal, tangent;

	public override void Start()
	{
		setupState = new SimpleState(SetupEnter, SetupUpdate, SetupExit, "SETUP");
		rideState = new SimpleState(RideEnter, RideUpdate, RideExit, "RIDE");
		jumpState = new SimpleState(JumpEnter, JumpUpdate, JumpExit, "JUMP");
		deathState = new SimpleState(DeathEnter, DeathUpdate, DeathExit, "DEATH");
		finishedState = new SimpleState(null, null, null, "FINISHED");

		Setup();
	}

	public override void Setup () 
	{
		AssignPlayer();

		rideSpeed = rideSpeed / 360f * (2f * Mathf.PI *  MainStateManager.instance.CIRCLE_RADIUS);
		defaultRideSpeed = defaultRideSpeed / 360f * (2f * Mathf.PI *  MainStateManager.instance.CIRCLE_RADIUS);

		jumpSpeed = jumpSpeed / 360f * (2f * Mathf.PI *  MainStateManager.instance.CIRCLE_RADIUS);
		defaultJumpSpeed = defaultJumpSpeed / 360f * (2f * Mathf.PI *  MainStateManager.instance.CIRCLE_RADIUS);

		stateMachine.SwitchStates(setupState);
	}

	public override void Update ()
	{
		circleCenter = MainStateManager.instance.CIRCLE_CENTER;
		riderToCenter = this.transform.position - circleCenter;
		normal = -riderToCenter.normalized;
		this.transform.rotation = Quaternion.LookRotation(-Mathf.Sign(rideSpeed) * Vector3.forward, normal);
		tangent = this.transform.right;

		rideSpeed = Mathf.Sign(rideSpeed) * Mathf.Clamp(Mathf.Abs(rideSpeed), defaultRideSpeed/2f, defaultRideSpeed * 5f);
		jumpSpeed = Mathf.Clamp(Mathf.Abs(jumpSpeed), defaultJumpSpeed/2f, defaultJumpSpeed * 5f);
		Execute();
	}

	public override void Execute ()
	{
		stateMachine.Execute();
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
			this.transform.RotateAround(Vector3.zero, -Vector3.forward, rideSpeed);
		}

		void Aim ()
		{
			Vector3 aimVector = new Vector3(input.LeftStick.x, input.LeftStick.y, 0f);
			if (aimVector.magnitude >= 0.3f && Vector3.Dot(aimVector.normalized, riderToCenter.normalized) < -0.4f)
			{
				jumpDirection = aimVector.normalized;
				Debug.DrawLine(this.transform.position, this.transform.position +  aimVector * 5f, Color.red);
				if (input.AButton) 
				{
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
		Debug.Log(Vector3.Dot(jumpDirection, tangent));
		float speedDelta = defaultJumpSpeed/2f * (Vector3.Dot(jumpDirection, tangent));
		if (Mathf.Sign(GetJumpAngle()) == Mathf.Sign(rideSpeed)) {
			jumpSpeed = rideSpeed + speedDelta;
		} else {
			jumpSpeed = rideSpeed - speedDelta;
		}
		rideSpeed += jumpSpeed;
	}

	void JumpUpdate () 
	{
		this.transform.position += jumpSpeed * jumpDirection;
		CircleDetection();
		JumpInput();
	}
		void JumpInput ()
		{
			
		}

		void CircleDetection ()
		{
			Vector3 centerToRider = circleCenter - this.transform.position;
			float distanceFromCenter = centerToRider.magnitude;
			float circleRadius = MainStateManager.instance.CIRCLE_RADIUS;

			if (distanceFromCenter >= circleRadius)
			{
				this.transform.position = circleCenter - centerToRider.normalized * circleRadius;
				stateMachine.SwitchStates(rideState);
			}
		}

	void JumpExit ()
	{
		// set correct direction for rider movement around circle
		if (GetJumpAngle() > 0) 
		{
			rideSpeed = Mathf.Abs(rideSpeed);
		} 
		else 
		{
			rideSpeed = -1f * Mathf.Abs(rideSpeed);
		}
	}
		float GetJumpAngle() 
		{
			Vector3 a = riderToCenter;
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
