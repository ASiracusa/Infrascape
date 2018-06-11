using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class item_data : MonoBehaviour {

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

	// Use this for initialization
	void Start () {
		item = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
