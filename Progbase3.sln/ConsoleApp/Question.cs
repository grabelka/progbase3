using System;

public class Question
{
    public int id;
    public int userId;
    public string title;
    public string text;
    public DateTime created;
    public User author;
    public Answer[] answers;
    public Question(int id, int userId, string title, string text, DateTime created, User author, Answer[] answers)
    {
        this.id = id;
        this.userId = userId;
        this.title = title;
        this.text = text;
        this.created = created;
        this.author = author;
        this.answers = answers;
    }
    public override string ToString()
    {
        return $"[{id}] - {title} ({created})";
    }
}