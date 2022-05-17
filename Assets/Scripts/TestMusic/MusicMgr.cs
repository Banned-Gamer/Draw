using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicMgr : MonoBehaviour
{
    public Text ScoreText;
    public Text ComboText;
    public Text ResultText;
    public MusicSo MusicData;
    public GameObject NoteParent;
    public float MoveTime;
    public float PerfectTime;
    public float GoodTime;

    private AudioSource _audioSource;
    private List<NoteMgr> _notes = new List<NoteMgr>();
    private List<NoteMgr> _onUsedNotes = new List<NoteMgr>();
    private List<float> _beginNoteTimes = new List<float>();
    private List<float> _beginTimes = new List<float>();
    private List<float> _endTimes = new List<float>();
    private List<float> _beginPerfectTimes = new List<float>();
    private List<float> _endPerfectTimes = new List<float>();

    private int _noteNumb;
    private int _pointNumb;
    private int _currentIndex;
    private int _currentUsableIndex;
    private float _sumTime;
    private int _gameScore;
    private int _comboNumb;
    private bool _isPlay = false;

    public void BeginPlay()
    {
        {
            _noteNumb = NoteParent.transform.childCount;

            {
                _onUsedNotes.Clear();
                _notes.Clear();
            } //����notes

            for (int i = 0; i < _noteNumb; i++)
            {
                GameObject currenObject = NoteParent.transform.GetChild(i).gameObject;
                _notes.Add(currenObject.GetComponent<NoteMgr>());
            }
        } //����note��Object

        {
            List<float> timePoints = MusicData.AttackPoints;
            _pointNumb = timePoints.Count;

            {
                _beginNoteTimes.Clear();
                _beginTimes.Clear();
                _endTimes.Clear();
                _beginPerfectTimes.Clear();
                _endPerfectTimes.Clear();
            } // ����times

            for (int i = 0; i < _pointNumb; i++)
            {
                _beginNoteTimes.Add(timePoints[i] - GoodTime / 1000);
                _beginTimes.Add(timePoints[i] - GoodTime / 1000 + MoveTime);
                _endTimes.Add(timePoints[i] + GoodTime / 1000 + MoveTime);
                _beginPerfectTimes.Add(timePoints[i] - PerfectTime / 1000 + MoveTime);
                _endPerfectTimes.Add(timePoints[i] + PerfectTime / 1000 + MoveTime);
            } //�������times
        } //���µǼ�ʱ��

        {
            _sumTime = 0;
            _currentIndex = 0;
            _currentUsableIndex = 0;
        } //ʱ���index����

        {
            _comboNumb = 0;
            _gameScore = 0;
            ComboText.text = "0";
            ScoreText.text = "0";
        } //score��combo����

        _isPlay = true;

        _audioSource.PlayDelayed(MoveTime);
    }

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


            if (_currentUsableIndex < _pointNumb)
            {
                if (_sumTime >= _beginNoteTimes[_currentUsableIndex])
                {
                    if (_notes.Count > 0)
                    {
                        NoteMgr currentNote = _notes[0];
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
                            NoteMgr currentNote = _onUsedNotes[0];
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
                } //Perfect�����°��°���

                else if (Input.GetAxisRaw("Submit") != 0)
                {
                    if (_onUsedNotes.Count > 0)
                    {
                        NoteMgr currentNote = _onUsedNotes[0];
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
                } //good�����°��°���
            }
            else if (_sumTime > _endTimes[_currentIndex])
            {
                if (_onUsedNotes.Count > 0)
                {
                    NoteMgr currentNote = _onUsedNotes[0];
                    _onUsedNotes.RemoveAt(0);
                    _notes.Add(currentNote);
                    currentNote.EndMove();
                    currentNote.gameObject.SetActive(false);
                }

                _comboNumb = 0;
                _currentIndex++;


                ComboText.text = _comboNumb.ToString();
                ResultText.text = "Miss";
            } //��ʱ����miss,���combo


            if (_currentIndex >= _pointNumb)
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
}