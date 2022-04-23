using UnityEngine;

public class NoteMgr : MonoBehaviour
{
    public bool IsUsable = true;

    public Vector3 BeginPosition;
    public Vector3 EndPosition;
    public float MoveTime;
    public float MoveSpeed;

    private float _endTime;

    private float _sumTime;
    private bool _isBegin = false;

    void Awake()
    {
        _isBegin = false;
    }

    void Update()
    {
        if (_isBegin)
        {
            _sumTime += Time.deltaTime;
            transform.position += Vector3.left * MoveSpeed * Time.deltaTime;

            if (_sumTime >= _endTime)
            {
                EndMove();
            }
        }
    }

    public void BeginMove(float goodTime)
    {
        _endTime = MoveTime + goodTime / 1000.0f;

        float distance = Vector3.Distance(BeginPosition, EndPosition);
        MoveSpeed = distance / MoveTime;

        IsUsable = false;
        _isBegin = true;
        _sumTime = 0;
        transform.position = BeginPosition;
    }

    public void EndMove()
    {
        _isBegin = false;
        IsUsable = true;
        this.gameObject.SetActive(false);
    }
}