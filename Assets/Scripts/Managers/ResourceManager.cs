using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManger 
{


    public void initManager()
    {

    }

    public UnityEngine.Object Load(string name, Type t)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        Type type = t;
        bool tIsComponent = (t.IsSubclassOf(typeof(UnityEngine.Component)) || t == typeof(UnityEngine.Component));
        object o = null;
        #region ResourcesLoad
        o = Resources.Load(name);
        #endregion
        #region AssetBundleLoad
        #endregion
        if (tIsComponent && o != null)
        {
            o = (o as GameObject).GetComponent(type);
        }
        return o as UnityEngine.Object;
    }



}  


