using UnityEngine;

public class PlayNote : MonoBehaviour
{
    private int   _i;
    private float _sumTime;
    private bool  _isBegin = false;
    private bool  _isStop  = false;

    private Animator _selfAnimator;

    private const string _beginAttackNote = "isBegin";
    private const string MoveAttackNote   = "isAttack";

    private void Start()
    {
        _selfAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!_isBegin) return;
        _sumTime += Time.deltaTime;
        if (_sumTime > 1 && _isStop)
        {
            _isStop = false;
            _selfAnimator.SetBool(_beginAttackNote, false);
        } //begin状态需要关闭，但又不是一开始就关闭，所以静置一秒

        if (!(_sumTime > 2)) return;
        _isBegin = false;
        _selfAnimator.SetBool(MoveAttackNote, false);
    }

    public void BeginAttackNote()
    {
        _selfAnimator.SetBool(_beginAttackNote, true);
        _selfAnimator.SetBool(MoveAttackNote,   true);
        _isBegin = true;
        _isStop  = true;
        _sumTime = 0;
    }

    public void EndAttackNote()
    {
        _isStop = false;
        _selfAnimator.SetBool(_beginAttackNote, false);

        _isBegin = false;
        _selfAnimator.SetBool(MoveAttackNote, false);
    }
}