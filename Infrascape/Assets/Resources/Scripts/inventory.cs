using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventory : MonoBehaviour {

	public List<Item> catalog = new List<Item> ();
	public List<Item> playerinventory = new List<Item> ();

	public class Item {

		public GameObject item;
		public string name;
		public string rarity;
		public Color color;
		public Sprite icon;
		public float weight;
		public string kind;
		public int damage;
		public float buffertime;

		// weapon
		public Item (GameObject item, string name, string rarity, Sprite icon, float weight, string kind, int damage, float buffertime) {
			this.item = item;
			this.name = name;
			this.rarity = rarity;
			this.icon = icon;
			this.weight = weight;
			this.kind = kind;
			this.damage = damage;
			this.buffertime = buffertime;

			this.setRarityColor(rarity);
		}

		public void setRarityColor (string r) {
			if (r == "common") {
				this.color = new Color(114, 114, 114);
			}
			if (r == "uncommon") {
				this.color = new Color(0, 50, 132);
			}
			if (r == "rare") {
				this.color = new Color(86, 191, 247);
			}
			if (r == "very rare") {
				this.color = new Color(242, 86, 2);
			}
			if (r == "artifact") {
				this.color = new Color(255, 180, 5);
			}
		}
			
	}

	// Use this for initialization
	void Start () {
		GameObject[] itemarray = Resources.LoadAll<GameObject> ("Items");
		foreach (GameObject go in itemarray) {
			if (go.GetComponent<item_data> ().kind == "weapon") {
				catalog.Add(new Item(go.GetComponent<item_data> ().item, go.GetComponent<item_data> ().name, go.GetComponent<item_data> ().rarity, go.GetComponent<item_data> ().icon, go.GetComponent<item_data> ().weight, "weapon", go.GetComponent<item_data> ().damage, go.GetComponent<item_data> ().buffertime));
			}
			if (go.GetComponent<item_data>().name == "Platinum Sword") {
				playerinventory.Add (findItem ("Platinum Sword"));
				GameObject w = Instantiate (catalog [catalog.Count - 1].item, this.transform);
			}
		}
		playerinventory.Add(findItem("Sword"));
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

		return null;
	}
}