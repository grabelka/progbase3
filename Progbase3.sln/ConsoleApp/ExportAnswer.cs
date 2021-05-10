using System;

public class ExportAnswer
{
    public int id;
    public int questionId;
    public string text;
    public DateTime created;
    public string pinned;
    public ExportAnswer(Question q)
    {
        this.id = q.answers[0].id;
        this.questionId = q.answers[0].questionId;
        this.text = q.answers[0].text;
        this.created = q.answers[0].created;
        this.pinned = q.answers[0].pinned;
    }
    public ExportAnswer()
    {}
}