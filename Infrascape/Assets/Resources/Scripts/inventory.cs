using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventory : MonoBehaviour {

	public List<Item> catalog = new List<Item> ();
	public List<Item> playerinventory = new List<Item> ();

	public GameObject cio;
	public Item curritem;

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
		public float buffertime;

		// weapon
		public Item (GameObject item, string name, string description, string rarity, Sprite icon, float weight, string kind, int damage, float buffertime) {
			this.item = item;
			this.name = name;
			this.description = description;
			this.rarity = rarity;
			this.icon = icon;
			this.weight = weight;
			this.kind = kind;
			this.damage = damage;
			this.buffertime = buffertime;

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
				this.color = new Color32(114, 114, 114, 255);
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
		GameObject[] itemarray = Resources.LoadAll<GameObject> ("Items");
		foreach (GameObject go in itemarray) {
			if (go.GetComponent<item_data> ().kind == "Weapon") {
				catalog.Add(new Item(go.GetComponent<item_data> ().item, go.GetComponent<item_data> ().name, go.GetComponent<item_data> ().description, go.GetComponent<item_data> ().rarity, go.GetComponent<item_data> ().icon, go.GetComponent<item_data> ().weight, "Weapon", go.GetComponent<item_data> ().damage, go.GetComponent<item_data> ().buffertime));
			}
			if (go.GetComponent<item_data> ().kind == "Key") {
				catalog.Add (new Item (go.GetComponent<item_data> ().item, go.GetComponent<item_data> ().name, go.GetComponent<item_data> ().description, go.GetComponent<item_data> ().rarity, go.GetComponent<item_data> ().icon, go.GetComponent<item_data> ().weight, "Key"));
			}
			if (go.GetComponent<item_data>().name == "Wooden Hatchet") {
				playerinventory.Add (findItem ("Wooden Hatchet"));
				GameObject w = Instantiate (catalog [catalog.Count - 1].item, this.transform);
			}
		}
		playerinventory.Add(findItem("Sword"));
		playerinventory.Add(findItem("Sword"));
		playerinventory.Add(findItem("Sword"));
		playerinventory.Add(findItem("Sword"));
		playerinventory.Add(findItem("Sword"));
		playerinventory.Add(findItem("Sword"));
		playerinventory.Add(findItem("Platinum Sword"));
		playerinventory.Add(findItem("Dungeon Key"));
		playerinventory.Add(findItem("Metal Hatchet"));
	}
	
	// Update is called once per frame
	void Update () {
		
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
		cio = item.item;

	}

	void OnCollisionEnter (Collision collision) {
		print (collision.transform.name);

		if (collision.transform.parent.gameObject.name == "enddoor(Clone)" && hasItem("Dungeon Key")) {
			GameObject.Destroy (collision.transform.parent.gameObject);
		}
	}
}