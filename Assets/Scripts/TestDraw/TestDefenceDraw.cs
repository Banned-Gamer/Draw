using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PDollarGestureRecognizer;

public class TestDefenceDraw : MonoBehaviour
{
    //������Parent��������������
    [SerializeField] private Transform _attackTransform;
    [SerializeField] private Transform _defenceTransform;
    [SerializeField] private PlayMusic _musicMgr;

    //����Prefab��ѵ���ĵ㼯����ǰ�����ϵ����е�
    public Transform GesturePrefabTransform;
    private List<Gesture> _trainSet = new List<Gesture>();
    private List<Point> _attackPoints = new List<Point>();
    private List<Point> _defencePoints = new List<Point>();

    //�����λ�ã��滭����
    private Vector3 _virtualKeyPosition = Vector2.zero;
    private Rect _drawArea;

    //�������ϣ���ǰ������
    private List<LineRenderer> _attackLineRenderers = new List<LineRenderer>();
    private List<LineRenderer> _defenceLineRenderers = new List<LineRenderer>();
    private LineRenderer _curretnLineRenderer;

    //��ǰ�����ĵ�������ı��
    private int _vertexCount = 0;
    private int _strokeId = -1;
    public bool IsDraw = false;
    private int _currentAttackArea = -1;

    void Start()
    {
        //�򶨻滭��Χ
        _drawArea = new Rect(Screen.width * 0.31f, Screen.height * 0.17f, Screen.width * 0.38f, Screen.height * 0.68f);

        //��ȡѵ���ĵ㼯
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
            } //��ȡ���λ��
            
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _currentAttackArea = 1;
                    ++_strokeId;

                    {
                        Transform tmpGesture =
                            Instantiate(GesturePrefabTransform, transform.position, transform.rotation,
                                _defenceTransform) as Transform;
                        _curretnLineRenderer = tmpGesture.GetComponent<LineRenderer>();

                        _defenceLineRenderers.Add(_curretnLineRenderer);
                    } //�½�һ����

                    _vertexCount = 0;
                } //��ʼ�±�

                if (Input.GetMouseButton(0))
                {
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
                        default:
                            break;
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    OnDefenseMouseUp();
                }
            } //����drawArea
            
        }
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
    

    public void OnDefenseMouseUp()
    {
        _currentAttackArea = -1;
        ClearDefenceDraw();
    }
}
