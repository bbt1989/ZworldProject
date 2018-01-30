using UnityEngine;
using UnityEngine.AI;

//待机状态
public class EventState : BaseState
{
    //本次待机时间
    public float IdleWaitTime;
    //已经经过的待机时间
    public float nowIdleWaitTime = 0f;
    //回调
    public stateCallback endCallback;
    private NavMeshAgent nowNMA;
    //public stateCallback endCallback;
    //0 为说话者 1为倾听者
    private uint chatType = 0;
    public EventState(float time, stateCallback endcallback,uint type = 0)
    {
        StateName = "Event";
        IdleWaitTime = time;
        endCallback = endcallback;
        chatType = type;
        //FLDebugger.Log("CHATPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP" + chatType);
    }

    public override void Execute(GameObject owner, float deltaTime)
    {
        //owner.stateIdle();
        nowIdleWaitTime += deltaTime;
        if (nowIdleWaitTime >= IdleWaitTime)
        {
            if (endCallback != null)
            {
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
        nowNMA = owner.GetComponent<NavMeshAgent>();
        nowNMA.ResetPath();
        owner.GetComponent<Unit>().SetAnimation(chatType==0?"stand":"chat");
        //判断状态 更新UI显示
        // FLDebugger.Log("交谈状态");
    }
}