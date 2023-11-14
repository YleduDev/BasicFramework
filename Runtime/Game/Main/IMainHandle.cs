using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMainHandle 
{
    public void OnAwake(MonoBehaviour mb);

    public void OnStart(MonoBehaviour mb);

    public void OnDestroy(MonoBehaviour mb);

}
