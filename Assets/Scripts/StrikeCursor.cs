using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeCursor : MonoBehaviour
{

    [SerializeField]
    private CameraZoom mCamera;

    [SerializeField]
    private GameObject marker;
    private GameObject cMarker;

    // Cursor
    private Transform[] cursorTransforms = new Transform[4];
    private Vector3[] cursorLocalPositions = new Vector3[4];
    private bool isHidden = true;
    private bool isMarkerSet = false;

    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            cursorTransforms[i] = transform.GetChild(i);
            cursorLocalPositions[i] = cursorTransforms[i].localPosition;
            cursorTransforms[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mCamera.isZoomedIn && !isMarkerSet)
        {
            FollowMouse();

            if (isHidden) ToggleCursor();
            if (Input.GetMouseButtonDown(0)) StartCoroutine(SetMarker());
        }

        else if (!mCamera.isZoomedIn && isHidden)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!isMarkerSet) mCamera.Zoom(mCamera.strikeZone.position, mCamera.zoomSize);
                SpeedBar.instance.StopCount();
            }
        }
    }


    public void ResetVariables()
    {
        isHidden = true;
        isMarkerSet = false;
    }


    private void ToggleCursor()
    {
        isHidden = !isHidden;
        Cursor.visible = isHidden;

        if (isHidden)
        {
            for (int i = 0; i < cursorTransforms.Length; i++)
            {
                cursorTransforms[i].gameObject.SetActive(false);
                cursorTransforms[i].localPosition = cursorLocalPositions[i];
            }
        }
        else
        {
            for (int i = 0; i < cursorTransforms.Length; i++)
            {
                cursorTransforms[i].gameObject.SetActive(true);
                cursorTransforms[i].localPosition *= SpeedMap(SpeedBar.amount);
            }
        }
         
    }


    private int SpeedMap(float speed)
    {
        if (speed < 12) return 1;
        else if (speed < 16) return 4;
        else return 8;
    }


    private void FollowMouse()
    {
        int speed = SpeedMap(SpeedBar.amount);
        float A = 0.025f * speed;
        float sin = A * Mathf.Sin(0.6f * speed * Time.time);
        float cos = A * Mathf.Cos(0.6f * speed * Time.time);
        var offset = new Vector3(sin, cos + sin);

        Vector3 mousePos = Input.mousePosition;
        Vector3 objectPos = Camera.main.ScreenToWorldPoint(mousePos);
        objectPos += offset;
        objectPos.z = 0;

        transform.position = Vector3.Lerp(transform.position, objectPos, Time.deltaTime * 20);
    }


    private IEnumerator SetMarker()
    {
        cMarker = Instantiate(marker, transform.position, Quaternion.identity);
        isMarkerSet = true;

        yield return new WaitForSeconds(0.5f);

        mCamera.Zoom(mCamera.initialPosition, mCamera.initialSize);
        ToggleCursor();

        yield return new WaitForSeconds(1);

        yield return StartCoroutine(Player.instance.Throw(cMarker.transform));
    }
}
