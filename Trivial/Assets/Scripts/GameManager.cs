using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class TriviaUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI resultText;
    public Button[] answerButtons;

    [Header("Data")]
    public TriviaResponse triviaData;
    private int currentQuestionIndex = 0;
    private List<string> currentAnswers;
    private string correctAnswer;

    void Start()
    {
        StartCoroutine(GetTriviaData());
    }

    IEnumerator GetTriviaData()
    {
        string url = "https://opentdb.com/api.php?amount=10&category=32";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error descargando preguntas: " + request.error);
            }
            else
            {
                string fixedJson = FixJson(request.downloadHandler.text);
                triviaData = JsonUtility.FromJson<TriviaResponse>(fixedJson);
                ShowQuestion();
            }
        }
    }

    void ShowQuestion()
    {
        if (currentQuestionIndex >= triviaData.resultsWrapper.Length)
        {
            questionText.text = "¡Fin del trivial!";
            foreach (var btn in answerButtons) btn.gameObject.SetActive(false);
            return;
        }

        Question q = triviaData.resultsWrapper[currentQuestionIndex];

        questionText.text = System.Net.WebUtility.HtmlDecode(q.question);
        correctAnswer = q.correct_answer;

        // Mezcla respuestas
        currentAnswers = new List<string>(q.incorrect_answers);
        currentAnswers.Add(q.correct_answer);
        Shuffle(currentAnswers);

        // Asigna texto a botones
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = System.Net.WebUtility.HtmlDecode(currentAnswers[i]);

            // Limpia listeners anteriores
            answerButtons[i].onClick.RemoveAllListeners();

            // Captura la respuesta seleccionada
            string selected = currentAnswers[i];
            answerButtons[i].onClick.AddListener(() => CheckAnswer(selected));
        }
    }

    void CheckAnswer(string selected)
    {
        if (selected == correctAnswer)
        {
            resultText.text = "¡Correcto!";
            resultText.color = Color.green;
        }
        else
        {
            resultText.text = "Incorrecto";
            resultText.color = Color.red;
        }

        // Espera 2 segundos y pasa a la siguiente
        StartCoroutine(NextQuestionDelay());
    }
    IEnumerator NextQuestionDelay()
    {
        yield return new WaitForSeconds(2f);
        resultText.text = "";
        currentQuestionIndex++;
        ShowQuestion();
    }

    void Shuffle(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    string FixJson(string value)
    {
        return "{\"resultsWrapper\":" + value.Substring(value.IndexOf("[")); 
    }
}