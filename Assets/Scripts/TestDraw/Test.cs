using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using PDollarGestureRecognizer;

public class Test : MonoBehaviour
{
    [SerializeField] private Transform _drawTransform;
    [SerializeField] private Button _addButton;
    [SerializeField] private Button _recognizeButton;
    [SerializeField] private Button _clearButton;
    [SerializeField] private InputField _inputField;
    [SerializeField] private Text _text;

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
    private bool _isDraw = true;

    void Start()
    {
        _drawArea = new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.8f, Screen.height * 0.8f);

        string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        foreach (string filePath in filePaths)
            _trainSet.Add(GestureIO.ReadGestureFromFile(filePath));

        _recognizeButton.onClick.AddListener(RecognizeGesture);
        _addButton.onClick.AddListener(OnButtonAdd);
        _clearButton.onClick.AddListener(ClearDraw);
    }

    void Update()
    {
        if (_isDraw)
        {
            if (Input.GetMouseButton(0))
            {
                _virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
            }

            if (_drawArea.Contains(_virtualKeyPosition))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (GesturePrefabTransform == null)
                    {
                        Debug.Log("prefab null");
                    }

                    if (_isrecognized)
                    {
                        _isrecognized = false;
                        ClearDraw();
                    }

                    ++_strokeId;

                    Transform tmpGesture =
                        Instantiate(GesturePrefabTransform, transform.position, transform.rotation,
                            _drawTransform) as Transform;
                    _curretnLineRendererl = tmpGesture.GetComponent<LineRenderer>();
                    if (_curretnLineRendererl == null)
                    {
                        Debug.Log("new line null");
                    }
                    _gestureLineRenderers.Add(_curretnLineRendererl);

                    _vertexCount = 0;
                }

                if (Input.GetMouseButton(0))
                {
                    _points.Add(new Point(_virtualKeyPosition.x, -_virtualKeyPosition.y, _strokeId));

                    _curretnLineRendererl.positionCount = ++_vertexCount;
                    _curretnLineRendererl.SetPosition(_vertexCount - 1,
                        Camera.main.ScreenToWorldPoint(new Vector3(_virtualKeyPosition.x, _virtualKeyPosition.y, 10)));
                }
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

    void RecognizeGesture()
    {
        _isrecognized = true;

        Gesture candidate = new Gesture(_points.ToArray());
        Result gestureResult = PointCloudRecognizer.Classify(candidate, _trainSet.ToArray());

        _text.text = gestureResult.GestureClass + " differ:" + gestureResult.Score;
    }

    void OnButtonAdd()
    {
        if (_inputField.text != "" && _points.Count > 0)
        {
            string newGestureName = _inputField.text;
            string fileName = String.Format("{0}/{1}-{2}.xml", Application.persistentDataPath, newGestureName,
                DateTime.Now.ToFileTime());

#if !UNITY_WEBPLAYER
            GestureIO.WriteGesture(_points.ToArray(), newGestureName, fileName);
#endif

            _trainSet.Add(new Gesture(_points.ToArray(), newGestureName));

            newGestureName = "";
        }
    }
}