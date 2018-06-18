using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using INV = inventory;

public class room_loader : MonoBehaviour {

	/* ////////// VARIABLES  ////////// */

	private GameObject Player;

	[Header("Dungeon Setup")]
	public GameObject self;
	public string DungeonSeed;
	public string RoomRootFile;
	public string DungeonType;

	[Tooltip("mono - only 1, # of rooms\ndelve - levels down (start at L1) and # of rooms\nclimb - levels up (start at L1) and # of rooms\nbasic - levels up (start at L1) and # of rooms")] public string[] DungeonVal1;
	[Tooltip("basic - level down (start at B1) and # of rooms")] public string[] DungeonVal2;

	[Header("Boundaries")]

	public int maxwidth;
	public int minwidth;
	public int maxheight;
	public int minheight;

	private string dungeonname;
	private int seednumber;

	private int crashcount = 0;
	private string roomtype;
	private List<GameObject> mainroomlist;
	private List<GameObject> connectorlist;
	private List<GameObject> stairroomlist;
	private List<GameObject> treasureroomlist;
	private List<GameObject> endroomlist;
	string recentdirection;

	public List<Room> roomlist = new List<Room>();
	GameObject[] roomloadarray;
	List<GameObject> roomloadlist;
	List<PTR> ptrlist = new List<PTR>();
	List<GameObject> roomchoices;
	private GameObject doorasset;
	private GameObject fullwallasset;
	private GameObject leftwallasset;
	private GameObject middlewallasset;
	private GameObject rightwallasset;
	private GameObject enddoorasset;
	private GameObject chestasset;

	private string stairname;

	private List<GameObject> chestlist = new List<GameObject> ();

	/* ////////// ROOM OBJECTS ////////// */

	public class Room {

		public int x;
		public int y;
		public GameObject r;
		public int level;
		public bool visited;

		public Room (int a, int b, GameObject c, int d) {

			x = a;
			y = b;
			r = c;
			level = d;
			visited = false;

		}

	}

	/* ////////// POSSIBLE TREASURE ROOM OBJECTS ////////// */

	public class PTR {

		public int x;
		public int y;
		public string kind;
		public string connections;
		public int level;
		// Note: Kind is equal to "one", "across", "corner", or "fourway"

		public PTR (int xc, int yc, string k, string c, int l) {

			x = xc;
			y = yc;
			kind = k;
			connections = c;
			level = l;

		}

	}

	/* ////////// POSSIBLE DOOR OBJECTS ////////// */

	public class Possibledoor {
		
		public GameObject firstroom;
		public GameObject secondroom;
		public string direction;
		// Note: Direction is the direction from the first room to the second room.

		public Possibledoor (GameObject a, GameObject b, string c) {

			firstroom = a;
			secondroom = b;
			direction = c;

		}

	}
		
	/* ////////// ROOM LIST ASSIGNMENT & ASSET LOADER ////////// */

	void Start () {

		Player = GameObject.Find ("Player");

		DungeonSeed = PlayerPrefs.GetString ("DungeonSeed");
		PlayerPrefs.SetInt ("Gold", 0);
		PlayerPrefs.SetInt ("NumOfChests", 0);
		PlayerPrefs.SetInt ("OpenedChests", 0);
		
		roomloadarray = Resources.LoadAll<GameObject> (RoomRootFile + "/mainrooms");
		roomloadlist = roomloadarray.ToList ();
		mainroomlist = roomloadlist;

		roomloadarray = Resources.LoadAll<GameObject> (RoomRootFile + "/connectors");
		roomloadlist = roomloadarray.ToList ();
		connectorlist = roomloadlist;

		roomloadarray = Resources.LoadAll<GameObject> (RoomRootFile + "/stairrooms");
		roomloadlist = roomloadarray.ToList ();
		stairroomlist = roomloadlist;

		roomloadarray = Resources.LoadAll<GameObject> (RoomRootFile + "/treasurerooms");
		roomloadlist = roomloadarray.ToList ();
		treasureroomlist = roomloadlist;

		roomloadarray = Resources.LoadAll<GameObject> (RoomRootFile + "/endrooms");
		roomloadlist = roomloadarray.ToList ();
		endroomlist = roomloadlist;

		doorasset = Resources.Load<GameObject>(RoomRootFile + "/door");
		fullwallasset = Resources.Load<GameObject>(RoomRootFile + "/fullwall");
		leftwallasset = Resources.Load<GameObject>(RoomRootFile + "/leftwall");
		middlewallasset = Resources.Load<GameObject>(RoomRootFile + "/middlewall");
		rightwallasset = Resources.Load<GameObject>(RoomRootFile + "/rightwall");
		enddoorasset = Resources.Load<GameObject>(RoomRootFile + "/enddoor");
		chestasset = Resources.Load<GameObject>(RoomRootFile + "/closedchest");

		minwidth *= -1;
		minheight *= -1;

		DungeonManager ();

	}
		
	/* ////////// MAKES ROOM ONE ////////// */

	void MakeBase () {

		GameObject baseroom = Instantiate (mainroomlist [0], new Vector3(0, -5, 0), Quaternion.identity) as GameObject;
		baseroom.name = "room_1";

		roomlist.Add (new Room (0, 0, baseroom, 0));
		this.GetComponent<pause_controller> ().currentroom = roomlist [0];

	}
		
	/* ////////// DUNGEON MANAGER ////////// */

	void DungeonManager () {

		SeedInterpreter ();

		string[] uplist = { "climb", "basic", "mono" };
		string[] downlist = { "delve", "basic" };

		MakeBase ();
		int currentlevel = 0;

		if (DungeonType == "mono") {
			MakeRooms (currentlevel, DungeonVal1 [currentlevel], "none");
		}
		else if (uplist.Contains(DungeonType) && downlist.Contains(DungeonType)) {
			MakeRooms (currentlevel, DungeonVal1 [currentlevel], "both");
		}
		else if (uplist.Contains(DungeonType)) {
			MakeRooms (currentlevel, DungeonVal1 [currentlevel], "up");
		}
		else {
			MakeRooms (currentlevel, DungeonVal1 [currentlevel], "down");
		}
		MakeDoors ();
		MakeFloorHazard ();
		MakeChests ();
		SpawnEnemies ();

		PlayerPrefs.SetInt ("NumOfRooms", roomlist.Count);

	}

	void SeedInterpreter () {

		if (DungeonSeed [0] == 'B')
			RoomRootFile = "basicdungeon_rooms";
		
		if (DungeonSeed [1] == 'B')
			DungeonType = "basic";
		if (DungeonSeed [1] == 'C')
			DungeonType = "climb";
		if (DungeonSeed [1] == 'D')
			DungeonType = "delve";
		if (DungeonSeed [1] == 'M')
			DungeonType = "mono";

		string midway = "";

		midway += DungeonSeed [2];
		int upnum = int.Parse(midway);
		DungeonVal1 = new string[upnum];
		midway = "";

		midway += DungeonSeed [3];
		int downnum = int.Parse(midway);
		DungeonVal2 = new string[downnum];
		midway = "";

		midway += DungeonSeed [4];
		maxheight = int.Parse(midway);
		midway = "";

		midway += DungeonSeed [5];
		minheight = int.Parse(midway);
		minheight *= -1;
		midway = "";

		midway += DungeonSeed [6];
		maxwidth = int.Parse(midway);
		midway = "";

		midway += DungeonSeed [7];
		minwidth = int.Parse(midway);
		minwidth *= -1;
		midway = "";

		int looplevel = 0;
		bool seedflip = false;
		string seedstring = "";
		for (int i = 8; i < DungeonSeed.Length; i++) {

			if (DungeonSeed [i] != '_' && !seedflip) {
				if (looplevel >= 0) {
					DungeonVal1 [looplevel] = DungeonVal1 [looplevel] + DungeonSeed [i];
				} else {
					if (looplevel != downnum * -1 - 1) {
						DungeonVal2 [looplevel * -1 - 1] = DungeonVal2 [looplevel * -1 - 1] + DungeonSeed [i];
					} else {
						seedflip = true;
						seedstring += DungeonSeed [i];
						continue;
					}
				}
			} else if (!seedflip) {
				if (looplevel < 0) {
					looplevel -= 1;
				} else if (looplevel == upnum - 1) {
					looplevel = -1;
				} else if (looplevel >= 0) {
					looplevel += 1;
				}
			} else {
				seedstring += DungeonSeed [i];
			}
		}

		seednumber = int.Parse (seedstring);
		seednumber += PlayerPrefs.GetInt ("CrashMod");

		self.GetComponent<dungeon_names> ().GenerateNames (seednumber);
		dungeonname = self.GetComponent<dungeon_names> ().dungeonname;
		PlayerPrefs.SetString ("DungeonName", dungeonname);
		print (dungeonname);

		GameObject.Find ("Main Camera Screen/GameMenu/ItemLog/ItemText").GetComponent<Text> ().text = "You entered the " + dungeonname;

	}
		
	/* ////////// MAKES A FLOOR ////////// */

	void MakeRooms (int currentlevel, string roomkey, string eledirection) {

		Random.seed = seednumber + currentlevel;

		roomtype = "mainrooms";
		int dir = 0;
		Room selroom = roomlist [Random.Range (0, roomlist.Count)];
		GameObject choroom;
		int t = 1;
		GameObject temproom;
		int tn;
		int sn;
		int rotnum;
		bool tempsl;
		bool tempsm;
		bool tempsr;
		int temprot = 0;
		roomchoices = mainroomlist;

		while (t < roomkey.Length) {

			if (roomkey[t] == 'c') {
				roomchoices = connectorlist;
				roomtype = "connectors";
			}
			if (roomkey[t] == 's') {
				roomchoices = stairroomlist;
				roomtype = "stairs";
			}
			if (roomkey [t] == 'e') {
				roomchoices = endroomlist;
				roomtype = "endrooms";
			}
			if (roomkey[t] == 't') {
				roomchoices = treasureroomlist;
				roomtype = "treasurerooms";
			}
			if (roomtype == "treasurerooms") {
				if (ptrlist.Count () != 0) {
					GenerateRandomTR (t);
					t += 1;
					continue;
				}
				else {
					UpdateTreasureroom (null);
				}
			}

			// CHOOSE ROOM
			dir = Random.Range (1, 5);
			while (true) {
				if (t == 1) {
					sn = roomlist.Count - 1;
					selroom = roomlist [sn];
					break;
				}
				sn = Random.Range (0, roomlist.Count);
				if (roomlist [sn].level == currentlevel) {
					selroom = roomlist [sn];
					break;
				}
			}
			choroom = GameObject.Find ("room_" + (sn+1).ToString());
			tn = Random.Range (0, roomchoices.Count);
			temproom = roomchoices [tn] as GameObject;
			if (roomtype == "stairs") {
				while (true) {
					if (((eledirection == "both" && roomkey [t + 1] == 's' || eledirection == "up") && tn % 2 == 0) || ((eledirection == "both" && roomkey [t - 1] == 's' || eledirection == "down") && tn % 2 == 1)) {
						break;
					} else {
						tn = Random.Range (0, roomchoices.Count);
					}
				}
			}
			temproom = roomchoices [tn] as GameObject;
			temprot = 0;

			// ROTATE ROOM
			rotnum = Random.Range(1, 5);
			for (int rn = rotnum; rn < 4; rn++) {

				tempsl = temproom.GetComponent<room_data> ().s_l;
				tempsm = temproom.GetComponent<room_data> ().s_m;
				tempsr = temproom.GetComponent<room_data> ().s_r;
				temproom.GetComponent<room_data> ().s_l = temproom.GetComponent<room_data> ().w_l;
				temproom.GetComponent<room_data> ().s_m = temproom.GetComponent<room_data> ().w_m;
				temproom.GetComponent<room_data> ().s_r = temproom.GetComponent<room_data> ().w_r;
				temproom.GetComponent<room_data> ().w_l = temproom.GetComponent<room_data> ().n_l;
				temproom.GetComponent<room_data> ().w_m = temproom.GetComponent<room_data> ().n_m;
				temproom.GetComponent<room_data> ().w_r = temproom.GetComponent<room_data> ().n_r;
				temproom.GetComponent<room_data> ().n_l = temproom.GetComponent<room_data> ().e_l;
				temproom.GetComponent<room_data> ().n_m = temproom.GetComponent<room_data> ().e_m;
				temproom.GetComponent<room_data> ().n_r = temproom.GetComponent<room_data> ().e_r;
				temproom.GetComponent<room_data> ().e_l = tempsl;
				temproom.GetComponent<room_data> ().e_m = tempsm;
				temproom.GetComponent<room_data> ().e_r = tempsr;
				temprot -= 90;

			}

			// TEST & PLACE ROOM

			// up
			if (dir == 1) {

				if (currentlevel == selroom.level && selroom.y + 1 <= maxheight && !OverlapCheck ("up", selroom) && (roomtype != "connectors" || ConnectionRespectsBoundary ("up", temproom, selroom, t)) && IsRoomPossible (temproom, choroom, "up") && !(DisruptsConnections (selroom, temproom, "up"))) {
					if (roomtype == "connectors") {
						if (!(ConnectsToRoom (selroom.x, selroom.y + 1, currentlevel)))
							AddPTR (selroom.x, selroom.y + 1, recentdirection, currentlevel);
					}
					t += 1;
					crashcount = 0;
					roomlist.Add (new Room (selroom.x, selroom.y + 1, temproom, currentlevel));
					GameObject obj = Instantiate (roomlist [roomlist.Count - 1].r, new Vector3 (-20 * roomlist [roomlist.Count - 1].x, -5 + 11 * currentlevel, -20 * roomlist [roomlist.Count - 1].y), Quaternion.Euler (0, temprot % 360, 0)) as GameObject;
					obj.name = "room_" + roomlist.Count;
					DeleteOverlappedPTR ("up", selroom);
				} else
					crashcount += 1;

			}

			// down
			else if (dir == 2) {

				if (currentlevel == selroom.level && selroom.y - 1 >= minheight && !OverlapCheck ("down", selroom) && (roomtype != "connectors" || ConnectionRespectsBoundary ("down", temproom, selroom, t)) && IsRoomPossible (temproom, choroom, "down") && !(DisruptsConnections (selroom, temproom, "down"))) {
					if (roomtype == "connectors") {
						if (!(ConnectsToRoom (selroom.x, selroom.y - 1, currentlevel)))
							AddPTR (selroom.x, selroom.y - 1, recentdirection, currentlevel);
					}
					t += 1;
					crashcount = 0;
					roomlist.Add (new Room (selroom.x, selroom.y - 1, temproom, currentlevel));
					GameObject obj = Instantiate (roomlist [roomlist.Count - 1].r, new Vector3 (-20 * roomlist [roomlist.Count - 1].x, -5 + 11 * currentlevel, -20 * roomlist [roomlist.Count - 1].y), Quaternion.Euler (0, temprot % 360, 0)) as GameObject;
					obj.name = "room_" + roomlist.Count;
					DeleteOverlappedPTR ("down", selroom);
				} else
					crashcount += 1;

			}

			// left
			else if (dir == 3) {
		
				if (currentlevel == selroom.level && selroom.x - 1 >= minwidth && !OverlapCheck ("left", selroom) && (roomtype != "connectors" || ConnectionRespectsBoundary ("left", temproom, selroom, t)) && IsRoomPossible (temproom, choroom, "left") && !(DisruptsConnections (selroom, temproom, "left"))) {
					if (roomtype == "connectors") {
						if (!(ConnectsToRoom (selroom.x - 1, selroom.y, currentlevel)))
							AddPTR (selroom.x - 1, selroom.y, recentdirection, currentlevel);
					}
					t += 1;
					crashcount = 0;
					roomlist.Add (new Room (selroom.x - 1, selroom.y, temproom, currentlevel));
					GameObject obj = Instantiate (roomlist [roomlist.Count - 1].r, new Vector3 (-20 * roomlist [roomlist.Count - 1].x, -5 + 11 * currentlevel, -20 * roomlist [roomlist.Count - 1].y), Quaternion.Euler (0, temprot % 360, 0)) as GameObject;
					obj.name = "room_" + roomlist.Count;
					DeleteOverlappedPTR ("left", selroom);
				} else
					crashcount += 1;
			
			}

			// right
			else if (dir == 4) {
				
				if (currentlevel == selroom.level && selroom.x + 1 <= maxwidth && !OverlapCheck ("right", selroom) && (roomtype != "connectors" || ConnectionRespectsBoundary ("right", temproom, selroom, t)) && IsRoomPossible (temproom, choroom, "right") && !(DisruptsConnections (selroom, temproom, "right"))) {
					if (roomtype == "connectors") {
						if (!(ConnectsToRoom (selroom.x + 1, selroom.y, currentlevel)))
							AddPTR (selroom.x + 1, selroom.y, recentdirection, currentlevel);
					}
					t += 1;
					crashcount = 0;
					roomlist.Add (new Room (selroom.x + 1, selroom.y, temproom, currentlevel));
					GameObject obj = Instantiate (roomlist [roomlist.Count - 1].r, new Vector3 (-20 * roomlist [roomlist.Count - 1].x, -5 + 11 * currentlevel, -20 * roomlist [roomlist.Count - 1].y), Quaternion.Euler (0, temprot % 360, 0)) as GameObject;
					obj.name = "room_" + roomlist.Count;
					DeleteOverlappedPTR ("right", selroom);
				} else
					crashcount += 1;
					
			}
				
			// ROTATES THE PREFAB BACK TO ITS ORIGINAL ROTATION
			for (int rn = rotnum; rn > 0; rn--) {

				tempsl = temproom.GetComponent<room_data> ().s_l;
				tempsm = temproom.GetComponent<room_data> ().s_m;
				tempsr = temproom.GetComponent<room_data> ().s_r;
				temproom.GetComponent<room_data> ().s_l = temproom.GetComponent<room_data> ().w_l;
				temproom.GetComponent<room_data> ().s_m = temproom.GetComponent<room_data> ().w_m;
				temproom.GetComponent<room_data> ().s_r = temproom.GetComponent<room_data> ().w_r;
				temproom.GetComponent<room_data> ().w_l = temproom.GetComponent<room_data> ().n_l;
				temproom.GetComponent<room_data> ().w_m = temproom.GetComponent<room_data> ().n_m;
				temproom.GetComponent<room_data> ().w_r = temproom.GetComponent<room_data> ().n_r;
				temproom.GetComponent<room_data> ().n_l = temproom.GetComponent<room_data> ().e_l;
				temproom.GetComponent<room_data> ().n_m = temproom.GetComponent<room_data> ().e_m;
				temproom.GetComponent<room_data> ().n_r = temproom.GetComponent<room_data> ().e_r;
				temproom.GetComponent<room_data> ().e_l = tempsl;
				temproom.GetComponent<room_data> ().e_m = tempsm;
				temproom.GetComponent<room_data> ().e_r = tempsr;
				temprot -= 90;

			}

			if (roomtype == "stairs" && crashcount == 0) {
				print (roomkey [t]);
				if (eledirection == "both" && roomkey [t] == 's' || eledirection == "up") {
					CreateStairHalf (roomlist [roomlist.Count - 1], 1, rotnum, t);
					MakeRooms (currentlevel + 1, DungeonVal1 [currentlevel + 1], "up");
				} else {
					CreateStairHalf (roomlist [roomlist.Count - 1], -1, rotnum, t);
					MakeRooms (currentlevel - 1, DungeonVal2 [currentlevel * -1], "down");
				}
			}

			if (crashcount >= 50) {
				crashcount = 0;
				PlayerPrefs.SetInt ("CrashMod", PlayerPrefs.GetInt ("CrashMod") + 1);
				SceneManager.LoadScene ("Game");
			}

		}

	}

	void MakeDoors () {

		int side;
		List<Possibledoor> doorlist = new List<Possibledoor> ();

		foreach (Room room1 in roomlist) {
			foreach (Room room2 in roomlist) {
				if (room1.x == room2.x && room1.y + 1 == room2.y && room1.level == room2.level) {
					doorlist.Add (new Possibledoor (GameObject.Find ("room_" + (roomlist.IndexOf (room1) + 1).ToString ()), GameObject.Find ("room_" + (roomlist.IndexOf (room2) + 1).ToString ()), "up"));
				}
				if (room1.x == room2.x && room1.y - 1 == room2.y && room1.level == room2.level) {
					doorlist.Add (new Possibledoor (GameObject.Find ("room_" + (roomlist.IndexOf (room1) + 1).ToString ()), GameObject.Find ("room_" + (roomlist.IndexOf (room2) + 1).ToString ()), "down"));
				}
				if (room1.x - 1 == room2.x && room1.y == room2.y && room1.level == room2.level) {
					doorlist.Add (new Possibledoor (GameObject.Find ("room_" + (roomlist.IndexOf (room1) + 1).ToString ()), GameObject.Find ("room_" + (roomlist.IndexOf (room2) + 1).ToString ()), "left"));
				}
				if (room1.x + 1 == room2.x && room1.y == room2.y && room1.level == room2.level) {
					doorlist.Add (new Possibledoor (GameObject.Find ("room_" + (roomlist.IndexOf (room1) + 1).ToString ()), GameObject.Find ("room_" + (roomlist.IndexOf (room2) + 1).ToString ()), "right"));
				}
			}
		}

		print (doorlist.Count);

		bool hasdoor = false;

		foreach (Possibledoor curdoor in doorlist) {

			string walltype = "";

			if (curdoor.direction == "up") {
				foreach (Transform t in curdoor.secondroom.transform) {
					if (t.name == "Downwall (Middle)") {
						hasdoor = true;
						walltype = "middle";
					}
				}
				if (!hasdoor && (curdoor.firstroom.GetComponent<room_data> ().n_l && curdoor.secondroom.GetComponent<room_data> ().s_r || curdoor.firstroom.GetComponent<room_data> ().n_m && curdoor.secondroom.GetComponent<room_data> ().s_m || curdoor.firstroom.GetComponent<room_data> ().n_r && curdoor.secondroom.GetComponent<room_data> ().s_l)) {
					GameObject door = Instantiate (doorasset, new Vector3 (0, 5, 0), Quaternion.identity);
					door.name = "Updoor";
					door.transform.parent = curdoor.firstroom.transform;
					if (curdoor.firstroom.GetComponent<room_data> ().n_m && curdoor.secondroom.GetComponent<room_data> ().s_m) {
						door.transform.position = new Vector3 (door.transform.parent.position.x, door.transform.parent.position.y, door.transform.parent.position.z - 10);
						CreateWall (0, "Upwall (Middle)", middlewallasset, curdoor.firstroom.gameObject, new Vector3(door.transform.parent.position.x, door.transform.parent.position.y, door.transform.parent.position.z - 10));
					} else if (curdoor.firstroom.GetComponent<room_data> ().n_l && curdoor.secondroom.GetComponent<room_data> ().s_r && curdoor.firstroom.GetComponent<room_data> ().n_r && curdoor.secondroom.GetComponent<room_data> ().s_l) {
						side = Random.Range (1, 3);
						if (side == 0) {
							door.transform.position = new Vector3 (door.transform.parent.position.x + 4.5f, door.transform.parent.position.y, door.transform.parent.position.z - 10);
						} else {
							door.transform.position = new Vector3 (door.transform.parent.position.x - 4.5f, door.transform.parent.position.y, door.transform.parent.position.z - 10);
						}
					} else if (curdoor.firstroom.GetComponent<room_data> ().n_l && curdoor.secondroom.GetComponent<room_data> ().s_r) {
						door.transform.position = new Vector3 (door.transform.parent.position.x + 4.5f, door.transform.parent.position.y, door.transform.parent.position.z - 10);
					} else {
						door.transform.position = new Vector3 (door.transform.parent.position.x - 4.5f, door.transform.parent.position.y, door.transform.parent.position.z - 10);
					}
				}
				else if (hasdoor && (curdoor.firstroom.GetComponent<room_data> ().n_l && curdoor.secondroom.GetComponent<room_data> ().s_r || curdoor.firstroom.GetComponent<room_data> ().n_m && curdoor.secondroom.GetComponent<room_data> ().s_m || curdoor.firstroom.GetComponent<room_data> ().n_r && curdoor.secondroom.GetComponent<room_data> ().s_l)) {
					GameObject door = Instantiate (doorasset, new Vector3 (0, 5, 0), Quaternion.identity);
					door.name = "Updoor";
					door.transform.parent = curdoor.firstroom.transform;

					if (walltype == "middle") {
						door.transform.position = new Vector3 (door.transform.parent.position.x, door.transform.parent.position.y, door.transform.parent.position.z - 10);
						CreateWall (0, "Upwall (Middle)", middlewallasset, curdoor.firstroom.gameObject, new Vector3(door.transform.parent.position.x, door.transform.parent.position.y, door.transform.parent.position.z - 10));
					}
				}
			}

			if (curdoor.direction == "down") {
				foreach (Transform t in curdoor.secondroom.transform) {
					if (t.name == "Upwall (Middle)") {
						hasdoor = true;
						walltype = "middle";
					}
				}
				if (!hasdoor && (curdoor.firstroom.GetComponent<room_data> ().s_l && curdoor.secondroom.GetComponent<room_data> ().n_r || curdoor.firstroom.GetComponent<room_data> ().s_m && curdoor.secondroom.GetComponent<room_data> ().n_m || curdoor.firstroom.GetComponent<room_data> ().s_r && curdoor.secondroom.GetComponent<room_data> ().n_l)) {
					GameObject door = Instantiate (doorasset, new Vector3 (0, 5, 0), Quaternion.identity);
					door.name = "Downdoor";
					door.transform.parent = curdoor.firstroom.transform;
					if (curdoor.firstroom.GetComponent<room_data> ().s_m && curdoor.secondroom.GetComponent<room_data> ().n_m) {
						door.transform.position = new Vector3 (door.transform.parent.position.x, door.transform.parent.position.y, door.transform.parent.position.z + 10);
						CreateWall (180, "Downwall (Middle)", middlewallasset, curdoor.firstroom.gameObject, new Vector3(door.transform.parent.position.x, door.transform.parent.position.y, door.transform.parent.position.z + 10));
					} else if (curdoor.firstroom.GetComponent<room_data> ().s_l && curdoor.secondroom.GetComponent<room_data> ().n_r && curdoor.firstroom.GetComponent<room_data> ().s_r && curdoor.secondroom.GetComponent<room_data> ().n_l) {
						side = Random.Range (1, 3);
						if (side == 0) {
							door.transform.position = new Vector3 (door.transform.parent.position.x - 4.5f, door.transform.parent.position.y, door.transform.parent.position.z + 10);
						} else {
							door.transform.position = new Vector3 (door.transform.parent.position.x + 4.5f, door.transform.parent.position.y, door.transform.parent.position.z + 10);
						}
					} else if (curdoor.firstroom.GetComponent<room_data> ().s_l && curdoor.secondroom.GetComponent<room_data> ().n_r) {
						door.transform.position = new Vector3 (door.transform.parent.position.x - 4.5f, door.transform.parent.position.y, door.transform.parent.position.z + 10);
					} else {
						door.transform.position = new Vector3 (door.transform.parent.position.x + 4.5f, door.transform.parent.position.y, door.transform.parent.position.z + 10);
					}
				}
				else if (hasdoor && (curdoor.firstroom.GetComponent<room_data> ().s_l && curdoor.secondroom.GetComponent<room_data> ().n_r || curdoor.firstroom.GetComponent<room_data> ().s_m && curdoor.secondroom.GetComponent<room_data> ().n_m || curdoor.firstroom.GetComponent<room_data> ().s_r && curdoor.secondroom.GetComponent<room_data> ().n_l)) {
					GameObject door = Instantiate (doorasset, new Vector3 (0, 5, 0), Quaternion.identity);
					door.name = "Downdoor";
					door.transform.parent = curdoor.firstroom.transform;
				
					if (walltype == "middle") {
						door.transform.position = new Vector3 (door.transform.parent.position.x, door.transform.parent.position.y, door.transform.parent.position.z + 10);
						CreateWall (180, "Downwall (Middle)", middlewallasset, curdoor.firstroom.gameObject, new Vector3(door.transform.parent.position.x, door.transform.parent.position.y, door.transform.parent.position.z + 10));
					}
				}
			}

			if (curdoor.direction == "left") {
				foreach (Transform t in curdoor.secondroom.transform) {
					if (t.name == "Rightwall (Middle)") {
						hasdoor = true;
						walltype = "middle";
					}
				}
				if (!hasdoor && (curdoor.firstroom.GetComponent<room_data> ().w_l && curdoor.secondroom.GetComponent<room_data> ().e_r || curdoor.firstroom.GetComponent<room_data> ().w_m && curdoor.secondroom.GetComponent<room_data> ().e_m || curdoor.firstroom.GetComponent<room_data> ().w_r && curdoor.secondroom.GetComponent<room_data> ().e_l)) {
					GameObject door = Instantiate (doorasset, new Vector3 (0, 5, 0), Quaternion.identity);
					door.name = "Leftdoor";
					door.transform.parent = curdoor.firstroom.transform;
					door.transform.rotation = Quaternion.Euler (0, 90, 0);
					if (curdoor.firstroom.GetComponent<room_data> ().w_m && curdoor.secondroom.GetComponent<room_data> ().e_m) {
						door.transform.position = new Vector3 (door.transform.parent.position.x + 10, door.transform.parent.position.y, door.transform.parent.position.z);
						CreateWall (270, "Leftwall (Middle)", middlewallasset, curdoor.firstroom.gameObject, new Vector3(door.transform.parent.position.x + 10, door.transform.parent.position.y, door.transform.parent.position.z));
					} else if (curdoor.firstroom.GetComponent<room_data> ().w_l && curdoor.secondroom.GetComponent<room_data> ().e_r && curdoor.firstroom.GetComponent<room_data> ().w_r && curdoor.secondroom.GetComponent<room_data> ().e_l) {
						side = Random.Range (1, 3);
						if (side == 0) {
							door.transform.position = new Vector3 (door.transform.parent.position.x + 10, door.transform.parent.position.y, door.transform.parent.position.z + 4.5f);
						} else {
							door.transform.position = new Vector3 (door.transform.parent.position.x + 10, door.transform.parent.position.y, door.transform.parent.position.z - 4.5f);
						}
					} else if (curdoor.firstroom.GetComponent<room_data> ().w_l && curdoor.secondroom.GetComponent<room_data> ().e_r) {
						door.transform.position = new Vector3 (door.transform.parent.position.x + 10, door.transform.parent.position.y, door.transform.parent.position.z + 4.5f);
					} else {
						door.transform.position = new Vector3 (door.transform.parent.position.x + 10, door.transform.parent.position.y, door.transform.parent.position.z - 4.5f);
					}
				}
				else if (hasdoor && (curdoor.firstroom.GetComponent<room_data> ().w_l && curdoor.secondroom.GetComponent<room_data> ().e_r || curdoor.firstroom.GetComponent<room_data> ().w_m && curdoor.secondroom.GetComponent<room_data> ().e_m || curdoor.firstroom.GetComponent<room_data> ().w_r && curdoor.secondroom.GetComponent<room_data> ().e_l)) {
					GameObject door = Instantiate (doorasset, new Vector3 (0, 5, 0), Quaternion.identity);
					door.name = "Leftdoor";
					door.transform.parent = curdoor.firstroom.transform;
					door.transform.rotation = Quaternion.Euler (0, 90, 0);
						
					if (walltype == "middle") {
						door.transform.position = new Vector3 (door.transform.parent.position.x + 10, door.transform.parent.position.y, door.transform.parent.position.z);
						CreateWall (270, "Leftwall (Middle)", middlewallasset, curdoor.firstroom.gameObject, new Vector3(door.transform.parent.position.x + 10, door.transform.parent.position.y, door.transform.parent.position.z));
					}
				}
			}

			if (curdoor.direction == "right") {
				foreach (Transform t in curdoor.secondroom.transform) {
					if (t.name == "Leftwall (Middle)") {
						hasdoor = true;
						walltype = "middle";
					}
				}
				if (!hasdoor && (curdoor.firstroom.GetComponent<room_data> ().e_l && curdoor.secondroom.GetComponent<room_data> ().w_r || curdoor.firstroom.GetComponent<room_data> ().e_m && curdoor.secondroom.GetComponent<room_data> ().w_m || curdoor.firstroom.GetComponent<room_data> ().e_r && curdoor.secondroom.GetComponent<room_data> ().w_l)) {
					GameObject door = Instantiate (doorasset, new Vector3 (0, 5, 0), Quaternion.identity);
					door.name = "Rightdoor";
					door.transform.parent = curdoor.firstroom.transform;
					door.transform.rotation = Quaternion.Euler (0, 90, 0);
					if (curdoor.firstroom.GetComponent<room_data> ().e_m && curdoor.secondroom.GetComponent<room_data> ().w_m) {
						door.transform.position = new Vector3 (door.transform.parent.position.x - 10, door.transform.parent.position.y, door.transform.parent.position.z);
						CreateWall (90, "Rightwall (Middle)", middlewallasset, curdoor.firstroom.gameObject, new Vector3(door.transform.parent.position.x - 10, door.transform.parent.position.y, door.transform.parent.position.z));
					} else if (curdoor.firstroom.GetComponent<room_data> ().e_l && curdoor.secondroom.GetComponent<room_data> ().w_r && curdoor.firstroom.GetComponent<room_data> ().e_r && curdoor.secondroom.GetComponent<room_data> ().w_l) {
						side = Random.Range (1, 3);
						if (side == 0) {
							door.transform.position = new Vector3 (door.transform.parent.position.x - 10, door.transform.parent.position.y, door.transform.parent.position.z - 4.5f);
						} else {
							door.transform.position = new Vector3 (door.transform.parent.position.x - 10, door.transform.parent.position.y, door.transform.parent.position.z + 4.5f);
						}
					} else if (curdoor.firstroom.GetComponent<room_data> ().e_l && curdoor.secondroom.GetComponent<room_data> ().w_r) {
						door.transform.position = new Vector3 (door.transform.parent.position.x - 10, door.transform.parent.position.y, door.transform.parent.position.z - 4.5f);
					} else {
						door.transform.position = new Vector3 (door.transform.parent.position.x - 10, door.transform.parent.position.y, door.transform.parent.position.z + 4.5f);
					}
				}
				else if (hasdoor && (curdoor.firstroom.GetComponent<room_data> ().e_l && curdoor.secondroom.GetComponent<room_data> ().w_r || curdoor.firstroom.GetComponent<room_data> ().e_m && curdoor.secondroom.GetComponent<room_data> ().w_m || curdoor.firstroom.GetComponent<room_data> ().e_r && curdoor.secondroom.GetComponent<room_data> ().w_l)) {
					GameObject door = Instantiate (doorasset, new Vector3 (0, 5, 0), Quaternion.identity);
					door.name = "Rightdoor";
					door.transform.parent = curdoor.firstroom.transform;
					door.transform.rotation = Quaternion.Euler (0, 90, 0);

					if (walltype == "middle") {
						door.transform.position = new Vector3 (door.transform.parent.position.x - 10, door.transform.parent.position.y, door.transform.parent.position.z);
						CreateWall (90, "Rightwall (Middle)", middlewallasset, curdoor.firstroom.gameObject, new Vector3(door.transform.parent.position.x - 10, door.transform.parent.position.y, door.transform.parent.position.z));
					}
				}
			}

			if (curdoor.firstroom.GetComponent<room_data> ().end && hasdoor) {

				foreach (Transform t in curdoor.firstroom.transform) {
					if (t.name == "Updoor") {
						GameObject enddoor = Instantiate(enddoorasset, new Vector3 (curdoor.secondroom.transform.position.x, curdoor.secondroom.transform.position.y + 1.5f, curdoor.secondroom.transform.position.z + 9), Quaternion.identity);
						enddoor.transform.parent = curdoor.secondroom.transform;
						//enddoor.transform.localPosition = new Vector3 (9, 1.5f, 0);
					}
					if (t.name == "Downdoor") {
						GameObject enddoor = Instantiate(enddoorasset, new Vector3 (curdoor.secondroom.transform.position.x, curdoor.secondroom.transform.position.y + 1.5f, curdoor.secondroom.transform.position.z - 9), Quaternion.identity);
						enddoor.transform.parent = curdoor.secondroom.transform;
						//enddoor.transform.localPosition = new Vector3 (0, 1.5f, -9);
					}
					if (t.name == "Leftdoor") {
						GameObject enddoor = Instantiate(enddoorasset, new Vector3 (curdoor.secondroom.transform.position.x - 9, curdoor.secondroom.transform.position.y + 1.5f, curdoor.secondroom.transform.position.z), Quaternion.identity);
						enddoor.transform.eulerAngles = new Vector3 (0, 90, 0);
						enddoor.transform.parent = curdoor.secondroom.transform;
						//enddoor.transform.localPosition = new Vector3 (-9, 1.5f, 0);
					}
					if (t.name == "Rightdoor") {
						GameObject enddoor = Instantiate(enddoorasset, new Vector3 (curdoor.secondroom.transform.position.x + 9, curdoor.secondroom.transform.position.y + 1.5f, curdoor.secondroom.transform.position.z), Quaternion.identity);
						enddoor.transform.eulerAngles = new Vector3 (0, 90, 0);
						enddoor.transform.parent = curdoor.secondroom.transform;
						//enddoor.transform.localPosition = new Vector3 (-9, 1.5f, 0);
					}
				}

			}

			hasdoor = false;

		}

		for (int n = 0; n < roomlist.Count; n++) {

			bool u = false;
			bool d = false;
			bool l = false;
			bool r = false;
			GameObject room = GameObject.Find (("room_" + (n + 1).ToString()));

			for (int i = 0; i < room.gameObject.transform.childCount; i++) {
				if (room.gameObject.transform.GetChild (i).name == "Updoor")
					u = true;
				if (room.gameObject.transform.GetChild (i).name == "Downdoor")
					d = true;
				if (room.gameObject.transform.GetChild (i).name == "Leftdoor")
					l = true;
				if (room.gameObject.transform.GetChild (i).name == "Rightdoor")
					r = true;
			}

			if (!u) {
				CreateWall(0, "Upwall", fullwallasset, room, new Vector3 (room.transform.position.x, room.transform.position.y, room.transform.position.z - 9));
			}
			if (!d) {
				CreateWall(180, "Downwall", fullwallasset, room, new Vector3 (room.transform.position.x, room.transform.position.y, room.transform.position.z + 9));
			}
			if (!l) {
				CreateWall(270, "Leftwall", fullwallasset, room, new Vector3 (room.transform.position.x + 9, room.transform.position.y, room.transform.position.z));
			}
			if (!r) {
				CreateWall(90, "Rightwall", fullwallasset, room, new Vector3 (room.transform.position.x - 9, room.transform.position.y, room.transform.position.z));
			}

		}

	}

	bool IsRoomPossible (GameObject temproom, GameObject choroom, string direction) {

		if (direction == "up") {
			if (temproom.GetComponent<room_data> ().s_l && choroom.GetComponent<room_data> ().n_r || temproom.GetComponent<room_data> ().s_m && choroom.GetComponent<room_data> ().n_m || temproom.GetComponent<room_data> ().s_r && choroom.GetComponent<room_data> ().n_l)
				return true;
		}

		if (direction == "down") {
			if (temproom.GetComponent<room_data> ().n_l && choroom.GetComponent<room_data> ().s_r || temproom.GetComponent<room_data> ().n_m && choroom.GetComponent<room_data> ().s_m || temproom.GetComponent<room_data> ().n_r && choroom.GetComponent<room_data> ().s_l)
				return true;
		}
		
		if (direction == "left") {
			if (temproom.GetComponent<room_data> ().e_l && choroom.GetComponent<room_data> ().w_r || temproom.GetComponent<room_data> ().e_m && choroom.GetComponent<room_data> ().w_m || temproom.GetComponent<room_data> ().e_r && choroom.GetComponent<room_data> ().w_l)
				return true;
		}

		if (direction == "right") {
			if (temproom.GetComponent<room_data> ().w_l && choroom.GetComponent<room_data> ().e_r || temproom.GetComponent<room_data> ().w_m && choroom.GetComponent<room_data> ().e_m || temproom.GetComponent<room_data> ().w_r && choroom.GetComponent<room_data> ().e_l)
				return true;
		}

		return false;

	}

	// Checks if a possible room overlaps an existing room
	bool OverlapCheck (string direction, Room selroom) {

		bool ovr = false;

		if (direction == "up") {
			foreach (Room curroom in roomlist) {
				if (selroom.x == curroom.x && selroom.y + 1 == curroom.y && selroom.level == curroom.level) {
					ovr = true;
				}
			}
		}

		if (direction == "down") {
			foreach (Room curroom in roomlist) {
				if (selroom.x == curroom.x && selroom.y - 1 == curroom.y && selroom.level == curroom.level) {
					ovr = true;
				}
			}
		}

		if (direction == "left") {
			foreach (Room curroom in roomlist) {
				if (selroom.x - 1 == curroom.x && selroom.y == curroom.y && selroom.level == curroom.level) {
					ovr = true;
				}
			}	
		}

		if (direction == "right") {
			foreach (Room curroom in roomlist) {
				if (selroom.x + 1 == curroom.x && selroom.y == curroom.y && selroom.level == curroom.level) {
					ovr = true;
				}
			}
		}

		return ovr;

	}

	bool ConnectionRespectsBoundary (string direction, GameObject roomdata, Room coordinates, int rn) {

		bool isRespectful = false;
		Room uproom = null;
		Room downroom = null;
		Room leftroom = null;
		Room rightroom = null;
		GameObject upind = GameObject.Find("Nullroom");
		GameObject downind = GameObject.Find("Nullroom");
		GameObject leftind = GameObject.Find("Nullroom");
		GameObject rightind = GameObject.Find("Nullroom");

		string opposite = "";

		if (roomtype == "connectors") {
			
			if (direction == "up") {
				coordinates.y += 1;
				opposite = "down";
			}
			if (direction == "down") {
				coordinates.y -= 1;
				opposite = "up";
			}
			if (direction == "left") {
				coordinates.x -= 1;
				opposite = "right";
			}
			if (direction == "right") {
				coordinates.x += 1;
				opposite = "left";
			}

			foreach (Room curroom in roomlist) {
				if (coordinates.x == curroom.x && coordinates.y + 1 == curroom.y && coordinates.level == curroom.level) {
					uproom = curroom;
					upind = GameObject.Find ("room_" + (roomlist.IndexOf (curroom) + 1).ToString ());
				}
				if (coordinates.x == curroom.x && coordinates.y - 1 == curroom.y && coordinates.level == curroom.level) {
					downroom = curroom;
					downind = GameObject.Find ("room_" + (roomlist.IndexOf (curroom) + 1).ToString ());
				}
				if (coordinates.x - 1 == curroom.x && coordinates.y == curroom.y && coordinates.level == curroom.level) {
					leftroom = curroom;
					leftind = GameObject.Find ("room_" + (roomlist.IndexOf (curroom) + 1).ToString ());
				}
				if (coordinates.x + 1 == curroom.x && coordinates.y == curroom.y && coordinates.level == curroom.level) {
					rightroom = curroom;
					rightind = GameObject.Find ("room_" + (roomlist.IndexOf (curroom) + 1).ToString ());
				}
			}

			if (opposite != "up" && isRespectful == false) {
				if ((roomdata.GetComponent<room_data> ().n_l || roomdata.GetComponent<room_data> ().n_m || roomdata.GetComponent<room_data> ().n_r) && coordinates.y + 1 <= maxheight && (uproom == null || ((roomdata.GetComponent<room_data> ().n_l && upind.GetComponent<room_data> ().s_r) || (roomdata.GetComponent<room_data> ().n_m && upind.GetComponent<room_data> ().s_m) || (roomdata.GetComponent<room_data> ().n_r && upind.GetComponent<room_data> ().s_l)))) {
					isRespectful = true;
					recentdirection = "up";
					//AddPTR (coordinates.x, coordinates.y, "up");
				}
			}

			if (opposite != "down" && isRespectful == false) {
				if ((roomdata.GetComponent<room_data> ().s_l || roomdata.GetComponent<room_data> ().s_m || roomdata.GetComponent<room_data> ().s_r) && coordinates.y - 1 >= minheight && (downroom == null || ((roomdata.GetComponent<room_data> ().s_l && downind.GetComponent<room_data> ().n_r) || (roomdata.GetComponent<room_data> ().s_m && downind.GetComponent<room_data> ().n_m) || (roomdata.GetComponent<room_data> ().s_r && downind.GetComponent<room_data> ().n_l)))) {
					isRespectful = true;
					recentdirection = "down";
					//AddPTR (coordinates.x, coordinates.y, "down");
				}
			}

			if (opposite != "left" && isRespectful == false) {
				if ((roomdata.GetComponent<room_data> ().w_l || roomdata.GetComponent<room_data> ().w_m || roomdata.GetComponent<room_data> ().w_r) && coordinates.x - 1 >= minwidth && (leftroom == null || ((roomdata.GetComponent<room_data> ().w_l && leftind.GetComponent<room_data> ().e_r) || (roomdata.GetComponent<room_data> ().w_m && leftind.GetComponent<room_data> ().e_m) || (roomdata.GetComponent<room_data> ().w_r && leftind.GetComponent<room_data> ().e_l)))) {
					isRespectful = true;
					recentdirection = "left";
					//AddPTR (coordinates.x, coordinates.y, "left");
				}
			}

			if (opposite != "right" && isRespectful == false) {
				if ((roomdata.GetComponent<room_data> ().e_l || roomdata.GetComponent<room_data> ().e_m || roomdata.GetComponent<room_data> ().e_r) && coordinates.x + 1 <= maxwidth && (rightroom == null || ((roomdata.GetComponent<room_data> ().e_l && rightind.GetComponent<room_data> ().w_r) || (roomdata.GetComponent<room_data> ().e_m && rightind.GetComponent<room_data> ().w_m) || (roomdata.GetComponent<room_data> ().e_r && rightind.GetComponent<room_data> ().w_l)))) {
					isRespectful = true;
					recentdirection = "right";
					//AddPTR (coordinates.x, coordinates.y, "right");
				}
			}

			if (direction == "up") {
				coordinates.y -= 1;
			}
			if (direction == "down") {
				coordinates.y += 1;
			}
			if (direction == "left") {
				coordinates.x += 1;
			}
			if (direction == "right") {
				coordinates.x -= 1;
			}
		}
			
		return isRespectful;

	}

	// Note: Use x and y of the connector, not the possible treasure room
	void AddPTR (int x, int y, string direction, int currentlevel) {

		List<PTR> tempptrlist = new List<PTR>();
		bool ovr = false;

		if (ptrlist.Count () != 0) {
			foreach (PTR curptr in ptrlist) {
				if (curptr.x == x && curptr.y == y + 1 && direction == "up") {
					if (curptr.connections.Length >= 2)
						tempptrlist.Add (new PTR (x, y + 1, "fourway", curptr.connections + "d", currentlevel));
					else if (curptr.connections == "u")
						tempptrlist.Add (new PTR (x, y + 1, "across", curptr.connections + "d", currentlevel));
					else
						tempptrlist.Add (new PTR (x, y + 1, "corner", curptr.connections + "d", currentlevel));
					ovr = true;
					continue;
				}
				else if (curptr.x == x && curptr.y == y - 1 && direction == "down") {
					if (curptr.connections.Length >= 2)
						tempptrlist.Add (new PTR (x, y - 1, "fourway", curptr.connections + "u", currentlevel));
					else if (curptr.connections == "d")
						tempptrlist.Add (new PTR (x, y - 1, "across", curptr.connections + "u", currentlevel));
					else
						tempptrlist.Add (new PTR (x, y - 1, "corner", curptr.connections + "u", currentlevel));
					ovr = true;
					continue;
				}
				else if (curptr.x == x - 1 && curptr.y == y && direction == "left") {
					if (curptr.connections.Length >= 2)
						tempptrlist.Add (new PTR (x - 1, y, "fourway", curptr.connections + "r", currentlevel));
					else if (curptr.connections == "l")
						tempptrlist.Add (new PTR (x - 1, y, "across", curptr.connections + "r", currentlevel));
					else
						tempptrlist.Add (new PTR (x - 1, y, "corner", curptr.connections + "r", currentlevel));
					ovr = true;
					continue;
				}
				else if (curptr.x == x + 1 && curptr.y == y && direction == "right") {
					if (curptr.connections.Length >= 2)
						tempptrlist.Add (new PTR (x + 1, y, "fourway", curptr.connections + "l", currentlevel));
					else if (curptr.connections == "r")
						tempptrlist.Add (new PTR (x + 1, y, "across", curptr.connections + "l", currentlevel));
					else
						tempptrlist.Add (new PTR (x + 1, y, "corner", curptr.connections + "l", currentlevel));
					ovr = true;
					continue;
				}
				else {
					tempptrlist.Add (curptr);
					if (ptrlist.IndexOf (curptr) == ptrlist.Count () - 1 && !(ovr)) {
						if (direction == "up")
							tempptrlist.Add (new PTR (x, y + 1, "one", "d", currentlevel));
						if (direction == "down")
							tempptrlist.Add (new PTR (x, y - 1, "one", "u", currentlevel));
						if (direction == "left")
							tempptrlist.Add (new PTR (x - 1, y, "one", "r", currentlevel));
						if (direction == "right")
							tempptrlist.Add (new PTR (x + 1, y, "one", "l", currentlevel));
					}
				}
			}
		}
		else {
			if (direction == "up")
				tempptrlist.Add (new PTR (x, y + 1, "one", "d", currentlevel));
			if (direction == "down")
				tempptrlist.Add (new PTR (x, y - 1, "one", "u", currentlevel));
			if (direction == "left")
				tempptrlist.Add (new PTR (x - 1, y, "one", "r", currentlevel));
			if (direction == "right")
				tempptrlist.Add (new PTR (x + 1, y, "one", "l", currentlevel));
		}
		ptrlist = tempptrlist;
	}

	void DeleteOverlappedPTR (string direction, Room selroom) {

		int xmove = 0;
		int ymove = 0;

		if (direction == "up")
			ymove = 1;
		if (direction == "down")
			ymove = -1;
		if (direction == "left")
			xmove = -1;
		if (direction == "right")
			xmove = 1;

		List<PTR> tempptrlist = new List<PTR> ();

		foreach (PTR curptr in ptrlist) {
			if (!(selroom.x + xmove == curptr.x && selroom.y + ymove == curptr.y && selroom.level == curptr.level)) {
				tempptrlist.Add (curptr);
			}
		}

		ptrlist = tempptrlist;

	}

	bool ConnectsToRoom (int x, int y, int level) {

		int xmove = 0;
		int ymove = 0;

		if (recentdirection == "up")
			ymove = 1;
		if (recentdirection == "down")
			ymove = -1;
		if (recentdirection == "left")
			xmove = -1;
		if (recentdirection == "right")
			xmove = 1;

		foreach (Room curroom in roomlist) {
			if (x + xmove == curroom.x && y + ymove == curroom.y && curroom.level == level) {
				return true;
			}
		}

		return false;

	}

	bool DisruptsConnections (Room selroom, GameObject roomdata, string direction) {

		int xmove = 0;
		int ymove = 0;
		Room uproom = null;
		Room downroom = null;
		Room leftroom = null;
		Room rightroom = null;
//		GameObject upind = GameObject.Find("Nullroom");
//		GameObject downind = GameObject.Find("Nullroom");
//		GameObject leftind = GameObject.Find("Nullroom");
//		GameObject rightind = GameObject.Find("Nullroom");

		if (direction == "up")
			ymove = 1;
		if (direction == "down")
			ymove = -1;
		if (direction == "left")
			xmove = -1;
		if (direction == "right")
			xmove = 1;

		foreach (Room curroom in roomlist) {
			if (selroom.x + xmove == curroom.x && selroom.y + ymove + 1 == curroom.y && selroom.level == curroom.level) {
				uproom = curroom;
//				upind = GameObject.Find ("room_" + (roomlist.IndexOf (curroom) + 1).ToString ());
			}
			if (selroom.x + xmove == curroom.x && selroom.y + ymove - 1 == curroom.y && selroom.level == curroom.level) {
				downroom = curroom;
//				downind = GameObject.Find ("room_" + (roomlist.IndexOf (curroom) + 1).ToString ());
			}
			if (selroom.x + xmove - 1 == curroom.x && selroom.y + ymove == curroom.y && selroom.level == curroom.level) {
				leftroom = curroom;
//				leftind = GameObject.Find ("room_" + (roomlist.IndexOf (curroom) + 1).ToString ());
			}
			if (selroom.x + xmove + 1 == curroom.x && selroom.y + ymove == curroom.y && selroom.level == curroom.level) {
				rightroom = curroom;
//				rightind = GameObject.Find ("room_" + (roomlist.IndexOf (curroom) + 1).ToString ());
			}
		}

		foreach (PTR curptr in ptrlist) {
			if (selroom.x + xmove == curptr.x && selroom.y + ymove == curptr.y) {


				foreach (char d in curptr.connections) {
					if (uproom != null) {
						if (!(d == 'u' && (selroom.r.GetComponent<room_data> ().n_l && uproom.r.GetComponent<room_data> ().s_r || selroom.r.GetComponent<room_data> ().n_m && uproom.r.GetComponent<room_data> ().s_m || selroom.r.GetComponent<room_data> ().n_r && uproom.r.GetComponent<room_data> ().s_l))) {
							return true;
						}
					}
					if (downroom != null) {
						if (!(d == 'd' && (selroom.r.GetComponent<room_data> ().s_l && downroom.r.GetComponent<room_data> ().n_r || selroom.r.GetComponent<room_data> ().s_m && downroom.r.GetComponent<room_data> ().n_m || selroom.r.GetComponent<room_data> ().s_r && downroom.r.GetComponent<room_data> ().n_l))) {
							return true;
						}
					}
					if (leftroom != null) {
						if (!(d == 'l' && (selroom.r.GetComponent<room_data> ().w_l && leftroom.r.GetComponent<room_data> ().e_r || selroom.r.GetComponent<room_data> ().w_m && leftroom.r.GetComponent<room_data> ().e_m || selroom.r.GetComponent<room_data> ().w_r && leftroom.r.GetComponent<room_data> ().e_l))) {
							return true;
						}
					}
					if (rightroom != null) {
						if (!(d == 'r' && (selroom.r.GetComponent<room_data> ().e_l && rightroom.r.GetComponent<room_data> ().w_r || selroom.r.GetComponent<room_data> ().e_m && rightroom.r.GetComponent<room_data> ().w_m || selroom.r.GetComponent<room_data> ().e_r && rightroom.r.GetComponent<room_data> ().w_l))) {
							return true;
						}
					}
				}
			}
		}

		return false;

	}

	void UpdateTreasureroom (PTR selptr) {

		if (selptr != null) {
			roomloadarray = Resources.LoadAll<GameObject> (RoomRootFile + "/treasurerooms/" + selptr.kind);
		}
		else
			roomloadarray = Resources.LoadAll<GameObject> (RoomRootFile + "/treasurerooms/one");
		roomloadlist = roomloadarray.ToList ();
		treasureroomlist = roomloadlist;
		roomchoices = treasureroomlist;

	}

	void GenerateRandomTR (int t) {
		
		PTR curptr = ptrlist[Random.Range(0, ptrlist.Count())];
		UpdateTreasureroom (curptr);

		GameObject temproom = roomchoices [Random.Range (0, roomchoices.Count ())] as GameObject;

		int rn = 0;
		List<string> rnlist1 = new List<string> ();
		List<string> rnlist2 = new List<string> ();
		List<string> rnlist3 = new List<string> ();
		rnlist1.Add ("lr");
		rnlist1.Add ("rl");
		rnlist1.Add ("ld");
		rnlist1.Add ("dl");
		rnlist1.Add ("l");
		rnlist2.Add ("ul");
		rnlist2.Add ("lu");
		rnlist2.Add ("u");
		rnlist3.Add ("ur");
		rnlist3.Add ("ru");
		rnlist3.Add ("r");

		if (rnlist1.Contains (curptr.connections)) {
			rn = 1;
		}
		if (rnlist2.Contains (curptr.connections)) {
			rn = 2;
		}
		if (rnlist3.Contains (curptr.connections)) {
			rn = 3;
		}

		bool tempsl;
		bool tempsm;
		bool tempsr;
		int temprot = 0;

		for (int r = rn; r < 4; r++) {

			tempsl = temproom.GetComponent<room_data> ().s_l;
			tempsm = temproom.GetComponent<room_data> ().s_m;
			tempsr = temproom.GetComponent<room_data> ().s_r;
			temproom.GetComponent<room_data> ().s_l = temproom.GetComponent<room_data> ().w_l;
			temproom.GetComponent<room_data> ().s_m = temproom.GetComponent<room_data> ().w_m;
			temproom.GetComponent<room_data> ().s_r = temproom.GetComponent<room_data> ().w_r;
			temproom.GetComponent<room_data> ().w_l = temproom.GetComponent<room_data> ().n_l;
			temproom.GetComponent<room_data> ().w_m = temproom.GetComponent<room_data> ().n_m;
			temproom.GetComponent<room_data> ().w_r = temproom.GetComponent<room_data> ().n_r;
			temproom.GetComponent<room_data> ().n_l = temproom.GetComponent<room_data> ().e_l;
			temproom.GetComponent<room_data> ().n_m = temproom.GetComponent<room_data> ().e_m;
			temproom.GetComponent<room_data> ().n_r = temproom.GetComponent<room_data> ().e_r;
			temproom.GetComponent<room_data> ().e_l = tempsl;
			temproom.GetComponent<room_data> ().e_m = tempsm;
			temproom.GetComponent<room_data> ().e_r = tempsr;
			temprot -= 90;

		}

		Room curroom = new Room (curptr.x, curptr.y, temproom, curptr.level);
		roomlist.Add (new Room (curptr.x, curptr.y, temproom, curptr.level));

		GameObject obj = Instantiate (roomlist [roomlist.Count - 1].r, new Vector3 (-20 * roomlist [roomlist.Count - 1].x, -5 + 11 * curroom.level, -20 * roomlist [roomlist.Count - 1].y), Quaternion.Euler (0, temprot % 360, 0)) as GameObject;
		obj.name = "room_" + roomlist.Count;

		for (int r = rn; r > 0; r--) {

			tempsl = temproom.GetComponent<room_data> ().s_l;
			tempsm = temproom.GetComponent<room_data> ().s_m;
			tempsr = temproom.GetComponent<room_data> ().s_r;
			temproom.GetComponent<room_data> ().s_l = temproom.GetComponent<room_data> ().w_l;
			temproom.GetComponent<room_data> ().s_m = temproom.GetComponent<room_data> ().w_m;
			temproom.GetComponent<room_data> ().s_r = temproom.GetComponent<room_data> ().w_r;
			temproom.GetComponent<room_data> ().w_l = temproom.GetComponent<room_data> ().n_l;
			temproom.GetComponent<room_data> ().w_m = temproom.GetComponent<room_data> ().n_m;
			temproom.GetComponent<room_data> ().w_r = temproom.GetComponent<room_data> ().n_r;
			temproom.GetComponent<room_data> ().n_l = temproom.GetComponent<room_data> ().e_l;
			temproom.GetComponent<room_data> ().n_m = temproom.GetComponent<room_data> ().e_m;
			temproom.GetComponent<room_data> ().n_r = temproom.GetComponent<room_data> ().e_r;
			temproom.GetComponent<room_data> ().e_l = tempsl;
			temproom.GetComponent<room_data> ().e_m = tempsm;
			temproom.GetComponent<room_data> ().e_r = tempsr;
			temprot -= 90;

		}

		ptrlist.RemoveAt (ptrlist.IndexOf (curptr));
		crashcount = 0;

	}

	public void ToggleRoomVisibility (Collider col) {

		for (int r = 1; r <= roomlist.Count; r++) {
			GameObject curroom = GameObject.Find ("room_" + (r).ToString());
			GameObject camtrig = GameObject.Find ("room_" + (r).ToString() + "/Cameratrigger");
			if (camtrig.transform.position != col.transform.position) {
				foreach (MeshRenderer go in curroom.GetComponentsInChildren<MeshRenderer>()) {
					go.enabled = false;
				}
			}
			else {
				roomlist [r - 1].visited = true;
				this.GetComponent<pause_controller> ().currentroom = roomlist [r - 1];
				this.GetComponent<pause_controller> ().currentroom.level = roomlist [r - 1].level;
				foreach (MeshRenderer go in curroom.GetComponentsInChildren<MeshRenderer>(true)) {
					go.enabled = true;
				}
			}

			if (roomlist [r - 1].visited) {
				print (r);
			}
		}

	}

	public void CreateWall (int rot, string name, GameObject asset, GameObject curdoor, Vector3 pos) {

		GameObject wall = Instantiate (asset, new Vector3 (pos.x, pos.y, pos.z), Quaternion.Euler(0, rot, 0));
		wall.name = name;
		wall.transform.parent = curdoor.transform;

	}

	public void CreateStairHalf (Room stemroom, int eledirection, int rotnum, int t) {
		
		GameObject temproom = stairroomlist.Where (obj => obj.name == stemroom.r.GetComponent<room_data> ().half).SingleOrDefault();
		print (temproom.name);
		bool tempsl;
		bool tempsm;
		bool tempsr;
		int temprot = 0;

		for (int rn = rotnum; rn < 4; rn++) {

			tempsl = temproom.GetComponent<room_data> ().s_l;
			tempsm = temproom.GetComponent<room_data> ().s_m;
			tempsr = temproom.GetComponent<room_data> ().s_r;
			temproom.GetComponent<room_data> ().s_l = temproom.GetComponent<room_data> ().w_l;
			temproom.GetComponent<room_data> ().s_m = temproom.GetComponent<room_data> ().w_m;
			temproom.GetComponent<room_data> ().s_r = temproom.GetComponent<room_data> ().w_r;
			temproom.GetComponent<room_data> ().w_l = temproom.GetComponent<room_data> ().n_l;
			temproom.GetComponent<room_data> ().w_m = temproom.GetComponent<room_data> ().n_m;
			temproom.GetComponent<room_data> ().w_r = temproom.GetComponent<room_data> ().n_r;
			temproom.GetComponent<room_data> ().n_l = temproom.GetComponent<room_data> ().e_l;
			temproom.GetComponent<room_data> ().n_m = temproom.GetComponent<room_data> ().e_m;
			temproom.GetComponent<room_data> ().n_r = temproom.GetComponent<room_data> ().e_r;
			temproom.GetComponent<room_data> ().e_l = tempsl;
			temproom.GetComponent<room_data> ().e_m = tempsm;
			temproom.GetComponent<room_data> ().e_r = tempsr;
			temprot -= 90;

		}
			
		roomlist.Add (new Room (stemroom.x, stemroom.y, temproom, stemroom.level + eledirection));
		GameObject go = Instantiate (roomlist [roomlist.Count - 1].r, new Vector3 (-20 * roomlist [roomlist.Count - 1].x, -5 + 11 * roomlist[roomlist.Count - 1].level, -20 * roomlist [roomlist.Count - 1].y), Quaternion.Euler (0, temprot % 360, 0)) as GameObject;
		go.name = "room_" + roomlist.Count;

		for (int rn = rotnum; rn > 0; rn--) {

			tempsl = temproom.GetComponent<room_data> ().s_l;
			tempsm = temproom.GetComponent<room_data> ().s_m;
			tempsr = temproom.GetComponent<room_data> ().s_r;
			temproom.GetComponent<room_data> ().s_l = temproom.GetComponent<room_data> ().w_l;
			temproom.GetComponent<room_data> ().s_m = temproom.GetComponent<room_data> ().w_m;
			temproom.GetComponent<room_data> ().s_r = temproom.GetComponent<room_data> ().w_r;
			temproom.GetComponent<room_data> ().w_l = temproom.GetComponent<room_data> ().n_l;
			temproom.GetComponent<room_data> ().w_m = temproom.GetComponent<room_data> ().n_m;
			temproom.GetComponent<room_data> ().w_r = temproom.GetComponent<room_data> ().n_r;
			temproom.GetComponent<room_data> ().n_l = temproom.GetComponent<room_data> ().e_l;
			temproom.GetComponent<room_data> ().n_m = temproom.GetComponent<room_data> ().e_m;
			temproom.GetComponent<room_data> ().n_r = temproom.GetComponent<room_data> ().e_r;
			temproom.GetComponent<room_data> ().e_l = tempsl;
			temproom.GetComponent<room_data> ().e_m = tempsm;
			temproom.GetComponent<room_data> ().e_r = tempsr;
			temprot -= 90;

		}

	}

	void MakeFloorHazard() {

		foreach (Room r1 in roomlist) {
			bool under = false;
			foreach (Room r2 in roomlist) {
				if (r1.x == r2.x && r1.y == r2.y && r1.level == r2.level + 1) {
					under = true;
				}
			}
			if (!under) {
				GameObject h = Instantiate (Resources.Load<GameObject> (RoomRootFile + "/floorhazard"), new Vector3 (0, 0, 0), Quaternion.identity);
				h.transform.parent = GameObject.Find("room_" + (roomlist.IndexOf(r1) + 1)).transform;
				h.transform.position = new Vector3 (h.transform.parent.transform.position.x, h.transform.parent.transform.position.y - 0, h.transform.parent.transform.position.z);
			}
		}

	}

	void MakeChests () {

		string[] raritylist = { "common", "common", "common", "common", "common", "uncommon", "uncommon", "uncommon", "uncommon", "rare", "rare", "rare", "very rare", "very rare", "artifact"};
		string chosenrarity = raritylist [Random.Range (0, raritylist.Length)];

		for (int i = 1; i <= roomlist.Count; i++) {

			GameObject r = GameObject.Find ("room_" + i);

			foreach (Transform t in r.transform) {
				if (t.gameObject.name == "Treasurespot") {
					PlayerPrefs.SetInt ("NumOfChests", PlayerPrefs.GetInt("NumOfChests") + 1);
					GameObject c = Instantiate (chestasset, t.position, t.localRotation);
					c.transform.rotation = t.rotation;
					c.transform.parent = r.transform;
					chestlist.Add (c);
				}
			}

		}

		int ind = Random.Range(0, chestlist.Count - 1);
		print (ind + " " + chestlist.Count);
		print (chestlist [ind].name);
		chestlist [ind].GetComponent<chest_items> ().storage.Add (Player.GetComponent<inventory>().findItem ("Dungeon Key"));

		foreach (GameObject g in chestlist) {

			while (true) {
				int rand = Random.Range (0, Player.GetComponent<inventory>().catalog.Count);
				print(rand + " " + Player.GetComponent<inventory>().catalog.Count);
				if (Player.GetComponent<inventory>().catalog [rand].name != "Dungeon Key" && chosenrarity == Player.GetComponent<inventory>().catalog [rand].rarity) {
					g.GetComponent<chest_items> ().storage.Add (Player.GetComponent<inventory>().catalog [rand]);
					g.GetComponent<chest_items> ().gold = Random.Range (20, 101);
					break;
				}
			}

		}

	}

	void SpawnEnemies () {

		for (int i = 1; i <= roomlist.Count; i++) {

			GameObject room = GameObject.Find ("room_" + i);
			foreach (Transform t in room.transform) {

				if (t.gameObject.name == "Enemyspot") {

					GameObject e = Instantiate (Resources.Load<GameObject> ("Basicenemy"), t.position, Quaternion.identity);
					e.transform.parent = t;

				}

			}

		}

	}

}