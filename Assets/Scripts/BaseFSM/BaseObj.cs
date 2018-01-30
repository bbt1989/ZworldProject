// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using UnityEngine;
// using UnityEngine.AI;

// public class BaseObj : MonoBehaviour
// {
//     public GameObject MainObj;
//     private Unit m_unit;
//     public FSM m_fsm;

//     public NavMeshAgent NavAgent;
//     //上一次使用的动作名字
//     private string m_lastActionName;
//     //创建本体
//     public BaseObj(string objName, Transform parent, Vector3 pos) {
//         string prefabName = "Prefabs/Character/" + objName;
//         var rolego = GameController.instance.ResourcesManager.Load<GameObject>(prefabName);
//         MainObj = GameObject.Instantiate(rolego);
//         MainObj.transform.parent = parent;
//         MainObj.transform.localPosition = pos;
//         m_unit = MainObj.GetComponent<Unit>();
//         NavAgent = MainObj.AddComponent<NavMeshAgent>();
//         //m_fsm = new FSM(this);
//     }

//     public void CreateFSM(BaseObj owner) {
//        // FLDebugger.Log("OWNERRRRRRRRRRRR" + owner);
//        // m_fsm = new FSM(owner);
//     }

    // public BaseState GetFSMCurrentState() {
    //     return m_fsm.getCurrent();
    // }

    // public void SetFSMState(BaseState state)
    // {
    //     if (m_fsm != null)
    //     {
    //         m_fsm.ChangeState(state);
    //     }
    // }

    // public void UpdateFSM(float deltaTime) {
    //     if (m_fsm != null) {
    //         m_fsm.Update(deltaTime);
    //     }
    // }

//     //移动
//     public void stateRun()
//     {
//         if (string.IsNullOrEmpty(m_lastActionName) || m_lastActionName != "Run")
//         {
//             m_unit.SetAnimation("Run");
//         }
//     }

//     //待机
//     public void stateIdle()
//     {
//         if (string.IsNullOrEmpty(m_lastActionName) || m_lastActionName != "Idle")
//         {
//             m_unit.SetAnimation("Idle");
//         }
//     }
// }
