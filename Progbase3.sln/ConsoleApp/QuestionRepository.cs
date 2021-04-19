using System;
using Microsoft.Data.Sqlite;
using System.Text;
using System.IO;
using static System.Console;

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
            Question question = new Question(Int32.Parse(reader.GetString(0)), Int32.Parse(reader.GetString(1)), reader.GetString(2), reader.GetString(3), DateTime.Parse(reader.GetString(4)));
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
}