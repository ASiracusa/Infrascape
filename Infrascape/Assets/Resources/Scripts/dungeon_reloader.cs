using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class dungeon_reloader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SceneManager.LoadSceneAsync ("Game");
//		SceneManager.UnloadSceneAsync (SceneManager.GetActiveScene());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
