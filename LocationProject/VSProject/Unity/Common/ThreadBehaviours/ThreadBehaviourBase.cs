using UnityEngine;
using System.Collections;

public class ThreadBehaviourBase : MonoBehaviour {

    public bool IsBusy;

    public bool IsFinished;

    public bool IsOnce;

    void Update()
    {
        if (IsFinished)
        {
            IsFinished = false;
            IsBusy = false;
            AfterFinished();
            if (IsOnce)
            {
                Destroy(this);
            }
        }
    }

    protected void BeforeRun()
    {
        IsBusy = true;
    }

    protected virtual void AfterFinished()
    {
        
    }
}
