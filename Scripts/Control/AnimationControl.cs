using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    public Animator anim;
    private string currentState;
    private bool dontoverlap;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void ChangeAnimation(string newState)
    {
        if (currentState == newState)
            return;
        anim.Play(newState);
        currentState = newState;
    }
}
