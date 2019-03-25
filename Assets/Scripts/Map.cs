using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

	public GameObject tile;
	private int x, z;

	private GameObject[,] visualMap;
	private int[,] tileMap;
	private int[,] qMap;

	private Vector2Int startIndex;
	private Vector2Int endIndex;

	void Awake(){
		x = z = 10;

		buildTileMap ();
		buildVisualMap ();
		buildQMap ();
	}

	public int[,] TileMap () 			{ return tileMap; }
	public int[,] QMap () 				{ return qMap; }
	public GameObject[,] VisualMap () 	{ return visualMap; }
	public int Size()					{ return x; }
	public Vector2Int StartIndex		{ get{ return startIndex; } }
	public Vector2Int EndIndex			{ get{ return endIndex; } }
	public Vector3 GetStartPos()		{ return visualMap [startIndex.x, startIndex.y].transform.position; }
	public Vector3 GetEndPos()			{ return visualMap [endIndex.x, endIndex.y].transform.position; }

	private void buildTileMap(){
		tileMap = new int[,] {
			{-1,  1,  1,  1,  1,  1,  1,  1,  1,  1 }, 
			{ 0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
			{ 1,  1,  1,  1,  1,  0,  1,  1, -50, 0 },
			{ 0,  0,  0,  0,  0,  0,  1,  1,  1,  0 },
			{ 0,  1,  1,  1,  0,  1,  0,  0,  1,  0 },
			{ 0,  0,  0,  1,  0,  0,  0,  1,  0,  0 },
			{ 1,  1,  0, 0,  0, -50,  0,  1,  0,  1 },
			{ 0,  0,  0,  1,  0,  1,  0,  1,  0,  0 },
			{ 0,  1,  1,  1,  0,  0,  0,  1,  1,  0 },
			{ 0,  0,  0,  0,  0,  1,  0,  0,  0, 100}
		};

		startIndex 	= new Vector2Int (0, 0);
		endIndex 	= new Vector2Int (9, 9);
	}

	private void buildVisualMap(){
		visualMap = new GameObject[x, z];

		for (int i = 0; i < x; i++) {
			for (int j = 0; j < z; j++) {

				GameObject newTile  = Instantiate (tile);
				visualMap [i, j] 	= newTile;

				newTile.transform.SetParent (gameObject.transform);
				newTile.transform.position = new Vector3 (
					i * newTile.transform.localScale.x, 
					0f, 
					j * newTile.transform.localScale.z
				);

				if (tileMap [i, j] == 1) {
					newTile.transform.localScale += Vector3.up * 1.5f;
					newTile.transform.position 	 += Vector3.up * 0.75f;
				}

				newTile.GetComponent<Renderer> ().material.color = getColor (tileMap [i, j]);
			}
		} 
		Destroy (tile);
	}

	private void buildQMap(){
		qMap = new int[x + 4, z + 4];

		for (int i = 0; i < qMap.GetLength (0); i++) {
			qMap [i, 0] = 1;
			qMap [i, qMap.GetLength (1) - 1] = 1;
			qMap [i, 1] = 1;
			qMap [i, qMap.GetLength (1) - 2] = 1;
		}

		for (int j = 0; j < qMap.GetLength (1); j++) {
			qMap [0, j] = 1;
			qMap [0, qMap.GetLength (0) - 1] = 1;
			qMap [1, j] = 1;
			qMap [0, qMap.GetLength (0) - 2] = 1;
		}

		for (int i = 2; i < qMap.GetLength (0) - 2; i++) {
			for (int j = 2; j < qMap.GetLength (1) - 2; j++) {
				qMap [i, j] = tileMap [i - 2, j - 2];
			}
		}
	}

	private Color getColor(int n){
		switch (n) {
		case 0:
			return Color.white;
		case 1:
			return Color.black;
		case -50:
			return Color.red;
		default:
			return Color.magenta;
		}
	}
}