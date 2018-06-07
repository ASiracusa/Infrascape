using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using RL = room_loader;
using INV = inventory;

public class pause_controller : MonoBehaviour {

	private static bool GameIsPaused = false;

	public GameObject Player;
	public GameObject Roomloader;
	public GameObject PauseMenu;
	public GameObject GameMenu;

	public GameObject MiniMap;
	public GameObject ItemLog;
	public GameObject CurrItem;
	public GameObject InvLog;

	public Button MiniMapHider;
	public Button ItemLogHider;
	public Button CurrItemHider;
	public Button InvLogHider;
	private string MiniMapOpen;
	private string ItemLogOpen;
	private string CurrItemOpen;
	private string InvLogOpen;
	private float MiniMapLerp;
	private float ItemLogLerp;
	private float CurrItemLerp;
	private float InvLogLerp;

	public RawImage Leatherbacking;
	public GameObject MapSection;
	public GameObject InventorySection;
	public GameObject InfoSection;
	public GameObject SettingsSection;
	public Button ResumeButton;
	public KeyCode PauseButton;

	private string currentpage;
	public RL.Room currentroom;
	public int currentmapfloor;

	void Start () {

		MiniMapOpen = "opened";
		ItemLogOpen = "opened";
		CurrItemOpen = "opened";
		InvLogOpen = "opened";
		ResumeButton.onClick.AddListener (ClosePauseMenu);
		MiniMapHider.onClick.AddListener (HideMiniMap);
		ItemLogHider.onClick.AddListener (HideItemLog);
		CurrItemHider.onClick.AddListener (HideCurrItem);
		InvLogHider.onClick.AddListener (HideInvLog);

		GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/RightPage/NextPage").GetComponent<Button>().onClick.AddListener (FlipPagesRight);
		GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage/PreviousPage").GetComponent<Button>().onClick.AddListener (FlipPagesLeft);
		GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/RightPage/NextPage").GetComponent<Button>().onClick.AddListener (FlipPagesRight);
		GameObject.Find ("Main Camera Screen/PauseMenu/InfoSection/LeftPage/PreviousPage").GetComponent<Button>().onClick.AddListener (FlipPagesLeft);
		GameObject.Find ("Main Camera Screen/PauseMenu/InfoSection/RightPage/NextPage").GetComponent<Button>().onClick.AddListener (FlipPagesRight);
		GameObject.Find ("Main Camera Screen/PauseMenu/SettingsSection/LeftPage/PreviousPage").GetComponent<Button>().onClick.AddListener (FlipPagesLeft);

		GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/UpLevelButton").GetComponent<Button> ().onClick.AddListener (MoveMapUp);
		GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/DownLevelButton").GetComponent<Button> ().onClick.AddListener (MoveMapDown);

		currentpage = "Map";
		currentmapfloor = 0;

		MiniMap.gameObject.transform.position = new Vector3 (GameMenu.GetComponent<RectTransform> ().rect.width / 8f, GameMenu.GetComponent<RectTransform>().rect.height / 16f * 13f, 0f);
		MiniMap.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameMenu.GetComponent<RectTransform> ().rect.width * 100 / 474, GameMenu.GetComponent<RectTransform> ().rect.height * 100 / 296);
		CurrItem.gameObject.transform.position = new Vector3 (GameMenu.GetComponent<RectTransform> ().rect.width / 8f * 7f, GameMenu.GetComponent<RectTransform>().rect.height / 16f * 13f, 0f);
		CurrItem.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameMenu.GetComponent<RectTransform> ().rect.width * 100 / 474, GameMenu.GetComponent<RectTransform> ().rect.height * 100 / 296);
		ItemLog.gameObject.transform.position = new Vector3 (GameMenu.GetComponent<RectTransform> ().rect.width / 8f, GameMenu.GetComponent<RectTransform>().rect.height * 43f / 128f, 0f);
		ItemLog.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameMenu.GetComponent<RectTransform> ().rect.width * 100 / 474, GameMenu.GetComponent<RectTransform> ().rect.height * 180 / 296);
		InvLog.gameObject.transform.position = new Vector3 (GameMenu.GetComponent<RectTransform> ().rect.width / 8f * 7f, GameMenu.GetComponent<RectTransform>().rect.height * 43f / 128f, 0f);
		InvLog.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameMenu.GetComponent<RectTransform> ().rect.width * 100 / 474, GameMenu.GetComponent<RectTransform> ().rect.height * 180 / 296);

		MiniMapHider.GetComponent<RectTransform> ().sizeDelta = new Vector2 (MiniMap.GetComponent<RectTransform> ().rect.width, MiniMap.GetComponent<RectTransform> ().rect.height);
		CurrItemHider.GetComponent<RectTransform> ().sizeDelta = new Vector2 (CurrItem.GetComponent<RectTransform> ().rect.width, CurrItem.GetComponent<RectTransform> ().rect.height);
		ItemLogHider.GetComponent<RectTransform> ().sizeDelta = new Vector2 (ItemLog.GetComponent<RectTransform> ().rect.width, ItemLog.GetComponent<RectTransform> ().rect.height);
		InvLogHider.GetComponent<RectTransform> ().sizeDelta = new Vector2 (MiniMap.GetComponent<RectTransform> ().rect.width, InvLog.GetComponent<RectTransform> ().rect.height);

		Leatherbacking.gameObject.transform.position = new Vector3 (PauseMenu.GetComponent<RectTransform> ().rect.width / 2f, GameMenu.GetComponent<RectTransform>().rect.height / 2f, 0f);
		Leatherbacking.GetComponent<RectTransform> ().sizeDelta = new Vector2 (PauseMenu.GetComponent<RectTransform> ().rect.width / 522 * 400, PauseMenu.GetComponent<RectTransform> ().rect.height / 326 * 250);
		GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage").gameObject.transform.position = new Vector3 (PauseMenu.GetComponent<RectTransform> ().rect.width / 512 * 161, GameMenu.GetComponent<RectTransform>().rect.height / 2f, 0f);
		GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().sizeDelta = new Vector2 (PauseMenu.GetComponent<RectTransform> ().rect.width / 512 * 175, PauseMenu.GetComponent<RectTransform> ().rect.height / 32 * 22);
		GameObject.Find("Main Camera Screen/PauseMenu/MapSection/RightPage").gameObject.transform.position = new Vector3 (PauseMenu.GetComponent<RectTransform> ().rect.width / 512 * 351, GameMenu.GetComponent<RectTransform>().rect.height / 2f, 0f);
		GameObject.Find("Main Camera Screen/PauseMenu/MapSection/RightPage").GetComponent<RectTransform> ().sizeDelta = new Vector2 (PauseMenu.GetComponent<RectTransform> ().rect.width / 512 * 175, PauseMenu.GetComponent<RectTransform> ().rect.height / 32 * 22);
		GameObject.Find("Main Camera Screen/PauseMenu/InventorySection/LeftPage").gameObject.transform.position = new Vector3 (PauseMenu.GetComponent<RectTransform> ().rect.width / 512 * 161, GameMenu.GetComponent<RectTransform>().rect.height / 2f, 0f);
		GameObject.Find("Main Camera Screen/PauseMenu/InventorySection/LeftPage").GetComponent<RectTransform> ().sizeDelta = new Vector2 (PauseMenu.GetComponent<RectTransform> ().rect.width / 512 * 175, PauseMenu.GetComponent<RectTransform> ().rect.height / 32 * 22);
		GameObject.Find("Main Camera Screen/PauseMenu/InventorySection/RightPage").gameObject.transform.position = new Vector3 (PauseMenu.GetComponent<RectTransform> ().rect.width / 512 * 351, GameMenu.GetComponent<RectTransform>().rect.height / 2f, 0f);
		GameObject.Find("Main Camera Screen/PauseMenu/InventorySection/RightPage").GetComponent<RectTransform> ().sizeDelta = new Vector2 (PauseMenu.GetComponent<RectTransform> ().rect.width / 512 * 175, PauseMenu.GetComponent<RectTransform> ().rect.height / 32 * 22);
		GameObject.Find("Main Camera Screen/PauseMenu/InfoSection/LeftPage").gameObject.transform.position = new Vector3 (PauseMenu.GetComponent<RectTransform> ().rect.width / 512 * 161, GameMenu.GetComponent<RectTransform>().rect.height / 2f, 0f);
		GameObject.Find("Main Camera Screen/PauseMenu/InfoSection/LeftPage").GetComponent<RectTransform> ().sizeDelta = new Vector2 (PauseMenu.GetComponent<RectTransform> ().rect.width / 512 * 175, PauseMenu.GetComponent<RectTransform> ().rect.height / 32 * 22);
		GameObject.Find("Main Camera Screen/PauseMenu/InfoSection/RightPage").gameObject.transform.position = new Vector3 (PauseMenu.GetComponent<RectTransform> ().rect.width / 512 * 351, GameMenu.GetComponent<RectTransform>().rect.height / 2f, 0f);
		GameObject.Find("Main Camera Screen/PauseMenu/InfoSection/RightPage").GetComponent<RectTransform> ().sizeDelta = new Vector2 (PauseMenu.GetComponent<RectTransform> ().rect.width / 512 * 175, PauseMenu.GetComponent<RectTransform> ().rect.height / 32 * 22);
		GameObject.Find("Main Camera Screen/PauseMenu/SettingsSection/LeftPage").gameObject.transform.position = new Vector3 (PauseMenu.GetComponent<RectTransform> ().rect.width / 512 * 161, GameMenu.GetComponent<RectTransform>().rect.height / 2f, 0f);
		GameObject.Find("Main Camera Screen/PauseMenu/SettingsSection/LeftPage").GetComponent<RectTransform> ().sizeDelta = new Vector2 (PauseMenu.GetComponent<RectTransform> ().rect.width / 512 * 175, PauseMenu.GetComponent<RectTransform> ().rect.height / 32 * 22);
		GameObject.Find("Main Camera Screen/PauseMenu/SettingsSection/RightPage").gameObject.transform.position = new Vector3 (PauseMenu.GetComponent<RectTransform> ().rect.width / 512 * 351, GameMenu.GetComponent<RectTransform>().rect.height / 2f, 0f);
		GameObject.Find("Main Camera Screen/PauseMenu/SettingsSection/RightPage").GetComponent<RectTransform> ().sizeDelta = new Vector2 (PauseMenu.GetComponent<RectTransform> ().rect.width / 512 * 175, PauseMenu.GetComponent<RectTransform> ().rect.height / 32 * 22);

		GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage/MapPanel").GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 5 * 4,GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 5 * 4);
		GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage/FloorIcon").GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 15 * 4,GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 25 * 4);
		GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage/UpLevelButton").GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 25 * 3,GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 25 * 3);
		GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage/DownLevelButton").GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 25 * 3,GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 25 * 3);
		GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/FloorIcon").gameObject.transform.localPosition = new Vector3 (0, GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / -15 * 7, 0);
		GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/UpLevelButton").gameObject.transform.localPosition = new Vector3 (GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / -5, GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / -15 * 7, 0);
		GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/DownLevelButton").gameObject.transform.localPosition = new Vector3 (GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 5, GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / -15 * 7, 0);

		GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage/InventoryPanel").GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage").GetComponent<RectTransform> ().rect.width / 5 * 4, GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage").GetComponent<RectTransform> ().rect.width / 5 * 4);
		GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage/InventoryPanel").gameObject.transform.localPosition = new Vector3 (GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage").GetComponent<RectTransform> ().rect.width / 175 * -5, GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage").GetComponent<RectTransform> ().rect.height / 225 * -20, 0);
		GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage/Scrollbar").GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage").GetComponent<RectTransform> ().rect.width / 10, GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage").GetComponent<RectTransform> ().rect.width / 5 * 4);
		GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage/Scrollbar").gameObject.transform.localPosition = new Vector3 (GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage").GetComponent<RectTransform> ().rect.width / 20 * 8, GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage").GetComponent<RectTransform> ().rect.height / 225 * -20, 0);

		GameObject.Find ("Main Camera Screen/GameMenu/HealthBar").gameObject.transform.position = new Vector3 (0, GameMenu.GetComponent<RectTransform> ().rect.height / -10 * 9);
		print (PauseMenu.GetComponent<RectTransform> ().rect.width);
		print (PauseMenu.GetComponent<RectTransform> ().rect.height);

		PauseMenu.SetActive (false);

	}

	// Update is called once per frame
	void Update () {

//		if (screenres.width != Screen.currentResolution.width && screenres.height != Screen.currentResolution.height) {
//
//			print (Screen.currentResolution.width + " " + Screen.currentResolution.height);
//			screenres.width = Screen.currentResolution.width;
//			screenres.height = Screen.currentResolution.height;
//
//			GameMenu.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);
//			PauseMenu.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);
//			MiniMap.gameObject.transform.position = new Vector3 (GameMenu.GetComponent<RectTransform> ().rect.width / 8f, GameMenu.GetComponent<RectTransform>().rect.height / 16f * 13f, 0f);
//			MiniMap.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameMenu.GetComponent<RectTransform> ().rect.width * 100 / 474, GameMenu.GetComponent<RectTransform> ().rect.height * 100 / 296);
//			CurrItem.gameObject.transform.position = new Vector3 (GameMenu.GetComponent<RectTransform> ().rect.width / 8f * 7f, GameMenu.GetComponent<RectTransform>().rect.height / 16f * 13f, 0f);
//			CurrItem.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameMenu.GetComponent<RectTransform> ().rect.width * 100 / 474, GameMenu.GetComponent<RectTransform> ().rect.height * 100 / 296);
//			ItemLog.gameObject.transform.position = new Vector3 (GameMenu.GetComponent<RectTransform> ().rect.width / 8f, GameMenu.GetComponent<RectTransform>().rect.height * 43f / 128f, 0f);
//			ItemLog.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameMenu.GetComponent<RectTransform> ().rect.width * 100 / 474, GameMenu.GetComponent<RectTransform> ().rect.height * 180 / 296);
//			InvLog.gameObject.transform.position = new Vector3 (GameMenu.GetComponent<RectTransform> ().rect.width / 8f * 7f, GameMenu.GetComponent<RectTransform>().rect.height * 43f / 128f, 0f);
//			InvLog.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameMenu.GetComponent<RectTransform> ().rect.width * 100 / 474, GameMenu.GetComponent<RectTransform> ().rect.height * 180 / 296);
//			MiniMapHider.GetComponent<RectTransform> ().sizeDelta = new Vector2 (MiniMap.GetComponent<RectTransform> ().rect.width, MiniMap.GetComponent<RectTransform> ().rect.height);
//			CurrItemHider.GetComponent<RectTransform> ().sizeDelta = new Vector2 (CurrItem.GetComponent<RectTransform> ().rect.width, CurrItem.GetComponent<RectTransform> ().rect.height);
//			ItemLogHider.GetComponent<RectTransform> ().sizeDelta = new Vector2 (ItemLog.GetComponent<RectTransform> ().rect.width, ItemLog.GetComponent<RectTransform> ().rect.height);
//			InvLogHider.GetComponent<RectTransform> ().sizeDelta = new Vector2 (MiniMap.GetComponent<RectTransform> ().rect.width, InvLog.GetComponent<RectTransform> ().rect.height);
//
//		}

		if (Input.GetKeyDown (PauseButton)) {

			if (GameIsPaused) {
				ClosePauseMenu ();
			} else if (!GameIsPaused) {
				OpenPauseMenu ();
				DrawMap ();
			}
				
		}

		if (MiniMapOpen == "open") {
			float timesincestartedlerping = Time.time - MiniMapLerp;
			MiniMap.gameObject.transform.position = Vector3.Lerp (MiniMap.gameObject.transform.position, new Vector3 (GameMenu.GetComponent<RectTransform>().rect.width / 8f, GameMenu.GetComponent<RectTransform>().rect.height / 16f * 13f, 0f), timesincestartedlerping / 0.75f);
			if (timesincestartedlerping > 0.75f)
				MiniMapOpen = "opened";
		} else if (MiniMapOpen == "close") {
			float timesincestartedlerping = Time.time - MiniMapLerp;
			MiniMap.gameObject.transform.position = Vector3.Lerp (MiniMap.gameObject.transform.position, new Vector3 (GameMenu.GetComponent<RectTransform>().rect.width / -12f, GameMenu.GetComponent<RectTransform>().rect.height / 16f * 13f, 0f), timesincestartedlerping / 0.75f);
			if (timesincestartedlerping > 0.75f)
				MiniMapOpen = "closed";
		}

		if (ItemLogOpen == "open") {
			float timesincestartedlerping = Time.time - ItemLogLerp;
			ItemLog.gameObject.transform.position = Vector3.Lerp (ItemLog.gameObject.transform.position, new Vector3 (GameMenu.GetComponent<RectTransform> ().rect.width / 8f, GameMenu.GetComponent<RectTransform>().rect.height * 43f / 128f, 0f), timesincestartedlerping / 0.75f);
			if (timesincestartedlerping > 0.75f)
				ItemLogOpen = "opened";
		} else if (ItemLogOpen == "close") {
			float timesincestartedlerping = Time.time - ItemLogLerp;
			ItemLog.gameObject.transform.position = Vector3.Lerp (ItemLog.gameObject.transform.position, new Vector3 (GameMenu.GetComponent<RectTransform> ().rect.width / -12f, GameMenu.GetComponent<RectTransform>().rect.height * 43f / 128f, 0f), timesincestartedlerping / 0.75f);
			if (timesincestartedlerping > 0.75f)
				ItemLogOpen = "closed";
		}

		if (CurrItemOpen == "open") {
			float timesincestartedlerping = Time.time - CurrItemLerp;
			CurrItem.gameObject.transform.position = Vector3.Lerp (CurrItem.gameObject.transform.position, new Vector3 (GameMenu.GetComponent<RectTransform> ().rect.width / 8f * 7f, GameMenu.GetComponent<RectTransform>().rect.height / 16f * 13f, 0f), timesincestartedlerping / 0.75f);
			if (timesincestartedlerping > 0.75f)
				CurrItemOpen = "opened";
		} else if (CurrItemOpen == "close") {
			float timesincestartedlerping = Time.time - CurrItemLerp;
			CurrItem.gameObject.transform.position = Vector3.Lerp (CurrItem.gameObject.transform.position, new Vector3 (GameMenu.GetComponent<RectTransform> ().rect.width / 12f * 13f, GameMenu.GetComponent<RectTransform>().rect.height / 16f * 13f, 0f), timesincestartedlerping / 0.75f);
			if (timesincestartedlerping > 0.75f)
				CurrItemOpen = "closed";
		}

		if (InvLogOpen == "open") {
			float timesincestartedlerping = Time.time - InvLogLerp;
			InvLog.gameObject.transform.position = Vector3.Lerp (InvLog.gameObject.transform.position, new Vector3 (GameMenu.GetComponent<RectTransform> ().rect.width / 8f * 7f, GameMenu.GetComponent<RectTransform>().rect.height * 43f / 128f, 0f), timesincestartedlerping / 0.75f);
			if (timesincestartedlerping > 0.75f)
				InvLogOpen = "opened";
		} else if (InvLogOpen == "close") {
			float timesincestartedlerping = Time.time - InvLogLerp;
			InvLog.gameObject.transform.position = Vector3.Lerp (InvLog.gameObject.transform.position, new Vector3 (GameMenu.GetComponent<RectTransform> ().rect.width / 12f * 13f, GameMenu.GetComponent<RectTransform>().rect.height * 43f / 128f, 0f), timesincestartedlerping / 0.75f);
			if (timesincestartedlerping > 0.75f)
				InvLogOpen = "closed";
		}
	
	}

	void OpenPauseMenu () {
		PauseMenu.SetActive (true);
		GameMenu.SetActive (false);
		Time.timeScale = 0;
		GameIsPaused = true;
		DisplayInventory ();
	}

	void ClosePauseMenu () {
		PauseMenu.SetActive (false);
		GameMenu.SetActive (true);
		Time.timeScale = 1;
		GameIsPaused = false;
	}

	void HideMiniMap () {

		if (MiniMapOpen == "closed") {
			MiniMapOpen = "open";
			MiniMapLerp = Time.time;
		}
		else if (MiniMapOpen == "opened") {
			MiniMapOpen = "close";
			MiniMapLerp = Time.time;
		}

	}

	void HideItemLog () {

		if (ItemLogOpen == "closed") {
			ItemLogOpen = "open";
			ItemLogLerp = Time.time;
		}
		else if (ItemLogOpen == "opened") {
			ItemLogOpen = "close";
			ItemLogLerp = Time.time;
		}

	}

	void HideCurrItem () {

		if (CurrItemOpen == "closed") {
			CurrItemOpen = "open";
			CurrItemLerp = Time.time;
		}
		else if (ItemLogOpen == "opened") {
			print (CurrItem.transform.position);
			CurrItemOpen = "close";
			CurrItemLerp = Time.time;
		}

	}

	void HideInvLog () {

		if (InvLogOpen == "closed") {
			InvLogOpen = "open";
			InvLogLerp = Time.time;
		}
		else if (ItemLogOpen == "opened") {
			print (InvLog.transform.position);
			InvLogOpen = "close";
			InvLogLerp = Time.time;
		}

	}

	void FlipPagesRight () {
		if (currentpage == "Map") {
			currentpage = "Inventory";
			InventorySection.SetActive (true);
			MapSection.SetActive (false);
		} else if (currentpage == "Inventory") {
			currentpage = "Info";
			InfoSection.SetActive (true);
			InventorySection.SetActive (false);
		} else if (currentpage == "Info") {
			currentpage = "Settings";
			SettingsSection.SetActive (true);
			InfoSection.SetActive (false);
		}
	}

	void FlipPagesLeft () {
		if (currentpage == "Inventory") {
			currentpage = "Map";
			MapSection.SetActive (true);
			InventorySection.SetActive (false);
		} else if (currentpage == "Info") {
			currentpage = "Inventory";
			InventorySection.SetActive (true);
			InfoSection.SetActive (false);
		} else if (currentpage == "Settings") {
			currentpage = "Info";
			InfoSection.SetActive (true);
			SettingsSection.SetActive (false);
		}
	}

	void MoveMapUp () {
		currentmapfloor += 1;
		DrawMap ();
	}

	void MoveMapDown () {
		currentmapfloor -= 1;
		DrawMap ();
	}

	void DrawMap () {
		foreach (Transform t in GameObject.Find("Main Camera Screen/PauseMenu/MapSection/LeftPage/MapPanel").transform) {
			GameObject.Destroy (t.gameObject);
		}

		for (int i = 1; i <= Roomloader.GetComponent<room_loader>().roomlist.Count; i++) {
			if (Roomloader.GetComponent<room_loader>().roomlist[i-1].visited && Roomloader.GetComponent<room_loader>().roomlist[i-1].level == currentmapfloor) {
				GameObject room = GameObject.Find ("room_" + i);
				foreach (Transform t in room.transform) {
					if (t.name == "Updoor") {
						GameObject door = Instantiate (Resources.Load ("Menu/Maptile"), new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
						door.transform.SetParent (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/MapPanel").transform);
						door.transform.localPosition = new Vector3 (Roomloader.GetComponent<room_loader>().roomlist[i-1].x * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14, Roomloader.GetComponent<room_loader>().roomlist[i-1].y * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14 + GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 20);
						door.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 25, GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 25);
						door.GetComponent<Image> ().color = Color.gray;
						GameObject newroom = Instantiate (Resources.Load ("Menu/Maptile"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
						newroom.transform.SetParent (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/MapPanel").transform);
						newroom.transform.localPosition = new Vector3 (Roomloader.GetComponent<room_loader>().roomlist[i-1].x * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14, (Roomloader.GetComponent<room_loader>().roomlist[i-1].y + 1) * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14);
						newroom.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 16, GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 16);
						newroom.GetComponent<Image> ().color = Color.black;
					}
					if (t.name == "Downdoor") {
						GameObject door = Instantiate (Resources.Load ("Menu/Maptile"), new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
						door.transform.SetParent (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/MapPanel").transform);
						door.transform.localPosition = new Vector3 (Roomloader.GetComponent<room_loader>().roomlist[i-1].x * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14, Roomloader.GetComponent<room_loader>().roomlist[i-1].y * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14 - GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 20);
						door.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 25, GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 25);
						door.GetComponent<Image> ().color = Color.gray;
						GameObject newroom = Instantiate (Resources.Load ("Menu/Maptile"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
						newroom.transform.SetParent (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/MapPanel").transform);
						newroom.transform.localPosition = new Vector3 (Roomloader.GetComponent<room_loader>().roomlist[i-1].x * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14, (Roomloader.GetComponent<room_loader>().roomlist[i-1].y - 1) * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14);
						newroom.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 16, GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 16);
						newroom.GetComponent<Image> ().color = Color.black;
					}
					if (t.name == "Leftdoor") {
						GameObject door = Instantiate (Resources.Load ("Menu/Maptile"), new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
						door.transform.SetParent (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/MapPanel").transform);
						door.transform.localPosition = new Vector3 (Roomloader.GetComponent<room_loader>().roomlist[i-1].x * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14 - GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 20, Roomloader.GetComponent<room_loader>().roomlist[i-1].y * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14);
						door.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 25, GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 25);
						door.GetComponent<Image> ().color = Color.gray;
						GameObject newroom = Instantiate (Resources.Load ("Menu/Maptile"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
						newroom.transform.SetParent (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/MapPanel").transform);
						newroom.transform.localPosition = new Vector3 ((Roomloader.GetComponent<room_loader>().roomlist[i-1].x - 1) * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14, Roomloader.GetComponent<room_loader>().roomlist[i-1].y * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14);
						newroom.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 16, GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 16);
						newroom.GetComponent<Image> ().color = Color.black;
					}
					if (t.name == "Rightdoor") {
						GameObject door = Instantiate (Resources.Load ("Menu/Maptile"), new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
						door.transform.SetParent (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/MapPanel").transform);
						door.transform.localPosition = new Vector3 (Roomloader.GetComponent<room_loader>().roomlist[i-1].x * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14 + GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 20, Roomloader.GetComponent<room_loader>().roomlist[i-1].y * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14);
						door.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 25, GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 25);
						door.GetComponent<Image> ().color = Color.gray;
						GameObject newroom = Instantiate (Resources.Load ("Menu/Maptile"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
						newroom.transform.SetParent (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/MapPanel").transform);
						newroom.transform.localPosition = new Vector3 ((Roomloader.GetComponent<room_loader>().roomlist[i-1].x + 1) * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14, Roomloader.GetComponent<room_loader>().roomlist[i-1].y * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14);
						newroom.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 16, GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 16);
						newroom.GetComponent<Image> ().color = Color.black;
					}
				}
			}
		}

		foreach (RL.Room r in Roomloader.GetComponent<room_loader>().roomlist) {
			
			if (currentmapfloor == r.level) {
				if (currentroom.x == r.x && currentroom.y == r.y && currentmapfloor == r.level && currentroom.level == r.level) {
					GameObject room = Instantiate (Resources.Load ("Menu/Maptile"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
					room.transform.SetParent (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/MapPanel").transform);
					room.transform.localPosition = new Vector3 (r.x * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14, r.y * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14);
					room.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 16, GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 16);
					room.GetComponent<Image> ().color = Color.white;
				}
				else if (r.visited) {
					GameObject room = Instantiate (Resources.Load ("Menu/Maptile"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
					room.transform.SetParent (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/MapPanel").transform);
					room.transform.localPosition = new Vector3 (r.x * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14, r.y * GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 14);
					room.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 16, GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage").GetComponent<RectTransform> ().rect.width / 16);
					room.GetComponent<Image> ().color = Color.gray;
				}
			}
		}

		if (currentmapfloor + 1 == (int)(Roomloader.GetComponent<room_loader> ().DungeonVal1.Count())) {
			GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/UpLevelButton").GetComponent<Button> ().interactable = false;
		} else {
			GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/UpLevelButton").GetComponent<Button> ().interactable = true;
		}
		if (currentmapfloor * -1 == (int)(Roomloader.GetComponent<room_loader> ().DungeonVal2.Count())) {
			GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/DownLevelButton").GetComponent<Button> ().interactable = false;
		} else {
			GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/DownLevelButton").GetComponent<Button> ().interactable = true;
		}

		if (currentmapfloor < 0) {
			GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/FloorIcon/Text").GetComponent<Text> ().text = "B" + (-1 * currentmapfloor);
		}
		else {
			GameObject.Find ("Main Camera Screen/PauseMenu/MapSection/LeftPage/FloorIcon/Text").GetComponent<Text> ().text = "L" + (1 + currentmapfloor);
		}
	}

	void DisplayInventory () {

		List<INV.Item> inventory = Player.GetComponent<inventory> ().playerinventory;
		List<GameObject> itempanels = new List<GameObject> ();

		for (int i = 0; i < inventory.Count; i++) {
			INV.Item curritem = inventory [i];
			GameObject go = Instantiate (Resources.Load("Menu/ItemContainer"), GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage/InventoryPanel/ItemArea").gameObject.transform) as GameObject;
			go.transform.parent = GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage/InventoryPanel/ItemArea").gameObject.transform;
			go.transform.localPosition = new Vector3(0, GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage/InventoryPanel/ItemArea").GetComponent<RectTransform>().rect.height / 5 * 2 - (GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage/InventoryPanel/ItemArea").GetComponent<RectTransform>().rect.height / 5 * i), 0);
			go.GetComponent<RectTransform>().sizeDelta = new Vector2 (GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage/InventoryPanel").GetComponent<RectTransform> ().rect.width, GameObject.Find ("Main Camera Screen/PauseMenu/InventorySection/LeftPage/InventoryPanel").GetComponent<RectTransform> ().rect.height / 5);
			go.gameObject.name = "Item (" + curritem.name + ")";
			go.transform.GetChild (0).GetComponent<Text> ().text = "  " + curritem.name;
			go.transform.GetChild (1).GetComponent<Text> ().text = "1x   ";
			print (curritem.kind);
			if (curritem.kind == "weapon") {
				go.GetComponent<Image> ().color = new Color32(193, 79, 79, 255);
			}

		}

	}
}
