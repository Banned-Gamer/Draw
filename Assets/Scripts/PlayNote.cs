using UnityEngine;

public class PlayNote : MonoBehaviour
{
    private int   _i;
    private float _sumTime;
    private bool  _isbegin = false;
    private bool  _isStop  = false;

    private Animator _selfAnimator;

    private const string _beginAttackNote = "isBegin";
    private const string _moveAttackNote  = "isAttack";

    void Start()
    {
        _selfAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (_isbegin)
        {
            _sumTime += Time.deltaTime;
            if (_sumTime > 1 && _isStop)
            {
                _isStop = false;
                _selfAnimator.SetBool(_beginAttackNote, false);
            } //begin状态需要关闭，但又不是一开始就关闭，所以静置一秒

            if (_sumTime > 2)
            {
                _isbegin = false;
                _selfAnimator.SetBool(_moveAttackNote, false);
            }
        }
    }

    public void BeginAttackNote()
    {
        _selfAnimator.SetBool(_beginAttackNote, true);
        _selfAnimator.SetBool(_moveAttackNote,  true);
        _isbegin = true;
        _isStop  = true;
        _sumTime = 0;
    }

    public void EndAttackNote()
    {
        _isStop = false;
        _selfAnimator.SetBool(_beginAttackNote, false);

        _isbegin = false;
        _selfAnimator.SetBool(_moveAttackNote, false);
    }
}