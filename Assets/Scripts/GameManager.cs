using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [SerializeField]
    private GameObject result;
    [SerializeField]
    private Text resultText;

    [HideInInspector]
    public int strikes = 0;
    [HideInInspector]
    public int fouls = 0;

    protected int maxStrikes = 3;
    protected int maxFouls = 2;
    
    public static bool isMarkerOnStrikeZone = false;
    public static bool batterSwung = false;
    public static bool batterHit = false;

    [HideInInspector]
    public Vector2Int score = Vector2Int.zero;

    public GameObject[] strikeObjects;
    public GameObject[] foulObjects;
    public StrikeCursor strikeCursor;
    public Text scoreText;

    [SerializeField]
    private AudioClip cheerClip;
    [SerializeField]
    private AudioClip booClip;

    protected virtual void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
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


    protected virtual void UpdateScore(bool playerWon)
    {
        if (playerWon)
        {
            if (++score.x == 3)
            {
                ++LoadBatter.batterNumber;
                string won = "You won the game!";
                StartCoroutine(ChangeBatter(LoadBatter.batterNumber, won, cheerClip));
            }
        }
        else
        {
            if (++score.y == 3)
            {
                string lost = "You lost on batter number " + (LoadBatter.batterNumber + 1) +
                    " out of 6 batters.";
                LoadBatter.batterNumber = 0;
                StartCoroutine(ChangeBatter(LoadBatter.batterNumber, lost, booClip));
            }
        }
        if (score.y < 3 && score.x < 3) StartCoroutine(SetResult(playerWon));
        scoreText.text = "Score:\nP: " + score.x + " x  C: " + score.y;
    }


    protected void ResetVariables()
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


    protected IEnumerator SetResult(bool won)
    {
        strikeCursor.gameObject.SetActive(false);
        result.SetActive(true);

        if (won) resultText.text = "You won the set!";
        else resultText.text = "You lost the set!";
        resultText.text += "\nScore:\nP: " + score.x + " x  C: " + score.y;

        var clip = won ? cheerClip : booClip;
        yield return StartCoroutine(PlayClip(clip));
        
        result.SetActive(false);
        yield return null;
        strikeCursor.gameObject.SetActive(true);
    }


    protected IEnumerator ChangeBatter(int n, string sentence, AudioClip clip)
    {
        strikeCursor.gameObject.SetActive(false);
        Save.SaveGame(n);
        result.SetActive(true);
        resultText.text = sentence;

        yield return StartCoroutine(PlayClip(clip));
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    protected IEnumerator PlayClip(AudioClip clip)
    {
        var audioSource = GetComponents<AudioSource>().First(a => a.clip == null);
        audioSource.clip = clip;
        audioSource.Play();

        yield return null;
        yield return new WaitWhile(() => audioSource.isPlaying);

        audioSource.clip = null;
    }

}
