using _01.InitialSetup;
using System;
using System.Data.SqlClient;

namespace _03.MinionNames
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            int input = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string villainNameQuery = @"SELECT Name FROM Villains WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(villainNameQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", input);
                    string villainName = (string)command.ExecuteScalar();

                    if (villainName == null)
                    {
                        Console.WriteLine($"No villain with ID {input} exists in the database.");
                        return;
                    }

                    Console.WriteLine($"Villain: {villainName}");
                }

                string minionsQuery = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                            m.Name, 
                                            m.Age
                                       FROM MinionsVillains AS mv
                                       JOIN Minions As m ON mv.MinionId = m.Id
                                       WHERE mv.VillainId = @Id
                                       ORDER BY m.Name";

                using (SqlCommand command = new SqlCommand(minionsQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", input);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            long rowNumber = (long)reader[0];
                            string minionName = (string)reader["Name"];
                            int age = (int)reader["Age"];

                            Console.WriteLine($"{rowNumber}. {minionName} {age}");
                        }

                        if (!reader.HasRows)
                        {
                            Console.WriteLine("(no minions)");
                        }
                    }
                }
            }
        }
    }
}
