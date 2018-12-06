using MonitorRange;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointToLineSegmentDisTest : MonoBehaviour {

    public Transform target;//位置点
    public List<Transform> vertsTrans;//多边形顶点

    public GameObject showSphere;//显示球

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ShowPosSphere();

    }

    /// <summary>
    /// 显示多边形到点最短距离的点的位置坐标，（只适用于二维）
    /// </summary>
    [ContextMenu("ShowPosSphere")]
    public void ShowPosSphere()
    {
        List<Vector2> verts = new List<Vector2>();
        foreach (Transform t in vertsTrans)
        {
            t.transform.position = new Vector3(t.position.x, 0, t.position.z);//设置y轴值为零，使其处于XZ平面
            verts.Add(new Vector2(t.position.x, t.position.z));
        }

        Vector2 targetPos = new Vector2(target.transform.position.x, target.transform.position.z);
        Vector2 v = MonitorRangeManager.PointForPointToPolygon(targetPos, verts);

        Vector3 p = new Vector3(v.x, 0, v.y);
        if (showSphere == null)
        {
            showSphere = CreateSphere();
        }

        showSphere.transform.position = p;

        //if (XZpointList == null) return true;
        List<double> vertx = new List<double>();
        List<double> verty = new List<double>();
        foreach (Vector2 vt in verts)
        {
            vertx.Add(vt.x);
            verty.Add(vt.y);
        }
        bool b= PositionPnpoly(verts.Count, vertx, verty, v.x, v.y);
        Debug.LogError("PositionPnpoly:" + b);
    }

    

    public GameObject CreateSphere()
    {
        GameObject o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        o.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        return o;
    }

    /// <summary>
    /// 计算某个点是否在多边形内部
    /// </summary>
    /// <param name="nvert">不规则形状的顶点数</param>
    /// <param name="vertx">不规则形状x坐标集合</param>（点逆时针，顺时针没测过）
    /// <param name="verty">不规则形状y坐标集合</param>（点逆时针，顺时针没测过）
    /// <param name="testx">当前x坐标</param>
    /// <param name="testy">当前y坐标</param>
    /// <returns></returns>
    private bool PositionPnpoly(int nvert, List<double> vertx, List<double> verty, double testx, double testy)
    {
        int i, j, c = 0;
        for (i = 0, j = nvert - 1; i < nvert; j = i++)
        {
            if (((verty[i] > testy) != (verty[j] > testy)) && (testx < (vertx[j] - vertx[i]) * (testy - verty[i]) / (verty[j] - verty[i]) + vertx[i]))
            {
                c = 1 + c; ;
            }
        }
        if (c % 2 == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
