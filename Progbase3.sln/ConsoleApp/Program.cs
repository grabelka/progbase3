using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Text;
using System.IO;
using Terminal.Gui;
using static System.Console;

namespace ConsoleApp
{
    class Program
    {
        static string dbPath = @"C:\Users\nasty.DESKTOP-UTJ8J96\OneDrive\Desktop\progbase3\data\data.db";
        static SqliteConnection connection = new SqliteConnection($"Data Source={dbPath}");
        static UserRepository userRepository = new UserRepository(connection);
        static QuestionRepository questionRepository = new QuestionRepository(connection);
        static AnswerRepository answerRepository = new AnswerRepository(connection);
        static ListView usersListView;
        static ListView questionsListView;
        static ListView answersListView;
        static Label page;
        static Label totalPages;
        static void Main(string[] args)
        {
            Application.Init();
            OnButtonUsersClicked();
            Application.Run();
            // while(true)
            // {
            //     WriteLine("Print a table name, a command and a value: ");
            //     string read = ReadLine();
            //     string[] com = read.Split(' ');
            //     if(com[0] == "exit")
            //     {
            //         break;
            //     }
            //     else if(com.Length != 3)
            //     {
            //         WriteLine("Not enought args");
            //     }
            //     if (com[0] == "users")
            //     {
            //         UserHandler(com, userRepository);
            //     }
            //     else if (com[0] == "questions")
            //     {
            //         QuestionHandler(com, questionRepository);
            //     }
            //     else if (com[0] == "answers")
            //     {
            //         AnswerHandler(com, answerRepository);
            //     }
            // }
        }
        static void OnButtonUsersClicked()
        {
            Toplevel top = Application.Top;
            top.RemoveAll();
            MenuBar menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("File", new MenuItem [] {
                    new MenuItem ("Import", "", OnImportClicked),
                    new MenuItem ("Export", "", OnExportClicked),
                    new MenuItem ("Exit", "", OnExit),
                }),
                new MenuBarItem ("Help", new MenuItem [] {
                    new MenuItem ("About", "", OnAbout),
                }),
            });
            Window win = new Window("Users")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            top.Add(menu, win);
            Button btnq = new Button(1, 1, "Go to questions");
            btnq.Clicked += OnButtonQuestionsClicked;
            Button btna = new Button(20, 1, "Go to answers");
            btna.Clicked += OnButtonAnswersClicked;
            win.Add(btnq, btna);
            Rect frame = new Rect(4, 8, top.Frame.Width, 200);
            if (userRepository.GetTotalPages() < 1) 
            {
                List<string> list = new List<string>();
                list.Add("Data Base is empty");
                usersListView = new ListView(frame, list);
            }
            else
            {
                usersListView = new ListView(frame, userRepository.GetPage(1));
                usersListView.OpenSelectedItem += OnOpenUser;
            }
            win.Add(usersListView);

            Button btn = new Button(1, 4, "Create new user");
            btn.Clicked += OnCreateUserClicked;
            win.Add(btn);
            Button prew = new Button(5, 20, "Prew");
            prew.Clicked += OnPrewUsersClicked;
            page = new Label(14, 20, "1 ");
            win.Add(prew, page);
            Button next = new Button(20, 20, "Next");
            next.Clicked += OnNextUsersClicked;
            totalPages = new Label(17, 20, Convert.ToString(userRepository.GetTotalPages()) + " ");
            win.Add(next, totalPages);
            Application.Run();
        }
        static void OnPrewUsersClicked()
        {
            int n = Int32.Parse(Convert.ToString(page.Text));
            if (n > 1)
            {
                n--;
                page.Text = Convert.ToString(n);
                usersListView.SetSource(userRepository.GetPage(n));
            }
        }
        static void OnNextUsersClicked()
        {
            int n = Int32.Parse(Convert.ToString(page.Text));
            if (n < userRepository.GetTotalPages())
            {
                n++;
                page.Text = Convert.ToString(n);
                usersListView.SetSource(userRepository.GetPage(n));
            }
        }
        static void OnOpenUser(ListViewItemEventArgs args)
        {
            User user = (User)args.Value;
            OpenUserDialog dialog = new OpenUserDialog();
            dialog.SetUser(user);
            Application.Run(dialog);
            if (dialog.deleted)
            {
                int pageNumber = Int32.Parse(Convert.ToString(page.Text));
                userRepository.DeleteById(user.id);
                int total = userRepository.GetTotalPages();
                if (pageNumber > total) pageNumber = total;
                if (total < 1) 
                {
                    List<string> list = new List<string>();
                    list.Add("Data Base is empty");
                    usersListView.SetSource(list);
                }
                else
                {
                    usersListView.SetSource(userRepository.GetPage(pageNumber));
                }
                page.Text = Convert.ToString(pageNumber);
                totalPages.Text = Convert.ToString(total);
            }
            if (dialog.updated)
            {
                int userId = user.id;
                int pageNumber = userId/10;
                if(userId % 10 != 0) pageNumber++;
                user = dialog.GetUser();
                userRepository.Update(userId, user);
                usersListView.SetSource(userRepository.GetPage(pageNumber));
            }
        }
        static void OnCreateUserClicked()
        {
            CreateUserDialog dialog = new CreateUserDialog();
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                User user = dialog.GetUser();
                userRepository.Insert(user);
                usersListView.SetSource(userRepository.GetPage(userRepository.GetTotalPages()));
                page.Text = Convert.ToString(userRepository.GetTotalPages());
                totalPages.Text = Convert.ToString(userRepository.GetTotalPages());
            }
        }

        static void OnButtonQuestionsClicked()
        {
            Toplevel top = Application.Top;
            top.RemoveAll();
            MenuBar menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("File", new MenuItem [] {
                    new MenuItem ("Import", "", OnImportClicked),
                    new MenuItem ("Export", "", OnExportClicked),
                    new MenuItem ("Exit", "", OnExit),
                }),
                new MenuBarItem ("Help", new MenuItem [] {
                    new MenuItem ("About", "", OnAbout),
                }),
            });
            Window win = new Window("Questions")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            top.Add(menu, win);
            Button btnu = new Button(1, 1, "Go to users");
            btnu.Clicked += OnButtonUsersClicked;
            Button btna = new Button(20, 1, "Go to answers");
            btna.Clicked += OnButtonAnswersClicked;
            win.Add(btnu, btna);
            Rect frame = new Rect(4, 8, top.Frame.Width, 200);
            if (questionRepository.GetTotalPages() < 1) 
            {
                List<string> list = new List<string>();
                list.Add("Data Base is empty");
                questionsListView = new ListView(frame, list);
            }
            else
            {
                questionsListView = new ListView(frame, questionRepository.GetPage(1));
                questionsListView.OpenSelectedItem += OnOpenQuestion;
            }
            win.Add(questionsListView);

            Button btn = new Button(1, 4, "Create new question");
            btn.Clicked += OnCreateQuestionClicked;
            win.Add(btn);
            Button prew = new Button(5, 20, "Prew");
            prew.Clicked += OnPrewQuestionsClicked;
            page = new Label(14, 20, "1 ");
            win.Add(prew, page);
            Button next = new Button(20, 20, "Next");
            next.Clicked += OnNextQuestionsClicked;
            totalPages = new Label(17, 20, Convert.ToString(questionRepository.GetTotalPages()) + " ");
            win.Add(next, totalPages);
            Application.Run();
        }
        static void OnPrewQuestionsClicked()
        {
            int n = Int32.Parse(Convert.ToString(page.Text));
            if (n > 1)
            {
                n--;
                page.Text = Convert.ToString(n);
                questionsListView.SetSource(questionRepository.GetPage(n));
            }
        }
        static void OnNextQuestionsClicked()
        {
            int n = Int32.Parse(Convert.ToString(page.Text));
            if (n < questionRepository.GetTotalPages())
            {
                n++;
                page.Text = Convert.ToString(n);
                questionsListView.SetSource(questionRepository.GetPage(n));
            }
        }
        static void OnOpenQuestion(ListViewItemEventArgs args) 
        {
            Question question = (Question)args.Value;
            OpenQuestionDialog dialog = new OpenQuestionDialog();
            dialog.SetQuestion(question);
            Application.Run(dialog);
            if (dialog.deleted)
            {
                int pageNumber = Int32.Parse(Convert.ToString(page.Text));
                questionRepository.DeleteById(question.id);
                int total = questionRepository.GetTotalPages();
                if (pageNumber > total) pageNumber = total;
                if (total < 1) 
                {
                    List<string> list = new List<string>();
                    list.Add("Data Base is empty");
                    questionsListView.SetSource(list);
                }
                else
                {
                    questionsListView.SetSource(questionRepository.GetPage(pageNumber));
                }
                page.Text = Convert.ToString(pageNumber);
                totalPages.Text = Convert.ToString(total);
            }
            if (dialog.updated)
            {
                int questionId = question.id;
                int pageNumber = questionId/10;
                if(questionId % 10 != 0) pageNumber++;
                question = dialog.GetQuestion();
                questionRepository.Update(questionId, question);
                questionsListView.SetSource(questionRepository.GetPage(pageNumber));
            }
        }
        static void OnCreateQuestionClicked()
        {
            CreateQuestionDialog dialog = new CreateQuestionDialog();
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Question question = dialog.GetQuestion();
                questionRepository.Insert(question);
                questionsListView.SetSource(questionRepository.GetPage(questionRepository.GetTotalPages()));
                page.Text = Convert.ToString(questionRepository.GetTotalPages());
                totalPages.Text = Convert.ToString(questionRepository.GetTotalPages());
            }
        }

        static void OnButtonAnswersClicked()
        {
            Toplevel top = Application.Top;
            top.RemoveAll();
            MenuBar menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("File", new MenuItem [] {
                    new MenuItem ("Import", "", OnImportClicked),
                    new MenuItem ("Export", "", OnExportClicked),
                    new MenuItem ("Exit", "", OnExit),
                }),
                new MenuBarItem ("Help", new MenuItem [] {
                    new MenuItem ("About", "", OnAbout),
                }),
            });
            Window win = new Window("Answers")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            top.Add(menu, win);
            Button btnu = new Button(1, 1, "Go to users");
            btnu.Clicked += OnButtonUsersClicked;
            Button btnq = new Button(20, 1, "Go to questions");
            btnq.Clicked += OnButtonQuestionsClicked;
            win.Add(btnu, btnq);
            Rect frame = new Rect(4, 8, top.Frame.Width, 200);
            if (answerRepository.GetTotalPages() < 1) 
            {
                List<string> list = new List<string>();
                list.Add("Data Base is empty");
                answersListView = new ListView(frame, list);
            }
            else
            {
                answersListView = new ListView(frame, answerRepository.GetPage(1));
                answersListView.OpenSelectedItem += OnOpenAnswer;
            }
            win.Add(answersListView);

            Button btn = new Button(1, 4, "Create new answer");
            btn.Clicked += OnCreateAnswerClicked;
            win.Add(btn);
            Button prew = new Button(5, 20, "Prew");
            prew.Clicked += OnPrewAnswersClicked;
            page = new Label(14, 20, "1 ");
            win.Add(prew, page);
            Button next = new Button(20, 20, "Next");
            next.Clicked += OnNextAnswersClicked;
            totalPages = new Label(17, 20, Convert.ToString(answerRepository.GetTotalPages()) + " ");
            win.Add(next, totalPages);
            Application.Run();
        }
        static void OnPrewAnswersClicked()
        {
            int n = Int32.Parse(Convert.ToString(page.Text));
            if (n > 1)
            {
                n--;
                page.Text = Convert.ToString(n);
                answersListView.SetSource(answerRepository.GetPage(n));
            }
        }
        static void OnNextAnswersClicked() 
        {
            int n = Int32.Parse(Convert.ToString(page.Text));
            if (n < answerRepository.GetTotalPages())
            {
                n++;
                page.Text = Convert.ToString(n);
                answersListView.SetSource(answerRepository.GetPage(n));
            }
        }
        static void OnOpenAnswer(ListViewItemEventArgs args)
        {
            Answer answer = (Answer)args.Value;
            OpenAnswerDialog dialog = new OpenAnswerDialog();
            dialog.SetAnswer(answer);
            Application.Run(dialog);
            if (dialog.deleted)
            {
                int pageNumber = Int32.Parse(Convert.ToString(page.Text));
                answerRepository.DeleteById(answer.id);
                int total = answerRepository.GetTotalPages();
                if (pageNumber > total) pageNumber = total;
                if (total < 1) 
                {
                    List<string> list = new List<string>();
                    list.Add("Data Base is empty");
                    answersListView.SetSource(list);
                }
                else
                {
                    answersListView.SetSource(answerRepository.GetPage(pageNumber));
                }
                page.Text = Convert.ToString(pageNumber);
                totalPages.Text = Convert.ToString(total);
            }
            if (dialog.updated)
            {
                int answerId = answer.id;
                int pageNumber = answerId/10;
                if(answerId % 10 != 0) pageNumber++;
                answer = dialog.GetAnswer();
                answerRepository.Update(answerId, answer);
                answersListView.SetSource(answerRepository.GetPage(pageNumber));
           }
        }
        static void OnCreateAnswerClicked()
        {
            CreateAnswerDialog dialog = new CreateAnswerDialog();
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Answer answer = dialog.GetAnswer();
                answerRepository.Insert(answer);
                answersListView.SetSource(answerRepository.GetPage(answerRepository.GetTotalPages()));
                page.Text = Convert.ToString(answerRepository.GetTotalPages());
                totalPages.Text = Convert.ToString(answerRepository.GetTotalPages());
            }
        }

        static void OnImportClicked()
        {
            ImportDialog dialog = new ImportDialog();
            Application.Run(dialog);
            if(!dialog.cancaled)
            {
                if (dialog.path != "")
                {
                    Import.Read(dialog.path + @"\questions.xml", dialog.path + @"\answers.xml", questionRepository, answerRepository);
                }
                else
                {
                    MessageBox.Query("Error", "Path was not selected", "Ok");
                }
            }

        }
        static void OnExportClicked()
        {
            ExportDialog dialog = new ExportDialog();
            Application.Run(dialog);
            if(!dialog.cancaled)
            {
                if (dialog.path != "")
                {
                    Export.Write(questionRepository.GetExportPinned(dialog.start, dialog.end), dialog.path+ @"\file.zip");
                }
                else
                {
                    MessageBox.Query("Error", "Path was not selected", "Ok");
                }
            }
        }
        static void OnAbout()
        {
            MessageBox.Query("_About", "Program version 1.0... Author: Anastasia Grabovska", "Ok");
        }
        static void OnExit()
        {
            Application.RequestStop();
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
                if(userFields.Length != 4)
                {
                    WriteLine("Wrong user's paramethers");
                    return;
                }
                try
                {
                    User user = new User(0, userFields[0], userFields[1], userFields[2], userFields[3], new Question[0]);
                    WriteLine($"User was added with id [{userRepository.Insert(user)}]");
                }
                catch (System.Exception)
                {
                    WriteLine("You can't use this login.");
                }
            }
            else if(com[1] == "update")
            {
                string[] userFields = com[2].Split(',');
                int n = 0;
                if(userFields.Length != 5 || !Int32.TryParse(userFields[0], out n))
                {
                    WriteLine("Wrong user's paramethers");
                    return;
                }
                try
                {
                    User user = new User(n, userFields[1], userFields[2], userFields[3], userFields[4], new Question[0]);
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
                    string[] names = {"Ivan Ivanenko", "Katya Adushkina", "Vlad A4", "Vasya Pupkin", "Danya Milokhin", "Crazy Frog", "Anonim User"};
                    for(int i = 0; i < n; i ++)
                    {
                        string login = "";
                        for (int j = 0; j < r.Next(1, 6); j++)
                        {
                            login += Convert.ToString((char)r.Next(97,123));
                        }
                        for (int j = 0; j < r.Next(4); j++)
                        {
                            login += Convert.ToString(r.Next(0,10));
                        }
                        try
                        {
                            userRepository.Insert(new User(0, names[r.Next(0,7)], login, moderator[r.Next(0,3)], passwords[r.Next(0,10)], new Question[0]));
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
                Question q = new Question(0, uId, qFields[1], qFields[2], date, null, null);
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
                Question q = new Question(id, uId, qFields[2], qFields[3], date, null, null);
                if (questionRepository.Update(q) == 1) 
                    WriteLine("Question was updated");
                else
                    WriteLine("This question doesn't exist");
            }
            else if(com[1] == "random")
            {
                if (Int32.TryParse(com[2], out int n))
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
                        string [] titles = File.ReadAllLines(@"C:\Users\nasty.DESKTOP-UTJ8J96\OneDrive\Desktop\progbase3\data\generator\title.txt");
                        string [] texts = File.ReadAllLines(@"C:\Users\nasty.DESKTOP-UTJ8J96\OneDrive\Desktop\progbase3\data\generator\text.txt");
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
                else WriteLine(com[2] + " isn't a number");
            }
            else if (com[1] == "getAllQuestions")
            {
                if (Int32.TryParse(com[2], out int n))
                {
                    Question[] questions = questionRepository.GetAllQuestions(n);
                    if (questions.Length > 0) 
                        foreach(Question q in questions)
                        {
                            WriteLine(q.ToString());
                        }
                    else
                        WriteLine("This user doesn't have any questions");
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
                Answer a = new Answer(0, qId, aFields[1], date, aFields[3], null);
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
                Answer a = new Answer(id, qId, aFields[2], date, aFields[4], null);
                if (answerRepository.Update(a) == 1) 
                    WriteLine("Answer was updated");
                else
                    WriteLine("This answer doesn't exist");
            } 
            else if(com[1] == "random")
            {
                if (Int32.TryParse(com[2], out int n))
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
                        string [] answers = File.ReadAllLines(@"C:\Users\nasty.DESKTOP-UTJ8J96\OneDrive\Desktop\progbase3\data\generator\answer.txt");
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
                else WriteLine(com[2] + " isn't a number");
            } 
            else if (com[1] == "getAllAnswers")
            {
                if (Int32.TryParse(com[2], out int n))
                {
                    Answer[] answers = answerRepository.GetAllAnswers(n);
                    if (answers.Length > 0) 
                        foreach(Answer a in answers)
                        {
                            WriteLine(a.ToString());
                        }
                    else
                        WriteLine("This question doesn't have any questions");
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
