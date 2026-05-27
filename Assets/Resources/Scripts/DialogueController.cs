using System.Collections;
using TMPro;
using UnityEngine;
using Lean.Localization; // Importamos la librería de traducción

public class DialogueController : MonoBehaviour
{
    [Header("UI")]
    public GameObject bubble;
    public TMP_Text dialogueText;

    public GameObject bubble1;
    public TMP_Text dialogueText1;

    [Header("Typewriter")]
    public float typingSpeed = 0.03f;

    [Header("Audio")] // ¡NUEVO! Variables para el sonido
    public AudioSource audioSource;
    public AudioClip typingSound;

    Coroutine typingCoroutine1;
    Coroutine typingCoroutine2;

    void Start()
    {
        bubble.SetActive(false);
        bubble1.SetActive(false);

        // Si te olvidas de asignar el AudioSource en el inspector, 
        // esto intentará buscar uno en el mismo GameObject de respaldo.
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // ====== texto_1 ======

    public void ShowDialogue(string message)
    {
        bubble.SetActive(true);

        if (typingCoroutine1 != null)
            StopCoroutine(typingCoroutine1);

        string translatedMessage = LeanLocalization.GetTranslationText(message);

        if (string.IsNullOrEmpty(translatedMessage))
        {
            translatedMessage = message;
        }

        typingCoroutine1 = StartCoroutine(TypeText1(translatedMessage));
    }

    IEnumerator TypeText1(string message)
    {
        dialogueText.text = "";

        foreach (char letter in message)
        {
            dialogueText.text += letter;

            // ¡NUEVO! Si la letra no es un espacio en blanco, reproduce el sonido
            if (audioSource != null && typingSound != null && !char.IsWhiteSpace(letter))
            {
                audioSource.PlayOneShot(typingSound);
            }

            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void HideDialogue()
    {
        bubble.SetActive(false);
    }


    // ====== texto_2 ======

    public void ShowDialogue2(string message)
    {
        bubble1.SetActive(true);

        if (typingCoroutine2 != null)
            StopCoroutine(typingCoroutine2);

        string translatedMessage = LeanLocalization.GetTranslationText(message);

        if (string.IsNullOrEmpty(translatedMessage))
        {
            translatedMessage = message;
        }

        typingCoroutine2 = StartCoroutine(TypeText2(translatedMessage));
    }

    IEnumerator TypeText2(string message)
    {
        dialogueText1.text = "";

        foreach (char letter in message)
        {
            dialogueText1.text += letter;

            // ¡NUEVO! Si la letra no es un espacio en blanco, reproduce el sonido
            if (audioSource != null && typingSound != null && !char.IsWhiteSpace(letter))
            {
                audioSource.PlayOneShot(typingSound);
            }

            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void HideDialogue2()
    {
        bubble1.SetActive(false);
    }
}