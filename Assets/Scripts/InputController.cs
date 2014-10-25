using UnityEngine;
using InControl;
using System.Collections;

public class InputController : MonoBehaviour {
	public PlayerStateManager player;
	public bool StartButton;
	public Vector2 LeftStick;
	public Vector2 RightStick;
	public bool AButton;
	public bool BButton;
	public bool XButton;
	public bool YButton;
	public float LeftTrigger;
	public float RightTrigger;
	public bool LeftBumper;
	public bool RightBumper;

	public void Start() {
		InputManager.Setup();
	}

	void Update() {
		InputManager.Update();
		if (HasPlayer()) {
			// Update input values
			StartButton = player.device.GetControl(InputControlType.Start).IsPressed;
			LeftStick = new Vector2(player.device.LeftStickX, player.device.LeftStickY);
			RightStick = new Vector2(player.device.RightStickX, player.device.RightStickY);
			AButton = player.device.GetControl(InputControlType.Action1).IsPressed;
			BButton = player.device.GetControl(InputControlType.Action2).IsPressed;
			XButton = player.device.GetControl(InputControlType.Action3).IsPressed;
			YButton = player.device.GetControl(InputControlType.Action4).IsPressed;
			LeftTrigger = player.device.GetControl(InputControlType.LeftTrigger).Value;
			RightTrigger = player.device.GetControl(InputControlType.RightTrigger).Value;
			LeftBumper = player.device.GetControl(InputControlType.LeftBumper).IsPressed;
			RightBumper = player.device.GetControl(InputControlType.RightBumper).IsPressed; 
		}
	}

	public void SetPlayer(PlayerStateManager newPlayer) {
		//Debug.Log("Number of Devices Available: " + InputManager.Devices.Count);
		if (newPlayer != null && newPlayer.playerNum <= 4 && newPlayer.playerNum >= 0) {
			Debug.Log("Player " + newPlayer.playerNum + " connected");
			this.player = newPlayer;
		} else {
			this.player = null;
			StartButton = false;
			LeftStick = Vector2.zero;
			RightStick = Vector2.zero;
			AButton = false;
			BButton = false;
			XButton = false;
			YButton = false;
			LeftTrigger = 0f;
			RightTrigger = 0f;
			LeftBumper = false;
			RightBumper = false;
		}
	}

	public void UnsetPlayer() {
		Debug.Log("Player " + player.playerNum + " disconnected");
		SetPlayer(null);
	}

	public bool HasPlayer() {
		return (player != null && (player.playerNum >= 0) && player.device != null);
	}
}
