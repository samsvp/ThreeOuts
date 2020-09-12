using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Batter : Character
{

    public static Batter instance;

    protected Rigidbody2D rb2d;
    protected BoxCollider2D bc2d;
    
    [HideInInspector]
    public bool willSwing = false;

    private List<Ball.BallHeights> ballHeights = new List<Ball.BallHeights>();
    private List<Ball.BallWidths> ballWidths = new List<Ball.BallWidths>();
    private List<Ball.SpeedType> ballSpeeds = new List<Ball.SpeedType>();

    [HideInInspector]
    public Ball.BallHeights predictedHeight;
    [HideInInspector]
    public Ball.BallWidths predictedWidth;
    [HideInInspector]
    public Ball.SpeedType predictedSpeed;

    // Eyes
    private SpriteRenderer eyeSR;
    [SerializeField]
    private Sprite fastEye;
    [SerializeField]
    private Sprite normalEye;
    [SerializeField]
    private Sprite slowEye;

    // HyperParameters
    // chance to detect that the ball was thrown out
    // of the strike zone not swing
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float reflexes = 0.0f;
    // how reliable the eye is
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float speedReliability = 1.0f;
    // chance that the batter will hit the ball if 
    // he gets the height correctly
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float willHitIfHeight = 0.5f;
    // chance that the batter will hit the ball if 
    // he gets the width correctly
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float willHitIfWidth = 0.5f;
    // chance that the batter will hit the ball if 
    // he gets the speed correctly
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float willHitIfSpeed = 0.5f;

    // strike cursor
    private GameObject strikeCursor;

    // sprite
    private Sprite defaultSprite;

    // idle variables
    [SerializeField]
    private float idleTimer;
    private float currentTimer = 0;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        rb2d = GetComponent<Rigidbody2D>();
        bc2d = GetComponent<BoxCollider2D>();
        eyeSR = transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>();

        strikeCursor = FindObjectOfType<StrikeCursor>().gameObject;
        strikeCursor.SetActive(false);

        bc2d.enabled = false;
        anim.enabled = false;

        defaultSprite = sR.sprite;

        StartCoroutine(PlayIntro());
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }


    private IEnumerator PlayIntro()
    {
        yield return new WaitForSeconds(0.2f);
        anim.enabled = true;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        yield return null;
        strikeCursor.SetActive(true);
        anim.SetBool("Idle", true);
    }


    public void UpdateLists(Ball.BallHeights height, Ball.BallWidths width, Ball.SpeedType speedType)
    {
        ballHeights.Add(height);
        ballWidths.Add(width);
        ballSpeeds.Add(speedType);
    }


    public bool WillHit(Ball.BallHeights height, Ball.BallWidths width, Ball.SpeedType speedType)
    {
        float reflexes;
        if (speedType == Ball.SpeedType.slow) reflexes = this.reflexes;
        else if (speedType == Ball.SpeedType.normal) reflexes = 0.9f * this.reflexes;
        else reflexes = 0.7f * this.reflexes;

        // print("heigh: " + height + "\twidth: " + width + "\tspeed: " + speedType);
        // print("predicted heigh: " + predictedHeight + "\tpredicted width: " + 
        //   predictedWidth + "\tspredicted peed: " + predictedSpeed);

        if (!willSwing) return false;
        if ((width == Ball.BallWidths.farLeft || width == Ball.BallWidths.farRight))
        {
            if (Random.Range(0.0f, 1.0f) < reflexes)
            {
                willSwing = false;
                return false;
            }
        }
        else if ((predictedWidth == Ball.BallWidths.farLeft || predictedWidth == Ball.BallWidths.farRight) &&
            Random.Range(0.0f, 1.0f) < reflexes)
        {
                predictedWidth = RePredictWidth();
        }

        bool rightSpeed = predictedSpeed == speedType && Random.Range(0.0f, 1.0f) < willHitIfSpeed * this.reflexes;

        if (predictedHeight == height && predictedWidth == width)
        {
            return true;
        }
        else if (predictedHeight == height && (rightSpeed ||
                Random.Range(0.0f, 1.0f) <= willHitIfHeight * reflexes))
        {
            return true;
        }
        else if (predictedWidth == width && (rightSpeed ||
                Random.Range(0.0f, 1.0f) <= willHitIfWidth * reflexes))
        {
            if ((width == Ball.BallWidths.farLeft || width == Ball.BallWidths.farRight))
                return false;
            else
                return true;
        }
        else if (rightSpeed)
        {
            return true;
        }
        return false;
    }


    public void MakePredictions()
    {
        willSwing = false;

        predictedHeight = PredictHeight();
        predictedWidth = PredictWidth();
        predictedSpeed = PredictSpeed();

        if ((predictedWidth == Ball.BallWidths.farLeft ||
            predictedWidth == Ball.BallWidths.farRight) &&
            Random.Range(0.0f, 1.0f) < 0.5f) willSwing = false;
        // 5% chance of not swinging
        else if (GameManager.instance.strikes < 2 && Random.Range(0.0f, 1.0f) > 0.95f) willSwing = false;
        else willSwing = true;

        SpeedPredictionFeedback();
    }


    private void SpeedPredictionFeedback()
    {
        var p = Random.Range(0.0f, 1.0f);
        switch (predictedSpeed)
        {
            case Ball.SpeedType.slow:
                if (p < speedReliability) eyeSR.sprite = slowEye;
                else eyeSR.sprite = normalEye;
                break;
            case Ball.SpeedType.normal:
                if (p < speedReliability) eyeSR.sprite = normalEye;
                else if (p < speedReliability + (1 - speedReliability) / 2) eyeSR.sprite = slowEye;
                else eyeSR.sprite = fastEye;
                break;
            case Ball.SpeedType.fast:
                if (p < speedReliability) eyeSR.sprite = fastEye;
                else eyeSR.sprite = normalEye;
                break;
            default:
                break;
        }
    }


    private Ball.BallWidths PredictWidth()
    {
        // Appply a sliding window
        if (ballWidths.Count > 10) ballWidths.RemoveAt(ballWidths.Count - 1);

        int leftCount = ballWidths.Count(b => b == Ball.BallWidths.left);
        int midCount = ballWidths.Count(b => b == Ball.BallWidths.mid);
        int rightCount = ballWidths.Count(b => b == Ball.BallWidths.right);

        int total = ballWidths.Count;

        if (total == 0) return new List< Ball.BallWidths > () { Ball.BallWidths.farLeft, Ball.BallWidths.farLeft,
                Ball.BallWidths.left, Ball.BallWidths.mid, Ball.BallWidths.right,
                Ball.BallWidths.farRight, Ball.BallWidths.farRight}[Random.Range(0, 6)];

        float p = Random.Range(0.0f, 1.0f);

        if (p < leftCount / (float)total) return Ball.BallWidths.left;
        if (p < (leftCount + midCount) / (float)total) return Ball.BallWidths.mid;
        if (p < (leftCount + midCount + rightCount) / (float)total) return Ball.BallWidths.right;
        return Ball.BallWidths.farRight;
    }


    private Ball.BallWidths RePredictWidth()
    {
        // Appply a sliding window
        if (ballWidths.Count > 10) ballWidths.RemoveAt(ballWidths.Count - 1);

        int leftCount = ballWidths.Count(b => b == Ball.BallWidths.left);
        int midCount = ballWidths.Count(b => b == Ball.BallWidths.mid);
        int rightCount = ballWidths.Count(b => b == Ball.BallWidths.right);

        int total = leftCount + midCount + rightCount;

        if (total == 0) return new List<Ball.BallWidths>() {
                    Ball.BallWidths.left, Ball.BallWidths.mid, Ball.BallWidths.right,
                }[Random.Range(0, 2)];

        float p = Random.Range(0.0f, 1.0f);

        if (p < leftCount / (float)total) return Ball.BallWidths.left;
        if (p < (leftCount + midCount) / (float)total) return Ball.BallWidths.mid;
        return Ball.BallWidths.right;
    }


    private Ball.BallHeights PredictHeight()
    {
        int lowCount = ballHeights.Count(b => b == Ball.BallHeights.low);
        int midCount = ballHeights.Count(b => b == Ball.BallHeights.mid);
        int highCount = ballHeights.Count(b => b == Ball.BallHeights.high);

        int total = ballHeights.Count;

        if (total == 0) return new List<Ball.BallHeights>() { Ball.BallHeights.farLow, Ball.BallHeights.farLow,
                Ball.BallHeights.low, Ball.BallHeights.mid, Ball.BallHeights.high,
                Ball.BallHeights.farHigh, Ball.BallHeights.farHigh}[Random.Range(0, 6)];

        float p = Random.Range(0.0f, 1.0f);

        if (p < lowCount / (float)total) return Ball.BallHeights.low;
        if (p < (lowCount + midCount) / (float)total) return Ball.BallHeights.mid;
        if (p < (lowCount + midCount + highCount) / (float)total) return Ball.BallHeights.high;
        return Ball.BallHeights.farHigh;
    }


    private Ball.SpeedType PredictSpeed()
    {
        int slowCount = ballSpeeds.Count(b => b == Ball.SpeedType.slow);
        int normalCount = ballSpeeds.Count(b => b == Ball.SpeedType.normal);
        int fastCount = ballSpeeds.Count(b => b == Ball.SpeedType.fast);

        int total = ballSpeeds.Count;

        if (total == 0) return new List<Ball.SpeedType>() {
                Ball.SpeedType.slow, Ball.SpeedType.normal, Ball.SpeedType.fast
            }[Random.Range(0, 2)];

        float p = Random.Range(0.0f, 1.0f);

        if (p < slowCount / (float)total) return Ball.SpeedType.slow;
        if (p < (slowCount + normalCount) / (float)total) return Ball.SpeedType.normal;
        return Ball.SpeedType.fast;
    }


    public void AddBallMensurements(Ball.BallWidths width, Ball.BallHeights height)
    {
        ballHeights.Add(height);
        ballWidths.Add(width);
    }


    public void Swing()
    {
        if (willSwing) StartCoroutine(ISwing());
    }


    private IEnumerator ISwing()
    {
        anim.enabled = true;
        anim.SetBool("Swing", true);
        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f);
        if (willSwing && Ball.instance.willBatterHit) bc2d.enabled = true;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f);
        bc2d.enabled = false;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1);
        anim.SetBool("Swing", false);
    }

}
