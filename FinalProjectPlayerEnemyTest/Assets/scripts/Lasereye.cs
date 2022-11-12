using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasereye : MonoBehaviour
{
    public GameObject laserPrefab;
    public float laserSpeed = 50f;
    public Animator anim;

    private void Update()
    {
     if(Input.GetMouseButtonDown(2))
        {
            Rigidbody laserclone;
            anim.SetBool("laserAttack", true);
            laserclone = Instantiate(laserPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            laserclone.velocity = this.transform.right * laserSpeed;
        }
     else anim.SetBool("laserAttack", false);
    }

}
