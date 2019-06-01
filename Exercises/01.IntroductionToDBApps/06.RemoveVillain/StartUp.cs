using _01.InitialSetup;
using System;
using System.Data.SqlClient;

namespace _06.RemoveVillain
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            int id = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string villainIdQuery = "SELECT Name FROM Villains WHERE Id = @villainId";
                string villainName;

                using (SqlCommand command = new SqlCommand(villainIdQuery, connection))
                {
                    command.Parameters.AddWithValue("@villainId", id);
                    villainName = (string)command.ExecuteScalar();

                    if (villainName == null)
                    {
                        Console.WriteLine("No such villain was found.");
                        return;
                    }
                }

                string deleteMinionsVillains = "DELETE FROM MinionsVillains WHERE VillainId = @villainId";
                int rows;

                using (SqlCommand command = new SqlCommand(deleteMinionsVillains, connection))
                {
                    command.Parameters.AddWithValue("@villainId", id);
                    rows = command.ExecuteNonQuery();
                }

                string deleteVillain = "DELETE FROM Villains WHERE Id = @villainId";

                using (SqlCommand command = new SqlCommand(deleteVillain, connection))
                {
                    command.Parameters.AddWithValue("@villainId", id);
                    command.ExecuteNonQuery();
                }

                Console.WriteLine($"{villainName} was deleted.");
                Console.WriteLine($"{rows} minions were released.");
            }
        }
    }
}
