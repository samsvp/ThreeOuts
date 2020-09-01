using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : GameManager
{
    private bool canContinueSentence = true;

    [SerializeField]
    private GameObject shadow;
    private GameObject strikeCursorGO;
    private CameraZoom cameraZoom;

    public Dialogue dialogueIntro;
    public Dialogue dialogueAim;
    public Dialogue dialogueBatter;
    public Dialogue dialogueWin;
    private DialogueManager dialogueManager;

    private int state = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        strikeCursorGO = strikeCursor.gameObject;
        dialogueManager = FindObjectOfType<DialogueManager>();
        cameraZoom = FindObjectOfType<CameraZoom>();
        StartCoroutine(DialogueTutorial());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene("MainMenu");
        if (canContinueSentence && Input.GetMouseButtonDown(0))
        {
            canContinueSentence = DisplayNextSentence();
            shadow.SetActive(canContinueSentence);
        }

    }

    private IEnumerator DialogueTutorial()
    {
        strikeCursorGO.SetActive(false);
        yield return null;

        TriggerDialogue(dialogueIntro);
        SpeedBar.instance.StopCount();

        yield return new WaitWhile(() => canContinueSentence);
        SpeedBar.instance.StartCount();
        strikeCursorGO.SetActive(true);

        yield return new WaitWhile(() => !cameraZoom.isZoomedIn);
        strikeCursorGO.SetActive(false);
        TriggerDialogue(dialogueAim);

        yield return new WaitWhile(() => canContinueSentence);
        strikeCursorGO.SetActive(true);

        yield return new WaitWhile(() => Input.GetMouseButtonDown(0));
        yield return new WaitWhile(() => Marker.instance == null);
        yield return new WaitWhile(() => Marker.instance != null);
        yield return new WaitWhile(() => isMarkerOnStrikeZone);
        strikeCursorGO.SetActive(false);
        TriggerDialogue(dialogueBatter);

        yield return new WaitWhile(() => canContinueSentence);
        strikeCursorGO.SetActive(true);

        yield return new WaitWhile(() => !cameraZoom.isZoomedIn);
        yield return new WaitWhile(() => Input.GetMouseButtonDown(0));
        yield return new WaitWhile(() => Marker.instance == null);
        yield return new WaitWhile(() => Marker.instance != null);
        strikeCursorGO.SetActive(false);
        TriggerDialogue(dialogueWin);

        yield return new WaitWhile(() => canContinueSentence);
        strikeCursorGO.SetActive(true);

        System.IO.File.WriteAllText(Application.persistentDataPath + "/tutorialDone.txt", "1");
    }


    public void TriggerDialogue(Dialogue dialogue)
    {
        canContinueSentence = true;
        shadow.SetActive(true);
        dialogueManager.StartDialogue(dialogue);
    }


    public bool DisplayNextSentence()
    {
        return dialogueManager.DisplayNextSentence();
    }


    protected override void UpdateScore(bool playerWon)
    {
        if (playerWon) ++score.x;
        else ++score.y;

        scoreText.text = "Score:\nP: " + score.x + " x  C: " + score.y;
    }
}
