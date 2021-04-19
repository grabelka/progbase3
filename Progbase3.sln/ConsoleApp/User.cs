using System;

public class User
{
    public int id;
    public string name;
    public string isModerator;
    public string password;
    public User(int id, string name, string isModerator, string password)
    {
        this.id = id;
        this.name = name;
        this.isModerator = isModerator;
        this.password = password;
    }
    public User(string name, string isModerator, string password)
    {
        this.id = 0;
        this.name = name;
        this.isModerator = isModerator;
        this.password = password;
    }
    public override string ToString()
    {
        return $"[{id}] - {name}, moderator: {isModerator}";
    }
}