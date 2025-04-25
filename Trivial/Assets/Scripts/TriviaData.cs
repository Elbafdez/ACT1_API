using System;

[Serializable]
public class TriviaResponse
{
    public Question[] resultsWrapper;
}

[Serializable]
public class Question
{
    public string category;
    public string type;
    public string difficulty;
    public string question;
    public string correct_answer;
    public string[] incorrect_answers;
}