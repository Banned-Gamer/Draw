using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayMusic : MonoBehaviour
{
    [Header("Show Text")] public        Text     ScoreText;
    [Header("Animators")] public        Animator ComboAnimator;
    public                              Animator MissAnimator;
    [Header("Data and Scripts")] public MusicSo  MusicData;
    public                              GamePlay MyGame;

    public                    Image HurtImage;
    [Header("Time")] public   float MoveTime;
    public                    float GoodTime;
    public                    float DefenceDelayTime;
    [Header("Number")] public int   CurrentDefenceIndex;

    [SerializeField, Header("Object father")]
    private Transform _attackCircleParent;

    [SerializeField] private Transform _defenceParent;

    private List<PlayNote> _attackCircles       = new();
    private List<PlayNote> _onUsedAttackCircles = new();

    private List<PlayDefenceNote> _defenseNotes       = new();
    private List<PlayDefenceNote> _onUsedDefenseNotes = new();

    private List<float> _attackBeginNoteTimes = new();
    private List<float> _attackBeginTimes     = new();
    private List<float> _attackEndTimes       = new();

    private List<float> _defenseBeginNoteTimes = new();
    private List<float> _defenseBeginTimes     = new();
    private List<float> _defenseEndTimes       = new();

    private AudioSource _audioSource;

    private int   _i;
    private int   _noteNumb;
    private int   _attackPointNumb;
    private int   _defensePointNumb;
    private int   _currentAttackIndex;
    private int   _currentAttackUsableIndex;
    private int   _currentDefenseUsableIndex;
    private int   _gameScore;
    private float _gameHealth;
    private float _sumTime;
    private float _targetTime;
    private bool  _isPlay = false;

    void Start()
    {
        _isPlay           = false;
        _audioSource      = GetComponent<AudioSource>();
        _audioSource.clip = MusicData.MyAudio;
        HurtImage.color   = new Color(255, 255, 255, 0);
    }

    void Update()
    {
        if (_isPlay)
        {
            //计算时间
            _sumTime += Time.deltaTime;

            if (_currentAttackUsableIndex < _attackPointNumb)
            {
                if (_sumTime >= _attackBeginNoteTimes[_currentAttackUsableIndex])
                {
                    BeginAttack();

                    _currentAttackUsableIndex++;
                }
            } //开始放置攻击note

            if (_currentDefenseUsableIndex < _defensePointNumb)
            {
                if (_sumTime >= _defenseBeginNoteTimes[_currentDefenseUsableIndex])
                {
                    BeginDefence();

                    _currentDefenseUsableIndex++;
                }
            } //开始放置防御note

            if (_currentAttackIndex < _attackPointNumb)
            {
                if (_sumTime > _attackEndTimes[_currentAttackIndex])
                {
                    MissAnimator.SetBool("IsAcitivity", true);
                    StartCoroutine(waitor2());
                    EndAttack();
                    _currentAttackIndex++;
                } //到达攻击note的结束时间
            }

            if (CurrentDefenceIndex < _defensePointNumb)
            {
                if (_sumTime > _defenseEndTimes[CurrentDefenceIndex])
                {
                    EndDefence();
                    MissAnimator.SetBool("IsAcitivity", true);
                    StartCoroutine(waitor2());

                    CurrentDefenceIndex++;
                } //到达防御note的结束时间
            }


            if (_sumTime >= _targetTime)
            {
                _isPlay = false;
                _audioSource.Stop();
                MyGame.end();
            } //超界，停止音乐和计时
        }
    }

    public void BeginPlay()
    {
        _isPlay = true;
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
        }     //更新登记攻击时间

        {
            {
                _onUsedDefenseNotes.Clear();
                _defenseNotes.Clear();
            }

            GameObject nowObject;
            _noteNumb = _defenceParent.childCount;
            for (_i = 0; _i < _noteNumb; _i++)
            {
                nowObject = _defenceParent.GetChild(_i).gameObject;
                _defenseNotes.Add(nowObject.GetComponent<PlayDefenceNote>());
            }
        } //更新防御note的列表

        {
            List<MusicSo.DefensePoint> defencePoints = MusicData.DefensePoints;
            _defensePointNumb = defencePoints.Count;

            {
                _defenseBeginNoteTimes.Clear();
                _defenseBeginTimes.Clear();
                _defenseEndTimes.Clear();
            }

            for (_i = 0; _i < _defensePointNumb; _i++)
            {
                _defenseBeginNoteTimes.Add(defencePoints[_i].TimePoint - GoodTime / 1000 - DefenceDelayTime);
                _defenseBeginTimes.Add(defencePoints[_i].TimePoint - GoodTime / 1000 + MoveTime);
                _defenseEndTimes.Add(defencePoints[_i].TimePoint + GoodTime / 1000 + MoveTime);
            }
        }

        {
            _gameHealth    = 100;
            _gameScore     = 0;
            ScoreText.text = _gameScore.ToString();
        }

        {
            _sumTime                  = 0;
            _currentAttackIndex       = 0;
            _currentAttackUsableIndex = 0;

            CurrentDefenceIndex        = 0;
            _currentDefenseUsableIndex = 0;
            _targetTime                = MusicData.MaxTime;
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
    } //开始放置攻击note

    void EndAttack()
    {
        ScoreText.text = _gameScore.ToString();
        if (_onUsedAttackCircles.Count > 0)
        {
            PlayNote nowObject;
            nowObject = _onUsedAttackCircles[0];
            _attackCircles.Add(nowObject);
            _attackCircles.Add(nowObject);
            _onUsedAttackCircles.RemoveAt(0);

            nowObject.EndAttackNote();
        }
    } //超时，结束攻击note

    public void AttackNote(int Score)
    {
        Debug.Log("add attack" + _currentAttackIndex + ' ' + _attackPointNumb);
        if (_currentAttackIndex < _attackPointNumb)
        {
            if (_sumTime >= _attackBeginTimes[_currentAttackIndex] && _sumTime <= _attackEndTimes[_currentAttackIndex])
            {
                Debug.Log("attack finish");
                {
                    _gameScore     += Score;
                    ScoreText.text =  _gameScore.ToString();
                }
                ComboAnimator.SetBool("IsAcitivity", true);
                StartCoroutine(waitor());
                EndAttack();
                _currentAttackIndex++;
            }
        }
    } //玩家攻击

    void BeginDefence()
    {
        List<MusicSo.DefensePoint> defencePoints = MusicData.DefensePoints;
        if (_defenseNotes.Count > 0)
        {
            PlayDefenceNote nowObject;
            nowObject = _defenseNotes[0];
            _defenseNotes.RemoveAt(0);
            _onUsedDefenseNotes.Add(nowObject);
            int x = _currentDefenseUsableIndex;
            nowObject.BeginDefenseNote(MusicData.DefensePoints[_currentDefenseUsableIndex], x);
        }
    } //开始放置防御note

    void EndDefence()
    {
        if (_onUsedDefenseNotes.Count > 0)
        {
            PlayDefenceNote nowObject;
            nowObject = _onUsedDefenseNotes[0];
            _defenseNotes.Add(nowObject);
            _onUsedDefenseNotes.RemoveAt(0);
            nowObject.EndDefenseNote();
            //_gameHealth -= 2.5f;
            HurtImage.color = new Color(255, 255, 255, (100 - _gameHealth) / 100);

            if (_gameHealth <= 0)
            {
                _isPlay = false;
                _audioSource.Stop();
                MyGame.Dead();
            }
        }
    } //超时，结束防御note

    public void Defence()
    {
        if (CurrentDefenceIndex < _defensePointNumb)
        {
            if (_sumTime >= _defenseBeginTimes[CurrentDefenceIndex] &&
                _sumTime <= _defenseEndTimes[CurrentDefenceIndex])
            {
                if (_onUsedDefenseNotes.Count > 0)
                {
                    PlayDefenceNote nowObject;
                    nowObject = _onUsedDefenseNotes[0];
                    _defenseNotes.Add(nowObject);
                    _onUsedDefenseNotes.RemoveAt(0);
                    CurrentDefenceIndex++;

                    _gameScore     += 10;
                    ScoreText.text =  _gameScore.ToString();
                    ComboAnimator.SetBool("IsAcitivity", true);
                    StartCoroutine(waitor());
                }
            }
        }
    } //主动进行防御

    IEnumerator waitor()
    {
        yield return new WaitForSeconds(0.05f);
        ComboAnimator.SetBool("IsAcitivity", false);
    }

    IEnumerator waitor2()
    {
        yield return new WaitForSeconds(0.05f);
        MissAnimator.SetBool("IsAcitivity", false);
    }
}