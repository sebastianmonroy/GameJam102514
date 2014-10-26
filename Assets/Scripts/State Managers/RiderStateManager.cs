using UnityEngine;
using System.Collections;

public class RiderStateManager : PlayerStateManager
{
	public SimpleState rideState, jumpState, deathState, finishedState;

	public float minSpeed, defaultSpeed, maxSpeed;
	public float jumpSpeed;
	private float radialSpeed, speed;

	private Vector3 jumpDirection, center, normal, tangent;

	public override void Start()
	{
		setupState = new SimpleState(SetupEnter, SetupUpdate, SetupExit, "SETUP");
		rideState = new SimpleState(RideEnter, RideUpdate, RideExit, "RIDE");
		jumpState = new SimpleState(JumpEnter, JumpUpdate, JumpExit, "JUMP");
		deathState = new SimpleState(DeathEnter, DeathUpdate, DeathExit, "DEATH");
		finishedState = new SimpleState(null, null, null, "FINISHED");
	}

	public override void Setup () 
	{
		AssignPlayer();
		speed = defaultSpeed;
		stateMachine.SwitchStates(setupState);
	}

	public override void Update ()
	{
		speed = Mathf.Sign(speed) * Mathf.Clamp(Mathf.Abs(speed), Mathf.Abs(minSpeed), Mathf.Abs(maxSpeed));
		radialSpeed = speed / (2f * Mathf.PI * MainStateManager.instance.CIRCLE_RADIUS) * 360f;

		center = MainStateManager.instance.CIRCLE_CENTER;
		normal = (center - this.transform.position).normalized;
		this.transform.rotation = Quaternion.LookRotation(-Mathf.Sign(speed) * Vector3.forward, normal);
		tangent = this.transform.right;

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
		Ride();
		RideInput();
	}
		void RideInput ()
		{
			Aim();
		}

		void Ride ()
		{
			this.transform.RotateAround(Vector3.zero, -Vector3.forward, radialSpeed * Time.deltaTime);
		}

		void Aim ()
		{
			Vector3 aimVector = new Vector3(input.LeftStick.x, input.LeftStick.y, 0f);
			if (aimVector.magnitude >= 0.3f && Vector3.Dot(aimVector.normalized, normal) > 0.4f)
			{
				jumpDirection = aimVector.normalized;
				Debug.DrawLine(this.transform.position, this.transform.position + aimVector * 5f, Color.red);
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
		float sign = Mathf.Sign(Vector3.Dot(tangent, jumpDirection));
		float factor = (1f + Vector3.Dot(tangent, jumpDirection))/2f;
		speed = sign * factor * speed * 1.5f;
	}

	void JumpUpdate () 
	{
		this.transform.position += (Mathf.Abs(speed) + jumpSpeed) * jumpDirection * Time.deltaTime;
		CircleDetection();
		JumpInput();
	}
		void JumpInput ()
		{
			
		}

		void CircleDetection ()
		{
			float distanceFromCenter = Vector3.Distance(this.transform.position, center);

			if (distanceFromCenter >= MainStateManager.instance.CIRCLE_RADIUS)
			{
				SnapToCircle();
				stateMachine.SwitchStates(rideState);
			}
		}

	void JumpExit ()
	{
		speed = (speed + Mathf.Sign(speed) * jumpSpeed) * Mathf.Abs(Vector3.Dot(tangent, jumpDirection));
	}
		void SnapToCircle ()
		{
			this.transform.position = center - normal * MainStateManager.instance.CIRCLE_RADIUS;
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
