using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingInfo : MonoBehaviour
{
    
    public Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void FadeIn()
    {
        animator.Play("FadeIn", 0, 0f);
    }
    public void FadeOut()
    {
        animator.Play("FadeOut", 0, 0f);
    }
}
