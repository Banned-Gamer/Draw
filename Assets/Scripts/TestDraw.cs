using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PDollarGestureRecognizer;


public class TestDraw : MonoBehaviour
{
    //线条的Parent，用来挂载线条
    [SerializeField] private Transform _attackTransform;
    [SerializeField] private Transform _defenceTransform;
    [SerializeField] private PlayMusic _musicMgr;

    //线条Prefab，训练的点集，当前画布上的所有点
    public Transform GesturePrefabTransform;
    private List<Gesture> _trainSet = new List<Gesture>();
    private List<Point> _attackPoints = new List<Point>();
    private List<Point> _defencePoints = new List<Point>();

    //鼠标点击位置，绘画区间
    private Vector3 _virtualKeyPosition = Vector2.zero;
    private Rect _drawArea;

    //线条集合，当前的线条
    private List<LineRenderer> _attackLineRenderers = new List<LineRenderer>();
    private List<LineRenderer> _defenceLineRenderers = new List<LineRenderer>();
    private LineRenderer _curretnLineRenderer;

    //当前线条的点数，点的编号
    private int _vertexCount = 0;
    private int _strokeId = -1;
    public bool IsDraw = false;
    private int _currentAttackArea = -1;
    private float _liveTime;

    void Start()
    {
        //框定绘画范围
        _drawArea = new Rect(Screen.width * 0.31f, Screen.height * 0.17f, Screen.width * 0.38f, Screen.height * 0.68f);

        //读取训练的点集
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
            } //获取鼠标位置

            if (_drawArea.Contains(_virtualKeyPosition))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _currentAttackArea = 0;
                    ++_strokeId;
                    _liveTime = 0;

                    {
                        Transform tmpGesture =
                            Instantiate(GesturePrefabTransform, transform.position, transform.rotation,
                                _attackTransform) as Transform;
                        _curretnLineRenderer = tmpGesture.GetComponent<LineRenderer>();

                        _attackLineRenderers.Add(_curretnLineRenderer);
                    } //新建一条线

                    _vertexCount = 0;
                } //开始下笔

                if (Input.GetMouseButton(0))
                {
                    _liveTime += Time.deltaTime;

                    switch (_currentAttackArea)
                    {
                        case 0:
                        {
                            _attackPoints.Add(new Point(_virtualKeyPosition.x, -_virtualKeyPosition.y, _strokeId));

                            _curretnLineRenderer.positionCount = ++_vertexCount;
                            _curretnLineRenderer.SetPosition(_vertexCount - 1,
                                Camera.main.ScreenToWorldPoint(
                                    new Vector3(_virtualKeyPosition.x, _virtualKeyPosition.y, 10)));
                            break;
                        }
                        case 1:
                        {
                            OnDefenseMouseUp();
                            break;
                        }
                        default:
                            break;
                    }

                    if (_liveTime >= 0.5f)
                    {
                        _currentAttackArea = -1;
                        OnAttackMouseUp();
                    }
                } //动笔

                if (Input.GetMouseButtonUp(0))
                {
                    OnAttackMouseUp();
                } //抬笔
            } //绘画位置位于drawArea

            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _currentAttackArea = 1;
                    ++_strokeId;
                    _liveTime = 0;
                    {
                        Transform tmpGesture =
                            Instantiate(GesturePrefabTransform, transform.position, transform.rotation,
                                _defenceTransform) as Transform;
                        _curretnLineRenderer = tmpGesture.GetComponent<LineRenderer>();

                        _defenceLineRenderers.Add(_curretnLineRenderer);
                    } //新建一条线

                    _vertexCount = 0;
                } //开始下笔

                if (Input.GetMouseButton(0))
                {
                    _liveTime += Time.deltaTime;

                    switch (_currentAttackArea)
                    {
                        case 1:
                        {
                            _defencePoints.Add(new Point(_virtualKeyPosition.x, -_virtualKeyPosition.y, _strokeId));

                            _curretnLineRenderer.positionCount = ++_vertexCount;
                            _curretnLineRenderer.SetPosition(_vertexCount - 1,
                                Camera.main.ScreenToWorldPoint(
                                    new Vector3(_virtualKeyPosition.x, _virtualKeyPosition.y, 10)));
                            break;
                        }
                        case 0:
                        {
                            OnAttackMouseUp();
                            break;
                        }
                        default:
                            break;
                    }

                    if (_liveTime >= 0.5f)
                    {
                        OnDefenseMouseUp();
                        _currentAttackArea = -1;
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    OnDefenseMouseUp();
                }
            } //不在drawArea

            if (Input.GetAxisRaw("Cancel") != 0)
            {
                ClearAttackDraw();
            }
        }
    }

    public void ClearAttackDraw()
    {
        _strokeId = -1;

        _attackPoints.Clear();

        foreach (LineRenderer lineRenderer in _attackLineRenderers)
        {
            lineRenderer.positionCount = 0;
            Destroy(lineRenderer.gameObject);
        }

        _attackLineRenderers.Clear();
    }

    void ClearDefenceDraw()
    {
        _strokeId = -1;

        _defencePoints.Clear();

        foreach (LineRenderer lineRenderer in _defenceLineRenderers)
        {
            lineRenderer.positionCount = 0;
            Destroy(lineRenderer.gameObject);
        }

        _defenceLineRenderers.Clear();
    }

    int RecognizeGesture()
    {
        Gesture candidate = new Gesture(_attackPoints.ToArray());
        Result gestureResult = PointCloudRecognizer.Classify(candidate, _trainSet.ToArray());
        if (gestureResult.GestureClass == "target") return 1;
        return 0;
    }

    void OnAttackMouseUp()
    {
        _currentAttackArea = -1;
        if (_attackPoints.Count > 1)
        {
            int result = RecognizeGesture();
            if (result == 1)
            {
                _musicMgr.AttackNote(1000);
                ClearAttackDraw();
            }
            else
            {
                _musicMgr.AttackNote(200);
            }
        }
    }

    public void OnDefenseMouseUp()
    {
        _currentAttackArea = -1;
        ClearDefenceDraw();
    }
}