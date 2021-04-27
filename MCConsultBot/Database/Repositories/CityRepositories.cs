using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace MCConsultBot.Database.Repositories
{
    class CityRepositories
    {
        private readonly string _connectionString =
            "Integrated Security=SSPI;" +
            "Persist Security Info=False;" +
            "Initial Catalog=EFIntroductionDb;" +
            "Data Source=WIN-6H5MS9G3H6I";

        public IEnumerable<string> GetCitiesNames()
        {
            List<string> cNames = new List<string>();
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var text = "select Cities.name_city from Cities";
                using (var sqlCommand = new SqlCommand(text, sqlConnection))
                {
                    var reader = sqlCommand.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            cNames.Add(
                                Convert.ToString(reader["name_city"])
                                );
                        }
                    }
                }
            }
            return cNames;
        }

        public IEnumerable<string> GetAddressesByCity(string CityName)
        {
            //select Addresses.place from Cities 
            //join Addresses on Cities.id = Addresses.id_city
            //where Cities.name_city = 'Алматы';
            List<string> addresses = new List<string>();

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var text = $"select Addresses.place from Cities join Addresses on Cities.id = Addresses.id_city where Cities.name_city = '{CityName}';";

                using (var sqlCommand = new SqlCommand(text, sqlConnection))
                {
                    var reader = sqlCommand.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            addresses.Add(
                                Convert.ToString(reader["place"])
                                );
                        }
                    }
                }
            }
            return addresses.ToArray();
        }
    }
}
