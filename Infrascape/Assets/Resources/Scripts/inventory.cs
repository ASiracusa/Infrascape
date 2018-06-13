using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class inventory : MonoBehaviour {

	public KeyCode AttackButton;

	public List<Item> catalog = new List<Item> ();
	public List<Item> playerinventory = new List<Item> ();

	public GameObject cio;
	public Item curritem;

	public bool attacking;

	public class Item {

		public GameObject item;
		public string name;
		public string description;
		public string rarity;
		public Color color;
		public Sprite icon;
		public float weight;
		public string kind;
		public int damage;
		public float speed;

		// weapon
		public Item (GameObject item, string name, string description, string rarity, Sprite icon, float weight, string kind, int damage, float speed) {
			this.item = item;
			this.name = name;
			this.description = description;
			this.rarity = rarity;
			this.icon = icon;
			this.weight = weight;
			this.kind = kind;
			this.damage = damage;
			this.speed = speed;

			this.setRarityColor(rarity);
		}

		//key
		public Item (GameObject item, string name, string description, string rarity, Sprite icon, float weight, string kind) {
			this.item = item;
			this.name = name;
			this.description = description;
			this.rarity = rarity;
			this.icon = icon;
			this.weight = weight;
			this.kind = kind;

			this.setRarityColor(rarity);
		}

		public void setRarityColor (string r) {
			if (r == "common") {
				this.color = new Color32(0, 0, 0, 255);
			}
			if (r == "uncommon") {
				this.color = new Color32(0, 50, 132, 255);
			}
			if (r == "rare") {
				this.color = new Color32(86, 191, 247, 255);
			}
			if (r == "very rare") {
				this.color = new Color32(242, 86, 2, 255);
			}
			if (r == "artifact") {
				this.color = new Color32(255, 180, 5, 255);
			}
		}
			
	}

	// Use this for initialization
	void Start () {
		attacking = false;

		GameObject[] itemarray = Resources.LoadAll<GameObject> ("Items");
		foreach (GameObject go in itemarray) {
			if (go.GetComponent<item_data> ().kind == "Weapon") {
				catalog.Add(new Item(go.GetComponent<item_data> ().item, go.GetComponent<item_data> ().name, go.GetComponent<item_data> ().description, go.GetComponent<item_data> ().rarity, go.GetComponent<item_data> ().icon, go.GetComponent<item_data> ().weight, "Weapon", go.GetComponent<item_data> ().damage, go.GetComponent<item_data> ().speed));
			}
			if (go.GetComponent<item_data> ().kind == "Key") {
				catalog.Add (new Item (go.GetComponent<item_data> ().item, go.GetComponent<item_data> ().name, go.GetComponent<item_data> ().description, go.GetComponent<item_data> ().rarity, go.GetComponent<item_data> ().icon, go.GetComponent<item_data> ().weight, "Key"));
			}
			if (go.GetComponent<item_data>().name == "Sword") {
				playerinventory.Add (findItem ("Sword"));
				SetCurrentItem (findItem ("Sword"));
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey (AttackButton) && !attacking) {
			print ("!!!");
			StartCoroutine(AttackWithWeapon ());
		}

	}

	public Item findItem (string name) {
		foreach (Item i in catalog) {
			if (i.name == name) {
				return i;
			}
		}

		return new Item (GameObject.Find("Nullroom"), "null", "null", "common", new Sprite(), 0, "null", 0, 0);
	}

	public bool hasItem (string name) {
		foreach (Item i in playerinventory) {
			if (i.name == name) {
				return true;
			}
		}
		return false;
	}

	public void SetCurrentItem (Item item) {

		curritem = item;
		GameObject.Destroy (cio);
		cio = Instantiate(item.item, GameObject.Find ("Player/ItemSlot").transform) as GameObject;
		cio.transform.localPosition = cio.transform.parent.position;

	}

	void OnCollisionEnter (Collision collision) {
		print (collision.transform.name);

		if (collision.transform.parent.gameObject.name == "enddoor(Clone)" && hasItem("Dungeon Key")) {
			GameObject.Destroy (collision.transform.parent.gameObject);
		}

		if (collision.gameObject.name == "ChestTop" || collision.gameObject.name == "ChestBottom") {
			OpenChest (collision.gameObject);
		}
	}

	void OpenChest (GameObject chest) {

		if (!chest.transform.parent.gameObject.GetComponent<chest_items> ().opened) {
			chest.transform.parent.gameObject.GetComponent<chest_items> ().opened = true;
			chest.transform.parent.gameObject.GetComponent<Animation> ().Play ("A_ChestOpen");

			GameObject panel = GameObject.Find ("Main Camera Screen/GameMenu/ItemLog/ItemText") as GameObject;

			string itemtext = "\n- <color=#FFA719>" + chest.transform.parent.gameObject.GetComponent<chest_items>().gold + " Gold</color>";
			foreach (Item i in chest.transform.parent.gameObject.GetComponent<chest_items>().storage) {
				playerinventory.Add (i);
				itemtext = itemtext + "\n - <color=#" + ColorUtility.ToHtmlStringRGB(i.color) + ">" + i.name + "</color>";
			}

			panel.GetComponent<Text> ().text += "\n---------- [" + PlayerPrefs.GetString("TimeSpent") + "] ----------\nYou found:" + itemtext;
		}

	}

	IEnumerator AttackWithWeapon () {

		print ("hello");

		attacking = true;

		int ind = 0;
		string anim = "";
		foreach (AnimationState state in cio.GetComponent<Animation> ()) {
			if (ind == 0) {
				anim = state.name;
			}
			ind++;
		}
		cio.GetComponent<Animation> () [anim].speed = curritem.speed;
		cio.GetComponent<Animation> ().Play (anim);
		yield return new WaitForSeconds (cio.GetComponent<Animation>()[anim].length / curritem.speed);
		cio.GetComponent<Animation> ().Play ("A_HoldWeapon");
			
		attacking = false;

		yield return null;

	}
}