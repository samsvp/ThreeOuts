using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    private Queue<string> sentences = new Queue<string>();

    public Text dialogueText;

    public Animator anim;

    private bool sentenceFinished = true;
    private bool finishSentence = false;


    public void StartDialogue(Dialogue dialogue)
    {
        sentences.Clear();

        anim.SetBool("isOpen", true);

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }


    public bool DisplayNextSentence()
    {
        // Only display the next sentence once the one displaying is finished
        if (sentenceFinished)
        {
            if (sentences.Count == 0)
            {
                EndDialogue();
                return false;
            }

            string sentence = sentences.Dequeue();
            StartCoroutine(TypeSentence(sentence));
        }
        // Tell TypeSentence to finish typing
        else { finishSentence = true; }
        return true;
    }


    private IEnumerator TypeSentence(string sentence)
    {
        sentenceFinished = false;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            if (finishSentence)
            {
                dialogueText.text = sentence;
                yield return new WaitForSeconds(0.3f);
                break;
            }
            yield return null;
        }

        finishSentence = false;
        sentenceFinished = true;
    }


    public void EndDialogue()
    {
        anim.SetBool("isOpen", false);
    }

}
