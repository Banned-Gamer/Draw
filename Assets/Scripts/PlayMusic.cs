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

    private readonly List<PlayNote> _attackCircles       = new();
    private readonly List<PlayNote> _onUsedAttackCircles = new();

    private readonly List<PlayDefenceNote> _defenseNotes       = new();
    private readonly List<PlayDefenceNote> _onUsedDefenseNotes = new();

    private readonly List<float> _attackBeginNoteTimes = new();
    private readonly List<float> _attackBeginTimes     = new();
    private readonly List<float> _attackEndTimes       = new();

    private readonly List<float> _defenseBeginNoteTimes = new();
    private readonly List<float> _defenseBeginTimes     = new();
    private readonly List<float> _defenseEndTimes       = new();

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
    private bool  _isPlay;

    private void Start()
    {
        _isPlay           = false;
        _audioSource      = GetComponent<AudioSource>();
        _audioSource.clip = MusicData.MyAudio;
        HurtImage.color   = new Color(255, 255, 255, 0);
    }

    private void Update()
    {
        if (!_isPlay) return;
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
                BeginDefense();

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
                EndDefense();
                MissAnimator.SetBool("IsAcitivity", true);
                StartCoroutine(waitor2());

                CurrentDefenceIndex++;
            } //到达防御note的结束时间
        }


        if (!(_sumTime >= _targetTime)) return;
        _isPlay = false;
        _audioSource.Stop();
        MyGame.end();
        //超界，停止音乐和计时
    }

    public void BeginPlay()
    {
        _isPlay = true;
        {
            {
                _onUsedAttackCircles.Clear();
                _attackCircles.Clear();
            }

            _noteNumb = _attackCircleParent.childCount;
            for (_i = 0; _i < _noteNumb; _i++)
            {
                var nowObject = _attackCircleParent.GetChild(_i).gameObject;
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

            _noteNumb = _defenceParent.childCount;
            for (_i = 0; _i < _noteNumb; _i++)
            {
                var nowObject = _defenceParent.GetChild(_i).gameObject;
                _defenseNotes.Add(nowObject.GetComponent<PlayDefenceNote>());
            }
        } //更新防御note的列表

        {
            List<MusicSo.DefencePoint> defensePoints = MusicData.DefencePoints;
            _defensePointNumb = defensePoints.Count;

            {
                _defenseBeginNoteTimes.Clear();
                _defenseBeginTimes.Clear();
                _defenseEndTimes.Clear();
            }

            for (_i = 0; _i < _defensePointNumb; _i++)
            {
                _defenseBeginNoteTimes.Add(defensePoints[_i].TimePoint - GoodTime / 1000 - DefenceDelayTime);
                _defenseBeginTimes.Add(defensePoints[_i].TimePoint - GoodTime / 1000 + MoveTime);
                _defenseEndTimes.Add(defensePoints[_i].TimePoint + GoodTime / 1000 + MoveTime);
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

    private void BeginAttack()
    {
        if (_attackCircles.Count <= 0) return;
        var nowObject = _attackCircles[0];
        _attackCircles.RemoveAt(0);
        _onUsedAttackCircles.Add(nowObject);
        nowObject.BeginAttackNote();
    } //开始放置攻击note

    private void EndAttack()
    {
        ScoreText.text = _gameScore.ToString();
        if (_onUsedAttackCircles.Count <= 0) return;
        var nowObject = _onUsedAttackCircles[0];
        _attackCircles.Add(nowObject);
        _attackCircles.Add(nowObject);
        _onUsedAttackCircles.RemoveAt(0);

        nowObject.EndAttackNote();
    } //超时，结束攻击note

    public void AttackNote(int Score)
    {
        Debug.Log("add attack" + _currentAttackIndex + ' ' + _attackPointNumb);
        if (_currentAttackIndex >= _attackPointNumb) return;
        if (!(_sumTime >= _attackBeginTimes[_currentAttackIndex]) ||
            !(_sumTime <= _attackEndTimes[_currentAttackIndex])) return;
        Debug.Log("attack finish");
        {
            _gameScore     += Score;
            ScoreText.text =  _gameScore.ToString();
        }
        ComboAnimator.SetBool("IsAcitivity", true);
        StartCoroutine(waitor());
        EndAttack();
        _currentAttackIndex++;
    } //玩家攻击

    private void BeginDefense()
    {
        List<MusicSo.DefencePoint> defensePoints = MusicData.DefencePoints;
        if (_defenseNotes.Count > 0)
        {
            var nowObject = _defenseNotes[0];
            _defenseNotes.RemoveAt(0);
            _onUsedDefenseNotes.Add(nowObject);
            var x = _currentDefenseUsableIndex;
            nowObject.BeginDefenseNote(MusicData.DefencePoints[_currentDefenseUsableIndex], x);
        }
    } //开始放置防御note

    private void EndDefense()
    {
        if (_onUsedDefenseNotes.Count <= 0) return;
        var nowObject = _onUsedDefenseNotes[0];
        _defenseNotes.Add(nowObject);
        _onUsedDefenseNotes.RemoveAt(0);
        nowObject.EndDefenseNote();
        _gameHealth     -= 2.5f;
        HurtImage.color =  new Color(255, 255, 255, (100 - _gameHealth) / 100);

        if (!(_gameHealth <= 0)) return;
        _isPlay = false;
        _audioSource.Stop();
        MyGame.Dead();
    } //超时，结束防御note

    public void Defense()
    {
        if (CurrentDefenceIndex >= _defensePointNumb) return;
        if (!(_sumTime >= _defenseBeginTimes[CurrentDefenceIndex]) ||
            !(_sumTime <= _defenseEndTimes[CurrentDefenceIndex])) return;
        if (_onUsedDefenseNotes.Count <= 0) return;
        var nowObject = _onUsedDefenseNotes[0];
        _defenseNotes.Add(nowObject);
        _onUsedDefenseNotes.RemoveAt(0);
        CurrentDefenceIndex++;

        _gameScore     += 10;
        ScoreText.text =  _gameScore.ToString();
        ComboAnimator.SetBool("IsAcitivity", true);
        StartCoroutine(waitor());
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