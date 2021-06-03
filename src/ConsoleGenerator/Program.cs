using System;
using System.IO;
using Microsoft.Data.Sqlite;
using ClassLibrary;
using static System.Console;

namespace ConsoleGenerator
{
    class Program
    {
        static string dbPath = @"..\..\data\data.db";
        static SqliteConnection connection = new SqliteConnection($"Data Source={dbPath}");
        static UserRepository userRepository = new UserRepository(connection);
        static QuestionRepository questionRepository = new QuestionRepository(connection);
        static AnswerRepository answerRepository = new AnswerRepository(connection);
        static void Main(string[] args)
        {
            while(true)
            {
                WriteLine("Print a table name and a quantity of entities: ");
                string read = ReadLine();
                string[] com = read.Split(' ');
                if(com[0] == "exit")
                {
                    break;
                }
                else if(com.Length != 2)
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
            if (Int32.TryParse(com[1], out int n))
            {
                Random r = new Random();
                string[] moderator = {"no", "no", "yes"};
                string[] passwords = {"123456", "123123", "password", "1234pass", "pass666", "87654321", "12344321", "696969", "aurora", "qwerty"};
                string[] names = {"Ivan Ivanenko", "Katya Adushkina", "Vlad A4", "Vasya Pupkin", "Danya Milokhin", "Crazy Frog", "Anonim User"};
                for(int i = 0; i < n; i ++)
                {
                    string login = "";
                    for (int j = 0; j < r.Next(1, 8); j++)
                    {
                        login += Convert.ToString((char)r.Next(97,123));
                    }
                    for (int j = 0; j < r.Next(5); j++)
                    {
                        login += Convert.ToString(r.Next(0,10));
                    }
                    Autentification.Register(userRepository, names[r.Next(0,7)], login, moderator[r.Next(0,3)], passwords[r.Next(0,10)]);
                }
            }
            else WriteLine(com[1] + " isn't a number");
        }
        static void QuestionHandler(string[] com, QuestionRepository questionRepository)
        {
            if (Int32.TryParse(com[1], out int n))
            {
                try
                {
                    Write("Print a user's ids gap: \t");
                    string users = ReadLine();
                    string[] ids = users.Split('-');
                    Write("Print a time gap: \t");
                    string gap = ReadLine();
                    string[] dt = gap.Split('-');
                    DateTime minDt = Convert.ToDateTime(dt[0]);
                    DateTime maxDt = Convert.ToDateTime(dt[1]);
                    Random r = new Random();
                    string [] titles = File.ReadAllLines(@"..\..\data\generator\title.txt");
                    string [] texts = File.ReadAllLines(@"..\..\data\generator\text.txt");
                    int range = ((TimeSpan)(maxDt - minDt)).Days;
                    for(int i = 0; i < n; i ++)
                    {
                        questionRepository.Insert(new Question(0, r.Next(Convert.ToInt32(ids[0]), Convert.ToInt32(ids[1]) + 1), titles[r.Next(0, titles.Length)], texts[r.Next(0, texts.Length)], minDt.AddDays(r.Next(range)), null, null));
                    }
                }
                catch (System.Exception)
                {
                    WriteLine("Wrong input");
                }
            }
            else WriteLine(com[1] + " isn't a number");
        }
        static void AnswerHandler(string[] com, AnswerRepository answerRepository)
        {
            if (Int32.TryParse(com[1], out int n))
            {
                try
                {
                    Write("Print a question's ids gap: \t");
                    string users = ReadLine();
                    string[] ids = users.Split('-');
                    Write("Print a time gap: \t");
                    string gap = ReadLine();
                    string[] dt = gap.Split('-');
                    DateTime minDt = Convert.ToDateTime(dt[0]);
                    DateTime maxDt = Convert.ToDateTime(dt[1]);
                    Random r = new Random();
                    string [] answers = File.ReadAllLines(@"..\..\data\generator\answer.txt");
                    int range = ((TimeSpan)(maxDt - minDt)).Days;
                    for(int i = 0; i < n; i ++)
                    {
                        answerRepository.Insert(new Answer(0, r.Next(Convert.ToInt32(ids[0]), Convert.ToInt32(ids[1]) + 1), answers[r.Next(0, answers.Length)], minDt.AddDays(r.Next(range)), "no", null));
                    }
                }
                catch (System.Exception)
                {
                    WriteLine("Wrong input");
                }
            }
            else WriteLine(com[1] + " isn't a number"); 
        }
    }
}
