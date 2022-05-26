using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI; 

public class PlayMusic : MonoBehaviour
{
    public Text HealthText;
    public Text ScoreText;
    public GameObject CombineText;

    public MusicSo MusicData;
    public float MoveTime;
    public float GoodTime;

    [SerializeField] private Transform _attackCircleParent;
    [SerializeField] private Transform _defenceParent;
    [SerializeField] private float _defenceDelay;

    private List<PlayNote> _attackCircles = new List<PlayNote>();
    private List<PlayNote> _onUsedAttackCircles = new List<PlayNote>();

    private List<PlayDefenceNote> _defenceNotes = new List<PlayDefenceNote>();
    private List<PlayDefenceNote> _onUsedDefenceNotes = new List<PlayDefenceNote>();

    private List<float> _attackBeginNoteTimes = new List<float>();
    private List<float> _attackBeginTimes = new List<float>();
    private List<float> _attackEndTimes = new List<float>();

    private List<float> _defenceBeginNoteTimes = new List<float>();
    private List<float> _defenceBeginTimes = new List<float>();
    private List<float> _defenceEndTimes = new List<float>();

    private AudioSource _audioSource;

    private int _i;
    private int _noteNumb;
    private int _attackPointNumb;
    private int _defencePointNumb;
    private int _currentAttackIndex;
    private int _currentDefenceIndex;
    private int _currentAttackUsableIndex;
    private int _currentDefenceUsableIndex;
    private int _gameScore;
    private int _gameHealth;
    private float _sumTime;
    private float _targetTime;
    private bool _isPlay = false;

    void Start()
    {
        _isPlay = false;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = MusicData.MyAudio;
    }

    void Update()
    {
        if (_isPlay)
        {
            //����ʱ��
            _sumTime += Time.deltaTime;
            //Debug.Log("time:" + _sumTime.ToString());

            if (_currentAttackUsableIndex < _attackPointNumb)
            {
                if (_sumTime >= _attackBeginNoteTimes[_currentAttackUsableIndex])
                {
                    Debug.Log("begin attack");
                    BeginAttack();

                    _currentAttackUsableIndex++;
                }
            } //��ʼ���ù���note

            if (_currentDefenceUsableIndex < _defencePointNumb)
            {
                if (_sumTime >= _defenceBeginNoteTimes[_currentDefenceUsableIndex])
                {
                    Debug.Log("begin defence");
                    BeginDefence();

                    _currentDefenceUsableIndex++;
                }
            } //��ʼ���÷���note

            if (_currentAttackIndex < _attackPointNumb)
            {
                if (_sumTime > _attackEndTimes[_currentAttackIndex])
                {
                    Debug.Log("attack over time");
                    EndAttack();
                    _currentAttackIndex++;
                } //���﹥��note�Ľ���ʱ��
            }

            if (_currentDefenceIndex < _defencePointNumb)
            {
                if (_sumTime > _defenceEndTimes[_currentDefenceIndex])
                {
                    Debug.Log("c " + _currentDefenceIndex);
                    EndDefence();

                    _currentDefenceIndex++;
                } //�������note�Ľ���ʱ��
            }


            if (_sumTime >= _targetTime)
            {
                _isPlay = false;
                _audioSource.Stop();
            } //���磬ֹͣ���ֺͼ�ʱ
        }

        else
        {
            if (Input.GetAxisRaw("Cancel") != 0)
            {
                _isPlay = true;
                BeginPlay();
            }
        }
    }

    public void BeginPlay()
    {
        {
            {
                _onUsedAttackCircles.Clear();
                _attackCircles.Clear();
            }
            GameObject nowObject;

            _noteNumb = _attackCircleParent.childCount;
            for (_i = 0; _i < _noteNumb; _i++)
            {
                nowObject = _attackCircleParent.GetChild(_i).gameObject;
                _attackCircles.Add(nowObject.GetComponent<PlayNote>());
            }
        } //���¹���note���б�

        {
            List<float> timePoints = MusicData.AttackPoints;
            _attackPointNumb = timePoints.Count;

            {
                _attackBeginNoteTimes.Clear();
                _attackBeginTimes.Clear();
                _attackEndTimes.Clear();
            } // ����times

            for (int i = 0; i < _attackPointNumb; i++)
            {
                _attackBeginNoteTimes.Add(timePoints[i] - GoodTime / 1000);
                _attackBeginTimes.Add(timePoints[i] - GoodTime / 1000 + MoveTime);
                _attackEndTimes.Add(timePoints[i] + GoodTime / 1000 + MoveTime);
            } //�������times
        } //���µǼǹ���ʱ��

        {
            {
                _onUsedDefenceNotes.Clear();
                _defenceNotes.Clear();
            }

            GameObject nowObject;
            _noteNumb = _defenceParent.childCount;
            for (_i = 0; _i < _noteNumb; _i++)
            {
                nowObject = _defenceParent.GetChild(_i).gameObject;
                _defenceNotes.Add(nowObject.GetComponent<PlayDefenceNote>());
            }
        } //���·���note���б�

        {
            List<MusicSo.DefencePoint> defencePoints = MusicData.DefencePoints;
            _defencePointNumb = defencePoints.Count;

            {
                _defenceBeginNoteTimes.Clear();
                _defenceBeginTimes.Clear();
                _defenceEndTimes.Clear();
            }

            for (_i = 0; _i < _defencePointNumb; _i++)
            {
                _defenceBeginNoteTimes.Add(defencePoints[_i].TimePoint - GoodTime / 1000 - _defenceDelay);
                _defenceBeginTimes.Add(defencePoints[_i].TimePoint - GoodTime / 1000 + MoveTime);
                _defenceEndTimes.Add(defencePoints[_i].TimePoint + GoodTime / 1000 + MoveTime);
            }
        }

        {
            _gameHealth = 100;
            _gameScore = 0;
        }

        {
            _sumTime = 0;
            _currentAttackIndex = 0;
            _currentAttackUsableIndex = 0;

            _currentDefenceIndex = 0;
            _currentDefenceUsableIndex = 0;
            _targetTime = MusicData.MaxTime;
        } //ʱ���index����

        _isPlay = true;


        _audioSource.PlayDelayed(MoveTime);
    }

    void BeginAttack()
    {
        if (_attackCircles.Count > 0)
        {
            PlayNote nowObject;
            nowObject = _attackCircles[0];
            _attackCircles.RemoveAt(0);
            _onUsedAttackCircles.Add(nowObject);
            nowObject.BeginAttackNote();
        }
    } //��ʼ���ù���note

    void EndAttack()
    {
        if (_onUsedAttackCircles.Count > 0)
        {
            PlayNote nowObject;
            nowObject = _onUsedAttackCircles[0];
            _attackCircles.Add(nowObject);
            _onUsedAttackCircles.RemoveAt(0);

            nowObject.EndAttackNote();
            ScoreText.text = _gameScore.ToString();
        }
    } //��ʱ����������note

    public void AttackNote(int Score)
    {
        if (_currentAttackIndex < _attackPointNumb)
        {
            if (_sumTime >= _attackBeginTimes[_currentAttackIndex] && _sumTime <= _attackEndTimes[_currentAttackIndex])
            {
                {
                    _gameScore += Score;
                    ScoreText.text = _gameScore.ToString();
                }

                EndAttack();
                _currentAttackIndex++;
            }
        }
    } //��ҹ���

    void BeginDefence()
    {
        List<MusicSo.DefencePoint> defencePoints = MusicData.DefencePoints;
        if (_defenceNotes.Count > 0)
        {
            PlayDefenceNote nowObject;
            nowObject = _defenceNotes[0];
            _defenceNotes.RemoveAt(0);
            _onUsedDefenceNotes.Add(nowObject);
            nowObject.BeginDefenceNote(MusicData.DefencePoints[_currentDefenceUsableIndex]);
        }
    } //��ʼ���÷���note

    void EndDefence()
    {
        if (_onUsedDefenceNotes.Count > 0)
        {
            PlayDefenceNote nowObject;
            nowObject = _onUsedDefenceNotes[0];
            _defenceNotes.Add(nowObject);
            _onUsedDefenceNotes.RemoveAt(0);
            nowObject.EndDefenceNote();
            _gameHealth -= 10;
            HealthText.text = _gameHealth.ToString();
            Debug.Log("defence over time");
        }
    } //��ʱ����������note

    public void Defence()
    {
        Debug.Log("defence success");
        if (_currentDefenceIndex < _defencePointNumb)
        {
            Debug.Log("a " + _defenceBeginNoteTimes[_currentDefenceIndex]);
            Debug.Log("b " + _defenceEndTimes[_currentDefenceIndex]);
            if (_sumTime >= _defenceBeginTimes[_currentDefenceIndex] &&
                _sumTime <= _defenceEndTimes[_currentDefenceIndex])
            {
                if (_onUsedDefenceNotes.Count > 0)
                {
                    PlayDefenceNote nowObject;
                    nowObject = _onUsedDefenceNotes[0];
                    _defenceNotes.Add(nowObject);
                    _onUsedDefenceNotes.RemoveAt(0);
                    _currentAttackIndex++;
                    HealthText.text = _gameHealth.ToString();
                }
            }
        }
    } //�������з���
}