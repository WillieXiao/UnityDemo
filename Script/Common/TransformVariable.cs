using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variable/Transform")]
public class TransformVariable : ScriptableObject
{
    public Transform value;

    public void Set(Transform v)
    {
        value = v;
    }

}

