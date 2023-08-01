using UnityEngine;

public class TestPrefab : MonoBehaviour
{
    public MusicSo data;

    // Start is called before the first frame update
    private void Start()
    {
        var len = data.DefencePoints.Count;
        for (var i = 0; i < len; i++)
        {
            Debug.Log(data.DefencePoints[i].x.ToString() + ' ' + data.DefencePoints[i].y.ToString());
        }
    }
}