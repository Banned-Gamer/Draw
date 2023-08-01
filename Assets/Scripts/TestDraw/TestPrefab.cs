using UnityEngine;

public class TestPrefab : MonoBehaviour
{
    public MusicSo data;

    // Start is called before the first frame update
    void Start()
    {
        var len = data.DefensePoints.Count;
        for (var i = 0; i < len; i++)
        {
            Debug.Log($"{data.DefensePoints[i].x} {data.DefensePoints[i].y}");
        }
    }
}