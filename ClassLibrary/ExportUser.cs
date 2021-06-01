using System;

namespace ClassLibrary
{
    public class ExportUser
    {
        public string command;
        public int id;
        public string login;
        public string name;
        public string isModerator;
        public string password;

        public ExportUser(string command, User user)
        {
            this.command = command;
            this.id = user.id;
            this.login = user.login;
            this.name = user.name;
            this.isModerator = user.isModerator;
            this.password = user.password;
        }
        public ExportUser()
        {}
    }
}