using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PDollarGestureRecognizer;


public class TestDraw : MonoBehaviour
{
    [SerializeField] private Transform _drawTransform;

    public Transform GesturePrefabTransform;
    private List<Gesture> _trainSet = new List<Gesture>();
    private List<Point> _points = new List<Point>();

    private Vector3 _virtualKeyPosition = Vector2.zero;
    private Rect _drawArea;

    private List<LineRenderer> _gestureLineRenderers = new List<LineRenderer>();
    private LineRenderer _curretnLineRendererl;

    private int _vertexCount = 0;
    private int _strokeId = -1;
    private bool _isrecognized;
    public bool IsDraw = false;
    private int _currentAttackArea = -1;

    void Start()
    {
        _drawArea = new Rect(Screen.width * 0.31f, Screen.height * 0.17f, Screen.width * 0.38f, Screen.height * 0.68f);

        string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        foreach (string filePath in filePaths)
            _trainSet.Add(GestureIO.ReadGestureFromFile(filePath));
    }

    void Update()
    {
        if (IsDraw)
        {
            if (Input.GetMouseButton(0))
            {
                _virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
            }

            if (_drawArea.Contains(_virtualKeyPosition))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _currentAttackArea = 0;
                    ++_strokeId;

                    {
                        Transform tmpGesture =
                            Instantiate(GesturePrefabTransform, transform.position, transform.rotation,
                                _drawTransform) as Transform;
                        _curretnLineRendererl = tmpGesture.GetComponent<LineRenderer>();

                        _gestureLineRenderers.Add(_curretnLineRendererl);
                    } //新建一条线

                    _vertexCount = 0;
                } //开始下笔

                if (Input.GetMouseButton(0))
                {
                    if (_currentAttackArea != 0)
                    {
                    } //超出范围
                    else
                    {
                        _points.Add(new Point(_virtualKeyPosition.x, -_virtualKeyPosition.y, _strokeId));

                        _curretnLineRendererl.positionCount = ++_vertexCount;
                        _curretnLineRendererl.SetPosition(_vertexCount - 1,
                            Camera.main.ScreenToWorldPoint(
                                new Vector3(_virtualKeyPosition.x, _virtualKeyPosition.y, 10)));
                    }
                } //动笔

                if (Input.GetMouseButtonUp(0))
                {
                    _currentAttackArea = -1;
                } //抬笔
            }
            else
            {
                _currentAttackArea = -1;
            }
        }
    }

    void ClearDraw()
    {
        _strokeId = -1;

        _points.Clear();

        foreach (LineRenderer lineRenderer in _gestureLineRenderers)
        {
            lineRenderer.positionCount = 0;
            Destroy(lineRenderer.gameObject);
        }

        _gestureLineRenderers.Clear();
    }

    int RecognizeGesture()
    {
        _isrecognized = true;

        Gesture candidate = new Gesture(_points.ToArray());
        Result gestureResult = PointCloudRecognizer.Classify(candidate, _trainSet.ToArray());

        string ss = gestureResult.GestureClass + " differ:" + gestureResult.Score;

        if (gestureResult.GestureClass == "target") return 1;
        return 0;
    }

    void OnAttackMouseUp()
    {
        _currentAttackArea = -1;
        int result = RecognizeGesture();
        if (result == 1)
        {
        }
    }

    void OnDefenseMouseUp()
    {
        _currentAttackArea = -1;
    }
}