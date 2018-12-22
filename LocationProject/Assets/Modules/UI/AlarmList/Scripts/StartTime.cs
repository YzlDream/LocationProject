using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartTime : MonoBehaviour {
    public static StartTime instance;
    public Text startTime;
	// Use this for initialization
	void Start () {
        ShowStartTime();
       

    }

// Update is called once per frame
void Update () {
		
	}
    public void ShowStartTime()
    {
        startTime.text = "2018年01月01日";
    }
}
