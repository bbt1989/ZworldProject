using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class battlerCenter : MonoBehaviour {

	GameObject soilder;
	Animator tempAni;

	GameObject mainCamera;
	GameObject secCamera;
	GameObject UIRoot;

	GameObject mapObj;
	GameObject mapPlaneObj;
	Plane mapPlane;
	battleMap BMap;

	GameObject btnTemp;

	GameObject tempZombie;

	GameObject tempSoilder;

	Unit PlayerSoilder;

	battleUI gameUI;

	//临时resmanager
	ResourceManger resManager;

	// Use this for initialization
	void Start () {
		resManager = new ResourceManger ();

		mainCamera = GameObject.Find ("mainCamera");
		secCamera = GameObject.Find ("secCamera");
		mapObj = GameObject.Find ("Map");
		mapPlaneObj = mapObj.transform.Find ("Plane").gameObject;
		// mapPlane = mapPlaneObj.GetComponent<Plane>();
		UIRoot = GameObject.Find ("GameUI");
		// btnTemp = UIRoot.transform.Find("Button").gameObject;
		// GameUtil.AddClick(btnTemp,delegate(GameObject obj){

		// });
		var uiobj = resManager.Load ("UI/battleUIPanel", typeof (GameObject)) as GameObject;
		var gameUIObj = Instantiate (uiobj);
		gameUIObj.transform.parent = UIRoot.transform;
		gameUIObj.transform.localPosition = new Vector3 (0, 0, 0);
		gameUIObj.transform.localScale = new Vector3 (1, 1, 1);
		gameUI = gameUIObj.AddComponent<battleUI> ();
		gameUI.initBattleUI (gameUIObj);

		gameUI.setKeyDownCallback (tempMoveKey);
		gameUI.setKeyUpCallback(changeWeaponKey);

		mapPlane = new Plane (Vector3.up, new Vector3 (115, 0, 115));
		BMap = mapObj.AddComponent<battleMap> ();
		BMap.initMapObj (mainCamera, secCamera, mapObj, mapPlane);

		tempZombie = GameObject.Find ("FatZombiePrefab");
		//tempSoilder = GameObject.Find("soilder");\

		var tempAniZ = tempZombie.GetComponent<Animation> ();
		tempAniZ.Play ("shamble");

		//玩家
		var newsoilder = resManager.Load ("soilders/soilder", typeof (GameObject)) as GameObject;
		tempSoilder = Instantiate (newsoilder);
		tempSoilder.transform.localScale = new Vector3 (1, 1, 1);
		tempSoilder.transform.localPosition = new Vector3 (91, 0, 79);

		PlayerSoilder = tempSoilder.AddComponent<Unit> ();
		// Debug.Log("BBBBBBBB" +BMap);
	}
	// 	soilder = GameObject.Find("soilder");
	// 	Debug.Log("CCCCCCCCCCCCCCC" + soilder);
	// 	if (soilder){
	// 		tempAni = soilder.GetComponent<Animator>();
	// 		tempAni.Play("infantry_combat_idle");
	// 	}
	// }

	private float speed = 0.25f;
	public void tempMoveKey (bool isPressing, float angle) {

		if (!isPressing) {
			PlayerSoilder.setUnitStopMoving ();
		} else {
			//转向
			PlayerSoilder.setUnitTurn(angle);

			float rad = Mathf.Deg2Rad * angle;

			float transX = speed * Mathf.Sin (rad) * speed;
			float transZ = speed * Mathf.Cos (rad) * speed;

		//	Debug.Log ("angle" + rad + " sin " + Mathf.Sin (rad) + "transx " + transX + " transz " + transZ);

			PlayerSoilder.setUnitMoveTarget(transX,transZ);
		}
		// Debug.Log("ISPRESSING:" + isPressing + " angle " +angle);
	}

	private void changeWeaponKey(uint wOrder)
	{
		PlayerSoilder.changeWeapon(wOrder);
	}


	private void createBuilding () {

	}

	// Update is called once per frame
	void Update () {
		if (PlayerSoilder) {
			PlayerSoilder.updateFSM (Time.deltaTime);
		}
	}
}