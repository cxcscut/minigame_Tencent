﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LightGameController : MonoBehaviour {

	public new Camera camera;

	public GameObject submit, reset;

	private Rect submit_frame, reset_frame;

	private int[,] game_state;

	public AudioSource wechat;

	private float LastTime;

	IEnumerator FadingUnload(string Scene_name)
	{
		yield return new WaitForSeconds (GameObject.Find("blackfading").GetComponent<FadingController>().BeginFade(1));

		SceneManager.UnloadSceneAsync (SceneManager.GetSceneByName(Scene_name));

		yield return new WaitForSeconds (GameObject.Find("blackfading").GetComponent<FadingController>().BeginFade(-1));

	}

	void OnMoonSelected(GameObject moon)
	{
		int index = int.Parse(moon.name.Substring (13, moon.name.Length - 13));

		int col = index % 5;
		int row;
		if (col == 0)
			row = index / 5;
		else
			row = index / 5 + 1;

		if (col == 0)
			col = 5;

		row--;col--;

		OnStateChange (row,col,game_state);
		
		DisplayState (game_state);

	}

	void OnStateChange(int row,int col,int [,] state)
	{

		if (row != 0) 
			state [row - 1, col] = game_state [row - 1, col] == 1 ? 0 : 1;

		if (row != 3)
			state [row + 1, col] = game_state [row + 1, col] == 1 ? 0 : 1;

		if (col != 0)
			state [row, col - 1] = game_state [row, col - 1] == 1 ? 0 : 1;

		if(col != 4)
			state [row, col + 1] = game_state [row, col + 1] == 1 ? 0 : 1;

		state [row, col] = game_state [row, col] == 1 ? 0 : 1;
	}

	void DisplayState(int [,] state)
	{
		for (int i = 1; i <= 4; i++)
			for (int j = 1; j <= 5; j++) 
			{
				int index = 5 * (i-1) + j;
				string name = "Moon_selected" + index.ToString ();
				if (state [i-1,j-1] == 1) {
					GameObject.Find (name).GetComponent<SpriteRenderer>().enabled = true;
				} else {
					GameObject.Find (name).GetComponent<SpriteRenderer>().enabled = false;
				}
			}
	}

	bool VerifyState(int[,] state)
	{
		foreach (int moon in state)
				if (moon != 1)
					return false;

		return true;
	}

	void OnReset()
	{
		game_state = new int[4,5]{
			{0,0,0,0,0},
			{0,1,0,1,0},
			{0,1,0,1,0},
			{0,0,0,0,0}
		};

		DisplayState (game_state);
	}

	void OnSubmit()
	{
		if (VerifyState (game_state)) {

			// Compute scores
			if (Time.time - LastTime < 20.0f) {
				// no more than 255 seconds
				GlobalVariables.LightGameScores = 40;
			} else if (Time.time - LastTime >= 20.0f && Time.time - LastTime < 40.0f) {
				// no more than 435 seconds
				GlobalVariables.LightGameScores = 30;
			} else if (Time.time - LastTime >= 40.0f && Time.time - LastTime < 60.0f) {
				// no more than 555 seconds
				GlobalVariables.LightGameScores = 20;
			} else if (Time.time - LastTime >= 60.0f && Time.time - LastTime < 120.0f) {
				// no more than 600 seconds
				GlobalVariables.LightGameScores = 10;
			}

			GlobalVariables.LightGameFinished = true;
			StartCoroutine (FadingUnload ("LightGame"));

			Debug.Log (GameObject.Find ("BGMplayer") != null);

			GameObject.Find ("BGMplayer").GetComponent<AudioSource> ().Play ();

			camera.GetComponent<AudioSource> ().Stop ();
		}
	}

	void InitializeState()
	{
		game_state = new int[4,5]{
			{0,0,0,0,0},
			{0,1,0,1,0},
			{0,1,0,1,0},
			{0,0,0,0,0}
		};
	
		DisplayState (game_state);
	}

	// Use this for initialization
	void Start () {
		if(GameObject.Find ("BGMplayer") != null)
			if (GameObject.Find ("BGMplayer").GetComponent<AudioSource> ().isPlaying)
				GameObject.Find ("BGMplayer").GetComponent<AudioSource> ().Stop();

		InitializeState ();

		LastTime = Time.time;

		submit_frame = new Rect (
			submit.transform.position.x - submit.GetComponent<SpriteRenderer>().bounds.size.x / 2,
			submit.transform.position.y + submit.GetComponent<SpriteRenderer>().bounds.size.y / 2,
			submit.GetComponent<SpriteRenderer>().bounds.size.x,
			-submit.GetComponent<SpriteRenderer>().bounds.size.y
		);

		reset_frame = new Rect (
			reset.transform.position.x - reset.GetComponent<SpriteRenderer>().bounds.size.x / 2,
			reset.transform.position.y + reset.GetComponent<SpriteRenderer>().bounds.size.y / 2,
			reset.GetComponent<SpriteRenderer>().bounds.size.x,
			-reset.GetComponent<SpriteRenderer>().bounds.size.y
		);
	}

	void FixedUpdate()
	{
		if (Time.time - LastTime < 20.0f) {
			// no more than 255 seconds
			wechat.Play();
		} else if (Time.time - LastTime >= 20.0f && Time.time - LastTime < 40.0f) {
			// no more than 435 seconds
			wechat.Play();
		} else if (Time.time - LastTime >= 40.0f && Time.time - LastTime < 60.0f) {
			// no more than 555 seconds
			wechat.Play();
		} else if (Time.time - LastTime >= 60.0f && Time.time - LastTime < 120.0f) {
			// no more than 600 seconds
			wechat.Play();
		}

		if (Time.time - LastTime >= GlobalVariables.TimeLimit) {
			GlobalVariables.LightGameFinished = true;

			Debug.Log (GameObject.Find ("BGMplayer") != null);

			GameObject.Find ("BGMplayer").GetComponent<AudioSource> ().Play ();

			camera.GetComponent<AudioSource> ().Stop ();

			StartCoroutine (FadingUnload ("LightGame"));
		}
	}

	// Update is called once per frame
	void Update () {

		Vector3 mouse_pos = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,0.0f));
		Vector3 touch_pos = Input.touchCount > 0 ? 
			camera.ScreenToWorldPoint (new Vector3 (Input.GetTouch (0).position.x, Input.GetTouch (0).position.y, 0.0f)) :
			new Vector3 (-255.0f,-255.0f,0.0f);

		if (Input.GetMouseButtonDown (0) || (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began)) {
			// Determine which Moon is clicked 
			RaycastHit2D hit_mouse = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero); 
			RaycastHit2D hit_touch = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero); 

			if (hit_mouse.collider != null) {
				Debug.Log (hit_mouse.collider.gameObject.name);
				if (hit_mouse.collider.gameObject.GetComponent<SpriteRenderer> ().enabled)
					hit_mouse.collider.gameObject.GetComponent<SpriteRenderer> ().enabled = false;
				else {
					hit_mouse.collider.gameObject.GetComponent<SpriteRenderer> ().enabled = true;
				}
					
				// Forward click event to callback
				OnMoonSelected(hit_mouse.collider.gameObject);
			}
			else if(hit_touch.collider != null)
			{
				Debug.Log (hit_touch.collider.gameObject.name);
				if (hit_touch.collider.gameObject.GetComponent<SpriteRenderer> ().enabled)
					hit_touch.collider.gameObject.GetComponent<SpriteRenderer> ().enabled = false;
				else {
					hit_touch.collider.gameObject.GetComponent<SpriteRenderer> ().enabled = true;
				}
					
				// Forward click event to callback
				OnMoonSelected(hit_mouse.collider.gameObject);
			}

			// Reset button pressed
			if (reset_frame.Contains (new Vector2 (mouse_pos.x, mouse_pos.y), true)
			    || reset_frame.Contains (new Vector2 (touch_pos.x, touch_pos.y), true)) {

				reset.transform.position += GlobalVariables.click_offset;
			}

			// Submit button pressed
			if (submit_frame.Contains (new Vector2 (mouse_pos.x, mouse_pos.y), true)
				|| submit_frame.Contains (new Vector2 (touch_pos.x, touch_pos.y), true)) {

				submit.transform.position += GlobalVariables.click_offset;
			}
		}

		if (Input.GetMouseButtonUp (0) || (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended)) {
			// Reset button released
			if (reset_frame.Contains (new Vector2 (mouse_pos.x, mouse_pos.y), true)
				|| reset_frame.Contains (new Vector2 (touch_pos.x, touch_pos.y), true)) {

				reset.transform.position -= GlobalVariables.click_offset;

				// Forward event to callback
				OnReset ();
			}

			// Submit button released
			if (submit_frame.Contains (new Vector2 (mouse_pos.x, mouse_pos.y), true)
				|| submit_frame.Contains (new Vector2 (touch_pos.x, touch_pos.y), true)) {

				submit.transform.position -= GlobalVariables.click_offset;

				// Forward event to callback
				OnSubmit ();
			}
		}

		if (Input.GetKey (KeyCode.LeftAlt) && Input.GetKey (KeyCode.F2)) {
			
		}
	}
}
