using System.Collections.Generic;
using System.Data.SqlClient;

namespace MCConsultBot.Database.Repositories
{
    interface IRateLimiterRepository
    {
        void Increase(string key);
        int Get(string key);
        void Refresh();
    }

    /// <summary>
    /// Здесь мы используем паттерн "Одиночка/Singleton" для получения одной
    /// и той же копии объекта InMemoryRateLimiter, чтобы состояние счетчика
    /// не терялось на каждый запрос
    /// </summary>
    class InMemoryRateLimiter : IRateLimiterRepository
    {
        private InMemoryRateLimiter()
        {
            _counter = new Dictionary<string, int>();
        }
        private static InMemoryRateLimiter _instance;
        private Dictionary<string, int> _counter;

        public static InMemoryRateLimiter GetInstance()
        {
            if (_instance == null)
                _instance = new InMemoryRateLimiter();

            return _instance;
        }
        public int Get(string key)
        {
            if (!_instance._counter.ContainsKey(key))
            {
                _instance._counter[key] = 0;
            }

            return _instance._counter[key];
        }
        public void Increase(string key)
        {
            Get(key);
            _instance._counter[key]++;
        }

        public void Refresh()
        {
            _instance._counter = new Dictionary<string, int>();
        }
    }

    public class DbRateLimiterRepository : IRateLimiterRepository
    {
        private readonly string _connectionString =
            "Integrated Security=SSPI;" +
            "Persist Security Info=False;" +
            "Initial Catalog=EFIntroductionDb;" +
            "Data Source=WIN-6H5MS9G3H6I";

        public int Get(string key)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var text = $"select Counter from [RateLimiterTb] where ChatId = {key}";

                using (var command = new SqlCommand(text, sqlConnection))
                {
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        return int.Parse(reader[1].ToString());
                    }
                    else
                    {
                        InitializeDefaultValues(key);
                        return 0;
                    }
                }
            }
        }

        private void InitializeDefaultValues(string chatId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var text = $"insert into [RateLimiterTb](ChatId, Counter) values({chatId}, 0)";

                using (var command = new SqlCommand(text, sqlConnection))
                {
                    command.ExecuteNonQuery();
                    return;
                }
            }
        }

        public void Increase(string key)
        {
            //UPDATE RateLimiterTb SET Counter = Counter + 1 WHERE chatId = 'hello';

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var text = $"UPDATE RateLimiterTb SET Counter = Counter + 1 WHERE chatId = '{key}';";

                using (var command = new SqlCommand(text, sqlConnection))
                {
                    command.ExecuteNonQuery();
                    return;
                }
            }
        }

        public void Refresh()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var text = "delete from RateLimiterTb;";

                using (var command = new SqlCommand(text, sqlConnection))
                {
                    command.ExecuteNonQuery();
                    return;
                }
            }
        }
    }
}
