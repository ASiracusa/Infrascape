using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dungeon_names : MonoBehaviour {

	public string[] firsts = { "The Northern", "The Southern", "The Western", "The Eastern" } ;
	public string[] lasts = { "Crypt", "Dungeon", "Keep", "Lair", "Sanctum", "Ruins" };
	public string dungeonname;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GenerateNames (int seed) {

		Random.seed = seed;
		int n1 = Random.Range (0, firsts.Length);
		int n2 = Random.Range (0, lasts.Length);
		dungeonname = firsts [n1] + " " + lasts [n2];

	}
}
