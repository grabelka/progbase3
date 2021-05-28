using System;
using Microsoft.Data.Sqlite;
using System.Text;
using System.IO;
using static System.Console;
using System.Collections.Generic;

namespace ClassLibrary
{
    public class QuestionRepository
    {
        private SqliteConnection connection;
        public QuestionRepository(SqliteConnection connection) 
        {
            this.connection = connection;
        }
        public int Insert(Question question)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO questions (user_id, title, text, created) 
                VALUES ($user_id, $title, $text, $created);

                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$user_id", question.userId);
            command.Parameters.AddWithValue("$title", question.title);   
            command.Parameters.AddWithValue("$text", question.text);   
            command.Parameters.AddWithValue("$created", question.created);
            int newId = Convert.ToInt32(command.ExecuteScalar());      
            connection.Close(); 
            return newId;
        }
        public Question GetById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM questions WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Question question = new Question(Int32.Parse(reader.GetString(0)), Int32.Parse(reader.GetString(1)), reader.GetString(2), reader.GetString(3), DateTime.Parse(reader.GetString(4)), null, null);
                reader.Close();
                connection.Close();
                return question;
            }
            else 
            {
                reader.Close();
                connection.Close();
                return null;
            }
        }
        public int DeleteById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM questions WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int n = command.ExecuteNonQuery();
            connection.Close(); 
            return n;
        }
        public int Update(Question question)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE questions 
            SET user_id = $user_id, title = $title, text = $text, created = $created
            WHERE id = $id";
            command.Parameters.AddWithValue("$user_id", question.userId);
            command.Parameters.AddWithValue("$title", question.title);   
            command.Parameters.AddWithValue("$text", question.text);   
            command.Parameters.AddWithValue("$created", question.created);
            command.Parameters.AddWithValue("$id", question.id);
            int n = command.ExecuteNonQuery();
            connection.Close(); 
            return n;
        }
        public int Update(int id, Question question)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE questions 
            SET user_id = $user_id, title = $title, text = $text, created = $created
            WHERE id = $id";
            command.Parameters.AddWithValue("$user_id", question.userId);
            command.Parameters.AddWithValue("$title", question.title);   
            command.Parameters.AddWithValue("$text", question.text);   
            command.Parameters.AddWithValue("$created", question.created);
            command.Parameters.AddWithValue("$id", id);
            int n = command.ExecuteNonQuery();
            connection.Close(); 
            return n;
        }
        public Question[] GetAllQuestions(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM questions WHERE user_id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            List<Question> list = new List<Question>();
            while (reader.Read())
            {
                Question question = new Question(Int32.Parse(reader.GetString(0)), Int32.Parse(reader.GetString(1)), reader.GetString(2), reader.GetString(3), DateTime.Parse(reader.GetString(4)), null, null);
                list.Add(question);
            }
            Question[] questions = list.ToArray();
            reader.Close();
            connection.Close();
            return questions;
        }
        public List<Question> GetExportPinned(string start, string end)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM questions, answers WHERE questions.id = answers.q_id AND questions.created > $start AND questions.created < $end AND answers.pinned = 'yes'";
            command.Parameters.AddWithValue("$start", start);
            command.Parameters.AddWithValue("$end", end);
            SqliteDataReader reader = command.ExecuteReader();
            List<Question> list = new List<Question>();
            while (reader.Read())
            {
                List<Answer> listAnswer = new List<Answer>();
                Answer answer = new Answer(Int32.Parse(reader.GetString(5)), Int32.Parse(reader.GetString(6)), reader.GetString(7), DateTime.Parse(reader.GetString(8)), reader.GetString(9), null);
                listAnswer.Add(answer);
                Answer[] answers = listAnswer.ToArray();
                Question question = new Question(Int32.Parse(reader.GetString(0)), Int32.Parse(reader.GetString(1)), reader.GetString(2), reader.GetString(3), DateTime.Parse(reader.GetString(4)), null, answers);
                list.Add(question);
            }
            reader.Close();
            connection.Close();
            return list;
        }
        public int GetImagePinned(string start, string end)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM questions, answers WHERE questions.id = answers.q_id AND questions.created > $start AND questions.created < $end AND answers.pinned = 'yes'";
            command.Parameters.AddWithValue("$start", start);
            command.Parameters.AddWithValue("$end", end);
            SqliteDataReader reader = command.ExecuteReader();
            List<Question> list = new List<Question>();
            while (reader.Read())
            {
                Question question = new Question(Int32.Parse(reader.GetString(0)), Int32.Parse(reader.GetString(1)), reader.GetString(2), reader.GetString(3), DateTime.Parse(reader.GetString(4)), null, null);
                list.Add(question);
            }
            reader.Close();
            connection.Close();
            return list.Count;
        }
        public int GetImageAll(string start, string end)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM questions, answers WHERE questions.id = answers.q_id AND questions.created > $start AND questions.created < $end";
            command.Parameters.AddWithValue("$start", start);
            command.Parameters.AddWithValue("$end", end);
            SqliteDataReader reader = command.ExecuteReader();
            List<Question> list = new List<Question>();
            while (reader.Read())
            {
                Question question = new Question(Int32.Parse(reader.GetString(0)), Int32.Parse(reader.GetString(1)), reader.GetString(2), reader.GetString(3), DateTime.Parse(reader.GetString(4)), null, null);
                list.Add(question);
            }
            reader.Close();
            connection.Close();
            return list.Count;
        }
        public int GetTotalPages() 
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM questions";
            int count = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            int pages = count/10;
            if (count%10 != 0) pages++;
            return pages;
        }
        public List<Question> GetPage(int pageNumber) 
        {
            int pages = GetTotalPages();
            if (pageNumber > pages || pageNumber <= 0)
            {
                return null;
            }
            else
            {
                List<Question> list = new List<Question>();
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM questions LIMIT 10 OFFSET $offset";
                command.Parameters.AddWithValue("$offset", 10 * (pageNumber - 1));
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Question question = new Question(Int32.Parse(reader.GetString(0)), Int32.Parse(reader.GetString(1)), reader.GetString(2), reader.GetString(3), DateTime.Parse(reader.GetString(4)), null, null);
                    list.Add(question);
                }
                reader.Close();
                connection.Close();
                return list;
            }
        }
    }
}