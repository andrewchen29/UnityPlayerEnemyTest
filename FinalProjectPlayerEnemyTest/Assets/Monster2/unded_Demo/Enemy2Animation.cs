using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2Animation : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;
    private int SpeedHash;
    private int AttackHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        SpeedHash = Animator.StringToHash("Speed");
        AttackHash = Animator.StringToHash("Attack"); 
    }

    public void Walk()
    {
        animator.SetFloat(SpeedHash, 1.0f);
    }

    public void Idle()
    {
        animator.SetFloat(SpeedHash, 0.0f);
    }

    public void Attack()
    {
        animator.SetTrigger(AttackHash);
    }
}
