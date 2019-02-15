using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class HistoryPlayBar : Graphic
{
    public float width;//Bar的宽度
    public float height;//Bar的高度

    public List<float> valueList;//值列表

    //网格模型顶点数量
    private int VERTICES_COUNT;
    public int radius = 100;
    //public Color ccolor = Color.white;
    //private bool IsCanDraw;//可以进行绘制了

    [Tooltip("顶点数必须小于>3")]
    //顶点数组
    Vector3[] vertices;
    //三角形数组索引
    int[] triangles;
    //偏移系数
    [Range(-0.5f, 0.5f)]
    public float offset = 0;


    float pi = Mathf.PI;

    public HistoryPlayBar()
    {
        valueList = new List<float>();
        valueList.Add(1f);
        valueList.Add(1f);
        valueList.Add(1f);
    }

    //public void Awake()
    //{
    //    SetVertices();
    //}

    //void Start()
    //{
    //    //SetVertices();
    //    print("");
    //}
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (valueList.Count < 3) return;
        vh.Clear();
        SetVertices(vh);
    }

    private void SetVertices(VertexHelper vh)
    {
        valueList.Add(0);
        vertices = new Vector3[(valueList.Count + 1) * 2];
        VERTICES_COUNT = vertices.Length;//包括起始点0
        int triangles_count = VERTICES_COUNT - 1;

        triangles = new int[triangles_count * 3];

        List<UIVertex> verts = new List<UIVertex>();

        for (int i = 0; i < valueList.Count; i++)
        {
            if (i == 0)
            {
                vertices[2 * i] = new Vector3(valueList[i], 0, 0);
                vertices[2 * i + 1] = new Vector3(valueList[i], 0, 0);
            }
            else
            {
                vertices[2 * i] = new Vector3(valueList[i] - valueList[i - 1], 0, 0);
                vertices[2 * i + 1] = new Vector3(valueList[i] - valueList[i - 1], height, 0);
            }

            UIVertex uiVertexT0 = CreateUIVertex(vertices[2 * i]);
            verts.Add(uiVertexT0);
            UIVertex uiVertexT1 = CreateUIVertex(vertices[2 * i + 1]);
            verts.Add(uiVertexT1);
        }

        SetTriangles();
        vh.AddUIVertexStream(verts, new List<int>(triangles));
    }

    private void SetTriangles()
    {
        int idx = 0;
        int value = 0;
        for (int i = 0; i < triangles.Length; i++)
        {
            if (i % 3 == 0)
            {
                triangles[i] = 0;
                value = idx;
                idx++;
            }
            else
            {
                value++;
                if (value == VERTICES_COUNT)
                    value = 1;
                //Debug.Log("value " + value);

                triangles[i] = value;
            }
        }
    }

    public UIVertex CreateUIVertex(Vector3 pos)
    {
        UIVertex uiVertex = new UIVertex();
        uiVertex.position = pos;
        uiVertex.color = color;
        uiVertex.uv0 = Vector2.zero;
        return uiVertex;
    }

    float GetValue(int i)
    {
        valueList[i] = valueList[i] > 1 ? 1 : valueList[i];
        valueList[i] = valueList[i] < 0 ? 0 : valueList[i];

        //valueList[i] = valueList[i] + offset;

        return valueList[i] + offset;
    }

    float GetRadians(float angle)
    {
        return pi / 180 * angle;
    }

    //void Apply()
    //{
    //    Vector3[] tmps = new Vector3[vertices.Length];
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        //tmps[i] = vertices[i] * vertices[i].z * scale;
    //        tmps[i] = vertices[i] * vertices[i].z;
    //    }

    //}

    public void SetValue(List<float> values)
    {
        //valueList = values;
        valueList.Clear();
        for (int i = 0; i < values.Count; i++)
        {
            valueList.Add(values[i]);
        }
        //很重要，界面才会重新刷新
        SetVerticesDirty();
    }
}
