using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasereye : MonoBehaviour
{
    public GameObject laserPrefab;
    public float laserSpeed = 5f;

    private void Update()
    {
     if(Input.GetMouseButtonDown(2))
        {
            Rigidbody laserclone;
            laserclone = Instantiate(laserPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            laserclone.velocity = this.transform.right * laserSpeed;
        }   
    }

}
