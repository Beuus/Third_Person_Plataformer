using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachable : MonoBehaviour
{
    //Publico en el inspector porque si es publico unity lo ignora y para no serlo HideInspector
    [SerializeField]
    private bool _IsAttachable;
    
    private bool _IsAttached;

    public bool IsAttachable
    {
        get { return _IsAttachable; }
        set { _IsAttachable = value; }
    }
    public bool IsAttached
    {
        get { return _IsAttached; }
        set { _IsAttached = value; }
    }

}
