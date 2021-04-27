using System;

public class Answer
{
    public int id;
    public int questionId;
    public string text;
    public DateTime created;
    public string pinned;
    public Question question;
    public Answer(int id, int questionId, string text, DateTime created, string pinned, Question question)
    {
        this.id = id;
        this.questionId = questionId;
        this.text = text;
        this.created = created;
        this.pinned = pinned;
        this.question = question;
    }
    public override string ToString()
    {
        return $"[{id}] - {text} ({created}) pined: {pinned}";
    }
}