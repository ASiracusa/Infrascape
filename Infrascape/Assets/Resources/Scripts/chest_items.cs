using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using INV = inventory;

public class chest_items : MonoBehaviour {

	public List<INV.Item> storage = new List<INV.Item>();
	public int gold;
	public bool opened;

	private GameObject Player;

	// Use this for initialization
	void Start () {

		opened = false;
		Player = GameObject.Find ("Player");

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
