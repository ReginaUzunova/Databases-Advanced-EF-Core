using _01.InitialSetup;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace _05.ChangeTownNamesCasing
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            string countryName = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE Towns SET Name = UPPER(Name) WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";

                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@countryName", countryName);
                    int rows = command.ExecuteNonQuery();

                    if (rows == 0)
                    {
                        Console.WriteLine("No town names were affected.");
                        return;
                    }

                    Console.WriteLine($"{rows} town names were affected.");
                }

                string townNamesQuery = "SELECT t.Name  FROM Towns as t JOIN Countries AS c ON c.Id = t.CountryCode WHERE c.Name = @countryName";

                List<string> towns = new List<string>();

                using (SqlCommand command = new SqlCommand(townNamesQuery, connection))
                {
                    command.Parameters.AddWithValue("@countryName", countryName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            towns.Add((string)reader[0]);
                        }

                        Console.WriteLine($"[{string.Join(", ", towns)}]");
                    }
                }
            }
        }
    }
}
