using UnityEngine;
using UnityEngine.EventSystems;
using Windows.Kinect;

[AddComponentMenu("Kinect/Kinect Input Module")]
[RequireComponent(typeof(EventSystem))]
public class KinectInputModule : BaseInputModule
{
    public KinectInputData[] _inputData = new KinectInputData[0];
    [SerializeField]
    private float _scrollTreshold = .5f;
    [SerializeField]
    private float _scrollSpeed = 3.5f;
    [SerializeField]
    private float _waitOverTime = 2f;

    PointerEventData _handPointerData;

    static KinectInputModule _instance = null;
    public static KinectInputModule instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(KinectInputModule)) as KinectInputModule;

                if (!_instance)
                {
                    if (EventSystem.current){
                        EventSystem.current.gameObject.AddComponent<KinectInputModule>();
                        Debug.LogWarning("KinectInputModule doit être dans les EVENTSYSTEM");
                    }
                }
            }
            return _instance;
        }
    }

    public void TrackBody(Body body)
    {
        for (int i = 0; i < _inputData.Length; i++)
        {
            _inputData[i].UpdateComponent(body);
        }
    }

    private PointerEventData GetLookPointerEventData(Vector3 componentPosition)
    {
        if (_handPointerData == null)
        {
            _handPointerData = new PointerEventData(eventSystem);
        }
        _handPointerData.Reset();
        _handPointerData.delta = Vector2.zero;
        _handPointerData.position = componentPosition;
        _handPointerData.scrollDelta = Vector2.zero;
        eventSystem.RaycastAll(_handPointerData, m_RaycastResultCache);
        _handPointerData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        m_RaycastResultCache.Clear();
        return _handPointerData;
    }
   
    public override void Process()
    {
        ProcessHover();
        ProcessPress();
        //ProcessDrag();
        ProcessWaitOver();
    }

    private void ProcessWaitOver()
    {
        for (int j = 0; j < _inputData.Length; j++)
        {
            if (!_inputData[j].IsHovering || _inputData[j].ClickGesture != KinectUIClickGesture.WaitOver) continue;
            _inputData[j].WaitOverAmount = (Time.time - _inputData[j].HoverTime) / _waitOverTime;
            if (Time.time >= _inputData[j].HoverTime + _waitOverTime)
            {
                PointerEventData lookData = GetLookPointerEventData(_inputData[j].GetHandScreenPosition());
                GameObject go = lookData.pointerCurrentRaycast.gameObject;
                ExecuteEvents.ExecuteHierarchy(go, lookData, ExecuteEvents.submitHandler);
                _inputData[j].HoverTime = Time.time;
            }
        }
    }

    private void ProcessDrag()
    {
        for (int i = 0; i < _inputData.Length; i++)
        {
            if (!_inputData[i].IsPressing)
                continue;

            if (Mathf.Abs(_inputData[i].TempHandPosition.x - _inputData[i].HandPosition.x) > _scrollTreshold || Mathf.Abs(_inputData[i].TempHandPosition.y - _inputData[i].HandPosition.y) > _scrollTreshold)
            {
                _inputData[i].IsDraging = true;
            }
            else
            {
                _inputData[i].IsDraging = false;
            }

            if (_inputData[i].IsDraging)
            {
                PointerEventData lookData = GetLookPointerEventData(_inputData[i].GetHandScreenPosition());
                eventSystem.SetSelectedGameObject(null);

                GameObject go = lookData.pointerCurrentRaycast.gameObject;

                PointerEventData pEvent = new PointerEventData(eventSystem)
                {
                    dragging = true,
                    scrollDelta = (_inputData[i].TempHandPosition - _inputData[i].HandPosition) * _scrollSpeed,
                    useDragThreshold = true
                };

                ExecuteEvents.ExecuteHierarchy(go, pEvent, ExecuteEvents.scrollHandler);
            }
        }
    }

    private void ProcessPress()
    {
        for (int i = 0; i < _inputData.Length; i++)
        {
            if (!_inputData[i].IsHovering || _inputData[i].ClickGesture != KinectUIClickGesture.HandState)
                continue;

            if (_inputData[i].CurrentHandState == HandState.NotTracked)
            {
                _inputData[i].IsPressing = false;
                _inputData[i].IsDraging = false;
            }

            if (!_inputData[i].IsPressing && _inputData[i].CurrentHandState == HandState.Closed)
            {
                _inputData[i].IsPressing = true;
            }

            else if (_inputData[i].IsPressing && (_inputData[i].CurrentHandState == HandState.Open))
            {
                PointerEventData lookData = GetLookPointerEventData(_inputData[i].GetHandScreenPosition());
                eventSystem.SetSelectedGameObject(null);

                if (lookData.pointerCurrentRaycast.gameObject != null && !_inputData[i].IsDraging)
                {
                    GameObject go = lookData.pointerCurrentRaycast.gameObject;
                    ExecuteEvents.ExecuteHierarchy(go, lookData, ExecuteEvents.submitHandler);
                }

                _inputData[i].IsPressing = false;
            }
        }
    }

    private void ProcessHover()
    {
        for (int i = 0; i < _inputData.Length; i++)
        {
            PointerEventData pointer = GetLookPointerEventData(_inputData[i].GetHandScreenPosition());
            var obj = _handPointerData.pointerCurrentRaycast.gameObject;
            HandlePointerExitAndEnter(pointer, obj);

            _inputData[i].IsHovering = obj != null ? true : false;
            _inputData[i].HoveringObject = obj;
        }
    }

    public KinectInputData GetHandData(KinectUIHandType handType)
    {
        for (int i = 0; i < _inputData.Length; i++)
        {
            if (_inputData[i].trackingHandType == handType)
                return _inputData[i];
        }
        return null;
    }
}

[System.Serializable]
public class KinectInputData
{
    public KinectUIHandType trackingHandType = KinectUIHandType.Right;
    public float handScreenPositionMultiplier = 5f;
    private bool _isPressing;
    private GameObject _hoveringObject;

    public JointType handType
    {
        get
        {
            if (trackingHandType == KinectUIHandType.Right)
                return JointType.HandRight;
            else
                return JointType.HandLeft;
        }
    }

    public GameObject HoveringObject
    {
        get { return _hoveringObject; }
        set
        {
            if (value != _hoveringObject)
            {
                HoverTime = Time.time;
                _hoveringObject = value;
                if (_hoveringObject == null) return;
                if (_hoveringObject.GetComponent<KinectUIWaitOverButton>())
                    ClickGesture = KinectUIClickGesture.WaitOver;
                else
                    ClickGesture = KinectUIClickGesture.HandState;
                WaitOverAmount = 0f;
            }
        }
    }

    public HandState CurrentHandState { get; private set; }

    public KinectUIClickGesture ClickGesture { get; private set; }

    public bool IsTracking { get; private set; }

    public bool IsHovering { get; set; }

    public bool IsDraging { get; set; }

    public bool IsPressing
    {
        get { return _isPressing; }
        set
        {
            _isPressing = value;
            if (_isPressing)
                TempHandPosition = HandPosition;
        }
    }

    public Vector3 HandPosition { get; private set; }

    public Vector3 TempHandPosition { get; private set; }

    public float HoverTime { get; set; }

    public float WaitOverAmount { get; set; }


    public void UpdateComponent(Body body)
    {
        HandPosition = GetVector3FromJoint(body.Joints[handType]);
        CurrentHandState = GetStateFromJointType(body, handType);
        IsTracking = true;
    }

    public Vector3 GetHandScreenPosition()
    {
        return Camera.main.WorldToScreenPoint(
            new Vector3(HandPosition.x,
            HandPosition.y,
            HandPosition.z - handScreenPositionMultiplier)
            );
    }

    private HandState GetStateFromJointType(Body body, JointType type)
    {
        switch (type)
        {
            case JointType.HandLeft:
                return body.HandLeftState;
            case JointType.HandRight:
                return body.HandRightState;
            default:
                return body.HandRightState;
        }
    }

    private Vector3 GetVector3FromJoint(Windows.Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}

public enum KinectUIClickGesture
{
    HandState, Push, WaitOver
}
public enum KinectUIHandType
{
    Right,Left
}