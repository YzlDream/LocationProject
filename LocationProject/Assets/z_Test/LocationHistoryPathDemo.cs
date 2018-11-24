using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class LocationHistoryPathDemo : MonoBehaviour
{

    /// <summary>
    /// 编码
    /// </summary>
    public string code;
    /// <summary>
    /// 路径点数，这里的点数跟传入的点无关
    /// </summary>
    public int segments = 250;
    /// <summary>
    /// 路径是否循环
    /// </summary>
    public bool doLoop = true;
    /// <summary>
    /// 路径演变物体
    /// </summary>
    public Transform obj;
    /// <summary>
    /// 路径演变速度
    /// </summary>
    public float speed = .05f;
    /// <summary>
    /// 路径颜色
    /// </summary>
    private Color color;
    /// <summary>
    /// 实际点位置
    /// </summary>
    private List<Vector3> splinePoints;
    /// <summary>
    /// 路径是否闭合
    /// </summary>
    public bool pathLoop = false;

    public GameObject lineObj;

    IEnumerator Start()
    {
        //LocationManager.Instance.AddHistoryPath(this);
        if (splinePoints.Count <= 1) yield break;
        //VectorLine: exceeded maximum vertex count of 65534 for ""...use fewer points (maximum is 16383 points for continuous lines and points, and 32767 points for discrete lines)
        if (segments >= 16000)
        {
            segments = 16000;
        }
        var line = new VectorLine("Spline", new List<Vector3>(segments + 1), 2.0f, LineType.Continuous);
        line.color = color;

        line.MakeSpline(splinePoints.ToArray(), segments, pathLoop);
        line.Draw3D();
        lineObj = line.rectTransform.gameObject;

        obj.gameObject.SetActive(true);

        // Make the cube "ride" the spline at a constant speed
        do
        {
            for (var dist = 0.0f; dist < 1.0f; dist += Time.deltaTime * speed)
            {
                if (line.drawEnd <= 0) yield break;
                obj.position = line.GetPoint3D01(dist);
                yield return null;
            }

        } while (doLoop);
    }



    /// <summary>
    /// 初始化编码
    /// </summary>
    /// <param name="codeT"></param>
    public void InitCode(string codeT)
    {
        code = codeT;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(string codeT, Transform cubeT, Color colorT, List<Vector3> splinePointsT, int segmentsT, float speedT, bool doLoopT, bool pathLoopT)
    {
        code = codeT;
        segments = segmentsT;
        doLoop = doLoopT;
        obj = cubeT;
        speed = speedT;
        splinePoints = splinePointsT;
        color = colorT;
        pathLoop = pathLoopT;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(string codeT, Transform cubeT, Color colorT, List<Vector3> splinePointsT, int segmentsT)
    {
        code = codeT;
        obj = cubeT;
        splinePoints = splinePointsT;
        color = colorT;
        segments = segmentsT;
    }
}
