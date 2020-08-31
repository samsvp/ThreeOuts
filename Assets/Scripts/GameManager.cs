using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [HideInInspector]
    public int strikes = 0;
    [HideInInspector]
    public int fouls = 0;

    private int maxStrikes = 3;
    private int maxFouls = 2;
    private int maxTurns = 3;
    
    public static bool isMarkerOnStrikeZone = false;
    public static bool batterSwung = false;
    public static bool batterHit = false;

    [HideInInspector]
    public Vector2Int score = Vector2Int.zero;

    public GameObject[] strikeObjects;
    public GameObject[] foulObjects;
    public StrikeCursor strikeCursor;
    public Text turnText;
    public Text scoreText;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void NextThrow()
    {
        strikeCursor.ResetVariables();

        Destroy(Ball.instance.gameObject);
        Ball.instance = null;
        Destroy(Marker.instance.gameObject);
        Marker.instance = null;

        SpeedBar.instance.StartCount();
        Player.instance.InstantiateNewBall();

        isMarkerOnStrikeZone = false;
    }


    public void AddStrike()
    {
        strikeObjects[strikes].SetActive(true);
        if (++strikes == maxStrikes)
        {
            ResetVariables();
            UpdateScore(true);
        }
    }


    public void AddFoul()
    {
        foulObjects[fouls].SetActive(true);
        if (++fouls == maxFouls)
        {
            ResetVariables();
            UpdateScore(false);
        }
    }


    private void UpdateScore(bool playerWon)
    {
        if (playerWon)
        {
            if (++score.x == 3)
            {
                print("won");
            }
        }
        else
        {
            if (++score.y == 3) print("lost");
        }

        scoreText.text = "Score:\nP: " + score.x + " x  C: " + score.y;
    }


    private void ResetVariables()
    {
        strikes = 0;
        fouls = 0;

        for (int i = 0; i < strikeObjects.Length; i++)
        {
            strikeObjects[i].SetActive(false);
        }

        for (int i = 0; i < foulObjects.Length; i++)
        {
            foulObjects[i].SetActive(false);
        }
    }
}
