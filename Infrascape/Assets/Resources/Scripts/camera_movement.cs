using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using RL = room_loader;

public class camera_movement : MonoBehaviour {

	public GameObject Camera; 
	public GameObject Player;
	public GameObject Roomloader;
	public int RotateSpeed;
	public KeyCode RotateLeftKey;
	public KeyCode RotateRightKey;
	public KeyCode ResetCameraKey;
	public Image Blackscreen;

	private GameObject enteredroom;

	// Use this for initialization
	void Start () {
		Blackscreen.color = new Color(0, 0, 0, 1);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (RotateLeftKey)) { 
			Camera.transform.RotateAround (new Vector3 (enteredroom.transform.position.x, enteredroom.transform.position.y, enteredroom.transform.position.z), Vector3.up, RotateSpeed * Time.deltaTime);
		}
		if (Input.GetKey (RotateRightKey)) { 
			Camera.transform.RotateAround (new Vector3 (enteredroom.transform.position.x, enteredroom.transform.position.y, enteredroom.transform.position.z), Vector3.up, -RotateSpeed * Time.deltaTime);
		}
		if (Input.GetKey (ResetCameraKey)) {
			Camera.transform.position = new Vector3 (enteredroom.transform.position.x, enteredroom.transform.position.y - 4, enteredroom.transform.position.z);
			Camera.transform.eulerAngles = new Vector3 (45f, 180f, 0f);
		}
	}

	void OnTriggerEnter (Collider collision) {

		if (collision.gameObject.name == "Endtrigger") {
			PlayerPrefs.SetString ("Cutscene", "true");
			int visittotal = 0;
			foreach (RL.Room r in Roomloader.GetComponent<room_loader>().roomlist) {
				if (r.visited) {
					visittotal += 1;
				}
			}
			PlayerPrefs.SetInt ("VisitedRooms", visittotal + 1);
			StartCoroutine (TransitionToWin ());
		}

		if (collision.gameObject.name == "Cameratrigger" && PlayerPrefs.GetString("Cutscene") == "false") {
			StartCoroutine (FadeIntoRoom (collision));
		}

	}

	void OnCollisionEnter (Collision collision) {
		if (collision.gameObject.name == "floorhazard(Clone)") {
			PlayerPrefs.SetString ("Cutscene", "true");
			int visittotal = 0;
			foreach (RL.Room r in Roomloader.GetComponent<room_loader>().roomlist) {
				if (r.visited) {
					visittotal += 1;
				}
			}
			PlayerPrefs.SetInt ("VisitedRooms", visittotal + 1);
			StartCoroutine (TransitionToDeath ());
		}
	}

	IEnumerator FadeIntoRoom (Collider collision) {

		float t = 0;

		while (t <= 0.3f) {
			Blackscreen.color = new Color (0, 0, 0, t / 0.3f);
			t += 0.03f;
			yield return new WaitForSeconds (0.03f);
		}
		Blackscreen.color = new Color (0, 0, 0, 1);

		Roomloader.GetComponent<room_loader> ().ToggleRoomVisibility (collision);
		enteredroom = collision.gameObject;
		Camera.transform.position = new Vector3 (collision.transform.position.x, collision.transform.position.y - 4, collision.transform.position.z);

		while (t >= 0) {
			Blackscreen.color = new Color (0, 0, 0, t / 0.3f);
			t -= 0.03f;
			yield return new WaitForSeconds (0.03f);
		}
			
		yield return null;

	}

	IEnumerator TransitionToWin () {
		float t = 0;

		while (t <= 0.1f) {
			Blackscreen.color = new Color (0, 0, 0, t / 0.1f);
			t += 0.01f;
			yield return new WaitForSeconds (0.03f);
		}
		Blackscreen.color = new Color (0, 0, 0, 1);

		SceneManager.LoadScene ("Winscreen");
	}

	IEnumerator TransitionToDeath () {
		float t = 0;
		Destroy (Player.GetComponent<Rigidbody> ());

		while (t <= 0.2f) {
			t += 0.01f;
			Player.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y - 0.05f, Player.transform.position.z);
			yield return new WaitForSeconds (0.03f);
		}
		t = 0;
		foreach (Transform t1 in Player.transform) {
			foreach (Transform t2 in t1) {
				t2.GetComponent<MeshRenderer> ().enabled = false;
			}
		}
		yield return new WaitForSeconds (0.4f);
		while (t <= 0.1f) {
			Blackscreen.color = new Color (0, 0, 0, t / 0.1f);
			t += 0.01f;
			yield return new WaitForSeconds (0.03f);
		}
		Blackscreen.color = new Color (0, 0, 0, 1);

		SceneManager.LoadScene ("Deathscreen");
	}
}
