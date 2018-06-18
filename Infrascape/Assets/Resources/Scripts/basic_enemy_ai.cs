using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using INV = inventory;

public class basic_enemy_ai : MonoBehaviour {

	public GameObject cio;
	public INV.Item item;

	public GameObject Player;
	public int health;
	public bool iframes;
	public bool agro;
	public bool leftroom;

	public Vector3 home;
	private bool toofar;

	// Use this for initialization
	void Start () {

		Player = GameObject.Find ("Player");
		health = 30;
		agro = false;
		leftroom = false;
		home = this.transform.position;

		this.GetComponent<Rigidbody>().freezeRotation = true;

		string[] raritylist = { "common", "common", "common", "common", "common", "uncommon", "uncommon", "uncommon", "uncommon", "rare", "rare", "rare", "very rare", "very rare", "artifact"};
		string chosenrarity = raritylist [Random.Range (0, raritylist.Length)];

		while (true) {
			INV.Item i = Player.GetComponent<inventory> ().catalog [Random.Range(0, Player.GetComponent<inventory> ().catalog.Count)];
			if (i.kind == "Weapon" && i.rarity == chosenrarity) {
				item = i;
				cio = Instantiate(i.item, this.transform.GetChild(1));
				cio.transform.localPosition = new Vector3 (0, 0, 0);
				break;
			}
		}

		StartCoroutine (Patrol ());

	}
	
	// Update is called once per frame
	void Update () {

		if (health <=0) {
			health = 0;
			Die ();
		}

		if (Vector3.Distance (Player.transform.position, this.transform.position) < 10 && !agro && !leftroom) {
			print ("triggered");
			StartCoroutine (Agroed ());
			print("close");
		}

		if (Vector3.Distance (home, this.transform.position) < 3 && leftroom) {
			leftroom = false;
		}

	}

	void OnTriggerEnter (Collider collision) {

		if (collision.transform.parent != null) {
			if (collision.transform.parent.gameObject.GetComponent<item_data> () && !iframes) {
				StartCoroutine(TakeDamage (collision.transform.parent.gameObject.GetComponent<item_data> ().damage));
			}
		}

		if (collision.gameObject.name == "Cameratrigger") {
			agro = false;
			leftroom = true;
		}

	}

	IEnumerator Patrol () {

		while (true) {

			if (!agro) {
				if (Vector3.Distance (this.transform.position, home) > 5) {
					toofar = true;
					this.transform.LookAt (home);
				} else {
					toofar = false;
					this.transform.eulerAngles = new Vector3 (0, Random.Range (0, 360), 0);
				}

				StartCoroutine (WalkForward ());

				yield return new WaitForSeconds (2.5f);

			}

			yield return new WaitForSeconds (0.1f);
		}

		yield return null;

	}

	IEnumerator WalkForward () {

		float t = 0;

		while (t < 1) {
			if (agro) {
				break;
			}
			this.transform.Translate (Vector3.forward * Time.deltaTime * 2);
			t += 0.01f;
			yield return new WaitForSeconds (0.01f);
		}

		yield return null;

	}

	public void Die () {

		string itemtext = "\n - <color=#" + ColorUtility.ToHtmlStringRGB(item.color) + ">" + item.name + "</color>";

		GameObject panel = GameObject.Find ("Main Camera Screen/GameMenu/ItemLog/ItemText") as GameObject;
		panel.GetComponent<Text> ().text += "\n---------- [" + PlayerPrefs.GetString ("TimeSpent") + "] ----------\nYou defeated an Enemy and found:" + itemtext;

		Player.GetComponent<inventory> ().playerinventory.Add (item);

		Destroy (this.gameObject);

	}

	IEnumerator TakeDamage (int damage) {

		health -= damage;
		iframes = true;
		yield return new WaitForSeconds (0.5f);
		iframes = false;

		yield return null;

	}

	IEnumerator Agroed () {

		agro = true;
		StartCoroutine (WeaponAnimation ());

		while (true) {
			this.transform.LookAt (new Vector3(Player.transform.position.x, this.transform.position.y, Player.transform.position.z));
			this.transform.Translate (Vector3.forward * Time.deltaTime * 2.5f);
			yield return new WaitForSeconds (0.01f);
			if (Vector3.Distance (Player.transform.position, this.transform.position) >= 10 && !leftroom || leftroom) {
//				if (!leftroom) {
//					home = this.transform.position;
//				}
				agro = false;
				break;
			}
		}

		yield return null;

	}

	IEnumerator WeaponAnimation () {

		print ("weapon");
		int ind = 0;
		string anim = "";
		foreach (AnimationState state in cio.GetComponent<Animation> ()) {
			print (state.name);
			if (state.name != "A_HoldWeapon") {
				anim = state.name;
				cio.GetComponent<Animation> ().Play (anim);
			}
			ind++;
		}

		cio.GetComponent<Animation> () [anim].speed = item.speed;

		while (true) {

			//cio.GetComponent<Animation> ().Stop ("A_HoldWeapon");
			cio.GetComponent<Animation> ().Play (anim);
			yield return new WaitForSeconds (cio.GetComponent<Animation>()[anim].length / item.speed * 2);
			cio.GetComponent<Animation> ().Play ("A_HoldWeapon");
			if (!agro) {
				break;
			}

		}

		//cio.GetComponent<Animation> ().Play ("A_HoldWeapon");

		yield return null;

	}
}
