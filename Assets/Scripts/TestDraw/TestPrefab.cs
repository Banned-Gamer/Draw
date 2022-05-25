using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPrefab : MonoBehaviour
{
    public MusicSo data;

    // Start is called before the first frame update
    void Start()
    {
        int len = data.DefencePoints.Count;
        for (int i = 0; i < len; i++)
        {
            Debug.Log(data.DefencePoints[i].x.ToString() + ' ' + data.DefencePoints[i].y.ToString());
        }
    }
}