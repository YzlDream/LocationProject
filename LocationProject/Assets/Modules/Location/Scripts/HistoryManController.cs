using HighlightingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 历史轨迹人员控制器
/// </summary>
public class HistoryManController : MonoBehaviour {

    public Color color = Color.green;

	// Use this for initialization
	void Start () {
        HighlightOn();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init(Color colorT)
    {
        color = colorT;
    }

    private void OnEnable()
    {
        HighlightOn();
    }

    private void OnDisable()
    {
        HighlightOff();
    }

    /// <summary>
    /// 开启高亮
    /// </summary>
    public void HighlightOn()
    {
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        h.ConstantOn(color);
    }

    /// <summary>
    /// 关闭高亮
    /// </summary>
    public void HighlightOff()
    {
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        h.ConstantOff();
    }
}
