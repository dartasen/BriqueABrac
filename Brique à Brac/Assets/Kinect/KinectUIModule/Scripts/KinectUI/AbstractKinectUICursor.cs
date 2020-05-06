using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Windows.Kinect;

public abstract class AbstractKinectUICursor : MonoBehaviour {

    protected KinectUIHandType _handType;
    protected KinectInputData _data;
    protected Image _image;

    public virtual void Start()
    {
        Setup();
    }

    protected void Setup()
    {
        _data = KinectInputModule.instance.GetHandData(_handType);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        GetComponent<CanvasGroup>().interactable = false;
        _image = GetComponent<Image>();
    }

    public virtual void Update()
    {
        if (_data == null || !_data.IsTracking)
            return;

        ProcessData();
    }

    public abstract void ProcessData();
}
