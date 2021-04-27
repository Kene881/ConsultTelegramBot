using MCConsultBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace MCConsultBot.Database.Repositories
{
    class DoctorsRepository
    {
        private readonly string _connectionString =
            "Integrated Security=SSPI;" +
            "Persist Security Info=False;" +
            "Initial Catalog=EFIntroductionDb;" +
            "Data Source=WIN-6H5MS9G3H6I";

        public List<Doctor> GetAllDoctors()
        {
            List<Doctor> doctors = new List<Doctor>();
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var text = "select Doctors.first_name, Doctors.last_name, Doctors.price, Schedule.time_receipt, Room.num from Doctors " +
                    "join Room on Room.id = Doctors.id_room " +
                    "join Schedule on Schedule.id = Doctors.id_schedule;";
                using (var sqlCommand = new SqlCommand(text, sqlConnection))
                {
                    var reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string fname = Convert.ToString(reader["first_name"]);
                            string lname = Convert.ToString(reader["last_name"]);
                            int price = Convert.ToInt32(reader["price"]);
                            string time_receipt = Convert.ToString(reader["time_receipt"]);
                            int room_num = Convert.ToInt32(reader["num"]);

                            doctors.Add(new Doctor(fname, lname, price, time_receipt, room_num));
                        }
                    }
                }
            }
            return doctors;
        }

        public IEnumerable<string> GetDoctorsName()
        {
            List<string> dNames = new List<string>();
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var text = "select Doctors.first_name, Doctors.last_name from Doctors";
                using (var sqlCommand = new SqlCommand(text, sqlConnection))
                {
                    var reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string fname = Convert.ToString(reader["first_name"]);
                            string lname = Convert.ToString(reader["last_name"]);

                            dNames.Add($"{fname} {lname}");
                        }
                    }
                }
            }
            return dNames;
        }

        public string GetDoctorScheduleByName(string flname)
        {
            //select Schedule.time_receipt from Doctors
            //join Schedule on Schedule.id = Doctors.id_schedule
            //where Doctors.first_name = 'Осипова'
            //;
            string fname = flname.Split(' ')[0];
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var text = $"select Schedule.time_receipt from Doctors join Schedule on Schedule.id = Doctors.id_schedule where Doctors.first_name = '{fname}'";
                using (var sqlCommand = new SqlCommand(text, sqlConnection))
                {
                    var reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return Convert.ToString(reader["time_receipt"]);
                        }
                    }
                }
            }
            return string.Empty;
        }

        public string GetDoctorRoomByName(string flname)
        {
            string fname = flname.Split(' ')[0];
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var text = $"select Room.num from Doctors join Room on Room.id = Doctors.id_room where Doctors.first_name = '{fname}';";

                using (var sqlCommand = new SqlCommand(text, sqlConnection))
                {
                    var reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return Convert.ToString(reader["num"]);
                        }
                    }
                }
            }
            return string.Empty;
        }

        public string GetDoctorPriceByName(string flname)
        {
            //select Doctors.price from Doctors where Doctors.first_name = 'Осипова';
            string fname = flname.Split(' ')[0];
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var text = $"select Doctors.price from Doctors where Doctors.first_name = '{fname}';";

                using (var sqlCommand = new SqlCommand(text, sqlConnection))
                {
                    var reader = sqlCommand.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return Convert.ToString(reader["price"]);
                        }
                    }
                }

            }
            return string.Empty;
        }
    }
}
