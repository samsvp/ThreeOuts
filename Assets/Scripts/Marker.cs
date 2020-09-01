using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public static Marker instance;


    [SerializeField]
    private GameObject crack;

    private bool ballHit = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Screenspace is defined in pixels. The bottom-left of the screen
        // is (0,0); the right-top is (pixelWidth,pixelHeight).
        // The z position is in world units from the camera.
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        Ball.instance.SetBallWidth(screenPos.x / Camera.main.pixelWidth);
        Ball.instance.SetBallHeight(screenPos.y / Camera.main.pixelHeight);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("StrikeZone"))
        {
            GameManager.isMarkerOnStrikeZone = true;
        }
        if (collision.CompareTag("Ball"))
        {
            ballHit = true;
        }
    }


    private void OnDestroy()
    {
        if (ballHit) Instantiate(crack, transform.position, Quaternion.identity);
    }
}
