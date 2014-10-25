using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainStateManager : MonoBehaviour {
	#region VARIABLES
	// INSTANCE
	public static MainStateManager instance;

	// STATES
	public SimpleStateMachine stateMachine;
	SimpleState menuState, gameState, endState;
	public List<string> subStateNames = new List<string>();

	// SUB-STATE MANAGERS
	public List<PlayerStateManager> playerStateManagers = new List<PlayerStateManager>();

	// GLOBAL STUFF
	public List<GameObject> Players = new List<GameObject>();
	public GameObject PLANE;
	public float CIRCLE_RADIUS;
	public Vector3 CIRCLE_CENTER;
	
	#endregion

	#region GAME LOOP
	void Awake () 
	{
		instance = this;
	}

	void Start () 
	{
		//DEFINE STATES
		menuState = new SimpleState(MenuEnter, MenuUpdate, MenuExit, "[MENU]");
		gameState = new SimpleState(GameEnter, GameUpdate, GameExit, "[GAME]");
		endState = new SimpleState(EndEnter, EndUpdate, EndExit, "[END]");
		
		Setup();

		stateMachine.SwitchStates(gameState);	//TEMPORARY
	}

	void Update() 
	{
		Execute();
		CIRCLE_CENTER = PLANE.transform.position;
	}

	private void Setup() 
	{
		foreach (GameObject p in Players)
		{
			playerStateManagers.Add(p.GetComponent<PlayerStateManager>());
			subStateNames.Add("");
		}
	}

	public void Execute () 
	{
		stateMachine.Execute();
	}
	#endregion

	#region MENU
	void MenuEnter() {
		
	}

	void MenuUpdate() {

	}	

	void MenuExit() {

	}
	#endregion

	#region GAME
	void GameEnter() 
	{
		for (int i = 0; i < playerStateManagers.Count; i++)
		{
			playerStateManagers[i].Setup();
		}
	}

	void GameUpdate() 
	{
		for (int i = 0; i < playerStateManagers.Count; i++)
		{
			playerStateManagers[i].Execute();
			subStateNames[i] = playerStateManagers[i].stateMachine.currentState;
		}
	}

	void GameExit() 
	{

	}
	#endregion

	#region END
	void EndEnter() 
	{

	}

	void EndUpdate() 
	{

	}

	void EndExit() 
	{
		
	}
	#endregion
}
