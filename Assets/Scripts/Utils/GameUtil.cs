using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUtil : MonoBehaviour {
    public delegate void btnClickCallback (GameObject obj);

    public static Vector3 getObjLocalPos (GameObject obj, Vector3 vec,Camera camera) {
        
        var mouseWorldPos = camera.ScreenToWorldPoint (vec); //屏幕坐标转世界坐标
        var mouseLocalPos = obj.transform.parent.transform.InverseTransformPoint (mouseWorldPos); //世界坐标转本地坐标
        // Debug.Log("VVVVVVV + " + vec.x + ":" + vec.y + " MMMM + " + mouseWorldPos.x + ":" + mouseWorldPos.y + " LLLLL " + mouseLocalPos.x + ":" + mouseLocalPos.y);
        return mouseLocalPos;
    }

    public static void AddClick (GameObject btn, btnClickCallback callback) {
        Button button = btn.GetComponent<Button> ();
        if (button) {
            button.onClick.AddListener (delegate () {
                if (callback != null) {
                    callback (btn);
                }
            });
        }
    }

    public static void AddTouchDown (GameObject btn) {
        Button button = btn.GetComponent<Button> ();
        if (button) { }
    }

}