using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GamePlay : MonoBehaviour
{
    private static readonly int IsAttack = Animator.StringToHash("IsAttack");
    private static readonly int IsBegin  = Animator.StringToHash("IsBegin");
    private static readonly int IsDead   = Animator.StringToHash("IsDead");
    private static readonly int IsEnd    = Animator.StringToHash("IsEnd");

    [Header("Player")]
    public PlayMusic MyPlayMusic;

    public VideoPlayer MyPlayer;
    public GameObject  BackToEndButtonGameObject;

    [Header("Scripts and Objects")]
    public TestDraw Draw;

    public GameObject Play;
    public Text       PlayText;

    [Header("Animator")]
    public Animator TextAnimator;

    public Animator DefenceAnimator;
    public Animator AttackAnimator;
    public Animator DeadAnimator;
    public Animator EndAnimator;

    private int  _step;
    private bool _isDown;

    private void Start()
    {
        TextAnimator.SetBool(IsBegin, true);
        _isDown = false;
    }

    public void Change()
    {
        if (_isDown) return;
        _isDown = true;
        switch (_step)
        {
            case 0 :
                TextAnimator.SetBool(IsEnd, true);
                TextAnimator.SetBool(IsBegin, false);
                StartCoroutine(Waiter1("你，是一位浪荡江湖的\n侠客"));
                _step++;
                break;
            case 1 :
                TextAnimator.SetBool(IsEnd, true);
                TextAnimator.SetBool(IsBegin, false);
                StartCoroutine(Waiter1("在白色圈圈内通过画线\n你可以“攻击”别人"));
                _step++;
                break;
            case 2 :
                TextAnimator.SetBool(IsEnd, true);
                TextAnimator.SetBool(IsBegin, false);
                StartCoroutine(Waiter5("这里有个红色的圈圈\n当它收缩到白圈大小的时候\n就是你攻击的时机"));
                _step++;
                break;
            case 3 :
                TextAnimator.SetBool(IsEnd, true);
                TextAnimator.SetBool(IsBegin, false);
                StartCoroutine(Waiter1("你攻击的每个痕迹都会留下\n当痕迹组成“仁”字的时候\n会对敌人造成大量伤害"));
                _step++;
                break;
            case 4 :
                TextAnimator.SetBool(IsEnd, true);
                TextAnimator.SetBool(IsBegin, false);
                StartCoroutine(Waiter1("同时，你可以按下E键\n会擦除屏幕上的所有痕迹"));
                _step++;
                break;
            case 5 :
                TextAnimator.SetBool(IsEnd, true);
                TextAnimator.SetBool(IsBegin, false);
                StartCoroutine(Waiter4("在白色圈圈外，有时会出现\n红色的敌人"));
                _step++;
                break;
            case 6 :
                TextAnimator.SetBool(IsEnd, true);
                TextAnimator.SetBool(IsBegin, false);
                StartCoroutine(Waiter3("用你的刀斩向他们，\n（鼠标划过敌人）\n防止他们对你造成伤害"));
                _step++;
                break;
            case 7 :
                TextAnimator.SetBool(IsEnd, true);
                TextAnimator.SetBool(IsBegin, false);
                StartCoroutine(Waiter1("话已至此\n不如来试试看吧！"));
                _step++;
                StartCoroutine(Waiter2());
                break;
        }
    }

    public void End()
    {
        Draw.isDraw = false;
        EndAnimator.gameObject.SetActive(true);
        EndAnimator.SetBool(IsEnd, false);
        EndAnimator.SetBool(IsBegin, true);

        StartCoroutine(Waiter1("恭喜通关！"));

        Draw.ClearAttackDraw();
        BackToEndButtonGameObject.SetActive(true);
    }

    public void Dead()
    {
        Draw.isDraw = false;
        Draw.ClearAttackDraw();
        DeadAnimator.SetBool(IsDead, true);
    }

    public void SwitchToBegin()
    {
        SceneManager.LoadScene("BeginScene");
    }

    private IEnumerator Waiter1(string textMessage)
    {
        yield return new WaitForSeconds(2);
        PlayText.text = textMessage;
        TextAnimator.SetBool(IsEnd, false);
        TextAnimator.SetBool(IsBegin, true);
        _isDown = false;
    }

    private IEnumerator Waiter2()
    {
        yield return new WaitForSeconds(3);
        TextAnimator.SetBool(IsEnd, true);
        TextAnimator.SetBool(IsBegin, false);

        yield return new WaitForSeconds(1);
        MyPlayMusic.BeginPlay();
        Draw.isDraw = true;
        Play.SetActive(false);
    }

    public void BeginPlay()
    {
        MyPlayMusic.BeginPlay();
        Draw.isDraw = true;
        Play.SetActive(false);
    }

    private IEnumerator Waiter3(string textMessage)
    {
        yield return new WaitForSeconds(2);
        DefenceAnimator.SetBool(IsBegin, false);
        DefenceAnimator.SetBool(IsAttack, true);
        PlayText.text = textMessage;
        TextAnimator.SetBool(IsEnd, false);
        TextAnimator.SetBool(IsBegin, true);
        _isDown = false;
    }

    private IEnumerator Waiter4(string textMessage)
    {
        yield return new WaitForSeconds(0.5f);
        DefenceAnimator.SetBool(IsBegin, true);
        yield return new WaitForSeconds(1.5f);
        PlayText.text = textMessage;
        TextAnimator.SetBool(IsEnd, false);
        TextAnimator.SetBool(IsBegin, true);
        _isDown = false;
    }

    private IEnumerator Waiter5(string textMessage)
    {
        yield return new WaitForSeconds(2);
        AttackAnimator.SetBool(IsBegin, true);
        AttackAnimator.SetBool(IsAttack, true);
        PlayText.text = textMessage;
        TextAnimator.SetBool(IsEnd, false);
        TextAnimator.SetBool(IsBegin, true);
        yield return new WaitForSeconds(0.4f);
        AttackAnimator.SetBool(IsBegin, false);
        AttackAnimator.SetBool(IsAttack, false);

        _isDown = false;
    }
}