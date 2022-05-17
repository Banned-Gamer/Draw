using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class TestDefence : MonoBehaviour
{
    private Animator _animator;

    private const string _Isbegin = "IsBegin";
    private const string _IsAttack = "IsAttack";

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.SetBool(_Isbegin, true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Blade")
        {
            _animator.SetBool(_Isbegin, false);
            _animator.SetBool(_IsAttack, true);
        }
    }
}