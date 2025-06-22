using MesApplicationAPI.Helpers.cs;
using MesApplicationAPI.Interface;
using System.Data;

namespace MesApplicationAPI.Dao
{
    public class UserDao : IBaseDao
    {
        public Task<int?> ExecuteInsertAsync(string caseKey, Dictionary<string, object> valueParam)
        {
            throw new NotImplementedException();
        }


        public async Task<DataTable?> ExecuteSelectAsync(string caseKey, Dictionary<string, object> param)
        {
            string sql = caseKey switch
            {
                "LOGIN" => @"
SELECT COUNT(*) 
FROM USER_INFO 
WHERE User_Id = @USER_ID AND user_Password = @USER_PASSWORD",

                "SEARCH_USER_INFO" => @"               
SELECT LEVEL
  FROM USER_INFO
 WHERE USER_ID			= @USER_ID
   AND USER_PASSWORD	= @USER_PASSWORD
   AND @NOW_DATE		BETWEEN START_DATE AND END_DATE",


                _ => "SELECT * FROM USER_INFO WHERE User_Id = @USER_ID"
            };

            return await DbHelper.ExecuteScalarAsync(sql, param); ;
        }

        public async Task<int?> ExecuteUpdatetAsync(string caseKey, Dictionary<string, object> setParam, Dictionary<string, object> whereParam)
        {
            string setClause = string.Join(", ", setParam.Keys.Select(k => $"{k} = @{k}"));
            string whereClause = string.Join(" AND ", whereParam.Keys.Select(k => $"{k} = @w_{k}"));


            string sql = caseKey switch
            {
                
                _ => string.Format("update user_info set {0} where {1}", setClause, whereClause)
            };

            return await DbHelper.ExecuteNonQueryAsync(sql, setParam, whereParam);
        }

    }
}
