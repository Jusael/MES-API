using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Linq;

namespace MesApplicationAPI.Helpers.cs
{
    public static class DbHelper
    {
        private static string _connStr = string.Empty;

        public static void Init(IConfiguration config)
        {
            _connStr = config.GetConnectionString("Connection");
        }

        public static async Task<DataTable?> ExecuteScalarAsync(string selectSql, Dictionary<string, object> parameters)
        {
            try
            {
                using var conn = new SqlConnection(_connStr);
                await conn.OpenAsync();

                using var cmd = new SqlCommand(selectSql, conn);

                foreach (var param in parameters)
                    cmd.Parameters.AddWithValue("@" + param.Key, param.Value ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();

                var dt = new DataTable();
                dt.Load(reader);
                return dt;
            }
            catch (SqlException sqlEx)
            {
                throw sqlEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<int?> ExecuteInsertAsync(string insertSql, Dictionary<string, object> values)
        {
            try
            {
                using var conn = new SqlConnection(_connStr);
                await conn.OpenAsync();

                using var cmd = new SqlCommand(insertSql, conn);

                foreach (var param in values)
                    cmd.Parameters.AddWithValue("@" + param.Key, param.Value ?? DBNull.Value);

                int affected = await cmd.ExecuteNonQueryAsync();

                if (affected == 0)
                    return -1;

                return affected;

            }
            catch (SqlException sqlEx)
            {
                throw sqlEx;
            }
            catch (Exception ex)
            {
                return -1;
            }

        }

        public static async Task<int?> ExecuteNonQueryAsync(string updateSql, Dictionary<string, object> setParameters, Dictionary<string, object> valueParameters)
        {
            try
            {
                using var conn = new SqlConnection(_connStr);
                await conn.OpenAsync();

                using var cmd = new SqlCommand(updateSql, conn);


                foreach (var param in setParameters)
                    cmd.Parameters.AddWithValue("@" + param.Key, param.Value ?? DBNull.Value);

                foreach (var param in valueParameters)
                    cmd.Parameters.AddWithValue("@w_" + param.Key, param.Value ?? DBNull.Value);

                int affected = await cmd.ExecuteNonQueryAsync();

                if (affected == 0)
                    return -1;

                return affected;

            }
            catch (SqlException sqlEx)
            {
                throw sqlEx;
            }
            catch (Exception ex)
            {
                return -1;
            }

        }


        public static async Task<int?> CallSp(string spName, Dictionary<string, object> parameters)
        {
            try
            {
                int result = -1;
                using var conn = new SqlConnection(_connStr);
                await conn.OpenAsync();

                using var cmd = new SqlCommand(spName, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                foreach (var param in parameters)
                    cmd.Parameters.AddWithValue("@" + param.Key, param.Value ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();

                return 1;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine(sqlEx.Message);
                throw sqlEx;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }


        public static async Task<int?> excuteStrSql(string sqlText)
        {
            try
            {
                using var conn = new SqlConnection(_connStr);
                await conn.OpenAsync();

                using var cmd = new SqlCommand(sqlText, conn)
                {
                    CommandType = CommandType.Text 
                };

                await cmd.ExecuteNonQueryAsync(); 
                return 1;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine(sqlEx.Message);
                throw sqlEx;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }


        public static async Task<int?> ExecuteNonQueryAsync2(string updateSql, Dictionary<string, object> setParameters, Dictionary<string, object> valueParameters)
        {
            try
            {
                using var conn = new SqlConnection(_connStr);
                await conn.OpenAsync();

                using var cmd = new SqlCommand(updateSql, conn);

                // 파라미터 변환
                var parameters = new List<SqlParameter>();

                foreach (var param in setParameters)
                    parameters.Add(CreateSqlParameter("@" + param.Key, param.Value));

                foreach (var param in valueParameters)
                    parameters.Add(CreateSqlParameter("@w_" + param.Key, param.Value));

                cmd.Parameters.AddRange(parameters.ToArray());

                int affected = await cmd.ExecuteNonQueryAsync();

                return (affected == 0) ? -1 : affected;
            }
            catch (SqlException sqlEx)
            {
                throw sqlEx;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private static SqlParameter CreateSqlParameter(string paramName, object value)
        {
            var param = new SqlParameter();
            param.ParameterName = paramName;

            if (value == null || value == DBNull.Value)
            {
                param.Value = DBNull.Value;
                param.SqlDbType = SqlDbType.VarChar;
                param.Size = 1;
                return param;
            }

            switch (value)
            {
                case string s:
                    param.SqlDbType = SqlDbType.NVarChar;
                    param.Size = (s.Length > 4000) ? -1 : s.Length;
                    param.Value = s;
                    break;

                case int i:
                    param.SqlDbType = SqlDbType.Int;
                    param.Value = i;
                    break;

                case long l:
                    param.SqlDbType = SqlDbType.BigInt;
                    param.Value = l;
                    break;

                case decimal d:
                    param.SqlDbType = SqlDbType.Decimal;
                    param.Value = d;
                    param.Precision = 18;
                    param.Scale = 4;
                    break;

                case DateTime dt:
                    param.SqlDbType = SqlDbType.DateTime;
                    param.Value = dt;
                    break;

                case bool b:
                    param.SqlDbType = SqlDbType.Bit;
                    param.Value = b;
                    break;

                case byte[] bArr:
                    param.SqlDbType = SqlDbType.VarBinary;
                    param.Size = (bArr.Length > 8000) ? -1 : bArr.Length;
                    param.Value = bArr;
                    break;

                default:
                    throw new InvalidOperationException($"지원되지 않는 파라미터 타입: {value.GetType()}");
            }

            return param;
        }
    }
}
