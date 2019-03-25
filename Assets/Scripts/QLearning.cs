using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QLearning
{
	private Game game;

	private int[,] map;
	private int x, z;
	private Vector2Int[,] currentState;
	private QMatrix q;
	public int currentAction;
	// Action 0 -> Up
	// Action 1 -> Down
	// Action 2 -> Left
	// Action 3 -> Rigth

	private float alpha;
	private float gamma;
	private float percentage;

	private int movs;
	private int randomMovs = 500;
	private int normalMovs = 2500;

	private List<Vector2Int[,]> fishMemo;

	public QLearning (Game game, int[,] map, int x, int z, Vector2Int pos)
	{
		this.game = game;

		this.map = map;
		this.x = x;
		this.z = z;

		currentState = getState (pos);
		currentAction = -1;
		q = new QMatrix ();

		alpha = 0.4f;
		gamma = 0.1f;
		percentage = 0;

		movs = 0;

		fishMemo = new List<Vector2Int[,]> ();
		fishMemo.Add (currentState);
	}

	public void ActualizeQMatrix(Vector2Int pos){

		Vector2Int[,] newState = getState (pos);
		int reward = getReward (newState, currentAction);
		float value = q.GetValue (currentState, currentAction, !isOutOfLimits(currentState)) 
			+ alpha * (reward + gamma * q.GetValue(newState, currentAction, !isOutOfLimits(newState)) - q.GetValue (currentState, currentAction, !isOutOfLimits(currentState)));

		q.SetValue (currentState, currentAction, value);

		if (!isOutOfLimits (newState)) {
			currentState = newState;
		}

		movs++;
		game.movs_label.text = movs.ToString ();

		if (movs == randomMovs) {
			percentage = 50;
			alpha = 0.3f;
			gamma = 0.4f;

			game.policy_label.text = "Normal";
		
		} else if (movs > normalMovs / 2 && movs < normalMovs) {
			percentage = 70;
			alpha = 0.1f;
			gamma = 0.6f;
		} else if (movs == normalMovs) {
			Debug.Log ("entro?");
			percentage = 90;
			game.policy_label.text = "Tactical";
		}
	}

	private bool isOutOfLimits(Vector2Int[,] state){
		if (map[state[1, 1].x, state[1, 1].y] == 1){
			return true;
		}
		return false;
	}

	public Vector2Int Action(Vector2Int pos){
		int action = -1;
		float r = Random.Range (0, 100);

		if (r < percentage) {
			float[] qValues = q.GetValues (currentState);
				int max = 0;
	
				for (int i = 1; i < qValues.Length; i++) {
					if (qValues [i] > qValues [max]) {
						max = i;
					}
				}
				action = max;
		} 
		else {
			action = Random.Range (0, 4);
		}

		currentAction = action;

		switch (action) {
		case 0: 
			return Up (pos);
		case 1:
			return Down(pos);
		case 2:
			return Left (pos);
		case 3:
			return Rigth (pos);
		}

		return pos;
	}

	// ------------ STATE --------------------------------------------------------------------------------------------------
	private Vector2Int[,] getState(Vector2Int pos){
		int x = pos.x + 2;
		int y = pos.y + 2;

		return new Vector2Int[3, 3] {
			{ new Vector2Int (x - 1, y - 1), 	new Vector2Int (x, y - 1), 	new Vector2Int (x + 1, y - 1) },
			{ new Vector2Int (x - 1, y), 		new Vector2Int (x, y), 		new Vector2Int (x + 1, y) },
			{ new Vector2Int (x - 1, y + 1),	new Vector2Int (x, y + 1), 	new Vector2Int (x + 1, y + 1) }
		
		};
	}

	// ------------ REWARD --------------------------------------------------------------------------------------------------
	private int getReward(Vector2Int[,] state, int action){
		int reward = 0;

		// Si estoy dentro de una pared o fuera del mapa
		if (map[state[1, 1].x, state[1, 1].y] == 1){
			reward -= 100;
		}
			
		// Si estoy junto a una casilla de MUERTE
		if (map[state [2, 1].x, state[2,1].y] == -50 || map[state [0, 1].x, state[0,1].y] == -50 
			|| map[state [1, 2].x, state[1,2].y] == -50 || map[state [1, 0].x, state[1,0].y] == -50) {
			reward -= 10000;
		}

		// Si estoy junto a la salida o en la salida
		if (map[state [1, 1].x, state[1,1].y] == 100 || map[state [2, 1].x, state[2,1].y] == 100 || map[state [0, 1].x, state[0,1].y] == 100 
			|| map[state [1, 2].x, state[1,2].y] == 100 || map[state [1, 0].x, state[1,0].y] == 100) {
			reward += 1000000000;
		}

		if (!isOutOfLimits (state) && !q.Equal(state, currentState)) {

			if (fishMemo.Count == 1){

				if (q.Equal (fishMemo [0], state)) {
					Debug.Log ("iguales");
					if (map [state [1, 1].x, state [1, 1].y] == -1) {

						reward -= 100;
					} else {
						reward -= 10;
					}
				} else {
					reward += 1000;
				}
			}
			fishMemo.Remove (fishMemo [0]);
			fishMemo.Add (currentState);
		}
		return reward;
	}

	// ------------ ACTIONS ------------------------------------------------------------------------------------------------
	public Vector2Int Up(Vector2Int pos){
		return new Vector2Int (pos.x - 1, pos.y);
	}

	public Vector2Int Down(Vector2Int pos){
		return new Vector2Int (pos.x + 1, pos.y);
	}

	public Vector2Int Left(Vector2Int pos){
		return new Vector2Int (pos.x, pos.y - 1);
	}

	public Vector2Int Rigth(Vector2Int pos){
		return new Vector2Int (pos.x, pos.y + 1);
	}
}