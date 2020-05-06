using UnityEngine;
using UnityEngine.UI;

using Windows.Kinect;

using System.Linq;
using System.Collections.Generic;
using System;

public sealed class KinectManager : MonoBehaviour
{
    #region Variables
    public static KinectManager instance = null;

    public Dictionary<Geste, bool> LGeste { get; private set; }

    public bool IsKinectAvailable { get; private set; }

    public float BodyLean { get; private set; }

    private BodyFrameReader _bodyFrameReader;
    private KinectSensor _sensor;

    private Body[] _bodies;
    public Body[] GetBodies()
    {
        return _bodies;
    }
    #endregion

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        //On initialise le dictionnaire avec les valeurs de l'enumération Geste
        LGeste = new Dictionary<Geste, bool>();
        foreach (Geste geste in Enum.GetValues(typeof(Geste)))
        {
            LGeste.Add(geste, false);
        }

        _sensor = KinectSensor.GetDefault();

        //Si la kinect est prête on récupère les informations du corps
        if (_sensor != null)
        {
            IsKinectAvailable = _sensor.IsAvailable;

            _bodyFrameReader = _sensor.BodyFrameSource.OpenReader();

            if (!_sensor.IsOpen)
            {
                _sensor.Open();
            }

            _bodies = new Body[_sensor.BodyFrameSource.BodyCount];
        }
    }

    public void Update()
    {
        IsKinectAvailable = _sensor.IsAvailable;

        if (_bodyFrameReader != null)
        {
            var frame = _bodyFrameReader.AcquireLatestFrame();

            if (frame != null)
            {
                frame.GetAndRefreshBodyData(_bodies);

                foreach (var body in _bodies.Where(b => b.IsTracked))
                {
                    IsKinectAvailable = true;

                    // On update le dictionnaire pour toutes les frames
                    if (body.HandLeftState == HandState.Open)
                    {
                        LGeste[Geste.LEFT_HAND_OPEN] = true;
                        LGeste[Geste.LEFT_HAND_CLOSE] = false;
                    }
                    else
                    {
                        LGeste[Geste.LEFT_HAND_OPEN] = false;
                        LGeste[Geste.LEFT_HAND_CLOSE] = true;
                    }

                    if (body.HandRightState == HandState.Open)
                    {
                        LGeste[Geste.RIGHT_HAND_OPEN] = true;
                        LGeste[Geste.RIGHT_HAND_CLOSE] = false;
                    }
                    else
                    {
                        LGeste[Geste.RIGHT_HAND_OPEN] = false;
                        LGeste[Geste.RIGHT_HAND_CLOSE] = true;
                    }

                    BodyLean = Rescale(-1, 1, 100, 490, body.Lean.X);
                }

                frame.Dispose();
                frame = null;
            }
        }
    }

    /// <summary>
    /// Lorsqu'on tue l'application on libère toutes les ressources
    /// </summary>
    public void OnApplicationQuit()
    {
        if (_bodyFrameReader != null)
        {
            _bodyFrameReader.IsPaused = true;
            _bodyFrameReader.Dispose();
            _bodyFrameReader = null;
        }

        if (_sensor != null)
        {
            if (_sensor.IsOpen)
            {
                _sensor.Close();
            }

            _sensor = null;
        }
    }

    #region Méthodes statiques
    public static float Rescale(float scaleAStart, float scaleAEnd, float scaleBStart, float scaleBEnd, float valueA)
    {
        return (((valueA - scaleAStart) * (scaleBEnd - scaleBStart)) / (scaleAEnd - scaleAStart)) + scaleBStart;
    }

    private static Vector3 GetVector3FromJoint(Windows.Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 100, joint.Position.Y * 200, joint.Position.Z * 100);
    }
    #endregion
}