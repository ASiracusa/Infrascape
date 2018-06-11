using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

public class menu_manager : MonoBehaviour {

	public Canvas playmenu;

	public Button playbtn;
	public Button quitbtn;

	public InputField seedinput;
	public Button simplestart;
	public Button seedstart;
	public Button playback;

	private float menulerpstarted = 0;
	private string lerpstatus = "none";

	// Use this for initialization
	void Start () {
		PlayerPrefs.SetInt ("CrashMod", 0);

		playbtn.onClick.AddListener (PlayButton);
		quitbtn.onClick.AddListener (QuitButton);
		playback.onClick.AddListener (PlayMenuBackButton);
		seedstart.onClick.AddListener (StartDungeonWithCode);
		simplestart.onClick.AddListener (StartDungeonWithoutCode);
	}
	
	// Update is called once per frame
	void Update () {

		if (lerpstatus == "up") {
			float timesincestartedlerping = Time.time - menulerpstarted;
			playmenu.gameObject.transform.position = Vector3.Lerp (playmenu.gameObject.transform.position, new Vector3 (0, 0, -1), timesincestartedlerping / 0.75f);
			if (timesincestartedlerping > 0.75f)
				lerpstatus = "none";
		}

		if (lerpstatus == "down") {
			float timesincestartedlerping = Time.time - menulerpstarted;
			playmenu.gameObject.transform.position = Vector3.Lerp (playmenu.gameObject.transform.position, new Vector3 (0, -300, -1), timesincestartedlerping / 0.75f);
			if (timesincestartedlerping > 0.75f)
				lerpstatus = "none";
		}

	}

	void PlayButton () {
	
		menulerpstarted = Time.time;
		lerpstatus = "up";

	}

	void PlayMenuBackButton () {

		menulerpstarted = Time.time;
		lerpstatus = "down";

	}

	void StartDungeonWithCode () {
		PlayerPrefs.SetString ("DungeonSeed", seedinput.text);
		SceneManager.LoadScene ("Game");
	}

	void StartDungeonWithoutCode () {
		PlayerPrefs.SetString ("DungeonSeed", GenerateSeed());
		SceneManager.LoadScene ("Game");
	}

	void QuitButton () {
		Application.Quit ();
	}

	public string GenerateSeed () {

		// 1
		string newseed = "B";

		// 2
		string[] dtypes = { "B", "C", "D", "M" };
		int rn = Random.Range (0, dtypes.Length);
		newseed += dtypes [rn];

		// 3 & 4
		string[] uplist = { "C", "B" };
		string[] downlist = { "D", "B" };
		if (newseed[1] == 'M') {
			newseed += "10";
		}
		else if (uplist.Contains ("" + newseed [1]) && downlist.Contains ("" + newseed [1])) {
			newseed += "" + Random.Range (2, 5) + Random.Range (1, 4);
		}
		else if (uplist.Contains ("" + newseed [1])) {
			newseed += "" + Random.Range (2, 5) + "0";
		}
		else if (downlist.Contains ("" + newseed [1])) {
			newseed += "1" + Random.Range (1, 4);
		}

		// 5 - 8
		string[] layoutlist = { "4022", "4011", "2022", "0411", "0222" };
		newseed += layoutlist [Random.Range (0, layoutlist.Length)];

		// room setup snippet
		print(newseed);
		int numfloors = int.Parse("" + newseed[2]) + int.Parse("" + newseed[3]);
		int maxrooms = (int.Parse("" + newseed[4]) + 1 + int.Parse("" + newseed[5])) * (int.Parse("" + newseed[6]) + 1 + int.Parse("" + newseed[7]));

		string[] ends = { "mmmmmccett", "mmmmccett", "mmmett", "mmmcet", "mmmccett" };
		string[] nostairs = { "mmmmmcctt", "mmmmcctt", "mmmtt", "mmmct", "mmmcctt" };
		string[] onestairs = { "mmmmmscctt", "mmmmscctt", "mmmstt", "mmmsct", "mmmscctt" };
		string[] twostairs = { "mmmmmsscctt", "mmmmsscctt", "mmmsstt", "mmmssct", "mmmsscctt" };

		bool endmade = false;

		int i = 1;
		while (i <= numfloors) {
			if (i == 1 && uplist.Contains ("" + newseed [1]) && downlist.Contains ("" + newseed [1])) {
				rn = Random.Range (0, twostairs.Length);
				if (twostairs [rn].Length > maxrooms)
					continue;
				newseed += "" + twostairs [rn] + "_";
				i++;

			} else if (i == numfloors || (uplist.Contains ("" + newseed [1]) && downlist.Contains ("" + newseed [1]) && i == int.Parse ("" + newseed [2]))) {
				rn = Random.Range (0, nostairs.Length);
				if (endmade && nostairs [rn].Length > maxrooms || !endmade && ends [rn].Length > maxrooms)
					continue;
				if (endmade) {
					newseed += "" + nostairs [rn] + "_";
				} else {
					newseed += "" + ends [rn] + "_";
					endmade = true;
					print("this shouldn't be printed");
				}
				i++;
			} else {
				rn = Random.Range (0, onestairs.Length);
				if (onestairs [rn].Length > maxrooms)
					continue;
				newseed += "" + onestairs [rn] + "_";
				i++;
			}
		}

		// random seed snippet
		newseed = newseed + Random.Range(0, 99999999);
	
		// send seed
		print(newseed);

		return newseed;

	}
}
