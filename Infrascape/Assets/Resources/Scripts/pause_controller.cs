using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class pause_controller : MonoBehaviour {

	public static bool GameIsPaused = false;

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

	private Resolution screenres;

	void Start () {

		//screenres = Screen.currentResolution;

		MiniMapOpen = "opened";
		ItemLogOpen = "opened";
		CurrItemOpen = "opened";
		InvLogOpen = "opened";
		ResumeButton.onClick.AddListener (ClosePauseMenu);
		MiniMapHider.onClick.AddListener (HideMiniMap);
		ItemLogHider.onClick.AddListener (HideItemLog);
		CurrItemHider.onClick.AddListener (HideCurrItem);
		InvLogHider.onClick.AddListener (HideInvLog);

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
		print (PauseMenu.GetComponent<RectTransform> ().rect.width);
		print (PauseMenu.GetComponent<RectTransform> ().rect.height);

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
}
