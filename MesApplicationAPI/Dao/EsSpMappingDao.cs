using MesApplicationAPI.Helpers.cs;
using MesApplicationAPI.Interface;
using System.Data;

namespace MesApplicationAPI.Dao
{
    public class EsSpMappingDao : IBaseDao
    {

        public Task<int?> ExecuteInsertAsync(string caseKey, Dictionary<string, object> valueParam)
        {
            throw new NotImplementedException();
        }

        public async Task<DataTable?> ExecuteSelectAsync(string caseKey, Dictionary<string, object> whereParam)
        {
            string whereClause = string.Join(" AND ", whereParam.Keys.Select(k => $"{k} = @{k}"));

            string sql = caseKey switch
            {
                _ => string.Format("SELECT * FROM ES_SP_MAPPING WHERE {0}", whereClause)
            };

            return await DbHelper.ExecuteScalarAsync(sql, whereParam); ;
        }

        public async Task<int?> ExecuteUpdatetAsync(string caseKey, Dictionary<string, object> setParam, Dictionary<string, object> whereParam)
        {
            string setClause = string.Join(", ", setParam.Keys.Select(k => $"{k} = @{k}"));
            string whereClause = string.Join(" AND ", whereParam.Keys.Select(k => $"{k} = @w_{k}"));


            string sql = caseKey switch
            {

                _ => string.Format("update ES_SP_MAPPING set {0} where {1}", setClause, whereClause)
            };

            return await DbHelper.ExecuteNonQueryAsync(sql, setParam, whereParam);
        }


    }

}
