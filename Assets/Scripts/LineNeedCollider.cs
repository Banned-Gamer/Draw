using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class LineNeedCollider : MonoBehaviour
{
    private EdgeCollider2D myEdgeCollider2D;
    private LineRenderer myLine;

    void Start()
    {
        myEdgeCollider2D = this.GetComponent<EdgeCollider2D>();
        myLine = this.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        SetEdgeCollider(myLine);
    }

    void SetEdgeCollider(LineRenderer lineRenderer)
    {
        List<Vector2> edges = new List<Vector2>();
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 newPoint = lineRenderer.GetPosition(i);
            edges.Add(new Vector2(newPoint.x, newPoint.y));
        }

        myEdgeCollider2D.SetPoints(edges);
    }
}