using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class PlayMusic : MonoBehaviour
{
    public Text ComboText;
    public Text ScoreText;

    public MusicSo MusicData;
    public float MoveTime;
    public float PerfectTime;
    public float GoodTime;

    [SerializeField] private Transform _attackCircleParent;
    [SerializeField] private Transform _defenceParent;

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
    private int _comboNumb;
    private float _sumTime;
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
            _sumTime += Time.deltaTime;

            if (_currentAttackUsableIndex < _attackPointNumb)
            {
                if (_sumTime >= _attackBeginNoteTimes[_currentAttackUsableIndex])
                {
                    BeginAttack();

                    _currentAttackUsableIndex++;
                }
            }

            if (_sumTime > _attackEndTimes[_currentAttackIndex])
            {
                EndAttack();

                _comboNumb = 0;
                _currentAttackIndex++;
            }

            if (_currentAttackIndex >= _attackPointNumb)
            {
                _isPlay = false;
                _audioSource.Stop();
            } //超界，停止音乐和计时
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
        } //更新攻击note的列表

        {
            List<float> timePoints = MusicData.AttackPoints;
            _attackPointNumb = timePoints.Count;

            {
                _attackBeginNoteTimes.Clear();
                _attackBeginTimes.Clear();
                _attackEndTimes.Clear();
            } // 清理times

            for (int i = 0; i < _attackPointNumb; i++)
            {
                _attackBeginNoteTimes.Add(timePoints[i] - GoodTime / 1000);
                _attackBeginTimes.Add(timePoints[i] - GoodTime / 1000 + MoveTime);
                _attackEndTimes.Add(timePoints[i] + GoodTime / 1000 + MoveTime);
            } //重新添加times
        } //更新登记攻击时间

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
        } //更新防御note的列表

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
                _defenceBeginNoteTimes.Add(defencePoints[_i].TimePoint - GoodTime / 1000);
                _defenceBeginTimes.Add(defencePoints[_i].TimePoint - GoodTime / 1000 + MoveTime);
                _defenceEndTimes.Add(defencePoints[_i].TimePoint+GoodTime/1000+MoveTime);
            }
        }

        {
            _comboNumb = 0;
            _gameScore = 0;
        }

        {
            _sumTime = 0;
            _currentAttackIndex = 0;
            _currentAttackUsableIndex = 0;

            _currentDefenceIndex = 0;
            _currentDefenceUsableIndex = 0;

        } //时间和index归零

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
    }

    void EndAttack()
    {
        if (_onUsedAttackCircles.Count > 0)
        {
            PlayNote nowObject;
            nowObject = _onUsedAttackCircles[0];
            _attackCircles.Add(nowObject);
            _onUsedAttackCircles.RemoveAt(0);

            nowObject.EndAttackNote();
        }
    }

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
    }

    void BeginDefence()
    {
        if (_defenceNotes.Count > 0)
        {
            PlayDefenceNote nowObject;
            nowObject = _defenceNotes[0];
            _defenceNotes.RemoveAt(0);
            _onUsedDefenceNotes.Add(nowObject);
            nowObject.BeginDefenceNote();
        }
    }
}