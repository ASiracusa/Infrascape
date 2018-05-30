using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_movement : MonoBehaviour {

	public Camera Camera;
	public Rigidbody player_rg;
	public float speed;
	public float turn_speed;
	public float jump_power;

	public KeyCode UpKey;
	public KeyCode DownKey;
	public KeyCode LeftKey;
	public KeyCode RightKey;

	private bool grounded;
	private Vector3 current_angle;
	private Vector3 target_angle;

	public int mousespeed;

	// Use this for initialization
	void Start () {
		player_rg = GetComponent<Rigidbody> ();
		player_rg.freezeRotation = true;
		current_angle = transform.eulerAngles;
		grounded = true;
	}

	// Update is called once per frame
	void FixedUpdate () {

		if (Input.GetKey (UpKey) && !Input.GetKey (DownKey)) {
			//player_rg.MovePosition (transform.position + transform.forward * Time.deltaTime * -speed);
			Vector3 direction = Camera.transform.forward;
			direction.y = 0;
			transform.Translate (direction * Time.deltaTime * speed, Space.World);
			//target_angle = new Vector3 (0f, 0f+Camera.transform.rotation.eulerAngles.y, 0f);
		}
		if (Input.GetKey (DownKey) && !Input.GetKey (UpKey)) {
			//player_rg.MovePosition (transform.position + transform.forward * Time.deltaTime * speed);
			Vector3 direction = -Camera.transform.forward;
			direction.y = 0;
			transform.Translate (direction * Time.deltaTime * speed, Space.World);
			//target_angle = new Vector3 (0f, 180f+Camera.transform.rotation.eulerAngles.y, 0f);
		}
		if (Input.GetKey (LeftKey) && !Input.GetKey (RightKey)) {
			//player_rg.MovePosition (transform.position + transform.right * Time.deltaTime * speed);
			Vector3 direction = -Camera.transform.right;
			direction.y = 0;
			transform.Translate (direction * Time.deltaTime * speed, Space.World);
			//target_angle = new Vector3 (0f, 270f+Camera.transform.rotation.eulerAngles.y, 0f);
		}
		if (Input.GetKey (RightKey) && !Input.GetKey (LeftKey)) {
			//player_rg.MovePosition (transform.position + transform.right * Time.deltaTime * -speed);
			Vector3 direction = Camera.transform.right;
			direction.y = 0;
			transform.Translate (direction * Time.deltaTime * speed, Space.World);
			//target_angle = new Vector3 (0f, 90f+Camera.transform.rotation.eulerAngles.y, 0f);
		}
		if (Input.GetKey (UpKey) && Input.GetKey (LeftKey) && !Input.GetKey (DownKey) && !Input.GetKey (RightKey)) {
			//target_angle = new Vector3 (0f, 315f+Camera.transform.rotation.eulerAngles.y, 0f);
		}
		if (Input.GetKey (UpKey) && Input.GetKey (RightKey) && !Input.GetKey (DownKey) && !Input.GetKey (LeftKey)) {
			//target_angle = new Vector3 (0f, 45f+Camera.transform.rotation.eulerAngles.y, 0f);
		}
		if (Input.GetKey (DownKey) && Input.GetKey (LeftKey) && !Input.GetKey (UpKey) && !Input.GetKey (RightKey)) {
			//target_angle = new Vector3 (0f, 225f+Camera.transform.rotation.eulerAngles.y, 0f);
		}
		if (Input.GetKey (DownKey) && Input.GetKey (RightKey) && !Input.GetKey (UpKey) && !Input.GetKey (LeftKey)) {
			//target_angle = new Vector3 (0f, 135+Camera.transform.rotation.eulerAngles.y, 0f);
		}

		//current_angle = new Vector3(Mathf.LerpAngle(current_angle.x, target_angle.x, Time.deltaTime * turn_speed), Mathf.LerpAngle(current_angle.y, target_angle.y, Time.deltaTime * turn_speed), Mathf.LerpAngle(current_angle.z, target_angle.z, Time.deltaTime * turn_speed));
		//transform.eulerAngles = current_angle;
	}

	void Update () {
		if (Input.GetKey (KeyCode.Space) && grounded) {
			grounded = false;
			player_rg.AddForce (Vector3.up * jump_power);
		}

		Plane playerPlane = new Plane(Vector3.up, transform.position);
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		float hitdist = 0.0f;
		// If the ray is parallel to the plane, Raycast will return false.
		if (playerPlane.Raycast (ray, out hitdist)) 
		{
			// Get the point along the ray that hits the calculated distance.
			Vector3 targetPoint = ray.GetPoint(hitdist);

			// Determine the target rotation.  This is the rotation if the transform looks at the target point.
			Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

			// Smoothly rotate towards the target point.
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
		}

	}

	void OnCollisionEnter(Collision collision) {

		if (collision.gameObject.layer == 8 && !grounded) {
			grounded = true;
		}

	}
}
