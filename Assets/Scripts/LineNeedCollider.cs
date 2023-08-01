using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class LineNeedCollider : MonoBehaviour
{
    private EdgeCollider2D _myEdgeCollider2D;
    private LineRenderer   _myLine;

    private void Start()
    {
        _myEdgeCollider2D = this.GetComponent<EdgeCollider2D>();
        _myLine           = this.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        SetEdgeCollider(_myLine);
    }

    void SetEdgeCollider(LineRenderer lineRenderer)
    {
        var edges = new List<Vector2>();
        for (var i = 0; i < lineRenderer.positionCount; i++)
        {
            var newPoint = lineRenderer.GetPosition(i);
            edges.Add(new Vector2(newPoint.x, newPoint.y));
        }

        _myEdgeCollider2D.SetPoints(edges);
    }
}