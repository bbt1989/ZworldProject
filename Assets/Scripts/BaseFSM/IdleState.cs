using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//待机状态
public class IdleState : BaseState
{
    //本次待机时间
    public float IdleWaitTime;
    //已经经过的待机时间
    public float nowIdleWaitTime = 0f;
    //回调
    public stateCallback endCallback;

    private string IdleType;
    public IdleState(float time,stateCallback callback,string idleType = "none") {
        StateName = "Idle";
        IdleWaitTime = time;
        endCallback = callback;
        IdleType = idleType;
    }

    public override void Execute(GameObject owner, float deltaTime)
    {
        //owner.stateIdle();
        nowIdleWaitTime += deltaTime;
        if (nowIdleWaitTime >= IdleWaitTime) {
            if (endCallback != null) {
                endCallback();
            }
        }
    }

    public override void ExitState(GameObject owner)
    {
        //FLDebugger.Log(owner.name + "离开状态： Idle");
    }

    public override void EnterState(GameObject owner)
    {
        //FLDebugger.Log(owner.name + "进入状态： Idle");
        // if (IdleType == "none"){
        //     owner.GetComponent<Unit>().SetAnimation("stand");
        // }else if (IdleType == "work"){
        //     owner.GetComponent<Unit>().SetAnimation("busy");
        // }else if (IdleType == "rest"){
        //     owner.GetComponent<Unit>().SetAnimation("stand");
        // }
        
        owner.GetComponent<Unit>().SetAnimation("Idle");
    }
}