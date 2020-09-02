using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Character
{

    public static Player instance;

    [SerializeField]
    private Ball ball;
    public float ballSpeed;

    [SerializeField]
    private Sprite defaultSprite;
    [SerializeField]
    private Sprite ballThrownSprite;

    private AudioSource aS;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        anim.enabled = false;
        aS = GetComponent<AudioSource>();
        Instantiate(ball, transform.position, Quaternion.identity).GetComponent<Ball>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }


    public void InstantiateNewBall()
    {
        sR.sprite = defaultSprite;
        Instantiate(ball, transform.position, Quaternion.identity).GetComponent<Ball>();
    }


    public IEnumerator Throw(Transform target)
    {
        yield return StartCoroutine(ThrowAnim());
        
        Ball.instance.SetSpeed(ballSpeed);
        Ball.instance.Throw(target);
    }


    private IEnumerator ThrowAnim()
    {
        anim.enabled = true;
        anim.SetBool("Throw", true);
        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);
        aS.Play();
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);

        anim.SetBool("Throw", false);
        yield return null; // Wait for the transition to take place
        anim.enabled = false;
        sR.sprite = ballThrownSprite;
    }
}
