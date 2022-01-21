using System.Data.SqlClient;
using System.Collections.Generic;
namespace SplitProject.Data
{
    public class Listing
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string Name { get; set; }

        public string Text { get; set; }

        public int PhoneNumber { get; set; }
    }
    public class ListingDB
    {
        private readonly string _connectionString;
        public ListingDB(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddList(Listing listing)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"Insert into Listing(DateCreated, Name, PhoneNumber, Text)
                               values(@datecreated, @name, @phonenumber, @text) select SCOPE_IDENTITY()";
           
            cmd.Parameters.AddWithValue("@datecreated", DateTime.Now);
            cmd.Parameters.AddWithValue("@phonenumber", listing.PhoneNumber);
            cmd.Parameters.AddWithValue("@text", listing.Text);
            object name = listing.Name;
            if(name == null) 
            {
             name = DBNull.Value;
            }
            cmd.Parameters.AddWithValue("@name", name);
            conn.Open();
            listing.Id = (int)(decimal)cmd.ExecuteScalar();


        }
        public List<Listing> GetAds()
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            {
                cmd.CommandText = "select * from Listing order by DateCreated desc";
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<Listing> listings = new List<Listing>();
                while (reader.Read())
                {
                    listings.Add(new Listing
                    {
                        Id = (int)reader["Id"],
                        DateCreated = (DateTime)reader["DateCreated"],
                        Name = reader.GetOrNull<string>("Name"),
                        Text = (string)reader["Text"],
                        PhoneNumber = (int)reader["PhoneNumber"]
                    });
                }
                return listings;
            }
           
        }
        public void DeleteAds(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            {
                cmd.CommandText = @"Delete from Listing where Id = @id";
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            };
        }
    }
    public static class Extensions
    {
        public static T GetOrNull<T>(this SqlDataReader reader, string column)
        {
            object value = reader[column];
            if (value == DBNull.Value)
            {
                return default(T);
            }
            return (T)value;
        }
    }
}
