using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brique : MonoBehaviour
{
    private GameObject particules;

    public void OnCollisionEnter(Collision collision)
    {
        gameManager.EnleverBrique();
        Destroy(gameObject);        
    }
}
