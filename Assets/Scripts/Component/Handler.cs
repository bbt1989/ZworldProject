using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Handler : MonoBehaviour {

    private GameObject handler;
    private GameObject btn;

    private float rotateAngle;

    private float dragDis;

    private bool isPressing = false;
    private bool isDraging = false;

    private float pressingTime = 0;
    private float pressDragTime = 0.1f;

    private Camera mainUICamera;

    private Rect touchResponseRect;

    //设置回调
    public delegate void handlerCallback (float angle);
    //轻回调
    public handlerCallback handlercb1;
    //重回调
    public handlerCallback handlercb2;

    public handlerCallback handlerUp;

    void Awake () {
        handler = gameObject.transform.Find ("handler").gameObject;
        // btn = gameObject.transform.Find ("BtnBg").gameObject;
        // float btnwidth = btn.GetComponent<Image>().flexibleWidth;
        // float btnheight = btn.GetComponent<Image>().flexibleHeight;

        // btnRect = new Rect();

        // EventTriggerListener.Get (btn).onDown += onHandlerDown;
        // EventTriggerListener.Get (btn).onUp += onHandlerUp;
        // EventTriggerListener.Get (btn).onExit += onHandlerUp;
        // btn.SetActive(false);
        mainUICamera = GameObject.Find ("UICamera").GetComponent<Camera> ();

    }

    public void setTouchResponseArea (string area) {
        if (area == "left") {
            touchResponseRect = new Rect (0, 0, Screen.width * 0.5f - 10, Screen.height * 0.6f);
        } else if (area == "right") {
            touchResponseRect = new Rect (Screen.width * 0.5f + 10, 0, Screen.width * 0.5f - 10, Screen.height * 0.6f);
        }
    }

    public void setHandlerCallback (handlerCallback cb1, handlerCallback cb2,handlerCallback upcb) {
        if (cb1 != null) {
            handlercb1 = cb1;
        }
        if (cb2 != null) {
            handlercb2 = cb2;
        }
        if (upcb != null){
            handlerUp = upcb;
        }
    }

    private void onHandlerDown (GameObject obj) {
        isPressing = true;
        Debug.Log ("~~~~~~~~~~~~~~~~~~~~btnDown");
    }

    private void onHandlerUp (GameObject obj) {
        isDraging = false;
        isPressing = false;
        pressingTime = 0f;
        prex = null;
        prey = null;
        //回归原位
        handler.transform.localPosition = new Vector2 (0, 0);

        if (handlerUp != null){
            handlerUp(0);
        }
        Debug.Log ("~~~~~~~~~~~~~~~~~~~btnUp");
    }

    private float? prex;
    private float? prey;

    private int nowTouchIdx;

    private Vector2 startTouchPoint;
    private float touchDis = 20;

    private float GetAngle (Vector2 a, Vector2 b) {
        Vector2 c = b - a;
        float deltaAngle = 0;
        if (b.x == 0 && b.y == 0) {
            return 0;
        } else if (b.x > 0 && b.y > 0) {
            deltaAngle = 0;
        } else if (b.x > 0 && b.y == 0) {
            return 90;
        } else if (b.x > 0 && b.y < 0) {
            deltaAngle = 180;
        } else if (b.x == 0 && b.y < 0) {
            return 180;
        } else if (b.x < 0 && b.y < 0) {
            deltaAngle = -180;
        } else if (b.x < 0 && b.y == 0) {
            return -90;
        } else if (b.x < 0 && b.y > 0) {
            deltaAngle = 0;
        }

        float angle = Mathf.Atan (b.x / b.y) * Mathf.Rad2Deg + deltaAngle;
        return angle;
    }

    void Update () {
        if (Input.touchCount > 0) {

        } else {
            if (Input.GetMouseButtonDown (0)) {
                Vector2 mouseposition = Input.mousePosition;
                if (touchResponseRect.Contains (mouseposition)) {
                    startTouchPoint = Input.mousePosition;
                    onHandlerDown (null);
                    prex = Input.mousePosition.x;
                    prey = Input.mousePosition.y;

                    gameObject.transform.localPosition = new Vector2 (startTouchPoint.x - Screen.width * .5f, startTouchPoint.y - Screen.height * .5f);
                }
            } else if (Input.GetMouseButton (0)) {
                Vector2 mouseposition = Input.mousePosition;

                if (isDraging) {

                    Vector3 tempstart = GameUtil.getObjLocalPos (gameObject, startTouchPoint, mainUICamera);
                    Vector3 tempmouse = GameUtil.getObjLocalPos (gameObject, mouseposition, mainUICamera);

                    Vector2 dir = tempmouse - tempstart;
                    // float angle = Vector2.Angle(tempstart,tempmouse);
                    // Debug.Log("startTouchPoint:" + startTouchPoint.x + ":" + startTouchPoint.y + " mouseposition:" + mouseposition.x + ":" + mouseposition.y);
                    //Debug.Log("tempstart:" + tempstart.x + ":" + tempstart.y + " tempmouse:" + tempmouse.x + ":" + tempmouse.y);
                    //float angle = Mathf.Acos (Vector2.Dot (tempstart.normalized, tempmouse.normalized)) * Mathf.Rad2Deg;
                    // float angle = GetAngle(tempstart,tempmouse);
                    float angle = Vector3.Angle (transform.up, tempmouse - tempstart);

                    float dis = Mathf.Min (touchDis, Vector2.Distance (startTouchPoint, mouseposition));
                    // float rad = Mathf.Deg2Rad * angle;

                    Vector3 newVec = new Vector3 (0, 0, 0) + (tempmouse - tempstart).normalized * dis;

                    handler.transform.localPosition = newVec;
                    if (newVec.x <= 0) {
                        angle = 180 + Mathf.Abs (angle - 180);
                    }
                    // Debug.Log ("angle " + angle + " tempmouse.x " + tempmouse.x);
                    if (dis >= touchDis * .2f && dis < touchDis * .6f) {
                        if (handlercb1 != null) {
                            handlercb1 (angle);
                        }
                    } else if (dis >= touchDis * .6f) {
                        if (handlercb2 != null) {
                            handlercb2 (angle);
                        }
                    }

                    // float mx = 0, my = 0;

                    // mx = Input.mousePosition.x;
                    // my = Input.mousePosition.y;

                    // // if (Input.touchCount <= 0) {
                    // //     mx = Input.mousePosition.x;
                    // //     my = Input.mousePosition.y;
                    // // } else {
                    // //     mx = Input.GetTouch ();
                    // // }

                    // if (prex == null || prey == null) {
                    //     prex = mx;
                    //     prey = my;
                    // }

                    // Vector2 objPos = handler.transform.localPosition;
                    // Vector2 dir = new Vector2 (mx - (float) prex, my - (float) prey);

                    // float dis = Vector2.Distance (new Vector2 (0, 0), objPos + dir);
                    // if (dis <= 20) {
                    //     handler.transform.localPosition = objPos + dir;
                    //     prex = mx;
                    //     prey = my;
                    // }
                }

                if (!touchResponseRect.Contains (mouseposition)) {
                    // onHandlerUp (null);
                }

            } else if (Input.GetMouseButtonUp (0)) {
                onHandlerUp (null);
            }

        }

        //
        // if (Input.touchCount <= 0){
        //     if (Input.){
        //     }
        // }
        //Debug.Log("-DDDDDDDDDDDDDDDDDDDDDD" + isPressing);
        if (isPressing) {
            if (pressingTime >= pressDragTime) {
                isDraging = true;
            } else {
                pressingTime += Time.deltaTime;
            }
        }

        if (isDraging) {

            // Debug.Log ("------------" + mx + "ooo" + my);

        }

    }

}