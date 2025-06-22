using MesApplicationAPI.Helpers.cs;
using MesApplicationAPI.Interface;
using System.Data;

namespace MesApplicationAPI.Dao
{
    public class SpExecLogDao : IBaseDao
    {
        public async Task<DataTable?> ExecuteSelectAsync(string caseKey, Dictionary<string, object> param)
        {
            string sql = caseKey switch
            {
                "SEARCH_EXCUTE_FAIL_SP" => @"
                SELECT * FROM SP_EXEC_LOG WHERE STATUS = @STATUS",

                _ => "SELECT * FROM SP_EXEC_LOG"
            };

            return await DbHelper.ExecuteScalarAsync(sql, param);
        }


        public async Task<int?> ExecuteInsertAsync(string caseKey, Dictionary<string, object> valueParam)
        {
            string columnList = string.Join(", ", valueParam.Keys);
            string parameterList = string.Join(", ", valueParam.Keys.Select(k => "@" + k));

            string sql = caseKey switch
            {

                _ => string.Format("INSERT SP_EXEC_LOG ({0}) VALUES ({1})", columnList, parameterList)
            };

            return await DbHelper.ExecuteInsertAsync(sql, valueParam);
        }


        public async Task<int?> ExecuteUpdatetAsync(string caseKey, Dictionary<string, object> setParam, Dictionary<string, object> whereParam)
        {
            string setClause = string.Join(", ", setParam.Keys.Select(k => $"{k} = @{k}"));
            string whereClause = string.Join(" AND ", whereParam.Keys.Select(k => $"{k} = @w_{k}"));


            string sql = caseKey switch
            {

                _ => string.Format("update SP_EXEC_LOG set {0} where {1}", setClause, whereClause)
            };

            return await DbHelper.ExecuteNonQueryAsync(sql, setParam, whereParam);
        }


    }

}
