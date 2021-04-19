using System;
using Microsoft.Data.Sqlite;
using System.Text;
using System.IO;
using static System.Console;

public class AnswerRepository
{
    private SqliteConnection connection;
    public AnswerRepository(SqliteConnection connection) 
    {
        this.connection = connection;
    }
    public int Insert(Answer answer)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = 
        @"
            INSERT INTO answers (q_id, text, created, pinned) 
            VALUES ($q_id, $text, $created, $pinned);

            SELECT last_insert_rowid();
        ";
        command.Parameters.AddWithValue("$q_id", answer.questionId);
        command.Parameters.AddWithValue("$text", answer.text);   
        command.Parameters.AddWithValue("$created", answer.created);
        command.Parameters.AddWithValue("$pinned", answer.pinned);
        int newId = Convert.ToInt32(command.ExecuteScalar());      
        connection.Close(); 
        return newId;
    }
    public Answer GetById(int id)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM answers WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        SqliteDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            Answer answer = new Answer(Int32.Parse(reader.GetString(0)), Int32.Parse(reader.GetString(1)), reader.GetString(2), DateTime.Parse(reader.GetString(3)), reader.GetString(4));
            reader.Close();
            connection.Close();
            return answer;
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
        command.CommandText = @"DELETE FROM answers WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        int n = command.ExecuteNonQuery();
        connection.Close(); 
        return n;
    }
    public int Update(Answer answer)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"UPDATE answers 
        SET q_id, text, created, pinned
        WHERE id = $id";
        command.Parameters.AddWithValue("$q_id", answer.questionId);
        command.Parameters.AddWithValue("$text", answer.text);   
        command.Parameters.AddWithValue("$created", answer.created);
        command.Parameters.AddWithValue("$pinned", answer.pinned);
        command.Parameters.AddWithValue("$id", answer.id);
        int n = command.ExecuteNonQuery();
        connection.Close(); 
        return n;
    }
}