using System;
using Microsoft.Data.Sqlite;
using System.Text;
using System.IO;
using static System.Console;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string dbPath = @"C:\Users\nasty.DESKTOP-UTJ8J96\OneDrive\Desktop\progbase3\data\data.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={dbPath}");
            UserRepository userRepository = new UserRepository(connection);
            QuestionRepository questionRepository = new QuestionRepository(connection);
            AnswerRepository answerRepository = new AnswerRepository(connection);
            while(true)
            {
                WriteLine("Print a table name, a command and a value: ");
                string read = ReadLine();
                string[] com = read.Split(' ');
                if(com[0] == "exit")
                {
                    break;
                }
                else if(com.Length != 3)
                {
                    WriteLine("Not enought args");
                }
                if (com[0] == "users")
                {
                    UserHandler(com, userRepository);
                }
                else if (com[0] == "questions")
                {
                    QuestionHandler(com, questionRepository);
                }
                else if (com[0] == "answers")
                {
                    AnswerHandler(com, answerRepository);
                }
            }
        }
        static void UserHandler(string[] com, UserRepository userRepository)
        {
            if(com[1] == "getById")
            {
                if (Int32.TryParse(com[2], out int n))
                    WriteLine(userRepository.GetById(n).ToString());
                else WriteLine(com[2] + " isn't a number");
            }
            else if(com[1] == "deleteById")
            {
                if (Int32.TryParse(com[2], out int n))
                {
                    if (userRepository.DeleteById(n) == 1) 
                        WriteLine("User was deleted");
                    else
                        WriteLine("This user doesn't exist");
                }
                else WriteLine(com[2] + " isn't a number");
            }
            else if(com[1] == "insert")
            {
                string[] userFields = com[2].Split(',');
                if(userFields.Length != 3)
                {
                    WriteLine("Wrong user's paramethers");
                    return;
                }
                try
                {
                    User user = new User(userFields[0], userFields[1], userFields[2]);
                    WriteLine($"User was added with id [{userRepository.Insert(user)}]");
                }
                catch (System.Exception)
                {
                    WriteLine("You can't use this name.");
                }
            }
            else if(com[1] == "update")
            {
                string[] userFields = com[2].Split(',');
                int n = 0;
                if(userFields.Length != 4 || !Int32.TryParse(userFields[0], out n))
                {
                    WriteLine("Wrong user's paramethers");
                    return;
                }
                try
                {
                    User user = new User(n, userFields[1], userFields[2], userFields[3]);
                    if (userRepository.Update(user) == 1) 
                        WriteLine("User was updated");
                    else
                        WriteLine("This user doesn't exist");
                }
                catch (System.Exception)
                {
                    WriteLine("You can't use this name.");
                }
            }
            else if(com[1] == "random")
            {
                if (Int32.TryParse(com[2], out int n))
                {
                    Random r = new Random();
                    string[] moderator = {"no", "no", "yes"};
                    string[] passwords = {"123456", "123123", "password", "1234pass", "pass666", "87654321", "12344321", "696969", "aurora", "qwerty"};
                    for(int i = 0; i < n; i ++)
                    {
                        string name = "";
                        for (int j = 0; j < r.Next(1, 6); j++)
                        {
                            name += Convert.ToString((char)r.Next(97,123));
                        }
                        for (int j = 0; j < r.Next(4); j++)
                        {
                            name += Convert.ToString(r.Next(0,10));
                        }
                        try
                        {
                            userRepository.Insert(new User(0, name, moderator[r.Next(0,3)], passwords[r.Next(0,10)]));
                        }
                        catch (System.Exception)
                        {
                            Console.WriteLine($"iteration [{i}]: Here was a colision");
                        }
                    }
                }
                else WriteLine(com[2] + " isn't a number");
            }
            else
            {
                WriteLine("Command doesn't exist. Please try again.");
            }
        }
        static void QuestionHandler(string[] com, QuestionRepository questionRepository)
        {
            if(com[1] == "getById")
            {
                if (Int32.TryParse(com[2], out int n))
                    WriteLine(questionRepository.GetById(n).ToString());
                else WriteLine(com[2] + " isn't a number");
            }
            else if(com[1] == "deleteById")
            {
                if (Int32.TryParse(com[2], out int n))
                {
                    if (questionRepository.DeleteById(n) == 1) 
                        WriteLine("Question was deleted");
                    else
                        WriteLine("This question doesn't exist");
                }
                else WriteLine(com[2] + " isn't a number");
            }
            else if(com[1] == "insert")
            {
                string[] qFields = com[2].Split(',');
                int uId = 0;
                DateTime date = new DateTime();
                if(qFields.Length != 4 || !Int32.TryParse(qFields[0], out uId) || !DateTime.TryParse(qFields[3], out date))
                {
                    WriteLine("Wrong question's paramethers");
                    return;
                }
                Question q = new Question(0, uId, qFields[1], qFields[2], date);
                WriteLine($"Question was added with id [{questionRepository.Insert(q)}]");
            }
            else if(com[1] == "update")
            {
                string[] qFields = com[2].Split(',');
                int id = 0;
                int uId = 0;
                DateTime date = new DateTime();
                if(qFields.Length != 5 || !Int32.TryParse(qFields[0], out id) || !Int32.TryParse(qFields[1], out uId) || !DateTime.TryParse(qFields[4], out date))
                {
                    WriteLine("Wrong question's paramethers");
                    return;
                }
                Question q = new Question(id, uId, qFields[2], qFields[3], date);
                if (questionRepository.Update(q) == 1) 
                    WriteLine("Question was updated");
                else
                    WriteLine("This question doesn't exist");
            }
            else if(com[1] == "random")
            {
                if (Int32.TryParse(com[2], out int n))
                {
                    for(int i = 0; i < n; i ++)
                    {
                        
                    }
                }
                else WriteLine(com[2] + " isn't a number");
            }
            else
            {
                WriteLine("Command doesn't exist. Please try again.");
            }
        }
        static void AnswerHandler(string[] com, AnswerRepository answerRepository)
        {
            if(com[1] == "getById")
            {
                if (Int32.TryParse(com[2], out int n))
                    WriteLine(answerRepository.GetById(n).ToString());
                else WriteLine(com[2] + " isn't a number");
            }
            else if(com[1] == "deleteById")
            {
                if (Int32.TryParse(com[2], out int n))
                {
                    if (answerRepository.DeleteById(n) == 1) 
                        WriteLine("Answer was deleted");
                    else
                        WriteLine("This answer doesn't exist");
                }
                else WriteLine(com[2] + " isn't a number");
            }
            else if(com[1] == "insert")
            {
                string[] aFields = com[2].Split(',');
                int qId = 0;
                DateTime date = new DateTime();
                if(aFields.Length != 4 || !Int32.TryParse(aFields[0], out qId) || !DateTime.TryParse(aFields[2], out date))
                {
                    WriteLine("Wrong question's paramethers");
                    return;
                }
                Answer a = new Answer(0, qId, aFields[1], date, aFields[3]);
                WriteLine($"Answer was added with id [{answerRepository.Insert(a)}]");
            }
            else if(com[1] == "update")
            {
                string[] aFields = com[2].Split(',');
                int id = 0;
                int qId = 0;
                DateTime date = new DateTime();
                if(aFields.Length != 5 || !Int32.TryParse(aFields[0], out id) || !Int32.TryParse(aFields[1], out qId) || !DateTime.TryParse(aFields[3], out date))
                {
                    WriteLine("Wrong question's paramethers");
                    return;
                }
                Answer a = new Answer(id, qId, aFields[2], date, aFields[4]);
                if (answerRepository.Update(a) == 1) 
                    WriteLine("Answer was updated");
                else
                    WriteLine("This answer doesn't exist");
            } 
            else if(com[1] == "random")
            {
                if (Int32.TryParse(com[2], out int n))
                {
                    for(int i = 0; i < n; i ++)
                    {}
                }
                else WriteLine(com[2] + " isn't a number");
            } 
            else
            {
                WriteLine("Command doesn't exist. Please try again.");
            }    
        }
    }
}
