using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定位历史轨迹管理
/// </summary>
public class LocationHistoryManager : MonoBehaviour {

    public static LocationHistoryManager Instance;
    private Transform historyPathParent;//历史记录父物体
    public GameObject characterPrefab;//人员预设
    public GameObject NameUIPrefab;//名称UI预设


    // Use this for initialization
    void Start () {
        Instance = this;
        historyPaths = new List<LocationHistoryPath>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}



    private GameObject CreateCharacter()
    {
        GameObject o = Instantiate(characterPrefab);
        //o.transform.SetParent(tagsParent);
        return o;
    }

    /// <summary>
    /// 创建历史轨迹父物体
    /// </summary>
    public Transform GetHistoryAllPathParent()
    {
        if (historyPathParent == null)
        {
            //historyPathParent = GameObject.Find("HistoryPathParent").transform;
            historyPathParent = new GameObject("HistoryPathParent").transform;
            return historyPathParent;
        }
        else
        {
            return historyPathParent;
        }
    }

    #region 单人历史轨迹相关管理

    /// <summary>
    /// 历史轨迹集合
    /// </summary>
    public List<LocationHistoryPath> historyPaths;

    /// <summary>
    /// 获取取定位卡历史位置信息
    /// </summary>
    /// <returns></returns>
    public LocationHistoryPath ShowLocationHistoryPath(Personnel personnelT, List<Vector3> points, int segmentsT, Color color, string name = "HistoryPathObj")
    {
        GameObject o = CreateCharacter();
        LocationHistoryPath path = o.AddComponent<LocationHistoryPath>();
        o.name = personnelT.Name + "(" + personnelT.Tag.Code + ")";
        path.Init(personnelT, color, points, segmentsT);
        o.SetActive(true);

        return path;
    }

    /// <summary>
    /// 添加历史轨迹路线
    /// </summary>
    public void AddHistoryPath(LocationHistoryPath path)
    {
        historyPaths.Add(path);
    }

    /// <summary>
    /// 设置历史轨迹执行的值
    /// </summary>
    public void SetHistoryPath(float v)
    {
        foreach (LocationHistoryPath hispath in historyPaths)
        {
            hispath.Set(v);
        }
    }

    /// <summary>
    /// 清除历史轨迹路线
    /// </summary>
    public void ClearHistoryPaths()
    {
        foreach (LocationHistoryPath path in historyPaths)
        {
            DestroyImmediate(path.pathParent.gameObject);//人员是轨迹的子物体
            //DestroyImmediate(path.gameObject);
        }

        historyPaths.Clear();
    }

    #endregion

    #region 多人历史轨迹相关管理

    public List<LocationHistoryPath_M> historyPath_Ms;// 历史轨迹集合

    /// <summary>
    /// 多人：显示历史轨迹
    /// </summary>
    /// <returns></returns>
    public LocationHistoryPath_M ShowLocationHistoryPath_M(Personnel personnelT, List<Vector3> points, int segmentsT, Color color)
    {
        GameObject o = CreateCharacter();
        LocationHistoryPath_M path = o.AddComponent<LocationHistoryPath_M>();
        o.name = personnelT.Name + "(" + personnelT.Tag.Code + ")";
        path.Init(personnelT, color, points, segmentsT);
        o.SetActive(true);

        return path;
    }

    /// <summary>
    /// 添加历史轨迹路线
    /// </summary>
    public void AddHistoryPath_M(LocationHistoryPath_M path)
    {
        historyPath_Ms.Add(path);
    }

    /// <summary>
    /// 设置历史轨迹执行的值
    /// </summary>
    public void SetHistoryPath_M(float v)
    {
        foreach (LocationHistoryPath_M hispath in historyPath_Ms)
        {
            hispath.Set(v);
        }
    }

    /// <summary>
    /// 多人：清除历史轨迹路线
    /// </summary>
    public void ClearHistoryPaths_M()
    {
        foreach (LocationHistoryPath_M path in historyPath_Ms)
        {
            DestroyImmediate(path.pathParent.gameObject);//人员是轨迹的子物体
            //DestroyImmediate(path.gameObject);
        }

        historyPath_Ms.Clear();
    }

    #endregion
}
