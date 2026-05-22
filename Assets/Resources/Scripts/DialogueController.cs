using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [Header("UI")]
    public GameObject bubble;
    public TMP_Text dialogueText;

    public GameObject bubble1;
    public TMP_Text dialogueText1;

    [Header("Typewriter")]
    public float typingSpeed = 0.03f;

    Coroutine typingCoroutine1;
    Coroutine typingCoroutine2;

    void Start()
    {
        bubble.SetActive(false);
        bubble1.SetActive(false);
    }

    // texto_1

    public void ShowDialogue(string message)
    {
        bubble.SetActive(true);

        if (typingCoroutine1 != null)
            StopCoroutine(typingCoroutine1);

        typingCoroutine1 = StartCoroutine(TypeText1(message));
    }

    IEnumerator TypeText1(string message)
    {
        dialogueText.text = "";

        foreach (char letter in message)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void HideDialogue()
    {
        bubble.SetActive(false);
    }


    // texto_2

    public void ShowDialogue2(string message)
    {
        bubble1.SetActive(true);

        if (typingCoroutine2 != null)
            StopCoroutine(typingCoroutine2);

        typingCoroutine2 = StartCoroutine(TypeText2(message));
    }

    IEnumerator TypeText2(string message)
    {
        dialogueText1.text = "";

        foreach (char letter in message)
        {
            dialogueText1.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void HideDialogue2()
    {
        bubble1.SetActive(false);
    }
}