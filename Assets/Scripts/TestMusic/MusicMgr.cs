using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicMgr : MonoBehaviour
{
    public Text       ScoreText;
    public Text       ComboText;
    public Text       ResultText;
    public MusicSo    MusicData;
    public GameObject NoteParent;
    public float      MoveTime;
    public float      PerfectTime;
    public float      GoodTime;

    private AudioSource _audioSource;

    private readonly List<NoteMgr> _notes             = new();
    private readonly List<NoteMgr> _onUsedNotes       = new();
    private readonly List<float>   _beginNoteTimes    = new();
    private readonly List<float>   _beginTimes        = new();
    private readonly List<float>   _endTimes          = new();
    private readonly List<float>   _beginPerfectTimes = new();
    private readonly List<float>   _endPerfectTimes   = new();

    private int   _noteNumb;
    private int   _pointNumb;
    private int   _currentIndex;
    private int   _currentUsableIndex;
    private float _sumTime;
    private float _targetTime;
    private int   _gameScore;
    private int   _comboNumb;
    private bool  _isPlay;

    public void BeginPlay()
    {
        {
            _noteNumb = NoteParent.transform.childCount;

            {
                _onUsedNotes.Clear();
                _notes.Clear();
            } //清理notes

            for (var i = 0; i < _noteNumb; i++)
            {
                var currentObject = NoteParent.transform.GetChild(i).gameObject;
                _notes.Add(currentObject.GetComponent<NoteMgr>());
            }
        } //更新note的Object

        {
            var timePoints = MusicData.AttackPoints;
            _pointNumb = timePoints.Count;

            {
                _beginNoteTimes.Clear();
                _beginTimes.Clear();
                _endTimes.Clear();
                _beginPerfectTimes.Clear();
                _endPerfectTimes.Clear();
            } // 清理times

            for (var i = 0; i < _pointNumb; i++)
            {
                _beginNoteTimes.Add(timePoints[i]                         - GoodTime / 1000);
                _beginTimes.Add(timePoints[i] - GoodTime / 1000           + MoveTime);
                _endTimes.Add(timePoints[i]                               + GoodTime / 1000 + MoveTime);
                _beginPerfectTimes.Add(timePoints[i] - PerfectTime / 1000 + MoveTime);
                _endPerfectTimes.Add(timePoints[i]                        + PerfectTime / 1000 + MoveTime);
            } //重新添加times
        }     //更新登记时间

        {
            _sumTime            = 0;
            _currentIndex       = 0;
            _currentUsableIndex = 0;
        } //时间和index归零

        {
            _comboNumb     = 0;
            _gameScore     = 0;
            ComboText.text = "0";
            ScoreText.text = "0";
        } //score和combo归零

        _isPlay = true;
        _audioSource.PlayDelayed(MoveTime);
    }

    private void Start()
    {
        _isPlay           = false;
        _audioSource      = GetComponent<AudioSource>();
        _audioSource.clip = MusicData.MyAudio;
    }

    private void Update()
    {
        if (_isPlay)
        {
            _sumTime += Time.deltaTime;


            if (_currentUsableIndex < _pointNumb)
            {
                if (_sumTime >= _beginNoteTimes[_currentUsableIndex])
                {
                    if (_notes.Count > 0)
                    {
                        var currentNote = _notes[0];
                        _notes.RemoveAt(0);
                        _onUsedNotes.Add(currentNote);

                        currentNote.gameObject.SetActive(true);
                        currentNote.BeginMove(GoodTime);
                    }

                    _currentUsableIndex++;
                }
            }


            if (_sumTime >= _beginTimes[_currentIndex] && _sumTime <= _endTimes[_currentIndex])
            {
                if (_sumTime >= _beginPerfectTimes[_currentIndex] && _sumTime <= _endPerfectTimes[_currentIndex])
                {
                    if (Input.GetAxisRaw("Submit") != 0)
                    {
                        if (_onUsedNotes.Count > 0)
                        {
                            var currentNote = _onUsedNotes[0];
                            _onUsedNotes.RemoveAt(0);
                            _notes.Add(currentNote);
                            currentNote.EndMove();
                            currentNote.gameObject.SetActive(false);
                        }

                        _gameScore += 3;
                        _comboNumb++;
                        _currentIndex++;

                        ScoreText.text = _gameScore.ToString();
                        ComboText.text = _comboNumb.ToString();

                        ResultText.text = "Perfect";
                    }
                } //Perfect条件下按下按键

                else if (Input.GetAxisRaw("Submit") != 0)
                {
                    if (_onUsedNotes.Count > 0)
                    {
                        var currentNote = _onUsedNotes[0];
                        _onUsedNotes.RemoveAt(0);
                        _notes.Add(currentNote);
                        currentNote.EndMove();
                        currentNote.gameObject.SetActive(false);
                    }

                    _gameScore++;
                    _comboNumb++;
                    _currentIndex++;

                    ScoreText.text = _gameScore.ToString();
                    ComboText.text = _comboNumb.ToString();


                    ResultText.text = "Good";
                } //good条件下按下按键
            }
            else if (_sumTime > _endTimes[_currentIndex])
            {
                if (_onUsedNotes.Count > 0)
                {
                    var currentNote = _onUsedNotes[0];
                    _onUsedNotes.RemoveAt(0);
                    _notes.Add(currentNote);
                    currentNote.EndMove();
                    currentNote.gameObject.SetActive(false);
                }

                _comboNumb = 0;
                _currentIndex++;


                ComboText.text  = _comboNumb.ToString();
                ResultText.text = "Miss";
            } //超时就是miss,清空combo


            if (_currentIndex < _pointNumb) return;
            _isPlay = false;
            _audioSource.Stop();
            //超界，停止音乐和计时
        }

        else
        {
            if (Input.GetAxisRaw("Cancel") == 0) return;
            _isPlay = true;
            BeginPlay();
        }
    }
}