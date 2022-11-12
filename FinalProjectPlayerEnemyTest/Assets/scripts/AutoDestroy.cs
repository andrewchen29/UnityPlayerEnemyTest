using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(killme());
    }

    IEnumerator killme()
    {
        
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
}
