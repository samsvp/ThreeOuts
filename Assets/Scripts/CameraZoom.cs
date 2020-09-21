using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{

    protected Camera mCamera;

    // Positions
    public Transform strikeZone;
    [HideInInspector]
    public Vector2 initialPosition;
    private int z = -10;

    // Zoom Sizes
    [HideInInspector]
    public float initialSize;
    public float zoomSize = 0.5f;

    // Speed
    [SerializeField]
    private float speed = 20;

    [HideInInspector]
    public bool isZoomedIn = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        mCamera = GetComponent<Camera>();
        initialSize = mCamera.orthographicSize;
        initialPosition = transform.position;
    }
    
    
    public void Zoom(Vector2 position, float size)
    {
        StopAllCoroutines();
        StartCoroutine(Zoom(position));
        StartCoroutine(Zoom(size));
    }


    private IEnumerator Zoom(float size)
    {
        while (Mathf.Abs(Camera.main.orthographicSize - size) > 0.05f)
        {
            mCamera.orthographicSize = Mathf.Lerp(mCamera.orthographicSize, size, speed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        mCamera.orthographicSize = size;

        isZoomedIn = !isZoomedIn;
    }


    private IEnumerator Zoom(Vector2 position)
    {
        while (Vector2.Distance(transform.position, position) > 0.05f)
        {
            var mPos = Vector2.Lerp(transform.position, position, speed * Time.fixedDeltaTime);
            transform.position = new Vector3(mPos.x, mPos.y, z);
            yield return new WaitForFixedUpdate();
        }

        transform.position = new Vector3(position.x, position.y, z);
    }
}
