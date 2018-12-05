using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTEditor;
using Location.WCFServiceReferences.LocationServices;
using System.Linq;
using Unity.Common.Utils;

public class ObjectEditEventListener : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        BindingGizmosEvent();
    }
    /// <summary>
    /// 绑定设备编辑事件
    /// </summary>
    private void BindingGizmosEvent()
    {
        EditorGizmoSystem GizmoSystem = EditorGizmoSystem.Instance;
        if (GizmoSystem == null)
        {
            Debug.LogError("EditorGizmoSystem.Instance is null!");
        }
        else
        {
            //移动编辑事件
            GizmoSystem.TranslationGizmo.GizmoDragStart += TranslationGizmosStart;
            GizmoSystem.TranslationGizmo.GizmoDragUpdate += TranslationGizmosUpdate;
            GizmoSystem.TranslationGizmo.GizmoDragEnd += TranslationGizmosEnd;
            //旋转编辑事件
            GizmoSystem.RotationGizmo.GizmoDragStart += RotationGizmosStart;
            //GizmoSystem.RotationGizmo.GizmoDragUpdate += RotationGizmosUpdate;
            GizmoSystem.RotationGizmo.GizmoDragEnd += RotationGizmosEnd;
            //缩放编辑事件
            GizmoSystem.ScaleGizmo.GizmoDragStart += ScaleGizmosStart;
            //GizmoSystem.ScaleGizmo.GizmoDragUpdate += ScaleGizmosUpdate;
            GizmoSystem.ScaleGizmo.GizmoDragEnd += ScaleGizmosEnd;
        }
        if(EditorObjectSelection.Instance)
        {
            //物体选中和取消选中的事件
            EditorObjectSelection.Instance.SelectionChanged += OnEditObjectSelectionChange;
        }
    }
    /// <summary>
    /// 编辑物体，选中和取消选中的事件
    /// </summary>
    /// <param name="selectionChangedEventArgs"></param>
    public void OnEditObjectSelectionChange(ObjectSelectionChangedEventArgs selectionChangedEventArgs)
    {
        List<GameObject> deSelectObjList = selectionChangedEventArgs.DeselectedObjects;
        List<GameObject> selectObjList = selectionChangedEventArgs.SelectedObjects;
        HideDevInfo(deSelectObjList,selectObjList);
        ShowDevInfo(selectObjList);
    }
    /// <summary>
    /// 关闭设备编辑界面
    /// </summary>
    /// <param name="deSelectObjList"></param>
    /// <param name="selectObjList"></param>
    private void HideDevInfo(List<GameObject> deSelectObjList, List<GameObject> selectObjList)
    {
        if (deSelectObjList.Count == 1)
        {
            if (deSelectObjList[0] != null)
            {
                DevNode dev = deSelectObjList[0].GetComponent<DevNode>();
                if (dev == null) return;
                DeviceEditUIManager.Instacne.Close();
                if (selectObjList.Count > 0) DeviceEditUIManager.Instacne.HideMultiDev();
            }
        }
        else if (deSelectObjList.Count > 1)
        {
            DeviceEditUIManager.Instacne.HideMultiDev();
        }
    }
    /// <summary>
    /// 显示设备编辑界面
    /// </summary>
    /// <param name="selectObjList"></param>
    private void ShowDevInfo(List<GameObject> selectObjList)
    {
        if (selectObjList.Count == 1)
        {
            DevNode dev = selectObjList[0].GetComponent<DevNode>();
            if (dev == null) return;
            DeviceEditUIManager.Instacne.Show(dev);
        }
        else if (selectObjList.Count > 1)
        {
            List<DevNode> devList = new List<DevNode>();
            foreach (var item in selectObjList)
            {
                DevNode dev = item.GetComponent<DevNode>();
                if (dev == null) continue;
                devList.Add(dev);
            }
            if (devList.Count == 1)
            {
                DeviceEditUIManager.Instacne.Show(devList[0]);
            }
            else if (devList.Count > 1)
            {
                DeviceEditUIManager.Instacne.ShowMultiDev(devList);
            }
        }
    }
    private CommunicationObject Service;
    /// <summary>
    /// 修改设备位置信息
    /// </summary>
    /// <param name="ObjList"></param>
    private void ModifyDevPos(List<GameObject>ObjList)
    {
        if (Service == null)
        {
            Service = CommunicationObject.Instance;
        }
        List<DevPos> posList = new List<DevPos>();
        foreach (var Obj in ObjList)
        {
            DevNode Dev = Obj.GetComponent<DevNode>();
            if (Dev == null) continue;
            DevPos pos = CaculateDevPos(Dev,Obj);
            //Service.ModifyDevPos(pos);  
            posList.Add(pos);
        }
        if (posList.Count != 0)
        {
            Service.ModifyDevPosByList(posList);
        }
    }
    /// <summary>
    /// 更新设备位置信息
    /// </summary>
    /// <param name="devInfo"></param>
    /// <param name="model"></param>
    private DevPos CaculateDevPos(DevNode devInfo,GameObject model)
    {
        DevPos posInfo = new DevPos();
        posInfo.DevID = devInfo.Info.DevID;
        Vector3 cadPos = GetCadPos(model,devInfo);
        posInfo.PosX = cadPos.x;
        posInfo.PosY = cadPos.y;
        posInfo.PosZ = cadPos.z;
        Vector3 rotation = model.transform.eulerAngles;
        posInfo.RotationX = rotation.x;
        posInfo.RotationY = rotation.y;
        posInfo.RotationZ = rotation.z;
        Vector3 scale = model.transform.localScale;
        posInfo.ScaleX = scale.x;
        posInfo.ScaleY = scale.y;
        posInfo.ScaleZ = scale.z;
        return posInfo;
    }
    /// <summary>
    /// 获取CAD位置
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="dev"></param>
    /// <returns></returns>
    private Vector3 GetCadPos(GameObject obj,DevNode dev)
    {
        //DeviceEdit editPart = DeviceEditUIManager.Instacne.EditPart;
        Vector3 cadPos;
        bool isLocalPos = !(dev.ParentDepNode==FactoryDepManager.Instance||dev is DepDevController);
        if (!isLocalPos)
        {
            cadPos = LocationManager.UnityToCadPos(obj.transform.position, false);
        }
        else
        {
            cadPos = LocationManager.UnityToCadPos(obj.transform.localPosition, true);
        }
        return cadPos;
    }
    #region TranslationGizmo
    /// <summary>
    /// 移动开始
    /// </summary>
    /// <param name="gizmo"></param>
    private void TranslationGizmosStart(Gizmo gizmo)
    {
        Debug.Log(gizmo);
    }
    /// <summary>
    /// 移动更新
    /// </summary>
    /// <param name="gizmo"></param>
    private void TranslationGizmosUpdate(Gizmo gizmo)
    {
        List<GameObject> controlObj = gizmo.ControlledObjects.ToList();
        if (controlObj.Count == 1)
        {
            GameObject modelT = controlObj[0];
            DevNode dev = modelT.GetComponent<DevNode>();
            if (dev == null||dev.Info==null) return;
            dev.Info.Pos = CaculateDevPos(dev, modelT);
            DeviceEditUIManager.Instacne.Show(dev);
        }
    }
    /// <summary>
    /// 移动结束
    /// </summary>
    /// <param name="gizmo"></param>
    private void TranslationGizmosEnd(Gizmo gizmo)
    {
        ModifyDevPos(gizmo.ControlledObjects.ToList());
        //foreach (var item in gizmo.ControlledObjects)
        //{
        //    Debug.Log(item.name);
        //}
    }
    #endregion
    #region RotationGizmo
    /// <summary>
    /// 旋转开始
    /// </summary>
    /// <param name="gizmo"></param>
    private void RotationGizmosStart(Gizmo gizmo)
    {
        Debug.Log(gizmo);
    }
    /// <summary>
    /// 旋转更新
    /// </summary>
    /// <param name="gizmo"></param>
    private void RotationGizmosUpdate(Gizmo gizmo)
    {

    }
    /// <summary>
    /// 旋转结束
    /// </summary>
    /// <param name="gizmo"></param>
    private void RotationGizmosEnd(Gizmo gizmo)
    {
        ModifyDevPos(gizmo.ControlledObjects.ToList());
        //foreach (var item in gizmo.ControlledObjects)
        //{
        //    Debug.Log(item.name);
        //}
    }
    #endregion
    #region ScaleGizmo
    /// <summary>
    /// 缩放开始
    /// </summary>
    /// <param name="gizmo"></param>
    private void ScaleGizmosStart(Gizmo gizmo)
    {
        Debug.Log(gizmo);
    }
    /// <summary>
    /// 缩放更新
    /// </summary>
    /// <param name="gizmo"></param>
    private void ScaleGizmosUpdate(Gizmo gizmo)
    {

    }
    /// <summary>
    /// 缩放结束
    /// </summary>
    /// <param name="gizmo"></param>
    private void ScaleGizmosEnd(Gizmo gizmo)
    {
        ModifyDevPos(gizmo.ControlledObjects.ToList());
        //foreach (var item in gizmo.ControlledObjects)
        //{
        //    Debug.Log(item.name);
        //}
    }
    #endregion
}
