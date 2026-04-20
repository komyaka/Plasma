// DbHelper.cs — Безопасная работа с БД (параметризованные запросы + using)
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Plazma.Controllers
{
    /// <summary>Утилита для безопасной работы с базой данных.</summary>
    public static class DbHelper
    {
        /// <summary>Создать новое подключение (вызывающий код ОБЯЗАН использовать using).</summary>
        public static SqlConnection CreateConnection() => new SqlConnection(AppConfig.ConnectionString);

        /// <summary>Выполнить SELECT и обработать каждую строку.</summary>
        public static void ExecuteReader(string query, Dictionary<string, object> parameters, Action<SqlDataReader> rowHandler)
        {
            Logger.Sql(query);
            using (var conn = CreateConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn) { CommandTimeout = 60 })
                {
                    AddParams(cmd, parameters);
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read()) rowHandler(reader);
                }
            }
        }

        /// <summary>Выполнить INSERT/UPDATE/DELETE. Возвращает кол-во затронутых строк.</summary>
        public static int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            Logger.Sql(query);
            using (var conn = CreateConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn) { CommandTimeout = 60 })
                {
                    AddParams(cmd, parameters);
                    try { return cmd.ExecuteNonQuery(); }
                    catch (Exception ex) { Logger.Error("ExecuteNonQuery: " + query, ex); throw; }
                }
            }
        }

        /// <summary>Выполнить запрос и вернуть скалярное значение.</summary>
        public static object ExecuteScalar(string query, Dictionary<string, object> parameters = null)
        {
            Logger.Sql(query);
            using (var conn = CreateConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn) { CommandTimeout = 60 })
                {
                    AddParams(cmd, parameters);
                    try { return cmd.ExecuteScalar(); }
                    catch (Exception ex) { Logger.Error("ExecuteScalar: " + query, ex); throw; }
                }
            }
        }

        private static void AddParams(SqlCommand cmd, Dictionary<string, object> parameters)
        {
            if (parameters == null) return;
            foreach (var p in parameters) cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
        }

        // --- Безопасное чтение из SqlDataReader ---
        public static string SafeStr(SqlDataReader r, string col, string def = "")
        { try { int o = r.GetOrdinal(col); return r.IsDBNull(o) ? def : Convert.ToString(r.GetValue(o)); } catch { return def; } }

        public static int SafeInt(SqlDataReader r, string col, int def = 0)
        { try { int o = r.GetOrdinal(col); return r.IsDBNull(o) ? def : Convert.ToInt32(r.GetValue(o)); } catch { return def; } }

        public static float SafeFloat(SqlDataReader r, string col, float def = 0)
        { try { int o = r.GetOrdinal(col); return r.IsDBNull(o) ? def : (float)Convert.ToDouble(r.GetValue(o)); } catch { return def; } }

        public static DateTime SafeDate(SqlDataReader r, string col)
        { var def = DateTime.Parse("1996/04/26"); try { int o = r.GetOrdinal(col); return r.IsDBNull(o) ? def : Convert.ToDateTime(r.GetValue(o)); } catch { return def; } }

        public static bool SafeBool(SqlDataReader r, string col, bool def = false)
        { try { int o = r.GetOrdinal(col); return r.IsDBNull(o) ? def : Convert.ToBoolean(r.GetValue(o)); } catch { return def; } }
    }
}
