using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class win_statistics : MonoBehaviour {

	// Use this for initialization
	void Start () {
		PlayerPrefs.SetString ("Cutscene", "false");

		GameObject.Find ("Canvas/Page/LeaveButton").GetComponent<Button> ().onClick.AddListener (delegate {SceneManager.LoadScene ("Menu");});

		GameObject.Find ("Canvas/Page/Name").GetComponent<Text> ().text = "You conquered " + PlayerPrefs.GetString("DungeonName") + "!";
		GameObject.Find ("Canvas/Page/Time").GetComponent<Text> ().text = "TOOK " + PlayerPrefs.GetString("TimeSpent") + " TO EXIT";
		GameObject.Find ("Canvas/Page/Gold").GetComponent<Text> ().text = PlayerPrefs.GetInt("Gold") + " GOLD FOUND";
		GameObject.Find ("Canvas/Page/NumOfRooms").GetComponent<Text> ().text = "VISITED " + PlayerPrefs.GetInt("VisitedRooms") + "/" +  PlayerPrefs.GetInt("NumOfRooms") + " ROOMS";
		GameObject.Find ("Canvas/Page/Chests").GetComponent<Text> ().text = PlayerPrefs.GetInt ("OpenedChests") + "/" + PlayerPrefs.GetInt ("NumOfChests") + " CHESTS LOOTED";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
