using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    public static Ball instance;
    
    private float speed = 0;
    private Rigidbody2D rb2d;

    private bool isHit = false;
    [HideInInspector]
    public bool willBatterHit = false;

    public enum SpeedType
    {
        slow, normal, fast
    }

    public enum BallHeights
    {
        farLow, low, mid, high, farHigh
    }

    public enum BallWidths
    {
        farLeft, left, mid, right, farRight
    }

    private SpeedType speedType;
    private BallHeights ballHeight;
    private BallWidths ballWidth;

    public BallHeights BallHeight
    {
        get { return ballHeight; }
    }

    public BallWidths BallWidth
    {
        get { return ballWidth; }
    }

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        Batter.instance.MakePredictions();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetSpeed(float mSpeed)
    {
        speed = mSpeed;
        SetSpeedType();
    }


    private void SetSpeedType()
    {
        if (speed < 12) speedType = SpeedType.slow;
        else if (speed < 16) speedType = SpeedType.normal;
        else speedType = SpeedType.fast;
    }


    public void SetBallHeight(float position)
    {
        if (position < 0.2f) ballHeight = BallHeights.farLow;
        else if (position < 0.4f) ballHeight = BallHeights.low;
        else if (position < 0.6f) ballHeight = BallHeights.mid;
        else if (position < 0.8f) ballHeight = BallHeights.high;
        else ballHeight = BallHeights.farHigh;
    }


    public void SetBallWidth(float position)
    {
        if (position < 0.2f) ballWidth = BallWidths.farLeft;
        else if (position < 0.4f) ballWidth = BallWidths.left;
        else if (position < 0.6f) ballWidth = BallWidths.mid;
        else if (position < 0.8f) ballWidth = BallWidths.right;
        else ballWidth = BallWidths.farRight;
    }


    public SpeedType GetSpeedType()
    {
        return speedType;
    }


    public void Throw(Transform marker)
    {
        Batter.instance.UpdateLists(ballHeight, ballWidth, speedType);
        willBatterHit = Batter.instance.WillHit(ballHeight, ballWidth, speedType);
        StartCoroutine(IThrow(marker));
    }


    private IEnumerator IThrow(Transform marker)
    {
        rb2d.velocity = speed * (marker.position - transform.position).normalized;
        yield return new WaitWhile(() => marker.position.y - transform.position.y > 0.1f);

        transform.position = marker.position;
    }


    private IEnumerator SlowDown()
    {
        while (rb2d.velocity.sqrMagnitude > 0.5f)
        {
            rb2d.velocity *= 0.95f;
            yield return new WaitForFixedUpdate();
        }

        rb2d.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.2f);

        GameManager.instance.NextThrow();
    }


    private IEnumerator Hit()
    {
        rb2d.velocity *= 1.2f;

        yield return new WaitForSeconds(1);

        GameManager.instance.NextThrow();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isHit) return;
        if (collision.CompareTag("Marker"))
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, -rb2d.velocity.y);

            if (GameManager.isMarkerOnStrikeZone || Batter.instance.willSwing) GameManager.instance.AddStrike();
            else GameManager.instance.AddFoul();

            isHit = true;
            StartCoroutine(SlowDown());
        }
        else if (collision.CompareTag("Batter"))
        {
            StopAllCoroutines();

            rb2d.velocity = new Vector2(rb2d.velocity.x + Random.Range(-4, 4), -rb2d.velocity.y);

            GameManager.instance.AddFoul();

            isHit = true;
            StartCoroutine(Hit());
        }
        else if (collision.CompareTag("SlowSwing") && speedType == SpeedType.slow)
        {
            Batter.instance.Swing();
        }
        else if (collision.CompareTag("NormalSwing") && speedType == SpeedType.normal)
        {
            Batter.instance.Swing();
        }
        else if (collision.CompareTag("FastSwing") && speedType == SpeedType.fast)
        {
            Batter.instance.Swing();
        }
    }

}
