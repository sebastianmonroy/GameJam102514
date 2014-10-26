using UnityEngine;
using System.Collections;

public class RiderStateManager : PlayerStateManager
{
	public SimpleState rideState, jumpState, deathState, finishedState;

	public float minSpeed, defaultSpeed, maxSpeed;
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
		RideInput();
		Ride();
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
		speed = Mathf.Sign(GetJumpAngle()) * Mathf.Abs(speed);
		speed += 0.2f * speed;
	}

	void JumpUpdate () 
	{
		this.transform.position += Mathf.Abs(speed) * jumpDirection * Time.deltaTime;
		CircleDetection();
		JumpInput();
	}
		void JumpInput ()
		{
			
		}

		void CircleDetection ()
		{
			float distanceFromCenter = Vector3.Distance(this.transform.position, center);
			float circleRadius = MainStateManager.instance.CIRCLE_RADIUS;

			if (distanceFromCenter >= circleRadius)
			{
				this.transform.position = center - normal * circleRadius;
				stateMachine.SwitchStates(rideState);
			}
		}

	void JumpExit ()
	{
		//speed = speed * Vector3.Dot(tangent, )
	}
		float GetJumpAngle() 
		{
			Vector3 a = normal;
			Vector3 b = jumpDirection;
			Vector3 n = Vector3.forward;
			float sign = (Vector3.Dot(Vector3.Cross(a,b), n) > 0) ? 1 : -1;
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
