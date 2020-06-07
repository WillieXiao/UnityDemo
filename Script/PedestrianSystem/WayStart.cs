using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayStart : MonoBehaviour
{
    public GameObject[]wayNode;
    public int startIndex;
    // Start is called before the first frame update
    void Start()
    {      

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for(int i = 0; i < wayNode.Length-1; i++)
        {
            
            Gizmos.DrawLine(wayNode[i].transform.position, wayNode[i + 1].transform.position);
            
        }
        
    }
}
