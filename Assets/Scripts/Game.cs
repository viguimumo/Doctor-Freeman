using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

	// Public
	public AudioSource audioS;
	public AudioClip win;
	public AudioClip fail;

	public Text  state_label;
	public Text  games_label;
	public Text  policy_label;
	public Text  movs_label;
	public float velocity;
	public float timer;

	// Private
	private GameObject Freeman;
	private Vector2Int pos;
	private Map map;
	private QLearning qLearning;

	private bool isOver;

	void Start () {
		Freeman = GameObject.FindGameObjectWithTag ("Player");
		map = GameObject.Find ("Map").GetComponent<Map> ();

		Initialize ();
	}

	public void Initialize(){
		isOver = false;

		Freeman2Start ();
	
		qLearning = new QLearning (this, map.QMap(), map.Size(), map.Size(), pos);
		timer = 0f;

		games_label.text = "0";
		policy_label.text = "Random";
		movs_label.text = "0";
	}

	private void Freeman2Start(){
		pos = map.StartIndex;
		Freeman.transform.position = map.GetStartPos ();
		Freeman.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 0));
	}
		
	private void step(){
		Vector2Int newPos = qLearning.Action (pos);
		qLearning.ActualizeQMatrix (newPos);

		// MOVE
		if (newPos.x >= 0 && newPos.x <= map.Size () - 1) {
			if (newPos.y >= 0 && newPos.y <= map.Size () - 1) {

				if (map.TileMap () [newPos.x, newPos.y] != 1) {
					state_label.text = "CAMINANDO";
					pos = newPos;
					Freeman.transform.position = map.VisualMap () [pos.x, pos.y].transform.position;
				} else {
					state_label.text = "PARED";
				}
			} else {
				state_label.text = "LIMITE";
			}
		} else {
			state_label.text = "LIMITE";
		}

		// TURN
		if (qLearning.currentAction == 0) {
			Freeman.transform.localRotation = Quaternion.Euler (new Vector3 (0, 180, 0));
		} else if (qLearning.currentAction == 1) {
			Freeman.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 0));
		} else if (qLearning.currentAction == 2) {
			Freeman.transform.localRotation = Quaternion.Euler (new Vector3 (0, 90, 0));
		}  else {
			Freeman.transform.localRotation = Quaternion.Euler (new Vector3 (0, 270, 0));
		} 

		timer = 0f;
		IsEndOfGame ();
	}

	private void IsEndOfGame(){
		if (map.TileMap () [pos.x, pos.y] == 100) {
			isOver = true;
			audioS.PlayOneShot (win);

			games_label.text = (int.Parse (games_label.text) + 1).ToString ();
			Freeman2Start ();
		} else {
			int i = pos.x + 2;
			int j = pos.y + 2;

			if (map.QMap () [i - 1, j] + map.QMap () [i, j - 1] + map.QMap () [i, j + 1] + map.QMap () [i + 1, j] < 0) {
				audioS.PlayOneShot (fail);
				games_label.text = (int.Parse (games_label.text) + 1).ToString ();
				Freeman2Start ();
			}
		}
	}
		
	public void ChangeVelocity(Scrollbar sb){
		velocity = sb.value;
	}

	public void Mute(Button b){
		if (b.image.color.Equals (Color.gray)) {
			b.image.color = Color.white;
		} else {
			b.image.color = Color.gray;
		}

		audioS.mute = !(audioS.mute);
	}

	void Update () {
		if (!isOver) {
			timer += Time.deltaTime;
			if (timer >= velocity) {
				step ();
			}
		}
	}
}