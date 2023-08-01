using PDollarGestureRecognizer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] private Transform  _drawTransform;
    [SerializeField] private Button     _addButton;
    [SerializeField] private Button     _recognizeButton;
    [SerializeField] private Button     _clearButton;
    [SerializeField] private InputField _inputField;
    [SerializeField] private Text       _text;

    public  Transform     GesturePrefabTransform;
    private List<Gesture> _trainSet = new();
    private List<Point>   _points   = new();

    private Vector3 _virtualKeyPosition = Vector2.zero;
    private Rect    _drawArea;

    private List<LineRenderer> _gestureLineRenderList = new();
    private LineRenderer       _currentLineRenderLine;

    private       int  _vertexCount = 0;
    private       int  _strokeId    = -1;
    private       bool _isRecognized;
    private const bool IsDraw = true;

    private void Start()
    {
        _drawArea = new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.8f, Screen.height * 0.8f);

        var filePaths = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        foreach (var filePath in filePaths)
            _trainSet.Add(GestureIO.ReadGestureFromFile(filePath));

        _recognizeButton.onClick.AddListener(RecognizeGesture);
        _addButton.onClick.AddListener(OnButtonAdd);
        _clearButton.onClick.AddListener(ClearDraw);
    }

    private void Update()
    {
        if (!IsDraw) return;
        if (Input.GetMouseButton(0))
        {
            _virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (!_drawArea.Contains(_virtualKeyPosition)) return;
        if (Input.GetMouseButtonDown(0))
        {
            if (GesturePrefabTransform == null)
            {
                Debug.Log("prefab null");
            }

            if (_isRecognized)
            {
                _isRecognized = false;
                ClearDraw();
            }

            ++_strokeId;

            var tmpGesture =
                Instantiate(GesturePrefabTransform, transform.position, transform.rotation,
                            _drawTransform) as Transform;
            _currentLineRenderLine = tmpGesture.GetComponent<LineRenderer>();
            if (_currentLineRenderLine == null)
            {
                Debug.Log("new line null");
            }

            _gestureLineRenderList.Add(_currentLineRenderLine);

            _vertexCount = 0;
        }

        if (Input.GetMouseButton(0))
        {
            _points.Add(new Point(_virtualKeyPosition.x, -_virtualKeyPosition.y, _strokeId));

            _currentLineRenderLine!.positionCount = ++_vertexCount;
            _currentLineRenderLine.SetPosition(_vertexCount - 1,
                                               Camera.main.ScreenToWorldPoint(new Vector3(_virtualKeyPosition.x,
                                                                                  _virtualKeyPosition.y, 10)));
        }
    }

    private void ClearDraw()
    {
        _strokeId = -1;

        _points.Clear();

        foreach (var lineRenderer in _gestureLineRenderList)
        {
            lineRenderer.positionCount = 0;
            Destroy(lineRenderer.gameObject);
        }

        _gestureLineRenderList.Clear();
    }

    private void RecognizeGesture()
    {
        _isRecognized = true;

        var candidate     = new Gesture(_points.ToArray());
        var gestureResult = PointCloudRecognizer.Classify(candidate, _trainSet.ToArray());

        _text.text = gestureResult.GestureClass + " differ:" + gestureResult.Score;
    }

    private void OnButtonAdd()
    {
        if (_inputField.text == "" || _points.Count <= 0) return;
        var newGestureName = _inputField.text;
        var fileName       = $"{Application.persistentDataPath}/{newGestureName}-{DateTime.Now.ToFileTime()}.xml";

#if !UNITY_WEBPLAYER
        GestureIO.WriteGesture(_points.ToArray(), newGestureName, fileName);
#endif

        _trainSet.Add(new Gesture(_points.ToArray(), newGestureName));

        newGestureName = "";
    }
}