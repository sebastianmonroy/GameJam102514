using UnityEngine;
using System.Collections;

public class PlayerStateManager : MonoBehaviour
{
	public SimpleStateMachine stateMachine;
	public SimpleState setupState;

	public virtual void Start ()
	{

	}

	public virtual void Setup ()
	{
		
	}
	
	public virtual void Update ()
	{
		
	}

	public virtual void Execute ()
	{
		
	}

	#region SETUP
	public virtual void SetupEnter ()
	{

	}

	public virtual void SetupUpdate ()
	{
		
	}

	public virtual void SetupExit ()
	{
		
	}
	#endregion
}
