using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

//跑步状态
public class RunState : BaseState {
    //当前需要跑到的位置
    public Vector3 nowTargetPos;

    //对象上一帧到这一帧的移动量 如果变化非常小就判断为卡住情况
    private float lastPlayerMove = 0f;
    private Vector3? lstPlayerPos;
    private int jamFrame;
    //卡住10帧后重置
    private int resetFrame = 10;
    //当前对象的navmeshagent
    private NavMeshAgent nowNMA;

    //跑到位置调用的回调
    public stateCallback endCallback;

    // 0为主场景 1为室内场景
    private uint sceneType = 0;
    public RunState (Vector3 nowpos, stateCallback callback, uint type = 0) {
        StateName = "Run";
        nowTargetPos = nowpos;
        endCallback = callback;
        sceneType = type;
        //FLDebugger.Log("nowTargetPosX " + nowTargetPos.x + " Z " + nowTargetPos.z);
    }

    public override void Execute (GameObject owner, float deltaTime) {
        /*owner.stateRun();*/
        if (owner.name.Contains ("soilder")) {
            //nowNMA.SetDestination (nowTargetPos);
            // if (!(nowNMA.pathStatus == NavMeshPathStatus.PathInvalid)) {
            // if (! (NavMesh.CalculatePath(owner.transform.localPosition,nowTargetPos,Layer.NavWalkableMask))){
            //      nowNMA.ResetPath();
            //     return;
            // }
            NavMeshPath path = new NavMeshPath ();

            Vector3 nowPos = owner.transform.localPosition;
            NavMeshHit hit;
            if (NavMesh.SamplePosition (nowPos, out hit, 0.25f, -1)) {
                nowPos = hit.position; //校准起始点  
            }
            Vector3 temp = nowTargetPos;
            if (NavMesh.SamplePosition (nowTargetPos, out hit, 0.25f, -1)) {
                temp = hit.position; //校准目标点  
            }

           // Debug.Log ("pathHHHHHH 点数" + path.corners.Length);
            // Debug.Log ("nowTargetPos" + owner.transform.localPosition.x + " | " + owner.transform.localPosition.z);
            // if (!NavMesh.CalculatePath (nowPos, path)) {
            if (!NavMesh.CalculatePath(nowPos,temp,NavMesh.AllAreas,path)){
                Debug.Log ("没有路径!!!!!!!!!!!!!!!");
                return;
            }
            
            owner.transform.localPosition = temp;
            // Debug.Log ("nowTargetPos" + owner.transform.localPosition.x + " | " + owner.transform.localPosition.z);

            lstPlayerPos = temp;
        } else {
            nowNMA.SetDestination (nowTargetPos);

            //直接获取vector的distance
            Vector3 playerpos = owner.transform.localPosition;
            float distance = Vector3.Distance (playerpos, nowTargetPos);
            if (lstPlayerPos != null) {
                lastPlayerMove = Vector3.Distance (playerpos, (Vector3) lstPlayerPos);
            }
            // FLDebugger.Log("JAMFRAME :" + jamFrame + " lastPlayerMove " + lastPlayerMove);
            if (lastPlayerMove >= 0.015f) {
                jamFrame = 0;
            } else {
                jamFrame++;
                //FLDebugger.Log("JAMFRAME :" + jamFrame);
                if (jamFrame >= resetFrame) {
                    //卡住重置
                    nowNMA.ResetPath ();
                    if (endCallback != null) {
                        endCallback (true);
                    }
                }
            }

            float remainDistance = nowNMA.remainingDistance;
            //FLDebugger.Log("distance1 " + distance + "distanceRemain " + remainDistance);
            //         if (nowNMA.pathStatus == NavMeshPathStatus.PathComplete)
            //         {
            //             nowNMA.ResetPath();
            //             if (endCallback != null)
            //             {
            //                 endCallback();
            //             }
            //         }
            if (distance <= 0.1f) {
                // owner.GetComponent<Unit>().SetAnimation("Idle");
                nowNMA.ResetPath ();
                //owner.GetComponent<NavMeshAgent>().enabled = false;
                //到达位置后 调用回调
                if (endCallback != null) {
                    endCallback ();
                }
            }

            lstPlayerPos = playerpos;
        }

    }

    public override void ExitState (GameObject owner) {
        nowNMA.ResetPath ();
        //FLDebugger.Log(owner.name + "离开状态： Run");
    }

    public override void EnterState (GameObject owner) {
        //FLDebugger.Log(owner.name + "进入状态： Run");
        //owner.GetComponent<NavMeshAgent>().enabled = true;
        nowNMA = owner.GetComponent<NavMeshAgent> ();

        Debug.Log (owner.name + "进入状态： Run " + nowNMA);

        nowNMA.ResetPath ();
        Unit u = owner.GetComponent<Unit> ();

        u.SetAnimation ("Run");
        // u.SetAnimation(sceneType == 0 ?"Run":"walk");
        //owner.GetComponent<Unit>().animationTrigger = 1;
    }
}