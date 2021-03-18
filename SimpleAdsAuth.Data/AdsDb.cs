using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SimpleAdsAuth.Data
{
    public class AdsDb
    {
        private readonly string _connectionString;
        public AdsDb(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddAd(Ad ad)
        {
            var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Ads " +
                "(Title, Date, PhoneNumber, Description, UserId) " +
                "VALUES(@title, @date, @phoneNumber, @description, @userId)";
            cmd.Parameters.AddWithValue("@title", ad.Title);
            cmd.Parameters.AddWithValue("@date", ad.Date);
            cmd.Parameters.AddWithValue("@phoneNumber", ad.PhoneNumber);
            cmd.Parameters.AddWithValue("@description", ad.Description);
            cmd.Parameters.AddWithValue("@userId", ad.UserId);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public void DeleteAd(int id)
        {
            var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE from Ads WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public List<Ad> GetAds()
        {
            var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT a.Id, a.Title, a.Date, a.PhoneNumber, a.Description, " +
                "a.UserId, u.Name From Ads a JOIN Users u ON a.UserId = u.Id ORDER BY Id Desc";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Ad> ads = new List<Ad>();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    Title = (string)reader["Title"],
                    Date = (DateTime)reader["Date"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Description = (string)reader["Description"],
                    UserId = (int)reader["UserId"],
                    UserName = (string)reader["Name"]
                }); 
            }
            return ads;
        }
        public void AddUser(User user, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Name, Email, PasswordHash) " +
                              "VALUES (@name, @email, @passwordHash)";
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            connection.Open();
            cmd.ExecuteNonQuery();

        }
        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isValid ? user : null;
        }

        public User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Email = (string)reader["Email"],
                PasswordHash = (string)reader["PasswordHash"]
            };
        }

    }
}