using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAbility : MonoBehaviour
{
    public Transform target;
    public Transform player_t;
    public float autoAttackRange = 0.2f;
    public Animator anim;
    public GameObject head_g;
    private CapsuleCollider head;

    // Start is called before the first frame update
    void Start()
    {
        // head = GameObject.Find("Bone002").GetComponent<CapsuleCollider>();
        head = head_g.GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            anim.SetBool("autoAttack", true);
            attack();
        }
        else anim.SetBool("autoAttack", false);

    }

    IEnumerator attack()
    {
        head.enabled = true;
        yield return new WaitForSeconds(0.5f);
        head.enabled = false;
    }

}
