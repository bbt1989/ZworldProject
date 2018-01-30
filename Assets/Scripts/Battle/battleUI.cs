using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class battleUI : MonoBehaviour {
    private GameObject mainObj;

    //temp
    private GameObject tempbtn;

    //主界面回调

    public delegate void keyPressCallbackDel (bool isPressing, float angle);
    public keyPressCallbackDel keyPressCallback;

    public delegate void keyUpCallbackDel (uint worder);
    public keyUpCallbackDel keyUpCallback;

    private Handler dirHandler;

    private GameObject dirHandlerObj;

    //当前选择的按钮
    public void initBattleUI (GameObject uiObj) {
        if (uiObj) {
            mainObj = uiObj;
            dirHandlerObj = mainObj.transform.Find("dirHandler").gameObject;
            dirHandler = dirHandlerObj.AddComponent<Handler>();
            dirHandler.setTouchResponseArea("left");
            
            dirHandler.setHandlerCallback(dirhandlerCallback1,dirhandlerCallback2,dirhandlerCallbackUp);
            // tempbtn = mainObj.transform.Find("Button").gameObject;
            // EventTriggerListener.Get(tempbtn).onDown += onButtonDown;
        }
    }

    public void dirhandlerCallback1(float angle)
    {
        keyPressCallback (true, angle);
    }

    public void dirhandlerCallback2(float angle)
    {
        keyPressCallback (true, angle);
    }

    public void dirhandlerCallbackUp(float angle)
    {
        keyPressCallback (false, 0);
    }

    //设置回调
    public void setKeyDownCallback (keyPressCallbackDel callback) {
        if (callback != null) {
            keyPressCallback = callback;
        }
    }

    public void setKeyUpCallback (keyUpCallbackDel callback) {
        if (callback != null) {
            keyUpCallback = callback;
        }
    }

    public void onButtonDown (GameObject obj) {
        Debug.Log (obj.name);
    }

    // 方向

    private float turnAngle;
    //摇杆移动距离
    private float controllerMoveDis;

    //电脑按键
    private bool pressingA = false;
    private bool pressingD = false;
    private bool pressingW = false;
    private bool pressingS = false;

    private float changeWeaponCD = 1f;
    //电脑按键
    public void getKeyStatus () {
        if (Input.GetKeyDown (KeyCode.A)) {
            turnAngle = 270;
            pressingA = true;
            Debug.Log ("------------AAAAA");
        } else if (Input.GetKeyUp (KeyCode.A)) {
            // turnAngle = 0;
            pressingA = false;
            Debug.Log ("------------AAAAAUp");
            checkIsAllKeyUp ();
        }


        if (Input.GetKeyDown (KeyCode.D)) {
            turnAngle = 90;
            pressingA = true;
            Debug.Log ("------------AAAAA");
        } else if (Input.GetKeyUp (KeyCode.D)) {
            // turnAngle = 0;
            pressingA = false;
            Debug.Log ("------------AAAAAUp");
            checkIsAllKeyUp ();
        }

        if (Input.GetKeyDown (KeyCode.W)) {
            turnAngle = 0;
            pressingA = true;
            Debug.Log ("------------AAAAA");
        } else if (Input.GetKeyUp (KeyCode.W)) {
            // turnAngle = 0;
            pressingA = false;
            Debug.Log ("------------AAAAAUp");
            checkIsAllKeyUp ();
        }

        if (Input.GetKeyDown (KeyCode.S)) {
            turnAngle = 180;
            pressingA = true;
            Debug.Log ("------------AAAAA");
        } else if (Input.GetKeyUp (KeyCode.S)) {
            // turnAngle = 0;
            pressingA = false;
            Debug.Log ("------------AAAAAUp");
            checkIsAllKeyUp ();
        }


        if (Input.GetKeyUp(KeyCode.Alpha1)){
            if (keyUpCallback != null){
                keyUpCallback(1);
            }
        }

        if (Input.GetKeyUp(KeyCode.Alpha2)){
            if (keyUpCallback != null){
                keyUpCallback(2);
            }
        }


    }

    private void checkIsAllKeyUp () {
        //判读是否全部抬起
        if (!pressingA && !pressingD && !pressingW && !pressingS) {
            if (keyPressCallback != null) {
                keyPressCallback (false, 0);
            }
        }
    }

    void Update () {
        getKeyStatus ();

        //判断是否有按键
        if ((pressingA || pressingD || pressingW || pressingS)) {
            if (keyPressCallback != null) {
                keyPressCallback (true, turnAngle);
            }
            // }else{

        }
    }

}