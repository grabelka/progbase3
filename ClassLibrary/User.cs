using System;

namespace ClassLibrary
{
    public class User
    {
        public int id;
        public string login;
        public string name;
        public string isModerator;
        public string password;
        public Question[] questions;

        public User(int id, string name, string login, string isModerator, string password, Question[] questions)
        {
            this.id = id;
            this.login = login;
            this.name = name;
            this.isModerator = isModerator;
            this.password = password;
            this.questions = questions;
        }
        public override string ToString()
        {
            return $"[{id}] - {name} ({login}), moderator: {isModerator}";
        }
    }
}