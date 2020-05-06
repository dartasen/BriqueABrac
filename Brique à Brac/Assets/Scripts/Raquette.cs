using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raquette : MonoBehaviour
{
    [Range(100f, 490f)]
    private float PosX;

    private float PosY;

    private const int VITESSE = 3;
    private const float BORDGAUCHE = 100f;
    private const float BORDDROIT = 490f;

    private void Update()
    {
        PosX = transform.position.x;
        PosY = transform.position.y;

        if (KinectManager.instance.IsKinectAvailable)
        {
            PosX = KinectManager.instance.BodyLean;
        }
        else
        {
            PosX = PosX + VITESSE * Input.GetAxis("Horizontal");
        }

        PosX = Mathf.Clamp(PosX, BORDGAUCHE, BORDDROIT);

        this.transform.position = new Vector3(PosX, PosY, 0f);
    }
}
