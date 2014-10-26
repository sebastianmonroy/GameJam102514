using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RiderStateManager : PlayerStateManager
{
	public bool allowRide = true;
	public SimpleState rideState, jumpState, deathState, finishedState;

	public float minSpeed, defaultSpeed, maxSpeed;
	public float jumpSpeed;
	public float radialSpeed, speed;
	public float killSpeedThreshold;

	public Vector3 jumpDirection, center, normal, tangent;

	public MeshFilter triangleMesh;
	public Vector3[] triangleVerts = new Vector3[3] { Vector3.zero, Vector3.zero, Vector3.zero };
	public int[] triangleTris = new int[3];
	public int triangleUV;
	public int triangleIndex;

	public List<RiderStateManager> hitPlayers = new List<RiderStateManager>();

	public ARLTimer timer;
	public Vector3 defaultScale;

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
		defaultScale = this.transform.localScale;
	}

	public override void Update ()
	{
		speed = Mathf.Sign(speed) * Mathf.Clamp(Mathf.Abs(speed), Mathf.Abs(minSpeed), Mathf.Abs(maxSpeed));
		radialSpeed = speed / (2f * Mathf.PI * MainStateManager.instance.CIRCLE_RADIUS) * 360f;

		center = MainStateManager.instance.CIRCLE_CENTER;
		normal = (center - this.transform.position).normalized;
		this.transform.rotation = Quaternion.LookRotation(-Mathf.Sign(speed) * Vector3.forward, normal);
		tangent = this.transform.right;

		if (isAlive) {
			DetectRiderHits();

			DrawDebugTriangle();
		}

		Execute();
	}
		void DetectRiderHits() {
			foreach (RiderStateManager otherRider in MainStateManager.instance.playerStateManagers) 
			{
				if (otherRider != this && otherRider.isAlive) 
				{
					if (Vector3.Distance(this.transform.position, otherRider.transform.position) <= this.transform.lossyScale.x/4f + otherRider.transform.lossyScale.x/4f)
					{
						if (!hitPlayers.Contains(otherRider) && (Mathf.Abs(speed) > Mathf.Abs(otherRider.speed))) 
						{
							hitPlayers.Add(otherRider);
							CheckRiderHit(hitPlayers.Count-1);
						} 
					} 
					else 
					{
						if (hitPlayers.Contains(otherRider)) 
						{
							hitPlayers.Remove(otherRider);
						}
					}
				}
			}
		}

		void CheckRiderHit(int hitPlayerIndex) {
			RiderStateManager otherRider =  hitPlayers[hitPlayerIndex];

			// both players handle reflection
			Vector3 collisionNormal = (this.transform.position - otherRider.transform.position).normalized;
			jumpDirection = jumpDirection - 2f * (Vector3.Dot(jumpDirection, collisionNormal)) * collisionNormal;

			if (stateMachine.currentState == "JUMP" && otherRider.stateMachine.currentState == "RIDE") 
			{
				
			}
			else if (stateMachine.currentState == "RIDE" && otherRider.stateMachine.currentState == "JUMP")
			{
				speed += (otherRider.jumpSpeed + Mathf.Abs(otherRider.speed))*2f * Vector3.Dot(-collisionNormal, this.tangent) * Vector3.Dot(otherRider.jumpDirection, this.tangent);
			}

			// only one player handles the speed and killing stuff
			if (Mathf.Abs(speed) > killSpeedThreshold && !OtherHasSimilarSpeed(otherRider))
			{
				Debug.Log("high speed collision, kill player " + otherRider.playerNum);
				hitPlayers[hitPlayerIndex].Kill();
			} else {
				Debug.Log("bump");
			}

			if (!otherRider.hitPlayers.Contains(this))
			{
				float tempSpeed = speed;
				speed = otherRider.speed;
				otherRider.speed = tempSpeed;
			}
		}

		bool OtherHasSimilarSpeed(RiderStateManager other) 
		{
			float otherSpeed = other.speed;
			return Mathf.Abs(speed - otherSpeed) <= killSpeedThreshold;
		}

		void CreateTriangle() 
		{
			// TO DO
			Mesh ret = new Mesh();
			ret.vertices = triangleVerts;
			//ret.triangles = 
		}

		void DrawDebugTriangle() {
			Debug.DrawLine(triangleVerts[0], triangleVerts[1], Color.red);
			Debug.DrawLine(triangleVerts[1], triangleVerts[2], Color.green);
			Debug.DrawLine(triangleVerts[2], triangleVerts[0], Color.blue);
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
		if (allowRide) 
		{
			Ride();
			RideInput();
			if (triangleIndex == 2) 
			{
				triangleVerts[triangleIndex] = this.transform.position;
			}
		}
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
			if (aimVector.magnitude >= 0.3f && Vector3.Dot(aimVector.normalized, normal) > 0.2f)
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
		this.transform.position += (Mathf.Abs(speed) + jumpSpeed) * 2f * jumpDirection * Time.deltaTime;
		CircleDetection();
		JumpInput();

		if (triangleIndex > 0) 
		{
			triangleVerts[triangleIndex] = this.transform.position;
		}
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

		void SnapToCircle ()
		{
			this.transform.position = center - normal * MainStateManager.instance.CIRCLE_RADIUS;
		}

	void JumpExit ()
	{
		speed = (speed + Mathf.Sign(speed) * jumpSpeed) * Mathf.Abs(Vector3.Dot(tangent, jumpDirection));
		float sign = Mathf.Sign(Vector3.Dot(tangent, jumpDirection));
		speed = sign * speed;
		/*if (triangleIndex == 0) 
		{
			triangleVerts[triangleIndex] = this.transform.position;
			triangleIndex = 1;
		}
		else if (triangleIndex == 1) 
		{
			triangleIndex = 2;
		}
		else if (triangleIndex == 2) 
		{
			KillInTriangle();
			triangleIndex = 0;
		}*/
	}
		void KillInTriangle() 
		{
			foreach (RiderStateManager otherRider in MainStateManager.instance.playerStateManagers) {
				if (otherRider != this) {
					Debug.Log("TRIANGLE KILL");
					Vector3 q = otherRider.transform.position;
					Vector3 p0 = triangleVerts[0], p1 = triangleVerts[1], p2 = triangleVerts[2];
					float triangleArea = 0.5f * ((-p1.y * p2.x) + p0.y * (-p1.x + p2.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y);
					float s = 1f / (2f * triangleArea) * (p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * q.x + (p0.x - p2.x) * q.y);
					float t = 1f / (2f * triangleArea) * (p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * q.x + (p1.x - p0.x) * q.y);

					bool kill = ((s > 0f) && (t > 0f) && (1f-s-t > 0f));

					if (kill) {
						otherRider.Kill();
					}
				}
			}
		}

	#endregion
	
	public void Kill() {
		stateMachine.SwitchStates(deathState);
	}

	#region DEATH
	void DeathEnter ()
	{
		Debug.Log("Player " + playerNum + " dying");
		hitPlayers.Clear();
		allowRide = false;
		speed = 0f;
		isAlive = false;

		timer = new ARLTimer(1f);
	}

	void DeathUpdate () 
	{
		this.transform.localScale = Vector3.Lerp(defaultScale, defaultScale*50f, timer.PercentDone());
		Color oldColor = this.GetComponent<SpriteRenderer>().color;
		float newAlpha = Mathf.Lerp(1f, 0f, timer.PercentDone());
		oldColor.a = newAlpha;
		this.GetComponent<SpriteRenderer>().color = oldColor;
		if (timer.IsDone())
		{
			stateMachine.SwitchStates(finishedState);
		}
	}

	void DeathExit ()
	{
		Debug.Log("Player " + playerNum + " died");
	}
	#endregion
}
