using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

//临时
public class SoilderShootEndListener : MonoBehaviour {
    public void ShootEnd () {
        // Debug.Log("SHOOT ENDDDDDDDDDDDDD" + gameObject);
        if (gameObject) {
            var tempAniS = gameObject.GetComponent<Animator> ();
            tempAniS.Play ("infantry_combat_shoot", 0, 0);
        }
    }
}

public class TouchChecker : MonoBehaviour {

    public delegate void touchCallback (string name, string touchname);
    public touchCallback touchcallback;

    public void setTouchCallback (touchCallback callback) {
        if (callback != null) {
            touchcallback = callback;
        }
    }

    //监测碰撞
    private void OnTriggerEnter (Collider other) {
        //FLDebugger.Log("TOUCH " + other.name);
        if (touchcallback != null) {
            touchcallback (gameObject.name, other.name);
        }
    }

    private void OnTriggerExit (Collider other) {
        ///FLDebugger.Log("EXIT " + other.name);
    }

    private void OnCollisionEnter (Collision collision) {
        Debug.Log ("TOUCH " + collision.gameObject.name);
    }
    private void OnCollisionExit (Collision collision) {
        Debug.Log ("EXIT " + collision.gameObject.name);
    }
}

public class Unit : MonoBehaviour {
    private GameObject UnitObj;

    private FSM fsm;

    private NavMeshAgent unitNavAgent;

    private TouchChecker touchChecker;

    private Animator unitAnimator;

    private Animation unitAnimation;

    //上一次使用的动作名字
    private string m_lastActionName;

    //当前使用的武器
    public enum WeaponType {
        Knife = 1,
        AK47 = 2,
    }

    private WeaponType nowWeaponType;

    public string nowWeaponName;

    private List<string> knifeActionBooList = new List<string>(){ "knifeIdle","knifeRun","knifeShoot1","knifeShoot2" };
    private List<string> infantryActionBooList = new List<string>(){ "infantryIdle","infantryRun","infantryShoot" };

    private Dictionary<string , GameObject> weaponNameObjDic = new Dictionary<string , GameObject>();

    void Awake () {
        UnitObj = gameObject;

        fsm = new FSM (UnitObj);

        unitNavAgent = UnitObj.GetComponent<NavMeshAgent> ();
        if (unitNavAgent == null) {
            unitNavAgent = UnitObj.AddComponent<NavMeshAgent> ();
        }

        touchChecker = UnitObj.AddComponent<TouchChecker> ();
        touchChecker.setTouchCallback (unitTouchCallback);

        UnitObj.AddComponent<SoilderShootEndListener> ();

        unitAnimator = UnitObj.GetComponent<Animator> ();
        unitAnimation = UnitObj.GetComponent<Animation> ();

        //初始化 小刀
        nowWeaponType = WeaponType.Knife;

        nowWeaponName = "knife";
        m_lastActionName = "knifeIdle";

        GameObject weaponContainer = 
        gameObject.transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/WeaponContainer").gameObject;

        weaponNameObjDic["knife"] = weaponContainer.transform.Find("weapon_knife").gameObject;
        weaponNameObjDic["ak"] = weaponContainer.transform.Find("weapon_ak47").gameObject;

        showNowWeapon();
    }

    private void showNowWeapon()
    {
        foreach(var obj in weaponNameObjDic){
            obj.Value.SetActive(obj.Key == nowWeaponName);
        }
    }

    private void unitTouchCallback (string name, string touchname) {
        Debug.Log ("本体：" + name + " 碰撞：" + touchname);
    }

    public void updateFSM (float deltaTime) {
        if (fsm != null) {
            fsm.Update (deltaTime);
        }
    }

    private void setActionBoo(WeaponType type,string str)
    {
        Debug.Log("SSSSSSSSSSSS" + str);
        if (type == WeaponType.Knife){
            for(int i=0;i<knifeActionBooList.Count;i++){
                var tempstr = knifeActionBooList[i];
                unitAnimator.SetBool(tempstr,tempstr == str);
            }
        }else if (type == WeaponType.AK47){
            for(int i=0;i<infantryActionBooList.Count;i++){
                var tempstr = infantryActionBooList[i];
                unitAnimator.SetBool(tempstr,tempstr == str);
            }
        }
    }

    private string aName = "";
    public void SetAnimation (string actionName) {
        if (nowWeaponType == WeaponType.AK47) {
            aName = "infantry" + actionName;
        } else if (nowWeaponType == WeaponType.Knife) {
            aName = "knife" + actionName;
        }

        if (!string.IsNullOrEmpty (m_lastActionName) && m_lastActionName == aName) {
            return;
        }
        //判断当前unit

        Debug.Log("当前动作" + m_lastActionName + " aName: " + aName + " actionName " + actionName);
        m_lastActionName = aName;
        if (gameObject.name.Contains ("soilder")) {
            if (actionName == "Run") {
                if (nowWeaponType == WeaponType.AK47) {
                    setActionBoo(nowWeaponType,"infantryRun");
                    //unitAnimator.SetBool ("infantryRun", true);
                } else if (nowWeaponType == WeaponType.Knife) {
                    setActionBoo(nowWeaponType,"knifeRun");
                    //unitAnimator.SetBool ("knifeRun", true);
                }
                // aName = "infantry_combat_run";
            } else if (actionName == "Idle") {
                // aName = "infantry_combat_idle";
                if (nowWeaponType == WeaponType.AK47) {
                    setActionBoo(nowWeaponType,"infantryIdle");
                    //unitAnimator.SetBool ("infantryIdle", true);
                } else if (nowWeaponType == WeaponType.Knife) {
                    setActionBoo(nowWeaponType,"knifeIdle");
                    //unitAnimator.SetBool ("knifeIdle", true);
                }
            }
            Debug.Log ("士兵做动作：" + actionName + " aName " + aName);
        }

        // if (unitAnimator != null) {
        //     unitAnimator.Play (aName, 0, 0);
        // } else if (unitAnimation != null) {
        //     unitAnimation.Play (aName);
        // }
    }

    //改变武器
    public void changeWeapon(uint order)
    {
        Debug.Log("m_lastActionName :" +m_lastActionName);
        if (order == 1){
            nowWeaponName = "knife";
            //判断当前动作状态
            if (m_lastActionName == "infantryIdle"){
                setActionBoo(WeaponType.AK47,"");
                setActionBoo(WeaponType.Knife,"knifeIdle");
                m_lastActionName = "knifeIdle";
            }else if (m_lastActionName == "infantryRun"){
                setActionBoo(WeaponType.AK47,"");
                setActionBoo(WeaponType.Knife,"knifeRun");
                m_lastActionName = "knifeRun";
            }
            nowWeaponType = WeaponType.Knife;
        }else if (order == 2){
            nowWeaponName = "ak";
            //判断当前动作状态
            if (m_lastActionName == "knifeIdle"){
                setActionBoo(WeaponType.Knife,"");
                setActionBoo(WeaponType.AK47,"infantryIdle");
                m_lastActionName = "infantryIdle";
            }else if (m_lastActionName == "knifeRun"){
                setActionBoo(WeaponType.Knife,"");
                setActionBoo(WeaponType.AK47,"infantryRun");
                m_lastActionName = "infantryRun";
            }
            nowWeaponType = WeaponType.AK47;
        }

        showNowWeapon();
    }

    public void setUnitMoveTarget (float offsetx, float offsetz) {
        float x = gameObject.transform.localPosition.x;
        float y = gameObject.transform.localPosition.y;
        float z = gameObject.transform.localPosition.z;

        Vector3 target = new Vector3 (x + offsetx, y, z + offsetz);
        RunState runState = new RunState (target, delegate (bool boo2) {
            //wanderAround(touchFSM);
        });
        fsm.ChangeState (runState);
    }

    public void setUnitStopMoving () {
        IdleState idleState = new IdleState (3, delegate (bool bo) { });
        fsm.ChangeState (idleState);
    }

    public BaseState GetFSMCurrentState () {
        return fsm.getCurrent ();
    }

    public void SetFSMState (BaseState state) {
        if (fsm != null) {
            fsm.ChangeState (state);
        }
    }

    public void UpdateFSM (float deltaTime) {
        if (fsm != null) {
            fsm.Update (deltaTime);
        }
    }

    public void setUnitTurn (float ang) {
        gameObject.transform.DORotate (new Vector3 (0, ang, 0), 0.1f);
    }

}