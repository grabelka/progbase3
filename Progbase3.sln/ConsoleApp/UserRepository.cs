using System;
using Microsoft.Data.Sqlite;
using System.Text;
using System.IO;
using static System.Console;

public class UserRepository
{
    private SqliteConnection connection;
    public UserRepository(SqliteConnection connection) 
    {
        this.connection = connection;
    }
    public int Insert(User user)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = 
        @"
            INSERT INTO users (name, moderator, password) 
            VALUES ($name, $moderator, $password);

            SELECT last_insert_rowid();
        ";
        command.Parameters.AddWithValue("$name", user.name);
        command.Parameters.AddWithValue("$moderator", user.isModerator);   
        command.Parameters.AddWithValue("$password", user.password);
        int newId = Convert.ToInt32(command.ExecuteScalar());      
        connection.Close(); 
        return newId;
    }
    public User GetById(int id)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM users WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        SqliteDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            User user = new User(Int32.Parse(reader.GetString(0)), reader.GetString(1), reader.GetString(2), reader.GetString(3));
            reader.Close();
            connection.Close();
            return user;
        }
        else 
        {
            User user = null;
            reader.Close();
            connection.Close();
            return user;
        }
    }
    public int DeleteById(int id)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"DELETE FROM users WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        int n = command.ExecuteNonQuery();
        connection.Close(); 
        return n;
    }
    public int Update(User user)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"UPDATE users 
        SET name = $name, moderator = $moderator, password = $password
        WHERE id = $id";
        command.Parameters.AddWithValue("$name", user.name);
        command.Parameters.AddWithValue("$moderator", user.isModerator); 
        command.Parameters.AddWithValue("$password", user.password);
        command.Parameters.AddWithValue("$id", user.id);
        int n = command.ExecuteNonQuery();
        connection.Close(); 
        return n;
    }
}