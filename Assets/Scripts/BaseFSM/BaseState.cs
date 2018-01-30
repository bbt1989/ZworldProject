using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BaseState
{
    public GameObject Owner;
    public delegate void stateCallback(bool param = false);
    public string StateName;

    public virtual void Execute(GameObject owner,float deltaTime) {

    }

    public virtual void EnterState(GameObject owner)
    {

    }
    public virtual void ExitState(GameObject owner)
    {

    }
}