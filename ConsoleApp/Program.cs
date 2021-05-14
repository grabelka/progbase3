using System;
using ClassLibrary;
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
            User user = Autentification.Verify(userRepository, "top1", "1234");
            
            Application.Init();
            OnButtonUsersClicked();
            Application.Run();
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
    }
}
