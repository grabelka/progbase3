using System;
using System.Collections.Generic;
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
            INSERT INTO users (login, name, moderator, password) 
            VALUES ($login, $name, $moderator, $password);

            SELECT last_insert_rowid();
        ";
        command.Parameters.AddWithValue("$login", user.login);
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
            User user = new User(Int32.Parse(reader.GetString(0)), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), new Question[0]);
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
        SET name = $name, login = $login moderator = $moderator, password = $password
        WHERE id = $id";
        command.Parameters.AddWithValue("$name", user.name);
        command.Parameters.AddWithValue("$login", user.login);
        command.Parameters.AddWithValue("$moderator", user.isModerator); 
        command.Parameters.AddWithValue("$password", user.password);
        command.Parameters.AddWithValue("$id", user.id);
        int n = command.ExecuteNonQuery();
        connection.Close(); 
        return n;
    }
    public int Update(int userId, User user)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"UPDATE users 
        SET name = $name, login = $login, moderator = $moderator, password = $password
        WHERE id = $id";
        command.Parameters.AddWithValue("$name", user.name);
        command.Parameters.AddWithValue("$login", user.login);
        command.Parameters.AddWithValue("$moderator", user.isModerator); 
        command.Parameters.AddWithValue("$password", user.password);
        command.Parameters.AddWithValue("$id", userId);
        int n = command.ExecuteNonQuery();
        connection.Close(); 
        return n;
    }
    public int GetTotalPages() 
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT COUNT(*) FROM users";
        int count = Convert.ToInt32(command.ExecuteScalar());
        connection.Close();
        int pages = count/10;
        if (count%10 != 0) pages++;
        return pages;
    }
    public List<User> GetPage(int pageNumber) 
    {
        int pages = GetTotalPages();
        if (pageNumber > pages || pageNumber <= 0)
        {
            return null;
        }
        else
        {
            List<User> list = new List<User>();
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users LIMIT 10 OFFSET $offset";
            command.Parameters.AddWithValue("$offset", 10 * (pageNumber - 1));
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                User user = new User(Int32.Parse(reader.GetString(0)), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), new Question[0]);
                list.Add(user);
            }
            reader.Close();
            connection.Close();
            return list;
        }
    }
}