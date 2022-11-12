using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotState : MonoBehaviour
{
    public static float playerhealth=100;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerhealth < 0)
            playerhealth = 100;
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Damage")
        {
            Debug.Log("Hurt!");
            playerhealth -= 10;
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("triggerEnter");
        if (other.tag == "Damage")
        {
            Debug.Log("Hurt!");
            playerhealth -= 10;
            GetComponent<HurtEffect>().position = transform.position + new Vector3(0.0f, 1.0f, 0.0f);
            GetComponent<HurtEffect>().Spawn();
        }
    }

}
