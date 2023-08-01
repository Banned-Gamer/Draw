using UnityEngine;

public class PlayDefenceNote : MonoBehaviour
{
    public PlayMusic MyPlayMusic;

    private MusicSo.DefensePoint _myDefensePoint;

    private Animator _animator;
    private bool     _isShow;
    private int      _currentNumb;

    private const string _Isbegin  = "IsBegin";
    private const string _IsEnd    = "IsEnd";
    private const string _IsAttack = "IsAttack";

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _isShow   = false;
    }

    public void BeginDefenseNote(MusicSo.DefensePoint targetDefensePoint, int numb)
    {
        _currentNumb    = numb;
        _myDefensePoint = targetDefensePoint;

        _isShow = true;

        _animator = GetComponent<Animator>();
        _animator.SetBool(_Isbegin, true);

        transform.position = new Vector3(_myDefensePoint.x, _myDefensePoint.y);
    }

    public void EndDefenseNote()
    {
        if (!_isShow) return;
        _animator.SetBool(_Isbegin, false);
        _animator.SetBool(_IsEnd,   true);
        _isShow = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isShow) return;
        if (_currentNumb != MyPlayMusic.CurrentDefenceIndex) return;
        if (collision.tag != "Blade") return;
        _animator.SetBool(_Isbegin,  false);
        _animator.SetBool(_IsAttack, true);

        MyPlayMusic.Defence();
    }
}