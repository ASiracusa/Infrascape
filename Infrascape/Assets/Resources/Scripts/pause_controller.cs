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

	public Button ResumeButton;
	public KeyCode PauseButton;

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

		MiniMap.gameObject.transform.position = new Vector3 (GameMenu.GetComponent<RectTransform> ().rect.width / 8f, 252f, 0f);
		//MiniMap.GetComponent<RectTransform> ().sizeDelta = new Vector2

	}

	// Update is called once per frame
	void Update () {

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
			MiniMap.gameObject.transform.position = Vector3.Lerp (MiniMap.gameObject.transform.position, new Vector3 (-45f, 252f, 0f), timesincestartedlerping / 0.75f);
			if (timesincestartedlerping > 0.75f)
				MiniMapOpen = "closed";
		}

		if (ItemLogOpen == "open") {
			float timesincestartedlerping = Time.time - ItemLogLerp;
			ItemLog.gameObject.transform.position = Vector3.Lerp (ItemLog.gameObject.transform.position, new Vector3 (63.5f, 102.5f, 0f), timesincestartedlerping / 0.75f);
			if (timesincestartedlerping > 0.75f)
				ItemLogOpen = "opened";
		} else if (ItemLogOpen == "close") {
			float timesincestartedlerping = Time.time - ItemLogLerp;
			ItemLog.gameObject.transform.position = Vector3.Lerp (ItemLog.gameObject.transform.position, new Vector3 (-45f, 102.5f, 0f), timesincestartedlerping / 0.75f);
			if (timesincestartedlerping > 0.75f)
				ItemLogOpen = "closed";
		}

		if (CurrItemOpen == "open") {
			float timesincestartedlerping = Time.time - CurrItemLerp;
			CurrItem.gameObject.transform.position = Vector3.Lerp (CurrItem.gameObject.transform.position, new Vector3 (438.5f, 252f, 0f), timesincestartedlerping / 0.75f);
			if (timesincestartedlerping > 0.75f)
				CurrItemOpen = "opened";
		} else if (CurrItemOpen == "close") {
			float timesincestartedlerping = Time.time - CurrItemLerp;
			CurrItem.gameObject.transform.position = Vector3.Lerp (CurrItem.gameObject.transform.position, new Vector3 (550f, 252f, 0f), timesincestartedlerping / 0.75f);
			if (timesincestartedlerping > 0.75f)
				CurrItemOpen = "closed";
		}

		if (InvLogOpen == "open") {
			float timesincestartedlerping = Time.time - InvLogLerp;
			InvLog.gameObject.transform.position = Vector3.Lerp (InvLog.gameObject.transform.position, new Vector3 (438.5f, 102.5f, 0f), timesincestartedlerping / 0.75f);
			if (timesincestartedlerping > 0.75f)
				InvLogOpen = "opened";
		} else if (InvLogOpen == "close") {
			float timesincestartedlerping = Time.time - InvLogLerp;
			InvLog.gameObject.transform.position = Vector3.Lerp (InvLog.gameObject.transform.position, new Vector3 (550f, 102.5f, 0f), timesincestartedlerping / 0.75f);
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
