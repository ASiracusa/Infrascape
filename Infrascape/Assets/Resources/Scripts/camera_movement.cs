using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

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

		if (collision.gameObject.name == "Cameratrigger") {
			StartCoroutine (FadeIntoRoom (collision));
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
}
