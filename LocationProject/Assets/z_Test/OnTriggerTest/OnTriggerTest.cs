using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter:" + gameObject.name);
    }

    ////20190118大量卡数据测试时，这里会导致卡顿
    //private void OnTriggerStay(Collider other)
    //{

    //}

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerEnter:" + gameObject.name);
    }
}
