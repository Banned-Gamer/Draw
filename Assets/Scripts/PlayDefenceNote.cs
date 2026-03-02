using UnityEngine;

public class PlayDefenceNote : MonoBehaviour
{
    private static readonly int IsBegin  = Animator.StringToHash("IsBegin");
    private static readonly int IsEnd    = Animator.StringToHash("IsEnd");
    private static readonly int IsAttack = Animator.StringToHash("IsAttack");

    public PlayMusic MyPlayMusic;

    private MusicSo.DefencePoint _myDefencePoint;

    private Animator _animator;
    private bool     _isShow;
    private int      _currentNumb;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _isShow   = false;
    }

    public void BeginDefenseNote(MusicSo.DefencePoint targetDefencePoint, int numb)
    {
        _currentNumb    = numb;
        _myDefencePoint = targetDefencePoint;

        _isShow = true;

        _animator = GetComponent<Animator>();
        _animator.SetBool(IsBegin, true);

        transform.position = new Vector3(_myDefencePoint.x, _myDefencePoint.y);
    }

    public void EndDefenseNote()
    {
        if (!_isShow) return;
        _animator.SetBool(IsBegin, false);
        _animator.SetBool(IsEnd, true);
        _isShow = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isShow) return;
        if (_currentNumb != MyPlayMusic.currentDefenceIndex) return;
        if (!collision.CompareTag("Blade")) return;
        _animator.SetBool(IsBegin, false);
        _animator.SetBool(IsAttack, true);

        MyPlayMusic.Defense();
    }
}