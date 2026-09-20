using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _animator.SetFloat("Thumb", 0.2f);
        _animator.SetFloat("Index", 0.4f);
        _animator.SetFloat("Middle", 0.6f);
        _animator.SetFloat("Ring", 0.8f);
        _animator.SetFloat("Pinky", 1.0f);
    }
}
