using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Image healthbar;
    public Transform player_t;

    private void Update()
    {
        healthbar.fillAmount = (100 - RobotState.playerhealth) / 100;
        transform.position = player_t.position + new Vector3(0, 2, 0);
        transform.rotation = player_t.rotation;
        Quaternion rotationAmount = Quaternion.Euler(0, 90, 0);
        Quaternion postRotation = player_t.rotation * rotationAmount;
        transform.rotation = postRotation;
    }
}
