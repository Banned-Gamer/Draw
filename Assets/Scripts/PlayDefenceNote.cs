using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDefenceNote : MonoBehaviour
{
    public PlayMusic MyPlayMusic;

    private MusicSo.DefencePoint _myDefencePoint;

    private Animator _animator;
    private bool _isShow;

    private const string _Isbegin = "IsBegin";
    private const string _IsEnd = "IsEnd";
    private const string _IsAttack = "IsAttack";

    void Start()
    {
        _animator = GetComponent<Animator>();
        _isShow = false;
    }

    public void BeginDefenceNote(MusicSo.DefencePoint targetDefencePoint)
    {
        _myDefencePoint = targetDefencePoint;

        _isShow = true;

        _animator = GetComponent<Animator>();
        _animator.SetBool(_Isbegin, true);

        transform.position = new Vector3(_myDefencePoint.x, _myDefencePoint.y);
    }

    public void EndDefenceNote()
    {
        if (_isShow)
        {
            _animator.SetBool(_Isbegin, false);
            _animator.SetBool(_IsEnd, true);
            _isShow = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isShow)
        {
            if (collision.tag == "Blade")
            {
                _animator.SetBool(_Isbegin, false);
                _animator.SetBool(_IsAttack, true);

                MyPlayMusic.Defence();
            }
        }
    }
}