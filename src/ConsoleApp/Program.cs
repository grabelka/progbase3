using System;
using System.IO;
using ClassLibrary;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Terminal.Gui;
using System.Xml.Serialization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        static string dbPath = @"..\..\data\data.db";
        static SqliteConnection connection = new SqliteConnection($"Data Source={dbPath}");
        static UserRepository userRepository = new UserRepository(connection);
        static QuestionRepository questionRepository = new QuestionRepository(connection);
        static AnswerRepository answerRepository = new AnswerRepository(connection);
        static string searchFilter = "";
        static TextField searchUser;
        static TextField searchQuestion;
        static TextField searchAnswer;
        static TextField inputLogin;
        static TextField inputPass;
        static User current;
        static ListView usersListView;
        static ListView questionsListView;
        static ListView answersListView;
        static Label page;
        static Label totalPages;
        static IPAddress ipAddress = IPAddress.Loopback;   
        static Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        static IPEndPoint remoteEP = new IPEndPoint(ipAddress, 3000);
        static void Main(string[] args)
        {
            Application.Init();
            Window win = new Window("Log In")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            Application.Top.Add(win);
            try
            {
                sender.Connect(remoteEP);
            }
            catch (Exception)
            {
                MessageBox.ErrorQuery("Error", "Can't connect to the server.", "Ok");
                OnExit();
            }
            Button btnv = new Button(20, 15, "Verify");
            btnv.Clicked += OnButtonVerifyClicked;
            win.Add(btnv);
            Button btnr = new Button(2, 1, "Registration");
            btnr.Clicked += OnButtonRegClicked;
            win.Add(btnr);
            Label lLabel = new Label(2, 4, "Login: ");
            inputLogin = new TextField(20, 4, 40, "");
            win.Add(lLabel, inputLogin);
            Label pLabel = new Label(2, 6, "Password: ");
            inputPass = new TextField(20, 6, 40, "");
            inputPass.Secret = true;
            win.Add(pLabel, inputPass);
            Application.Run();
        }
        static void OnButtonRegClicked()
        {
            if(!File.Exists(dbPath))
            {
                MessageBox.ErrorQuery("Error", "DataBase not found.", "Ok");
                Application.RequestStop();
            }
            else
            {
                RegistrationDialog dialog = new RegistrationDialog();
                Application.Run(dialog);
                if (!dialog.canceled)
                {
                    User user = dialog.GetUser();
                    ExportUser export = new ExportUser("registration", user);
                    XmlSerializer ser = new XmlSerializer(typeof(ExportUser));
                    StringWriter writer = new StringWriter();
                    ser.Serialize(writer, export);
                    writer.Close();
                    try
                    {
                        byte[] msg = Encoding.ASCII.GetBytes(writer.ToString());

                        int bytesSent = sender.Send(msg);
                    }
                    catch (Exception)
                    {
                        MessageBox.ErrorQuery("Error", "Oops! Something went wrong", "Ok");
                    }
                    byte[] bytes = new byte[1024];
                    int bytesRec = sender.Receive(bytes);
                    string message = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (bool.Parse(message)) 
                    {
                        MessageBox.Query("Message", "You were registered!", "Ok");
                    }
                    else 
                        MessageBox.Query("Error", "You can't use this login. It is already taken.", "Ok");
                }
            }
        }
        static void OnButtonVerifyClicked()
        {
            if(!File.Exists(dbPath))
            {
                MessageBox.ErrorQuery("Error", "DataBase not found.", "Ok");
                Application.RequestStop();
            }
            ExportUser export = new ExportUser("verification", new User(0, "", inputLogin.Text.ToString(), "", inputPass.Text.ToString(), null));
            XmlSerializer ser = new XmlSerializer(typeof(ExportUser));
            StringWriter writer = new StringWriter();
            ser.Serialize(writer, export);
            writer.Close();
            try
            {
                byte[] msg = Encoding.ASCII.GetBytes(writer.ToString());
                int bytesSent = sender.Send(msg);
            }
            catch (Exception)
            {
                MessageBox.ErrorQuery("Error", "Oops! Something went wrong", "Ok");
            }
            byte[] bytes = new byte[1024];
            int bytesRec = sender.Receive(bytes);
            string[] message = Encoding.ASCII.GetString(bytes, 0, bytesRec).Split('#');
            if (bool.Parse(message[0])) 
            {
                current = new User(0, message[1], inputLogin.Text.ToString(), message[2], inputPass.Text.ToString(), null);
                OnButtonUsersClicked();
            }
            else
                MessageBox.Query("Error", "Wrong login or password", "Ok");
        }
        static void OnButtonUsersClicked()
        {
            Toplevel top = Application.Top;
            top.RemoveAll();
            MenuBar menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("File", new MenuItem [] {
                    new MenuItem ("Import", "", OnImportClicked),
                    new MenuItem ("Export", "", OnExportClicked),
                    new MenuItem ("Get stats", "", OnGetImage),
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
            Label userLabel = new Label(70, 1, $"You: {current.name}({current.login})" );
            Button btnex = new Button(100, 1, "Change user");
            btnex.Clicked += OnChangeUser;
            Button btnq = new Button(1, 1, "Go to questions");
            btnq.Clicked += OnButtonQuestionsClicked;
            Button btna = new Button(20, 1, "Go to answers");
            btna.Clicked += OnButtonAnswersClicked;
            win.Add(btnq, btna, btnex, userLabel);
            searchUser = new TextField(4, 6, 20, "");
            searchUser.KeyPress += OnSearchUser;
            win.Add(searchUser);
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
        static void OnSearchUser(View.KeyEventEventArgs args)
        {
            if(args.KeyEvent.Key == Key.Enter)
            {
                searchFilter = searchUser.Text.ToString();
                usersListView.SetSource(userRepository.GetSearchPage(1, searchFilter));
                totalPages.Text = Convert.ToString(userRepository.GetSearchUsers(searchFilter).Count/10 + 1);
            }
        }
        static void OnPrewUsersClicked()
        {
            int n = Int32.Parse(Convert.ToString(page.Text));
            if (n > 1)
            {
                n--;
                page.Text = Convert.ToString(n);
                usersListView.SetSource(userRepository.GetSearchPage(n, searchFilter));
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
                if (user.id != current.id && current.isModerator == "no")
                {
                    MessageBox.Query("Error", "You have no access to this function", "Ok");
                    return;
                }
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
                if (user.id != current.id && current.isModerator == "no")
                {
                    MessageBox.Query("Error", "You have no access to this function", "Ok");
                    return;
                }
                int userId = user.id;
                int pageNumber = userId/10;
                if(userId % 10 != 0) pageNumber++;
                user = dialog.GetUser();
                if(user.name == "" || user.login == "" || user.password == "") 
                {
                    MessageBox.Query("Error", "Wrong input", "Ok");
                    return;
                }
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
                if(user.name == "" || user.login == "" || user.password == "") 
                {
                    MessageBox.Query("Error", "Wrong input", "Ok");
                    return;
                }
                Autentification.Register(userRepository, user.name, user.login, user.isModerator, user.password);
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
                    new MenuItem ("Get stats", "", OnGetImage),
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
            Label userLabel = new Label(70, 1, $"You: {current.name}({current.login})" );
            Button btnex = new Button(100, 1, "Change user");
            btnex.Clicked += OnChangeUser;
            Button btnu = new Button(1, 1, "Go to users");
            btnu.Clicked += OnButtonUsersClicked;
            Button btna = new Button(20, 1, "Go to answers");
            btna.Clicked += OnButtonAnswersClicked;
            win.Add(btnu, btna, btnex, userLabel);
            searchQuestion = new TextField(4, 6, 20, "");
            searchQuestion.KeyPress += OnSearchUser;
            win.Add(searchQuestion);
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
                if (question.userId != current.id && current.isModerator == "no")
                {
                    MessageBox.Query("Error", "You have no access to this function", "Ok");
                    return;
                }
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
                if (question.userId != current.id && current.isModerator == "no")
                {
                    MessageBox.Query("Error", "You have no access to this function", "Ok");
                    return;
                }
                int questionId = question.id;
                int pageNumber = questionId/10;
                if(questionId % 10 != 0) pageNumber++;
                question = dialog.GetQuestion();
                if(question.title == "" || question.text == "") 
                {
                    MessageBox.Query("Error", "Wrong input", "Ok");
                    return;
                }
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
                if(question.title == "" || question.text == "") 
                {
                    MessageBox.Query("Error", "Wrong input", "Ok");
                    return;
                }
                if (current.isModerator == "no")
                {
                    MessageBox.Query("Warning", "You added a question(urer id was changed)", "Ok");
                    question.userId = current.id;
                }
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
                    new MenuItem ("Get stats", "", OnGetImage),
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
            Label userLabel = new Label(70, 1, $"You: {current.name}({current.login})" );
            Button btnex = new Button(100, 1, "Change user");
            btnex.Clicked += OnChangeUser;
            Button btnu = new Button(1, 1, "Go to users");
            btnu.Clicked += OnButtonUsersClicked;
            Button btnq = new Button(20, 1, "Go to questions");
            btnq.Clicked += OnButtonQuestionsClicked;
            win.Add(btnu, btnq, btnex, userLabel);
            searchAnswer = new TextField(4, 6, 20, "");
            searchAnswer.KeyPress += OnSearchUser;
            win.Add(searchAnswer);
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
                if (questionRepository.GetById(answer.questionId).userId != current.id && current.isModerator == "no")
                {
                    MessageBox.Query("Error", "You have no access to this function", "Ok");
                    return;
                }
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
                if(answer.text == "") 
                {
                    MessageBox.Query("Error", "Wrong input", "Ok");
                    return;
                }
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
                if(answer.text == "") 
                {
                    MessageBox.Query("Error", "Wrong input", "Ok");
                    return;
                }
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
                    try
                    {
                        Import.Read(dialog.path + @"\questions.xml", dialog.path + @"\answers.xml", questionRepository, answerRepository);
                    }
                    catch
                    {
                        MessageBox.Query("Error", "Files don`t exist", "Ok");
                    }
                }
                else
                {
                    MessageBox.Query("Error", "Path was not selected", "Ok");
                }
            }

        }
        static void OnGetImage()
        {
            ImageDialog dialog = new ImageDialog();
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                int pinned = questionRepository.GetImagePinned(dialog.start, dialog.end);
                int all = questionRepository.GetImageAll("2020-01-01", "2022-01-01");
                var plt = new ScottPlot.Plot(600, 400);
                double[] xs = {-1, 0, 1};
                double[] valuesA = {0, all, 0};
                double[] valuesB = {0, all+pinned, 0};
                plt.PlotBar(xs, valuesB, label: "Pinned");
                plt.PlotBar(xs, valuesA, label: "All");
                plt.Legend();
                plt.Title("Questions stats");
                plt.SaveFig("QuestionsStats.png");
                string s = File.ReadAllText(@"..\..\docs\StatsTeamplate.xml");
                s = s.Replace("{\\start\\}",$"{dialog.start}");
                s = s.Replace("{\\end\\}", $"{dialog.end}");
                s = s.Replace("{\\all\\}", $"{all}");
                s = s.Replace("{\\pinned\\}", $"{pinned}");
                File.WriteAllText(@"..\..\docs\Stats.xml", s);
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
        static void OnChangeUser()
        {
            Toplevel top = Application.Top;
            top.RemoveAll();
            Window win = new Window("Log In")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            Application.Top.Add(win);
            Button btnv = new Button(20, 15, "Verify");
            btnv.Clicked += OnButtonVerifyClicked;
            win.Add(btnv);
            Button btnr = new Button(2, 1, "Registration");
            btnr.Clicked += OnButtonRegClicked;
            win.Add(btnr);
            Label lLabel = new Label(2, 4, "Login: ");
            inputLogin = new TextField(20, 4, 40, "");
            win.Add(lLabel, inputLogin);
            Label pLabel = new Label(2, 6, "Password: ");
            inputPass = new TextField(20, 6, 40, "");
            inputPass.Secret = true;
            win.Add(pLabel, inputPass);
            Application.Run();
        }
        static void OnAbout()
        {
            MessageBox.Query("_About", "Program version 1.0... Author: Anastasia Grabovska", "Ok");
        }
        static void OnExit()
        {
            sender.Close();
            Application.RequestStop();
        }
    }
}
