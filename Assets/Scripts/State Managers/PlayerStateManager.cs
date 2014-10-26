using UnityEngine;
using System.Collections;
using InControl;

public class PlayerStateManager : MonoBehaviour
{
	public SimpleStateMachine stateMachine;
	public SimpleState setupState;
	public InputDevice device;
	public InputController input;
	public int playerNum = -1;
	public int actualPlayerNum;

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

	public void AssignPlayer() {
		if (playerNum > -1) {
			actualPlayerNum = FindActualPlayerNum();
			device = InputManager.Devices[playerNum];
			Debug.Log((device as UnityInputDevice).Profile);
		}
		
	}

	public int FindActualPlayerNum() {
		int i = 0;
		int found = 0;
		while (i < InputManager.Devices.Count) {
			if ((InputManager.Devices[i] as UnityInputDevice).Profile.IsKnown) {
				if (found == playerNum) {
					return i;
				}

				found++;
			}
			i++;
		}

		return -1;
	}
}
