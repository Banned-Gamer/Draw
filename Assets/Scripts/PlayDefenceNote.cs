using UnityEngine;

public class PlayDefenceNote : MonoBehaviour
{
    public PlayMusic MyPlayMusic;

    private MusicSo.DefencePoint _myDefensePoint;

    private Animator _animator;
    private bool     _isShow;
    private int      _currentNumb;

    private const string IsBegin  = "IsBegin";
    private const string IsEnd    = "IsEnd";
    private const string IsAttack = "IsAttack";

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _isShow   = false;
    }

    public void BeginDefenseNote(MusicSo.DefencePoint targetDefensePoint, int numb)
    {
        _currentNumb    = numb;
        _myDefensePoint = targetDefensePoint;

        _isShow = true;

        _animator = GetComponent<Animator>();
        _animator.SetBool(IsBegin, true);

        transform.position = new Vector3(_myDefensePoint.x, _myDefensePoint.y);
    }

    public void EndDefenseNote()
    {
        if (!_isShow) return;
        _animator.SetBool(IsBegin, false);
        _animator.SetBool(IsEnd,   true);
        _isShow = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isShow) return;
        if (_currentNumb != MyPlayMusic.CurrentDefenceIndex) return;
        if (collision.tag != "Blade") return;
        _animator.SetBool(IsBegin,  false);
        _animator.SetBool(IsAttack, true);

        MyPlayMusic.Defense();
    }
}