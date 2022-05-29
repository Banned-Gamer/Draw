using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class GamePlay : MonoBehaviour
{
    [Header("Player")] public PlayMusic MyPlayMusic;
    public VideoPlayer MyPlayer;
    [Header("Scripts and Objects")] public TestDraw Draw;
    public GameObject Play;
    public GameObject VideoPlay;
    public Text PlayText;
    [Header("Animator")] public Animator TextAnimator;
    public Animator DefenceAnimator;
    public Animator AttackAnimator;
    public Animator DeadAnimator;

    private int step = 0;
    private bool isDown;

    void Start()
    {
        TextAnimator.SetBool("isbegin", true);
        isDown = false;
    }

    public void Change()
    {
        if (!isDown)
        {
            isDown = true;
            switch (step)
            {
                case 0:
                    TextAnimator.SetBool("isend", true);
                    TextAnimator.SetBool("isbegin", false);
                    StartCoroutine(waiter1("也是一个和武侠相关的\n游戏"));
                    step++;
                    break;
                case 1:
                    TextAnimator.SetBool("isend", true);
                    TextAnimator.SetBool("isbegin", false);
                    StartCoroutine(waiter1("在白色圈圈内画线进行\n“攻击”"));
                    step++;
                    break;
                case 2:
                    TextAnimator.SetBool("isend", true);
                    TextAnimator.SetBool("isbegin", false);
                    StartCoroutine(waiter4("在白色圈圈外，有时会出现\n“敌人”\n就是那些红色的饼"));
                    step++;
                    break;
                case 3:
                    TextAnimator.SetBool("isend", true);
                    TextAnimator.SetBool("isbegin", false);
                    StartCoroutine(waiter3("像切水果一样切掉他们，\n防止他们对你造成伤害"));
                    step++;
                    break;
                case 4:
                    TextAnimator.SetBool("isend", true);
                    TextAnimator.SetBool("isbegin", false);
                    StartCoroutine(waiter5("当攻击的时刻来临的时候\n会有红色的圈圈不断变小"));
                    step++;
                    break;
                case 5:
                    TextAnimator.SetBool("isend", true);
                    TextAnimator.SetBool("isbegin", false);
                    StartCoroutine(waiter1("当你“攻击”的线条形成汉字\n“仁”的时候\n造成的伤害会显著提高"));
                    step++;
                    break;
                case 6:
                    TextAnimator.SetBool("isend", true);
                    TextAnimator.SetBool("isbegin", false);
                    StartCoroutine(waiter1("不如来试试看吧！"));
                    step++;
                    StartCoroutine(waiter2());
                    break;
            }
        }
    }

    public void end()
    {
        Draw.IsDraw = false;
        Draw.ClearAttackDraw();
        VideoPlay.SetActive(true);
        StartCoroutine(waiterFinal());
    }

    public void Dead()
    {
        Draw.IsDraw = false;
        Draw.ClearAttackDraw();
        DeadAnimator.SetBool("IsDead",true);
    }

    public void SwitchToBegin()
    {
        SceneManager.LoadScene("BeginScene");
    }

    IEnumerator waiter1(string textMessage)
    {
        yield return new WaitForSeconds(2);
        PlayText.text = textMessage;
        TextAnimator.SetBool("isend", false);
        TextAnimator.SetBool("isbegin", true);
        isDown = false;
    }

    IEnumerator waiter2()
    {
        yield return new WaitForSeconds(3);
        TextAnimator.SetBool("isend", true);
        TextAnimator.SetBool("isbegin", false);

        yield return new WaitForSeconds(1);
        MyPlayMusic.BeginPlay();
        Draw.IsDraw = true;
        Play.SetActive(false);
    }

    IEnumerator waiter3(string textMessage)
    {
        yield return new WaitForSeconds(2);
        DefenceAnimator.SetBool("IsBegin", false);
        DefenceAnimator.SetBool("IsAttack", true);
        PlayText.text = textMessage;
        TextAnimator.SetBool("isend", false);
        TextAnimator.SetBool("isbegin", true);
        isDown = false;
    }

    IEnumerator waiter4(string textMessage)
    {
        yield return new WaitForSeconds(0.5f);
        DefenceAnimator.SetBool("IsBegin", true);
        yield return new WaitForSeconds(1.5f);
        PlayText.text = textMessage;
        TextAnimator.SetBool("isend", false);
        TextAnimator.SetBool("isbegin", true);
        isDown = false;
    }

    IEnumerator waiter5(string textMessage)
    {
        yield return new WaitForSeconds(2);
        AttackAnimator.SetBool("isBegin", true);
        AttackAnimator.SetBool("isAttack", true);
        PlayText.text = textMessage;
        TextAnimator.SetBool("isend", false);
        TextAnimator.SetBool("isbegin", true);
        yield return new WaitForSeconds(0.4f);
        AttackAnimator.SetBool("isBegin", false);
        AttackAnimator.SetBool("isAttack", false);

        isDown = false;
    }

    IEnumerator waiterFinal()
    {
        yield return new WaitForSeconds(2);
        MyPlayer.Play();

        yield return new WaitForSeconds(105);
        SwitchToBegin();
    }
}