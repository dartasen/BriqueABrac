using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balle : MonoBehaviour
{
    private const float initialSpeed = 300;

    public Rigidbody Corps { get; private set; }

    public bool IsMoving { get; set; }

    private bool Tmp;

    internal readonly IList<int> angleLancementBalle = new List<int>() {
            -300,-400,-500, 300, 400, 500
    };

    public void Awake()
    {
        Corps = GetComponent<Rigidbody>();
        IsMoving = false;
        transform.parent = null;
        Corps.isKinematic = false;
    }

    public void Update()
    {
        if (IsMoving)
        {
            return;
        }

        if (KinectManager.instance.IsKinectAvailable) {

            if (KinectManager.instance.LGeste.TryGetValue(Geste.RIGHT_HAND_OPEN, out Tmp))
            {
                if (Tmp)
                {
                    LancerBalle();
                    return;
                }
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            LancerBalle();
        }
    }

    public void LancerBalle()
    {
        transform.parent = null;
        Corps.isKinematic = false;

        int valAngle = angleLancementBalle[Random.Range(0, angleLancementBalle.Count - 1)];

        Corps.AddForce(valAngle, valAngle, 0);
        Corps.velocity = new Vector3(GetVelocity(valAngle), valAngle > 0 ? GetVelocity(valAngle) : GetVelocity(-valAngle), 0);

        IsMoving = true;

        Debug.Log("Angle : " + valAngle);
        Debug.Log("Vitesse : " + Corps.velocity);
    }

    private int GetVelocity(int i)
    {
        int tmp = (i < 0) ? -1 : 1;

        return Mathf.Abs(i) > 200 ? tmp * 200 : i;
    }
}
