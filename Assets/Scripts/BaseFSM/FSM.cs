using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FSM
{
    //当前状态
    private BaseState m_NowState;
    //上一个状态
    private BaseState m_PreState;
    //状态机本体
    private GameObject stateOwner;

    //构造 owner 本体
    public FSM(GameObject owner) {
        if (owner != null) {
            stateOwner = owner;
        }
        //FLDebugger.Log("stateOwner" + stateOwner);
    }

    public BaseState getCurrent() {
        return m_NowState;
    }

    public GameObject getOwner()
    {
        return stateOwner;
    }

    public string getOwnerName()
    {
        return stateOwner.name;
    }


    //在mono的update中调用
    public void Update(float deltaTime) {
        //执行当前状态
        if (m_NowState != null) {
            m_NowState.Execute(stateOwner, deltaTime);
        }
    }

    public void ChangeState(BaseState currentState = null) {
        if (m_NowState != null) {
            m_NowState.ExitState(stateOwner);
        }
        //保存上一次状态
        //m_PreState = m_NowState;
        m_NowState = currentState;
        if (m_NowState != null) {
            m_NowState.EnterState(stateOwner);
        }
    }
}