using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraIntro : CameraZoom
{
    public Vector3 target;
    public float size;

    public Camera slaveCamera;

    [Range(0,1)]
    public float X;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }


    public void StartIntro()
    {
        mCamera.rect = new Rect(0, 0, 0.5f, 1);

        slaveCamera.gameObject.SetActive(true);
        slaveCamera.rect = new Rect(0.5f, 0, 0.5f, 1);

        Zoom();
        slaveCamera.gameObject.GetComponent<CameraIntro>().Zoom();
    }


    public void EndIntro()
    {
        mCamera.rect = new Rect(0, 0, 1, 1);
        slaveCamera.gameObject.SetActive(false);
        Zoom(Vector2.zero, initialSize);
    }


    public void Zoom()
    {
        if (mCamera == null) mCamera = GetComponent<Camera>();
        Zoom(target, size);
    }
}
