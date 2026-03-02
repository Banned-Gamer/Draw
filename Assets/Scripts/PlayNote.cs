using UnityEngine;

public class PlayNote : MonoBehaviour
{
    private static readonly int IsBegin  = Animator.StringToHash("IsBegin");
    private static readonly int IsAttack = Animator.StringToHash("IsAttack");

    private int   _i;
    private float _sumTime;
    private bool  _isBegin;
    private bool  _isStop;

    private Animator _selfAnimator;

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
            _selfAnimator.SetBool(IsBegin, false);
        } //begin状态需要关闭，但又不是一开始就关闭，所以静置一秒

        if (!(_sumTime > 2)) return;
        _isBegin = false;
        _selfAnimator.SetBool(IsAttack, false);
    }

    public void BeginAttackNote()
    {
        _selfAnimator.SetBool(IsBegin, true);
        _selfAnimator.SetBool(IsAttack, true);
        _isBegin = true;
        _isStop  = true;
        _sumTime = 0;
    }

    public void EndAttackNote()
    {
        _isStop = false;
        _selfAnimator.SetBool(IsBegin, false);

        _isBegin = false;
        _selfAnimator.SetBool(IsAttack, false);
    }
}
