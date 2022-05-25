using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPosition : MonoBehaviour
{
    void Start()
    {
        
    }

    void OnMouseDown()
    {
        transform.position = Input.mousePosition;
        Debug.Log(Input.mousePosition);
    }
}
