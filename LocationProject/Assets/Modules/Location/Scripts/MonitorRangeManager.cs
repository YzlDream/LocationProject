using Location.WCFServiceReferences.LocationServices;
using RTEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types = Location.WCFServiceReferences.LocationServices.AreaTypes;

/// <summary>
/// 监控范围（区域范围）
/// </summary>
namespace MonitorRange
{
    public class MonitorRangeManager : MonoBehaviour
    {
        public static MonitorRangeManager Instance;
        /// <summary>
        /// 显示范围Render
        /// </summary>
        public bool isShowRangeRender = true;
        /// <summary>
        /// 监控范围列表
        /// </summary>
        private List<MonitorRangeObject> rangelist;
        /// <summary>
        /// 区域预设
        /// </summary>
        public GameObject areaPrefab;
        /// <summary>
        /// 区域飘浮文字UI;
        /// </summary>
        public GameObject NameUI;
        /// <summary>
        /// 区块物体父物体
        /// </summary>
        private Transform areasParent;
        /// <summary>
        /// 根区域范围节点
        /// </summary>
        public RangeNode rootRangeNode;
        /// <summary>
        /// 区域信息根节点
        /// </summary>
        public DepNode rootDepNode;
        /// <summary>
        /// 是否是区域编辑状态
        /// </summary>
        public bool IsEditState;
        /// <summary>
        /// 是否显示告警区域
        /// </summary>
        private bool isShowAlarmArea;
        /// <summary>
        /// 是否显示告警区域
        /// </summary>
        public bool IsShowAlarmArea
        {
            get
            {
                return isShowAlarmArea;
            }

            set
            {
                isShowAlarmArea = value;
            }
        }

        // Use this for initialization
        void Start()
        {
            Instance = this;
            rangelist = new List<MonitorRangeObject>();
            //ShowAreas();

            SceneEvents.DepNodeChanged += SceneEventsDepNodeChanged;
            SceneEvents.BuildingOpenStartAction += SceneEvents_BuildingOpenStartAction;
            SceneEvents.BuildingOpenCompleteAction += SceneEvents_BuildingOpenCompleteAction;
            SceneEvents.BuildingStartCloseAction += SceneEvents_BuildingStartCloseAction;
            SceneEvents.BuildingCloseCompleteAction += SceneEvents_BuildingCloseCompleteAction;
        }

        /// <summary>
        /// 大楼开始合拢监听方法
        /// </summary>
        private void SceneEvents_BuildingStartCloseAction()
        {
            Debug.Log("大楼开始合拢了！");
            HideAllRanges();
        }

        /// <summary>
        /// 大楼开始展开监听方法
        /// </summary>
        private void SceneEvents_BuildingOpenStartAction()
        {
            Debug.Log("大楼开始展开了！");
            HideAllRanges();
        }

        /// <summary>
        /// 大楼合拢完毕监听方法
        /// </summary>
        private void SceneEvents_BuildingCloseCompleteAction()
        {
            Debug.Log("大楼合拢完毕了！");

            //else
            //{
            FilterRangesByMRObject(SceneEvents.DepNode);
            //}
            if (IsEditState)
            {
                ShowAreaEdit(SceneEvents.DepNode);
            }
            SetMonitorRangeFollowPosition(FactoryDepManager.currentDep);
        }

        /// <summary>
        /// 大楼展开完毕监听方法
        /// </summary>
        private void SceneEvents_BuildingOpenCompleteAction()
        {
            Debug.Log("大楼展开了完毕！");
            //else
            //{
            FilterRangesByMRObject(SceneEvents.DepNode);
            //}

            if (IsEditState)
            {
                ShowAreaEdit(SceneEvents.DepNode);
            }
            SceneEvents.DepNode.monitorRangeObject.SetRendererEnable(false);//把大楼的区域范围隐藏掉
            SetMonitorRangeFollowPosition(FactoryDepManager.currentDep);
        }

        private void SceneEventsDepNodeChanged(DepNode arg1, DepNode arg2)
        {
            //Log.Info("MonitorRangeManager.SceneEvents_DepModeChanged");
            Debug.Log("SceneEventsDepNodeChanged！");
            if (arg1 != null)
            {
                Log.Info("LastDep:" + arg1);
            }
            if (arg2 != null)
            {
                Log.Info("CurrentDep:" + arg2);
                //else
                //{
                ShowRanges(arg2);
                //}
                if (IsEditState)
                {
                    ShowAreaEdit(arg2);
                }
            }
        }

        Transform rootparent;

        /// <summary>
        /// 创建所有区域范围
        /// </summary>
        public void CreateAllRanges()
        {
            rootDepNode = FactoryDepManager.Instance;
            PhysicalTopology topoNode = rootDepNode.TopoNode;
            rootparent = CreateRangeParent(); //区域根物体
            rootRangeNode = new RangeNode();
            CreateRangesByRootNode(topoNode, rootRangeNode, rootparent);
            if (!isShowRangeRender)
            {
                HideAllRanges(true);
            }
        }

        /// <summary>
        /// 创建定位区域
        /// create by cww
        /// </summary>
        /// <param name="node"></param>
        public void CreateRanges(DepNode node)
        {

            Log.Info("CreateChilrenRange", node.ToString());
            //ClearRanges();
            PhysicalTopology topoNode = node.TopoNode;
            if (topoNode == null)
            {
                Log.Alarm("CreateChilrenRange", "topoNode == null");
                return;
            }
            if (node.depType == DepType.Factory)//厂区
            {
                //CreateRanges(FilterParkMonitorRange(topoNode.Children));
                Transform pparent = CreateRangeParent(); //区域根物体
                rootRangeNode = new RangeNode();
                CreateRangesByRootNode(topoNode, rootRangeNode, pparent);
            }
            else if (topoNode.Type == Types.楼层)
            {
                //CreateChilrenRange_Floor(node);
            }
            else if (topoNode.Type == Types.大楼)
            {
                //CreateChilrenRange_Building(node);
            }
            else
            {
                Log.Alarm("CreateChilrenRange", "不是楼层和厂区");
            }
        }

        /// <summary>
        /// 创建所有区域范围根据顶部节点（递归）
        /// </summary>
        /// <param name="pNode">数据节点</param>
        /// <param name="rangeNode">区域范围节点</param>
        /// <param name="pparentT">父物体</param>
        public void CreateRangesByRootNode(PhysicalTopology pNode, RangeNode rangeNode, Transform pparentT)
        {
            Transform pparent = pparentT; //区域根物体
            //Transform content = CreateRangeParent(pNode.Type.ToString() + "(" + pNode.Id + ")" + ":" + pNode.Name, pparent); //创建父物体
            Transform content = null;
            rangeNode.info = pNode;
            rangeNode.ranges = new List<MonitorRangeObject>();
            rangeNode.subNodes = new List<RangeNode>();
            if (pNode.Name.Contains("调压站控制室"))
            {
                int i = 0;
            }
            if (pNode.Name.Contains("集控楼"))
            {
                int i = 0;
            }
            MonitorRangeObject thisRangeObject = null;
            if (pNode.Type != Types.分组)
            {
                thisRangeObject = CreateRange(pNode, rangeNode, pparent); //创建自身区域范围
            }

            if (thisRangeObject == null)
            {
                //content = CreateRangeParent(pNode.Type.ToString() + "(" + pNode.Id + ")" + ":" + pNode.Name, pparent); //创建父物体
                string pName = pNode.Type.ToString() + "(" + pNode.Id + ")" + ":" + pNode.Name;
                GameObject o = new GameObject(pName); ;
                o.transform.SetParent(rootparent);
                content = o.transform;
            }
            else
            {

                content = thisRangeObject.transform;
                thisRangeObject.name = pNode.Type.ToString() + "(" + pNode.Id + ")" + ":" + pNode.Name;
                //if (pNode.Transfrom.IsCreateAreaByData)
                //{
                //    if (!pNode.Transfrom.IsRelative)
                //    {
                //        //float posx = LocationManager.Instance.axisZero.x * thisRangeObject.transform.parent.lossyScale.x;
                //        //float posy = LocationManager.Instance.axisZero.y * thisRangeObject.transform.parent.lossyScale.y;
                //        //float posz = LocationManager.Instance.axisZero.z * thisRangeObject.transform.parent.lossyScale.z;
                //        //Vector3 posT = new Vector3(posx, posy, posz);
                //        float posx = thisRangeObject.transform.localPosition.x * thisRangeObject.transform.parent.lossyScale.x;
                //        float posy = thisRangeObject.transform.localPosition.y * thisRangeObject.transform.parent.lossyScale.y;
                //        float posz = thisRangeObject.transform.localPosition.z * thisRangeObject.transform.parent.lossyScale.z;
                //        Vector3 posT = new Vector3(posx, posy, posz);
                //        thisRangeObject.transform.position = LocationManager.Instance.axisZero + posT;
                //    }
                //    else
                //    {
                //        if (rangeNode.parentNode != null)
                //        {
                //            PhysicalTopology buldingNode = rangeNode.parentNode.info;
                //            TransformM tm = buldingNode.Transfrom;
                //            Vector3 buildPos = Vector3.zero;
                //            if (tm != null && tm.IsCreateAreaByData)
                //            {
                //                //Vector3 pos2D = new Vector3((float)(tm.X - tm.SX / 2f), (float)(tm.Y - tm.SY / 2 + pNode.Transfrom.SY), (float)(tm.Z - tm.SZ / 2));//建筑物的右下角坐标
                //                Vector3 pos2D = new Vector3((float)(-tm.SX / 2f), (float)(-tm.SY / 2), (float)(-tm.SZ / 2));//建筑物的左下角坐标
                //                                                                                                            //Log.Info("建筑物的右下角坐标:" + pos2D);

                //                //content.transform.localPosition = buildPos;
                //                buildPos = LocationManager.GetRealSizeVector(pos2D);
                //            }
                //            else
                //            {
                //                Vector3 pSize = rangeNode.parentNode.thisRange.gameObject.GetGlobalSize();
                //                buildPos = new Vector3((float)(pSize.x / 2f), (float)(-(pSize.y + rangeNode.parentNode.thisRange.yOffset) / 2), (float)(pSize.z / 2));//建筑物的左下角坐标
                //            }

                //            float posx = buildPos.x / thisRangeObject.transform.parent.lossyScale.x;
                //            float posy = buildPos.y / thisRangeObject.transform.parent.lossyScale.y;
                //            float posz = buildPos.z / thisRangeObject.transform.parent.lossyScale.z;
                //            Vector3 posT = new Vector3(posx, posy, posz);
                //            thisRangeObject.transform.localPosition += posT;
                //        }
                //    }
                //}
                //else//
                //{

                //}
                //rangeNode.thisRange = thisRangeObject; //创建自身区域范围
            }



            if (pNode.Children != null)
            {
                foreach (PhysicalTopology child in pNode.Children)
                {
                    RangeNode subNode = new RangeNode();
                    subNode.parentNode = rangeNode;
                    rangeNode.subNodes.Add(subNode);
                    CreateRangesByRootNode(child, subNode, content);
                }
            }
        }

        ///// <summary>
        ///// 创建所有区域范围根据顶部节点（递归）
        ///// </summary>
        ///// <param name="pNode">数据节点</param>
        ///// <param name="rangeNode">区域范围节点</param>
        ///// <param name="pparentT">父物体</param>
        //public void CreateRangesByRootNode(PhysicalTopology pNode, RangeNode rangeNode, Transform pparentT)
        //{
        //    Transform pparent = pparentT; //区域根物体
        //    //Transform content = CreateRangeParent(pNode.Type.ToString() + "(" + pNode.Id + ")" + ":" + pNode.Name, pparent); //创建父物体
        //    Transform content = null;
        //    rangeNode.info = pNode;
        //    rangeNode.ranges = new List<MonitorRangeObject>();
        //    rangeNode.subNodes = new List<RangeNode>();
        //    //if (pNode.Name.Contains("集控楼"))
        //    //{
        //    //    int i = 0;
        //    //}
        //    MonitorRangeObject thisRangeObject = CreateRange(pNode, pparent); //创建自身区域范围
        //    if (thisRangeObject == null)
        //    {
        //        content = CreateRangeParent(pNode.Type.ToString() + "(" + pNode.Id + ")" + ":" + pNode.Name, pparent); //创建父物体
        //    }
        //    else
        //    {

        //        content = thisRangeObject.transform;
        //        thisRangeObject.name = pNode.Type.ToString() + "(" + pNode.Id + ")" + ":" + pNode.Name;
        //        if (!pNode.Transfrom.IsRelative)
        //        {
        //            //float posx = LocationManager.Instance.axisZero.x * thisRangeObject.transform.parent.lossyScale.x;
        //            //float posy = LocationManager.Instance.axisZero.y * thisRangeObject.transform.parent.lossyScale.y;
        //            //float posz = LocationManager.Instance.axisZero.z * thisRangeObject.transform.parent.lossyScale.z;
        //            //Vector3 posT = new Vector3(posx, posy, posz);
        //            float posx = thisRangeObject.transform.localPosition.x * thisRangeObject.transform.parent.lossyScale.x;
        //            float posy = thisRangeObject.transform.localPosition.y * thisRangeObject.transform.parent.lossyScale.y;
        //            float posz = thisRangeObject.transform.localPosition.z * thisRangeObject.transform.parent.lossyScale.z;
        //            Vector3 posT = new Vector3(posx, posy, posz);
        //            thisRangeObject.transform.position = LocationManager.Instance.axisZero + posT;
        //        }
        //        else
        //        {
        //            if (rangeNode.parentNode != null)
        //            {
        //                PhysicalTopology buldingNode = rangeNode.parentNode.info;
        //                TransformM tm = buldingNode.Transfrom;
        //                if (tm != null)
        //                {
        //                    //Vector3 pos2D = new Vector3((float)(tm.X - tm.SX / 2f), (float)(tm.Y - tm.SY / 2 + pNode.Transfrom.SY), (float)(tm.Z - tm.SZ / 2));//建筑物的右下角坐标
        //                    Vector3 pos2D = new Vector3((float)(-tm.SX / 2f), (float)(-tm.SY / 2), (float)(-tm.SZ / 2));//建筑物的右下角坐标
        //                    //Log.Info("建筑物的右下角坐标:" + pos2D);
        //                    Vector3 buildPos = LocationManager.GetRealSizeVector(pos2D);
        //                    //content.transform.localPosition = buildPos;
        //                    float posx = buildPos.x / thisRangeObject.transform.parent.lossyScale.x;
        //                    float posy = buildPos.y / thisRangeObject.transform.parent.lossyScale.y;
        //                    float posz = buildPos.z / thisRangeObject.transform.parent.lossyScale.z;
        //                    Vector3 posT = new Vector3(posx, posy, posz);
        //                    thisRangeObject.transform.localPosition += posT;

        //                }
        //            }
        //        }
        //        rangeNode.thisRange = thisRangeObject; //创建自身区域范围
        //    }

        //    if (pNode.Children != null)
        //    {
        //        foreach (PhysicalTopology child in pNode.Children)
        //        {
        //            RangeNode subNode = new RangeNode();
        //            subNode.parentNode = rangeNode;
        //            rangeNode.subNodes.Add(subNode);
        //            CreateRangesByRootNode(child, subNode, content);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 创建子区域范围
        ///// </summary>
        ///// <param name="node"></param>
        //public void CreateSubRanges(DepNode node)
        //{
        //    if (node == null) return;

        //}

        ///// <summary>
        ///// 创建大楼里面所有的范围区域
        ///// </summary>
        ///// <param name="node"></param>
        //private void CreateChilrenRange_Building(DepNode node)
        //{
        //    LocationManager.LoadTransferOfAxesConfig();

        //    PhysicalTopology topoNode = node.TopoNode;
        //    if (topoNode == null) return;
        //    if (topoNode.Children == null)
        //    {
        //        Log.Alarm("CreateChilrenRange_Floor", "topoNode.Children == null");
        //        return;
        //    }
        //    Transform root = CreateRangeParent(); //区域根物体
        //                                          //MonitorRangeObject rootRange=CreateRange(topoNode, root);
        //    Transform parent = CreateRangeParent(topoNode.Id + ":" + topoNode.Name, root); //创建父物体

        //    //List<MonitorRangeObject> ranges = new List<MonitorRangeObject>();
        //    foreach (PhysicalTopology child in topoNode.Children)//房间
        //    {
        //        Log.Info("CreateChild", child.ToString());
        //        if (child.Type == Types.楼层)
        //        {
        //            //MonitorRangeObject rangeObject = CreateRange(child, parent); //创建子区域
        //            //if (rangeObject == null) continue;
        //            //rangeObject.transform.localPosition -= LocationManager.Instance.axisZero;
        //            ////ranges.Add(rangeObject);

        //            //rangelist.Add(rangeObject);

        //            CreateChilrenRange_BuildingFloor(child, parent);
        //        }
        //    }


        //}

        //private void CreateChilrenRange_BuildingFloor(PhysicalTopology pnode, Transform p_parent = null)
        //{
        //    PhysicalTopology topoNode = pnode;
        //    if (topoNode == null) return;
        //    if (topoNode.Children == null)
        //    {
        //        Log.Alarm("CreateChilrenRange_Floor", "topoNode.Children == null");
        //        return;
        //    }

        //    if (p_parent == null)
        //    {
        //        p_parent = CreateRangeParent(); //区域根物体
        //    }
        //    //MonitorRangeObject rootRange=CreateRange(topoNode, root);
        //    Transform parent = CreateRangeParent(topoNode.Id + ":" + topoNode.Name, p_parent); //创建父物体

        //    List<MonitorRangeObject> ranges = new List<MonitorRangeObject>();
        //    foreach (PhysicalTopology child in topoNode.Children)//房间
        //    {
        //        Log.Info("CreateChild", child.ToString());

        //        MonitorRangeObject rangeObject = CreateRange(child, parent); //创建子区域
        //        if (rangeObject == null) continue;
        //        rangeObject.transform.localPosition -= LocationManager.Instance.axisZero;
        //        ranges.Add(rangeObject);

        //        rangelist.Add(rangeObject);
        //    }

        //    PhysicalTopology buldingNode = pnode;
        //    TransformM tm = buldingNode.Transfrom;
        //    if (tm != null)
        //    {
        //        Vector3 pos2D = new Vector3((float)(tm.X - tm.SX / 2f), (float)(tm.Y - tm.SY / 2 + topoNode.Transfrom.SY), (float)(tm.Z - tm.SZ / 2));//建筑物的右下角坐标
        //        Log.Info("建筑物的右下角坐标:" + pos2D);
        //        Vector3 buildPos = LocationManager.GetRealVector(pos2D);
        //        parent.localPosition = buildPos;
        //    }

        //}

        //private void CreateChilrenRange_Floor(DepNode node)
        //{
        //    LocationManager.LoadTransferOfAxesConfig();

        //    PhysicalTopology topoNode = node.TopoNode;
        //    if (topoNode == null) return;
        //    if (topoNode.Children == null)
        //    {
        //        Log.Alarm("CreateChilrenRange_Floor", "topoNode.Children == null");
        //        return;
        //    }
        //    Transform root = CreateRangeParent(); //区域根物体
        //                                          //MonitorRangeObject rootRange=CreateRange(topoNode, root);
        //    Transform parent = CreateRangeParent(topoNode.Id + ":" + topoNode.Name, root); //创建父物体

        //    List<MonitorRangeObject> ranges = new List<MonitorRangeObject>();
        //    foreach (PhysicalTopology child in topoNode.Children)//房间
        //    {
        //        Log.Info("CreateChild", child.ToString());

        //        MonitorRangeObject rangeObject = CreateRange(child, parent); //创建子区域
        //        if (rangeObject == null) continue;
        //        rangeObject.transform.localPosition -= LocationManager.Instance.axisZero;
        //        ranges.Add(rangeObject);

        //        rangelist.Add(rangeObject);
        //    }

        //    PhysicalTopology buldingNode = node.ParentNode.TopoNode;
        //    TransformM tm = buldingNode.Transfrom;
        //    if (tm != null)
        //    {
        //        Vector3 pos2D = new Vector3((float)(tm.X - tm.SX / 2f), (float)(tm.Y - tm.SY / 2 + topoNode.Transfrom.SY), (float)(tm.Z - tm.SZ / 2));//建筑物的右下角坐标
        //        Log.Info("建筑物的右下角坐标:" + pos2D);
        //        Vector3 buildPos = LocationManager.GetRealVector(pos2D);
        //        parent.localPosition = buildPos;
        //    }
        //    //Vector3 size = parent.gameObject.GetLocalSize();
        //    //parent.localPosition = parent.localPosition - LocationManager.Instance.axisZero +
        //    //                       new Vector3(size.x/2, 0, size.z/2); //偏移位置

        //    //foreach (MonitorRangeObject range in ranges)
        //    //{
        //    //    range.transform.localPosition += new Vector3(size.x / 2, 0, size.z / 2);
        //    //}
        //}



        void OnDestroy()
        {
            SceneEvents.DepNodeChanged -= SceneEventsDepNodeChanged;
            SceneEvents.BuildingOpenCompleteAction -= SceneEvents_BuildingOpenCompleteAction;
            SceneEvents.BuildingCloseCompleteAction -= SceneEvents_BuildingCloseCompleteAction;
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 显示区域
        /// </summary>
        [ContextMenu("ShowAreas")]
        public void ShowRanges(DepNode depNodeT)
        {
            if (depNodeT == null || FullViewController.Instance.IsFullView) return;
            if (depNodeT.depType == DepType.Factory)
            {
                if (rootDepNode != null)
                {
                    ShowAllRanges();
                }
                else
                {
                    CreateAllRanges();
                }
            }
            else
            {
                FilterRangesByMRObject(depNodeT);
            }
        }

        /// <summary>
        /// 筛选园区下的监控范围
        /// </summary>
        /// isDeep,是否继续往子节点下面搜
        private List<PhysicalTopology> FilterParkMonitorRange(IEnumerable<PhysicalTopology> listT)
        {
            List<PhysicalTopology> ps = new List<PhysicalTopology>();
            if (listT == null)
            {
                return ps;
            }
            foreach (PhysicalTopology p in listT)
            {
                if (p.Type == Types.区域 || p.Type == Types.分组 || p.Type == Types.大楼 || p.Type == Types.范围)
                {
                    if (p.Transfrom != null)
                    {
                        ps.Add(p);
                    }
                    ps.AddRange(FilterParkMonitorRange(p.Children));
                }
            }
            return ps;
        }

        ///// <summary>
        ///// 获取园区下监控范围
        ///// </summary>
        //[ContextMenu("GetParkRangesInfo")]
        //public List<PhysicalTopology> GetParkRangesInfo()
        //{
        //    IList<PhysicalTopology> listT = CommunicationObject.Instance.GetParkMonitorRange();
        //    List<PhysicalTopology> infolist = new List<PhysicalTopology>(listT);
        //    return infolist;
        //}

        ///// <summary>
        ///// 创建监控范围
        ///// </summary>
        //public void CreateRanges(IEnumerable<PhysicalTopology> infolist)
        //{
        //    Log.Info("CreateRanges");
        //    Transform parent = CreateRangeParent();
        //    if (infolist == null) return;
        //    foreach (PhysicalTopology topoNode in infolist)
        //    {
        //        MonitorRangeObject rangeObject = CreateRange(topoNode, parent);
        //        if (rangeObject != null)
        //            rangelist.Add(rangeObject);
        //        //}
        //    }
        //}

        public MonitorRangeObject CreateRange(PhysicalTopology p, RangeNode rangenodeT, Transform parent)
        {
            if (p == null)
            {
                Log.Alarm("MonitorRangeManager.CreateRange", "p == null");
                return null;
            }
            if (p.Transfrom == null)
            {
                Log.Alarm("MonitorRangeManager.CreateRange", "p.Transfrom == null");
                return null;
            }
            //Log.Info(string.Format("CreateRange：{0},({1},{2},{3})", p.Name, p.Transfrom.X, p.Transfrom.Y, p.Transfrom.Z));
            DepNode depNodeT = RoomFactory.Instance.GetDepNodeById(p.Id);
            if (depNodeT == null || depNodeT.NodeObject == null)
            {
                return null;
            }
            MonitorRangeObject rangeObject = rangelist.Find((item) => item.info.Id == p.Id);
            if (!rangeObject)
            {
                //GameObject o = CreateAreaObject(parent);
                GameObject o = CreateAreaObject();
                o.name = GetTopoNodeObjectName(p);
                o.SetLayerAll(Layers.IgnoreRaycast);//Layers.Range
                MonitorRangeObject mapAreaObject = o.AddComponent<MonitorRangeObject>();
                mapAreaObject.Init(p.Id, p, rangenodeT, depNodeT);
                rangeObject = mapAreaObject;
                o.SetActive(true);
            }
            else
            {
                rangeObject.Init(p.Id, p, rangenodeT, depNodeT);
                rangeObject.gameObject.SetActive(true);
            }
            return rangeObject;
        }


        private string GetTopoNodeObjectName(PhysicalTopology p)
        {
            return string.Format("{0}:{1}", p.Id, p.Name);
        }


        ///// <summary>
        ///// 清除监控范围
        ///// </summary>
        //public void ClearRanges()
        //{
        //    foreach (MonitorRangeObject range in rangelist)
        //    {
        //        if (range != null)
        //        {
        //            DestroyImmediate(range.gameObject);
        //        }
        //    }

        //    rangelist.Clear();
        //}

        /// <summary>
        /// 创建区域物体
        /// </summary>
        /// <returns></returns>
        public GameObject CreateAreaObject()
        {
            GameObject o = Instantiate(areaPrefab);
            //o.transform.SetParent(parent);
            o.transform.SetParent(rootparent);
            //BoxCollider boxCollider = o.GetComponent<BoxCollider>();
            //if (boxCollider)
            //{
            //    boxCollider.isTrigger = true;
            //}
            return o;
        }

        /// <summary>
        /// 创建定位卡的空间父物体
        /// </summary>
        private static Transform CreateRangeParent(string pName, Transform parent)
        {
            Transform tParent = parent.Find(pName);
            if (tParent == null)
            {
                GameObject o = new GameObject(pName);
                tParent = o.transform;
                tParent.SetParent(parent);
                tParent.transform.localPosition = Vector3.zero;
            }
            return tParent;
        }

        /// <summary>
        /// 创建定位卡的空间父物体
        /// </summary>
        private Transform CreateRangeParent()
        {
            Transform tParent = CreateRangeParent("Areas Parent", transform);
            areasParent = tParent;
            return tParent;
        }

        /// <summary>
        /// 设置监控区域范围跟随位置
        /// </summary>
        public void SetMonitorRangeFollowPosition(DepNode depNodeT)
        {
            //MonitorRangeObject[] rangesT = o.GetComponentsInChildren<MonitorRangeObject>();
            //foreach (MonitorRangeObject rangeT in rangesT)
            //{
            //    rangeT.SetFollowTarget();
            //}
            if (depNodeT == null) return;
            if (depNodeT.monitorRangeObject != null)
            {
                depNodeT.monitorRangeObject.SetFollowTarget();
            }

            if (depNodeT.ChildNodes != null)
            {
                foreach (DepNode childNodeT in depNodeT.ChildNodes)
                {
                    //if (childNodeT.monitorRangeObject == null)
                    //{
                    //    continue;
                    //}
                    SetMonitorRangeFollowPosition(childNodeT);
                }
            }
        }

        /// <summary>
        /// 过滤区域范围根据区域范围节点,只显示当前区域范围节点下的子区域节点
        /// </summary>
        public void FilterRangesByMRObject(DepNode depNodeT)
        {
            //if (rootDepNode == null) return;
            if (depNodeT == null) return;
            HideAllRanges();

            //MonitorRangeObject[] filterRangeObjs = mrObject.GetComponentsInChildren<MonitorRangeObject>();
            //foreach (MonitorRangeObject obj in filterRangeObjs)
            //{
            //    if (isShowRangeRender)
            //    {
            //        obj.SetRendererEnable(true);
            //    }
            //    obj.SetColliderEnable(true);
            //}

            FilterRangesByMRObjectop(depNodeT);

        }

        private void FilterRangesByMRObjectop(DepNode depNodeT)
        {
            if (depNodeT.monitorRangeObject != null)
            {
                if (isShowRangeRender)
                {
                    depNodeT.monitorRangeObject.SetRendererEnable(true);
                }
                else
                {
                    if (depNodeT.monitorRangeObject.IsOnAlarmArea && MonitorRangeManager.Instance.IsShowAlarmArea)
                    {
                        depNodeT.monitorRangeObject.SetRendererEnable(true);
                    }
                }
                depNodeT.monitorRangeObject.SetColliderEnable(true);
            }

            if (depNodeT.ChildNodes != null)
            {
                foreach (DepNode childNodeT in depNodeT.ChildNodes)
                {
                    //if (childNodeT.monitorRangeObject == null)
                    //{
                    //    continue;
                    //}
                    FilterRangesByMRObjectop(childNodeT);
                }
            }
        }

        /// <summary>
        /// 显示所有区域范围
        /// </summary>
        public void ShowAllRanges()
        {
            MonitorRangeObject[] rangeObjs = areasParent.GetComponentsInChildren<MonitorRangeObject>();
            foreach (MonitorRangeObject obj in rangeObjs)
            {
                if (isShowRangeRender)
                {
                    obj.SetRendererEnable(true);
                }
                else
                {
                    if (obj.IsOnAlarmArea && MonitorRangeManager.Instance.IsShowAlarmArea)
                    {
                        obj.SetRendererEnable(true);
                    }
                }
                obj.SetColliderEnable(true);
            }
        }

        /// <summary>
        /// 隐藏所有区域范围
        /// </summary>
        /// <param name="isCollider">设置Collder状态</param>
        private void HideAllRanges(bool isCollider = false)
        {
            MonitorRangeObject[] rangeObjs = areasParent.GetComponentsInChildren<MonitorRangeObject>();
            foreach (MonitorRangeObject obj in rangeObjs)
            {
                obj.SetRendererEnable(false);
                if (!isCollider)
                {
                    obj.SetColliderEnable(false);
                }
            }
        }

        /// <summary>
        /// 设置告警区域
        /// </summary>
        /// <param name="b"></param>
        public void SetIsAlarmArea(bool b)
        {
            IsShowAlarmArea = b;
        }

        /// <summary>
        /// 显示告警区域
        /// </summary>
        public void ShowAlarmArea()
        {
            IsShowAlarmArea = true;
            //MonitorRangeObject[] rangeObjs = areasParent.GetComponentsInChildren<MonitorRangeObject>();
            //foreach (MonitorRangeObject obj in rangeObjs)
            //{
            //    if (obj.IsOnAlarmArea)
            //    {
            //        obj.SetRendererEnable(true);
            //    }
            //}

            FilterRangesByMRObject(SceneEvents.DepNode);
        }

        /// <summary>
        /// 隐藏告警区域
        /// </summary>
        public void HideAlarmArea()
        {
            IsShowAlarmArea = false;
            MonitorRangeObject[] rangeObjs = areasParent.GetComponentsInChildren<MonitorRangeObject>();
            foreach (MonitorRangeObject obj in rangeObjs)
            {
                if (obj.IsOnAlarmArea&& obj.alarmPersons.Count==0)
                {
                    obj.SetRendererEnable(false);
                }
            }
        }

        #region 区域编辑

        /// <summary>
        /// 当前编辑区域的建筑节点
        /// </summary>
        public DepNode currentEditDepNode;

        /// <summary>
        /// 展示区域可编辑
        /// </summary>
        public void ShowAreaEdit(DepNode depnodeT)
        {
            //ObjectsEditManage.Instance.SetEditorGizmoSystem(true);
            SetAreaEditEnable(currentEditDepNode, false);//清除当前编辑
            SetAreaEditEnable(depnodeT, true);
            SetEditorObjectSelection(true);
        }

        /// <summary>
        /// 设置当前建筑节点下的区域（类型为范围，机房）是否可编辑
        /// </summary>
        /// <param name="depnodeT">建筑节点</param>
        public void SetAreaEditEnable(DepNode depnodeT, bool isEnable)
        {
            if (depnodeT != null)
            {
                IsEditState = isEnable;
                if (isEnable)
                {
                    currentEditDepNode = depnodeT;
                }
                else
                {
                    currentEditDepNode = null;
                }
                if (depnodeT.ChildNodes != null)
                {
                    foreach (DepNode childNodeT in depnodeT.ChildNodes)
                    {
                        if (childNodeT.TopoNode == null)
                        {
                            continue;
                        }
                        if (childNodeT.TopoNode.Type == Types.范围 || childNodeT.TopoNode.Type == Types.机房)
                        {
                            if (childNodeT.monitorRangeObject == null) continue;

                            childNodeT.monitorRangeObject.SetEditEnable(isEnable);
                            if (childNodeT.monitorRangeObject.IsOnAlarmArea && MonitorRangeManager.Instance.IsShowAlarmArea && !isEnable)
                            {
                            }
                            else
                            {
                                childNodeT.monitorRangeObject.SetRendererEnable(isEnable);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 退出当前的区域编辑
        /// </summary>
        public void ExitCurrentEditArea()
        {
            //ObjectsEditManage.Instance.SetEditorGizmoSystem(false);
            SetAreaEditEnable(currentEditDepNode, false);//清除当前编辑
            SetEditorObjectSelection(false);
            EditorObjectSelection.Instance.ClearSelection(false);
        }

        /// <summary>
        /// 设置EditorObjectSelection
        /// </summary>
        public void SetEditorObjectSelection(bool isEditAreaT)
        {
            if (isEditAreaT)
            {
                EditorObjectSelection.Instance.ObjectSelectionSettings.CanMultiSelect = false;
                //RotationGizmo.
                EditorGizmoSystem.Instance.RotationGizmo.SetAxisVisibility(false, 0);
                EditorGizmoSystem.Instance.RotationGizmo.SetAxisVisibility(false, 2);
                EditorGizmoSystem.Instance.RotationGizmo.ShowRotationSphere = false;
                EditorGizmoSystem.Instance.RotationGizmo.ShowCameraLookRotationCircle = false;
            }
            else
            {
                EditorObjectSelection.Instance.ObjectSelectionSettings.CanMultiSelect = true;
                //RotationGizmo.
                EditorGizmoSystem.Instance.RotationGizmo.SetAxisVisibility(true, 0);
                EditorGizmoSystem.Instance.RotationGizmo.SetAxisVisibility(true, 2);
                EditorGizmoSystem.Instance.RotationGizmo.ShowRotationSphere = true;
                EditorGizmoSystem.Instance.RotationGizmo.ShowCameraLookRotationCircle = true;
            }
        }

        #endregion
    }

    #region 范围节点类
    /// <summary>
    /// 范围节点类
    /// </summary>
    public class RangeNode
    {
        /// <summary>
        /// 节点信息
        /// </summary>
        public PhysicalTopology info;
        /// <summary>
        /// 该节点自身的区域范围
        /// </summary>
        public MonitorRangeObject thisRange;
        /// <summary>
        /// 父范围节点
        /// </summary>
        public RangeNode parentNode;
        /// <summary>
        /// 该节点下的区域范围物体集合
        /// </summary>
        public List<MonitorRangeObject> ranges;
        /// <summary>
        /// 子节点集合
        /// </summary>
        public List<RangeNode> subNodes;

    }
    #endregion
}