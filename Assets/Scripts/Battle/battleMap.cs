using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class battleMap : SlideLayer
{
    GameObject mainMap;


    public void initMapObj(GameObject mainC, GameObject secC, GameObject layerObj, Plane mapPlane)
    {
        mainMap = layerObj;

        initLayer(mainC,secC,layerObj,mapPlane);
    }


    void Start()
    {
        
        
    }

    
    void Update()
    {
        MoveUpdate();
    }

}