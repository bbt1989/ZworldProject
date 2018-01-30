using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;


//游戏中普遍使用一种配置
public class SlideLayerParams
{
    //手指操作参数
    
    //双指缩放速度
    public float touchScaleSpeed = 1f;
    //speed
    public float speed = 0.006f;
    
    //地图边界
    public float maxX = 230f;
    public float maxY = 230f;
    public float minX = 0f;
    public float minY = 10f;

    //地图弹性边界
    public float edgeMaxX = 230f - 0.5f;
    public float edgeMaxY = 230f - 0.5f;
    public float edgeMinX = 0f + 0.5f;
    public float edgeMinY = 10f + 0.5f;

    //镜头高度
    public int originY = 26;
    public int zoomY = 16;

    //高度弹性
    public float bounceYUp = 122f;
    public float bounceYDown = 112 + 1f;

    //X轴旋转角度
    public float rotateXMax = 45f;
    public float rotateXMin = 25f;

    //镜头尺寸
    public float cameraViewFieldMin = 24;
    public float cameraViewFieldMax = 32;

    //镜头离建筑距离
    public float buildingDis = 3f;

    //镜头拉近时的高度
    public float cameraHeight = 1f;

    //镜头拉近时间
    public float cameraPushTime = 1.5f;

    //建筑中心点高度
    public float buildingCenterHeight = 0.5f;

    //镜头离中心点位置
    public float cameraMoveRadius = 57f;

    //镜头缓动函数
    // public UnionMapController.EaseTypeEnum easeEnum;

}

public class SlideLayer : MonoBehaviour
{
    private GameObject layer;
    private GameObject mainCameraObj;
    public GameObject secCameraObj;
    private GameObject staticCameraObj;
    private Camera mainCamera;
    private Camera secCamera;
    private Camera staticCamera;

    protected GameObject CameraFocusObj;
    public float focusRadius;
    public float focusX;
    public float focusY;

    protected SlideLayerParams commonParams;

    //模拟双指点击位置
    public Vector3 simulatePoint = new Vector3();
    //上一帧双指中间点
    public Vector2? lastMidPoint = null;
    //根据中间点与Y值比例换算的地图世界坐标
    public Vector3? lastTransPoint = null;

    //是否双指
    private bool isDoubleFinger = false;

    //判断双击
    private bool isDoubleClick = false;
    //双击判断时间
    private float? doubleClickTime = null;
    //当前地图缩放状态
    private bool nowMapIsZoom = false;
    //当前地图是否正在缩放
    private bool nowMapIsZooming = false;
    //地图双击缩放量
    private float doubleClickZoomNum = 0.3f;
    //当前地图是否正在强制移动
    private bool mapForceMoving = false;

    //地图弹性区域状态
    private float? bounceLeftRight;
    private float? bounceUpDown;

    //双指上一帧距离
    private float lastFingerDis = 0f;

    private float? targetZoomY = null;

    //双指触摸开始时的Y值
    public float touchOriginY = 0f;
    //模拟双指上一帧的距离
    public float touchLastDis = 0f;

    protected bool mapIsMoving = false;
    protected bool mapCanDrag = false;
    //当前状态
    private string nowStatus = "none";
    //上一帧的x
    public float preX = 0f;
    //上一帧的y
    public float preY = 0f;

    //上一帧的手指在地图上的投射点
    private Vector3 lastPoint;

    //拖动帧数
    public uint dragFrame = 0;

    //结束拖动前一帧移动量
    public float lastMoveX = 0;
    public float lastMoveY = 0;

    //双指移动量
    private float lastMoveX1 = 0;
    private float lastMoveY1 = 0;
    private float lastMoveX2 = 0;
    private float lastMoveY2 = 0;
    private Vector2 touchPos1;
    private Vector2 touchPos2;

    //当前手指触摸状态
    public string nowTouchPhase = "none";
    //当前是否是双指
    public bool isMultiTouch = false;
    //双指触摸开始的位置
    public Vector3 fingerStartPos1 = new Vector3();
    public Vector3 fingerStartPos2 = new Vector3();
    //滑动 按下回调
    public delegate void SlideTouchCallback(bool isBlock, GameObject touchObj);
    public SlideTouchCallback touchCallback;
    //滑动 放开回调1
    public delegate void SlideReleaseCallback(bool isBlock, GameObject touchObj);
    public SlideReleaseCallback releaseCallback;
    //滑动 放开回调2
    public delegate void SlideReleaseCallbackImmediate();
    public SlideReleaseCallbackImmediate releaseCallbackImmediate;
    //滑动 拖动回调
    public delegate void SlideCameraMoveCallback(float nowx, float nowy, float preX, float preY, bool isWorldPoint);
    public SlideCameraMoveCallback camerMoveCallback;
    //滑动 缓动回调
    public delegate void SlideCameraEaseCallback(float lastMoveX, float lastMoveY);
    public SlideCameraEaseCallback cameraEaseCallback;
    //滑动 缩放回调
    public delegate bool SlideZoomCallback(float finaly, bool isoverride = false);
    public SlideZoomCallback zoomCallback;

    //开启双指模拟基本
    private bool baseFingerSimulationOpen = false;
    //模拟双指
    private GameObject simulate;
    //开始按钮
    private GameObject startButton;
    //状态label
    // private UILabel statusLabel;

    //地图平面
    private Plane plane;
    private float rate = 0f;

    //拖动层类型
    protected enum SlideType
    {
        BaseSlide, // 基本滑动层 大地图
        LockZ, // 主界面地图 Z轴小移动 缩放随着Y值变化摄像机视野大小与摄像机X轴旋转
        twoD, // 2d拖拽
    };

    //当前类型
    private SlideType nowType;

    //打开弹窗时需要隐藏的对象列表
    private GameObject[] hideObjList;

    // 2d
    private GameObject main2dMap;
    private Camera main2dCamera;
    private float mapWidth;
    private float mapHeight;
    private float map2dEdgeMaxX;
    private float map2dEdgeMinX;
    private float map2dEdgeMaxY;
    private float map2dEdgeMinY;
    public bool guideSwitch = false;


//     protected void initTwoD(GameObject mapObj)
//     {
//         nowType = SlideType.twoD;
//         main2dMap = mapObj;

//         float width, height;
//         UIWidget widget = mapObj.GetComponent<UIWidget>();
//         width = widget.width;
//         height = widget.height;

//         mapWidth = width;
//         mapHeight = height;

//         //         map2dEdgeMaxX = width* .5f - Screen.width * .5f;
//         //         map2dEdgeMinX = Screen.width * .5f - width* .5f;
//         // 
//         //         map2dEdgeMaxY = height * .5f - Screen.height * .5f;
//         //         map2dEdgeMinY = Screen.height * .5f - height * .5f;

//         map2dEdgeMaxX = width * .5f - 1280f * .5f;
//         map2dEdgeMinX = 1280f * .5f - width * .5f;

//         map2dEdgeMaxY = height * .5f - 720f * .5f;
//         map2dEdgeMinY = 720f * .5f - height * .5f;

//         FLDebugger.Log("SCREEN WIDTH + " + Screen.width * .5f + " SCREEN HEIGHT " + Screen.height * .5f);
//     }

    protected void initLayer(GameObject mainC, GameObject secC, GameObject layerObj, Plane mapPlane) {
        layer = layerObj;
        mainCameraObj = mainC;
        secCameraObj = secC;
        plane = mapPlane;
        nowType = SlideType.BaseSlide;
        // nowType = type;
        mainCamera = mainCameraObj.GetComponent<Camera>();
        secCamera = secCameraObj.GetComponent<Camera>();
        commonParams = new SlideLayerParams();

        
        staticCameraObj = GameObject.Find("MapCameraSlider");
        staticCamera = staticCameraObj.GetComponent<Camera>();
    }


    protected void MoveUpdate()
    {
        if (CameraFocusObj != null) {
            float x = layer.transform.position.x;
            float y = layer.transform.position.y;
            float z = layer.transform.position.z;

            CameraFocusObj.transform.position = new Vector3(x + focusX, y + focusY, z + focusRadius);
        }

        
        //Debug.Log("UPDATE" + nowMapIsZooming + " UICAMERA " +  " nowStatus " + nowStatus);
        //判断是否按在UI上
        //判断当前是否正在缩放
        //鼠标操作
        if (Input.touchCount <= 0)
        {
            nowTouchPhase = "none";
            
            isDoubleFinger = false;
            lastFingerDis = 0f;
            //滚轮操作
            
            if (Input.GetMouseButtonDown(0) && !nowMapIsZooming)
            {
                lastMidPoint = null;
                lastTransPoint = null;
                targetZoomY = null;
                //FLDebugger.Log("TOUCHSTART");
                //                 preX = Input.mousePosition.x;
                //                 preY = Input.mousePosition.y;
                lastPoint = getNowTouchMapPoint(Input.mousePosition);
                preX = lastPoint.x;
                preY = lastPoint.z;

                mapCanDrag = true;

                mainCameraObj.transform.DOKill(false);
                secCameraObj.transform.DOKill(false);

                nowMapIsZooming = false;

                bool hasBlocked = false;
                //Debug.Log("isWindowOpen" + isWindowOpen + " mapIsMoving " + mapIsMoving);
                //Debug.Log("mapIsMoving __ " + mapIsMoving);
                if (!mapIsMoving)
                {
                    //var ray = ScreenPointToRay(Input.mousePosition);
                    var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                    RaycastHit hit;
                    hasBlocked = Physics.Raycast(ray, out hit, 1000f);

                    if (touchCallback != null && hasBlocked)
                    {
                        touchCallback(hasBlocked, hit.transform.gameObject);
                    }
                }
            }
            // && !UICamera.Raycast(Input.mousePosition)
            // FLDebugger.Log("camerMoveCallback " + camerMoveCallback + " mapCanDrag" + mapCanDrag + " nowStatus " + nowStatus);
            if (Input.GetMouseButton(0) && mapCanDrag && nowStatus == "none")
            {
                //Debug.Log("IS CLICK ON UI" + UICamera.Raycast(Input.mousePosition));
                //获取摄像机旋转角度
                // float cAngle = MapCameraObj.transform.eulerAngles.x;

                //float nowy = Input.mousePosition.y;
                // FLDebugger.Log("NOWXXXXXXXXXXXX" + nowx);
                //float nowx = worldPoint.x;
                //          float nowy = worldPoint.z;
                dragFrame++;
                dragFrame = Math.Min(1, dragFrame);
                float nowx;
                float nowy;
                Vector3 nowTouch = new Vector3();
                getNowXNowY(out nowx, out nowy, out nowTouch);
            
                //FLDebugger.Log("NOWX" + nowTouch.x + " NOWY " + nowTouch.z + " LASTX " + lastPoint.x + " LASTY " + lastPoint.z);
                //FLDebugger.Log("DISSSSSSSSSSSSSSSSSSS " + Math.Abs(preX - nowx));
                //判断地图是否拖拽
                if (Math.Abs(preX - nowx) >= 0.2f || Math.Abs(preY - nowy) >= 0.2f)
                {
                    mapIsMoving = true;
                    doubleClickTime = null;
                    nowMapIsZooming = false;
                }

                //setCameraPos(nowx, nowy);

                //根据触摸点在地图上的坐标移动

                if (nowStatus == "none")
                {
                    if (camerMoveCallback != null)
                    {
                        //FLDebugger.Log("____________________________");
                        camerMoveCallback(nowx, nowy, preX, preY, false);
                    }
                    else {
                        setCameraPos(nowx, nowy, preX, preY, true);
                    }
                }
             
                lastPoint = nowTouch;
                    preX = lastPoint.x;
                    preY = lastPoint.z;
                
            }
            if (Input.GetMouseButtonUp(0) && !(nowStatus == "end"))
            {
                if (releaseCallbackImmediate != null)
                {
                    releaseCallbackImmediate();
                }
                //全部变亮列表中全部变暗
                //                     for (int i = 0; i < touchMapList.Count; i++)
                //                     {
                //                         touchMapList[i] = "dark";
                //                     }

                mapCanDrag = false;
                dragFrame = 0;
                if (nowStatus == "none")
                {
                    //抬起镜头缓动
                    //                     if (cameraEaseCallback != null)
                    //                     {
                    //                         cameraEaseCallback(lastMoveX, lastMoveY);
                    //                     }
                    cameraEaseMove(lastMoveX, lastMoveY);
                }

                lastMoveX = 0;
                lastMoveY = 0;

                //Debug.Log("mapIsMoving" + mapIsMoving);
                //判断是否有点中建筑
                //移动地图时不判断

                bool hasBlocked = false;
                //Debug.Log("isWindowOpen" + isWindowOpen + " mapIsMoving " + mapIsMoving);
                if (!mapIsMoving)
                {
                    //var ray = ScreenPointToRay(Input.mousePosition);
                    var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                    RaycastHit hit;
                    hasBlocked = Physics.Raycast(ray, out hit, 100f);
                    if (releaseCallback != null && hasBlocked)
                    {
                        releaseCallback(hasBlocked, hit.transform.gameObject);
                    }
                }

                mapIsMoving = false;
            }
            else if (nowStatus == "end" && Input.GetMouseButtonUp(0))
            {
                nowStatus = "none";
            }

        }
        else if (Input.touchCount > 0)
        {
            mainCameraObj.transform.DOKill(false);
            secCameraObj.transform.DOKill(false);
            // && !nowMapIsZooming && !mapIsMoving
            //双指操作 缩放
            if (Input.touchCount == 2){
                isDoubleFinger = true;
                if (baseFingerSimulationOpen) {
                    //开始双指触摸
                    if (Input.GetTouch(1).phase == TouchPhase.Began)
                    {
                        nowTouchPhase = "zoom";
                        fingerStartPos1 = Input.GetTouch(0).position;
                        fingerStartPos2 = Input.GetTouch(1).position;
                        touchOriginY = mainCameraObj.transform.localPosition.y;
                        isMultiTouch = true;
                    }
                    else if (Input.GetTouch(1).phase == TouchPhase.Moved && isDoubleFinger)
                    {
                        nowTouchPhase = "zoom";
                        Touch touch1 = Input.GetTouch(0);
                        Touch touch2 = Input.GetTouch(1);
                        touchPos1 = touch1.position;
                        touchPos2 = touch2.position;

                        dragFrame++;
                        dragFrame = Math.Min(1, dragFrame);

                        if (dragFrame >= 1)
                        {
                            lastMoveX1 = (fingerStartPos1.x - touchPos1.x) * commonParams.speed * .5f;
                            lastMoveY1 = (fingerStartPos1.y - touchPos1.y) * commonParams.speed * .5f;

                            lastMoveX2 = (fingerStartPos2.x - touchPos2.x) * commonParams.speed * .5f;
                            lastMoveY2 = (fingerStartPos2.y - touchPos2.y) * commonParams.speed * .5f;
                        }
                        //初始双指距离
                        float distanceOrigin = Vector2.Distance(fingerStartPos1, fingerStartPos2);
                        //当前双指距离
                        float distanceNow = Vector2.Distance(touchPos1, touchPos2);

                        lastMidPoint = new Vector2((touchPos1.x + touchPos2.x) / 2, (touchPos1.y + touchPos2.y) / 2);

                        //Debug.Log("lastMidPoint" + ((Vector2)lastMidPoint).x + " " + ((Vector2)lastMidPoint).y + " x " + (touchPos1.x + touchPos2.x) / 2 + " y " + (touchPos1.y + touchPos2.y) / 2);
                        float finalY = touchOriginY + (distanceOrigin - distanceNow) / 100 * commonParams.touchScaleSpeed;
                        //float finalY = Mathf.Lerp(touchOriginY, touchOriginY + (distanceOrigin - distanceNow) / 100 * commonParams.touchScaleSpeed,0.8f);
                        targetZoomY = Mathf.Clamp(finalY,commonParams.zoomY,commonParams.originY);

                        fingerZoomCamera(finalY, true);
                    }
                    else if (Input.GetTouch(1).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        //zoomBounce();
                        nowTouchPhase = "none";
                        //lastMidPoint = null;
                        //lastTransPoint = null;
                        // FLDebugger.Log("双指结束双指结束双指结束双指结束双指结束双指结束");
                    }
                }
            }
            //单指操作
            else if (Input.touchCount == 1 && !isDoubleFinger)
            {
                nowTouchPhase = "none";
                lastFingerDis = 0f;
                lastMidPoint = null;
                lastTransPoint = null;

                targetZoomY = null;
                lastTransPoint = null;
                //Debug.Log("Input.GetTouch(0).phase" + Input.GetTouch(0).phase);
                Touch touch1 = Input.GetTouch(0);
                Vector2 touchPos1 = touch1.position;
                //根据触摸状态
                //开始触摸
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    nowTouchPhase = "move";
                    //nowTouchPhase = "none";
                    //                     preX = Input.GetTouch(0).position.x;
                    //                     preY = Input.GetTouch(0).position.y;

                    lastPoint = getNowTouchMapPoint(Input.GetTouch(0).position);
                        preX = lastPoint.x;
                        preY = lastPoint.z;

                    mapCanDrag = true;
                    mainCameraObj.transform.DOKill(true);
                    secCameraObj.transform.DOKill(true);
                    nowMapIsZooming = false;

                    bool hasBlocked = false;
                    //Debug.Log("isWindowOpen" + isWindowOpen + " mapIsMoving " + mapIsMoving);
                    if (!mapIsMoving)
                    {
                        //var ray = ScreenPointToRay(Input.mousePosition);
                        var ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);

                        RaycastHit hit;
                        hasBlocked = Physics.Raycast(ray, out hit, 1000f);
                        if (touchCallback != null && hasBlocked)
                        {
                            touchCallback(hasBlocked, hit.transform.gameObject);
                        }
                    }
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Moved && mapCanDrag)
                {
                    if (!isDoubleFinger)
                    {
                        nowTouchPhase = "move";
                        //                         float nowx = Input.GetTouch(0).position.x;
                        //                         float nowy = Input.GetTouch(0).position.y;

                        dragFrame++;
                        dragFrame = Math.Min(1, dragFrame);

                        float nowx;
                        float nowy;
                        Vector3 nowTouch = new Vector3();
                        getNowXNowY(out nowx, out nowy, out nowTouch);
                        
//                         if (dragFrame >= 1)
//                         {
//                             lastMoveX = (preX - nowx) * commonParams.speed * 1.5f;
//                             lastMoveY = (preY - nowy) * commonParams.speed * 1.5f;
//                         }

                        if (Math.Abs(preX - nowx) >= 0.2f || Math.Abs(preY - nowy) >= 0.2f)
                        {
                            mapIsMoving = true;
                            doubleClickTime = null;
                            nowMapIsZooming = false;
                        }

                        //根据触摸点在地图上的坐标移动
                        if (camerMoveCallback != null)
                        {
                            camerMoveCallback(nowx, nowy, preX, preY, false);
                        }
                        else {
                            setCameraPos(nowx, nowy, preX, preY, true);
                        }
                        //                         preX = Input.GetTouch(0).position.x;
                        //                         preY = Input.GetTouch(0).position.y;
                        lastPoint = nowTouch;
                            preX = lastPoint.x;
                            preY = lastPoint.z;
                    }
                }
                // && !isDoubleFinger
                else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    //全部变亮列表中全部变暗
                    //                         for (int i = 0; i < touchMapList.Count; i++)
                    //                         {
                    //                             touchMapList[i] = "dark";
                    //                         }
                    if (releaseCallbackImmediate != null)
                    {
                        releaseCallbackImmediate();
                    }
                    mapCanDrag = false;
                    dragFrame = 0;
                    lastFingerDis = 0f;
                    //抬起镜头缓动
                    cameraEaseMove(lastMoveX, lastMoveY);
                    //                     if (cameraEaseCallback != null)
                    //                     {
                    //                         cameraEaseCallback(lastMoveX, lastMoveY);
                    //                     }
                    //zoomBounce();
                    lastMoveX = 0;
                    lastMoveY = 0;

                    //Debug.Log("mapIsMoving" + mapIsMoving);
                    //判断是否有点中建筑
                    //移动地图时不判断

                    bool hasBlocked = false;
                    //Debug.Log("isWindowOpen" + isWindowOpen + " mapIsMoving " + mapIsMoving);
                    if (!mapIsMoving && !isDoubleFinger)
                    {
                        //var ray = ScreenPointToRay(Input.mousePosition);
                        var ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);

                        RaycastHit hit;
                        hasBlocked = Physics.Raycast(ray, out hit, 1000f);
                        if (releaseCallback != null && hasBlocked)
                        {
                            releaseCallback(hasBlocked, hit.transform.gameObject);
                        }
                        //                             if (hasBlocked)
                        //                             {
                        //                                 isDoubleClick = false;
                        //                                 string name = hit.transform.gameObject.name;
                        //                                 //判断点中的格子
                        //                                 //openStageQuestInfoPanel(hit.transform.gameObject);
                        //                             }
                    }

                    //判断双击
                    if (!hasBlocked && !mapIsMoving)
                    {
                        //第一次点击
                        if (doubleClickTime == null)
                        {
                            doubleClickTime = 0f;
                        }
                        else
                        {
                            //第二次点击
                            //                             isDoubleClick = true;
                            //                             doubleClickTime = null;
                            //                             mapZooming();
                        }
                    }
                    isDoubleFinger = false;
                    nowTouchPhase = "none";
                    mapIsMoving = false;
                }
            }
        }
    }


    public void setCameraPos(float nowx, float nowy, float px, float py, bool isWorldPoint, float time = 0f)
    {
        if (guideSwitch)return;
        //FLDebugger.Log("NOWSSSSSSSSSSSSSSSSSSS"+ nowx);
        //获取摄像机位置
        float x, y, z;
        x = mainCameraObj.transform.localPosition.x;
        y = mainCameraObj.transform.localPosition.y;
        z = mainCameraObj.transform.localPosition.z;

        float targetX;
        float targetZ;

        float offsetX, offsetZ;
        //         offsetX = (preX - nowx) * speed;
        //         offsetZ = (preY - nowy) * speed;

        if (!isWorldPoint)
        {
            offsetX = (px - nowx) / 100;
            offsetZ = (py - nowy) / 100;
        }
        else
        {
            offsetX = (px - nowx);
            offsetZ = (py - nowy);
        }

        //Debug.Log(" offsetX " + offsetX + " offsetZ " + offsetZ);

        Vector3 bottomleft, bottomright, topleft, topright;
        bottomleft = getCameraEdge()[0];
        bottomright = getCameraEdge()[1];
        topleft = getCameraEdge()[2];
        topright = getCameraEdge()[3];
        // Debug.Log(" bottomleft " + bottomleft + " bottomright " + bottomright);
        //弹性边界
        if (topright.x + offsetX > commonParams.edgeMaxX || topleft.x + offsetX < commonParams.edgeMinX)
        {
            offsetX = offsetX / 5;
            if (topright.x + offsetX > commonParams.edgeMaxX)
            {
                bounceLeftRight = commonParams.edgeMaxX - (topright.x + offsetX);
            }
            else if (topleft.x + offsetX < commonParams.edgeMinX)
            {
                bounceLeftRight = commonParams.edgeMinX - (topleft.x + offsetX);
            }
        }
        if (topleft.z + offsetZ > commonParams.edgeMaxY || topright.z + offsetZ > commonParams.edgeMaxY || bottomleft.z + offsetZ < commonParams.edgeMinY)
        {
            offsetZ = offsetZ / 5;
            if (topleft.z + offsetZ > commonParams.edgeMaxY || topright.z + offsetZ > commonParams.edgeMaxY)
            {
                bounceUpDown = commonParams.edgeMaxY - (topleft.z + offsetZ);
            }
            else if (bottomleft.z + offsetZ < commonParams.edgeMinY)
            {
                bounceUpDown = commonParams.edgeMinY - (bottomleft.z + offsetZ);
            }
        }
        // Debug.Log(
        //"bounceLeftRight " + bounceLeftRight + " bounceUpDown: " + bounceUpDown + 
        //" offsetX: " + offsetX + " offsetZ: " + offsetZ + " topright.x " + topright.x + " topleft.z " + topleft.z);
        //判断两边顶点
        //如果左边顶点x加上offsetX小于minX
        if (topleft.x + offsetX < commonParams.minX || topright.x + offsetX > commonParams.maxX)
        {
            offsetX = 0;
        }
        //如果左边顶点z加上offsetZ大于maxY
        if (topleft.z + offsetZ > commonParams.maxY || topright.z + offsetZ > commonParams.maxY || bottomleft.z + offsetZ < commonParams.minY)
        {
            offsetZ = 0;
        }

        targetX = x + offsetX;
        targetZ = z + offsetZ;

        mainCameraObj.transform.localPosition = new Vector3(targetX, y, targetZ);
        secCameraObj.transform.localPosition = new Vector3(targetX, y, targetZ);
        //如果是lockZ
        // if (nowType == SlideType.LockZ)
        // {
        //     //Z轴量随Y值变
        //     targetZ = 0;
        //     lastMoveY = 0;
        //     bounceUpDown = null;

        //     float cameray = mainCameraObj.transform.localPosition.y;

        //     float yRate = (cameray - commonParams.zoomY) / (commonParams.originY - commonParams.zoomY);
        //     float nowRotate = commonParams.rotateXMin + (commonParams.rotateXMax - commonParams.rotateXMin) * yRate;
        //     //mainCameraObj.transform.Rotate(new Vector3(nowRotate,0,0));
        //     //secCameraObj.transform.Rotate(new Vector3(nowRotate, 0, 0));
        //     //mainCameraObj.transform.eulerAngles.x = nowRotate;
        //     mainCameraObj.transform.eulerAngles = new Vector3(nowRotate, 0f, 0f);

        //     //限制Z轴
        //     float cameraz = (commonParams.maxY - yRate * (commonParams.maxY - commonParams.minY));
        //     mainCameraObj.transform.localPosition = new Vector3(targetX, y, cameraz);
        //     secCameraObj.transform.localPosition = new Vector3(targetX, y, cameraz);

        //     //mainCamera.fieldOfView = commonParams.cameraViewFieldMin + yRate * (commonParams.cameraViewFieldMax - commonParams.cameraViewFieldMin);
        //     //secCameraObj.transform.eulerAngles = new Vector3(nowRotate, 0f, 0f);
        // }
        // else {
        //     //             targetX = Mathf.Lerp(x, targetX, Time.deltaTime * 6f);
        //     //             targetZ = Mathf.Lerp(z, targetZ, Time.deltaTime * 6f);
        //     mainCameraObj.transform.localPosition = new Vector3(targetX, y, targetZ);
        //     secCameraObj.transform.localPosition = new Vector3(targetX, y, targetZ);
        // }

    }

    private Vector3 getNowTouchMapPoint(Vector3 vec) {
        Ray midRay = staticCamera.ScreenPointToRay(vec);
        Vector3 nowTouch = new Vector3();
        GetRayPlaneIntersection(ref plane, midRay, out nowTouch);
        return nowTouch;
    }

    protected void GetRayPlaneIntersection(ref Plane plane, Ray ray, out Vector3 intersection)
    {
        float enter;
        if (!plane.Raycast(ray, out enter))
        {
            intersection = Vector3.zero;
        }
        // 下面是获取t的公式
        // 注意，你需要先判断射线与平面是否平行，如果平面和射线平行，那么平面法线和射线方向的点积为0，即除数为0.
        //float t = (Vector3.Dot(normal, planePoint) - Vector3.Dot(normal, ray.origin)) / Vector3.Dot(normal, ray.direction.normalized);
        if (enter >= 0)
        {
            intersection = ray.origin + enter * ray.direction.normalized;
        }
        else
        {
            intersection = Vector3.zero;
        }
    }


    //获取摄像机边界
    protected List<Vector3> getCameraEdge()
    {
        List<Vector3> corner = new List<Vector3>();
        // 根据摄像机视野范围与地图的交点
        //Plane plane = new Plane(Vector3.up, new Vector3(0, 100, 0));
        Ray rayBL = secCamera.ViewportPointToRay(new Vector3(0, 0, 1));     // bottom left
        Ray rayBR = secCamera.ViewportPointToRay(new Vector3(1, 0, 1));     // bottom right
        Ray rayTL = secCamera.ViewportPointToRay(new Vector3(0, 1, 1));     // top left
        Ray rayTR = secCamera.ViewportPointToRay(new Vector3(1, 1, 1));     // top right

        Vector3 bottomLeftPoiont, bottomRightPoiont, topLeftPoiont, topRightPoiont;

        GetRayPlaneIntersection(ref plane, rayBL, out bottomLeftPoiont);
        GetRayPlaneIntersection(ref plane, rayBR, out bottomRightPoiont);
        GetRayPlaneIntersection(ref plane, rayTL, out topLeftPoiont);
        GetRayPlaneIntersection(ref plane, rayTR, out topRightPoiont);

        //FLDebugger.Log("bottom left:" + bottomLeftPoiont.x +"-"+ bottomLeftPoiont.z + " "
        //         + "bottom right:" + bottomRightPoiont.x + "-" + bottomRightPoiont.z + " "
        //          + "top left:" + topLeftPoiont.x + "-" + topLeftPoiont.z + " "
        //          + "top right:" + topRightPoiont.x + "-" + topRightPoiont.z + " ");

        //根据获取到的四个交点限制移动
        corner.Add(bottomLeftPoiont);
        corner.Add(bottomRightPoiont);
        corner.Add(topLeftPoiont);
        corner.Add(topRightPoiont);

        return corner;
    }

    public void cameraEaseMove(float lastMoveX, float lastMoveY)
    {
        if (guideSwitch)return;
        //         if (Math.Abs(lastMoveX) >= 0.05f || Math.Abs(lastMoveY) >= 0.05f)
        //         {
        //Debug.Log("bounceLeftRight " + bounceLeftRight + " bounceUpDown " + bounceUpDown);
        if (Math.Abs(lastMoveX) >= 0.05f || Math.Abs(lastMoveY) >= 0.05f)
        {
            float x, y, z;
            x = secCameraObj.transform.localPosition.x;
            y = secCameraObj.transform.localPosition.y;
            z = secCameraObj.transform.localPosition.z;
            float targetx, targetz;

            //判断边界

            //获取上一帧镜头边界
            List<Vector3> cameraEdge = getCameraEdge();
            Vector3 bottomleft = cameraEdge[0];
            Vector3 bottomright = cameraEdge[1];
            Vector3 topleft = cameraEdge[2];
            Vector3 topright = cameraEdge[3];
            //判断两边顶点
            //如果左边顶点x加上offsetX小于minX
            if (topleft.x + lastMoveX < commonParams.edgeMinX)
            {
                lastMoveX = commonParams.edgeMinX - topleft.x;
            }
            //如果左边顶点z加上offsetZ大于maxY
            if (topleft.z + lastMoveY > commonParams.edgeMaxY)
            {
                lastMoveY = commonParams.edgeMaxY - topleft.z;
            }
            //如果右边顶点x加上offsetX大于maxX
            if (topright.x + lastMoveX > commonParams.edgeMaxX)
            {
                lastMoveX = commonParams.edgeMaxX - topright.x;
            }
            //如果右边顶点z加上offsetZ大于maxY
            if (topright.z + lastMoveY > commonParams.edgeMaxY)
            {
                lastMoveY = commonParams.edgeMaxY - topright.z;
            }
            //如果左边顶点z 加上offsetZ小于minY
            if (bottomleft.z + lastMoveY < commonParams.edgeMinY)
            {
                lastMoveY = commonParams.edgeMinY - bottomleft.z;
            }

            targetx = x + lastMoveX;
            targetz = z + lastMoveY;

            if (nowType == SlideType.LockZ)
            {
                targetz = z;
            }
            //Debug.Log("EASE?????????????????????????????????????????????????");
            Tweener move = mainCameraObj.transform.DOLocalMove(new Vector3(targetx, y, targetz), 1.5f);
            Tweener move2 = secCameraObj.transform.DOLocalMove(new Vector3(targetx, y, targetz), 1.5f);
            move.SetEase(Ease.OutExpo);
            move2.SetEase(Ease.OutExpo);
        }
        else if (bounceLeftRight != null || bounceUpDown != null)
        {
            // FLDebugger.Log("L--O--G");
            float x, y, z;
            x = secCameraObj.transform.localPosition.x;
            y = secCameraObj.transform.localPosition.y;
            z = secCameraObj.transform.localPosition.z;
            float targetx, targetz;
            if (bounceLeftRight != null)
            {
                targetx = x + (float)bounceLeftRight;
            }
            else
            {
                targetx = x;
            }
            if (bounceUpDown != null)
            {
                targetz = z + (float)bounceUpDown;
            }
            else
            {
                targetz = z;
            }
            Tweener move = mainCameraObj.transform.DOLocalMove(new Vector3(targetx, y, targetz), 0.5f);
            Tweener move2 = secCameraObj.transform.DOLocalMove(new Vector3(targetx, y, targetz), 0.5f);
            move.SetEase(Ease.OutExpo);
            move2.SetEase(Ease.OutExpo);
            // Debug.Log("BOUNCE?????????????????????????????????????????????????");
            bounceLeftRight = null;
            bounceUpDown = null;
        }

    }

    private void getNowXNowY(out float nowx, out float nowy, out Vector3 nowTouch) {
        if (nowType == SlideType.LockZ || nowType == SlideType.twoD)
        {
            nowTouch = new Vector3();
            nowx = Input.mousePosition.x;
            nowy = Input.mousePosition.y;
            if (dragFrame >= 1)
            {
                lastMoveX = (preX - nowx) * 1.5f;
                lastMoveY = (preY - nowy) * 1.5f;
            }
        }
        else
        {
            nowTouch = getNowTouchMapPoint(Input.mousePosition);
            nowx = nowTouch.x;
            nowy = nowTouch.z;
            if (dragFrame >= 1)
            {
                lastMoveX = (preX - nowx) * commonParams.speed * 100;
                lastMoveY = (preY - nowy) * commonParams.speed * 100;
            }
        }
    }
        //根据量缩放
    private bool fingerZoomCamera(float offsety, bool isOverride = false)
    {
        bool ismaxmin = false;
        if (isOverride)
        {
            float x, y, z;
            x = mainCameraObj.transform.localPosition.x;
            y = mainCameraObj.transform.localPosition.y;
            z = mainCameraObj.transform.localPosition.z;

            //判断上下边界
            if (offsety > commonParams.originY)
            {
                offsety = commonParams.originY;
            }
            else if (offsety < commonParams.zoomY)
            {
                offsety = commonParams.zoomY;
            }
            //float speed = 1f;
            //offsety = Mathf.SmoothDamp(y, offsety, ref speed, 0.1f);
            // offsety = Mathf.Lerp(y,offsety,Time.deltaTime * 6);
            //mainCameraObj.transform.localPosition = new Vector3(x, offsety, z);

            float sx = staticCameraObj.transform.position.x;
            float sz = staticCameraObj.transform.position.z;
            staticCameraObj.transform.localPosition = new Vector3(sx, offsety, sz);
        }
        else
        {
            float x, y, z;
            x = mainCameraObj.transform.localPosition.x;
            y = mainCameraObj.transform.localPosition.y;
            z = mainCameraObj.transform.localPosition.z;

            float targety = y + offsety;
            // FLDebugger.Log("TARGETTTTTTTYYYY" + y);
            //FLDebugger.Log("targety"+targety+"clamp"+ Mathf.Clamp(targety, originY, zoomY));
            if (targety <= commonParams.zoomY)
            {
                targety = commonParams.zoomY;
                ismaxmin = true;
                //双击缩放结束
                nowMapIsZooming = false;
            }
            else if (targety >= commonParams.originY)
            {
                targety = commonParams.originY;
                ismaxmin = true;
                //双击缩放结束
                nowMapIsZooming = false;
            }
            // FLDebugger.Log("TARGETTTTTTTYYYY2222222" + targety);
            //判断边界
            Vector3 bottomleft, bottomright, topleft, topright;
            bottomleft = getCameraEdge()[0];
            bottomright = getCameraEdge()[1];
            topleft = getCameraEdge()[2];
            topright = getCameraEdge()[3];

            bool canZoom = !(((topleft.x <= commonParams.minX) || (topleft.z >= commonParams.maxY) || (topright.x >= commonParams.maxX)
               || (topright.z >= commonParams.maxY) || (bottomleft.z <= commonParams.minY)) && offsety > 0);

            if (canZoom)
            {
                mainCameraObj.transform.localPosition = new Vector3(x, targety, z);
            }
        }

        //         float tempy = mainCameraObj.transform.localPosition.y;
        //         rate = (commonParams.originY - tempy) / (commonParams.originY - commonParams.zoomY);
        //         //根据Y值比例换算 
        //         moveCameraByMidPointY();
        return ismaxmin;
    }
}
