using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QMatrix
{
	private List<Entry> matrix;

	public QMatrix ()
	{
		matrix = new List<Entry> ();
	}

	public int Count(){
		return matrix.Count;
	}

	private int stateIndex(Vector2Int[,] state){
		foreach (Entry e in matrix) {
			if (Equal (e.State, state)) {
				return matrix.IndexOf (e);
			}
		}
		return -1;
	}

	public bool Contains(Vector2Int[,] state){
		foreach (Entry e in matrix) {
			if (Equal (e.State, state)) {
				return true;
			}
		}
		return false;
	}

	public float GetValue(Vector2Int[,] state, int action, bool saveState){
		if (!Contains (state)) {
			if (saveState) {
				Add (state, new float[] { 0, 0, 0, 0 });
			} else {
				return 0;
			}
		}
		return matrix [stateIndex (state)].Actions [action];
	}

	public void SetValue(Vector2Int[,] state, int action, float value){
		matrix [stateIndex (state)].Actions [action] = value;
	}

	public float[] GetValues(Vector2Int[,] state){
		if (!Contains (state)) {
			Add (state, new float[] {0, 0, 0, 0});
		}
		return matrix [stateIndex (state)].Actions;
	}

	public bool Equal(Vector2Int[,] a, Vector2Int[,] b){
		if (a.GetLength (0) != b.GetLength (0) || a.GetLength (1) != b.GetLength (1)) {
			return false;
		}
			
		for (int i = 0; i < a.GetLength (0); i++) {
			for (int j = 0; j < a.GetLength (1); j++) {
				if (a [i, j] != b [i, j]) {
					return false;
				}
			}
		}
		return true;
	}

	public void Add(Vector2Int[,] state, float[] actions){
		matrix.Add (new Entry (state, actions));
	}

	public class Entry{
		private Vector2Int[,] state;
		private float[] actions;

		public Entry(Vector2Int[,] state, float[] actions){
			this.state = state;
			this.actions = actions;
		}

		public Vector2Int[,] State{
			get{ return state;}
			set{ state = value;}
		}

		public float[] Actions{
			get{ return actions;}
			set{ actions = value;}
		}
	}
}