using MesApplicationAPI.Helpers.cs;
using MesApplicationAPI.Interface;
using System.Data;

namespace MesApplicationAPI.Dao
{
    public class SignDao : IBaseDao
    {

        public Task<int?> ExecuteInsertAsync(string caseKey, Dictionary<string, object> valueParam)
        {
            throw new NotImplementedException();
        }


        public async Task<DataTable?> ExecuteSelectAsync(string caseKey, Dictionary<string, object> param)
        {
            string sql = caseKey switch
            {
                "SEARCH_USER_AUTHORITY" => @"

SELECT	*
  FROM	SIGN_RECORD
 WHERE	SIGN_ID = @SIGN_ID 
   AND	SIGN_CD = @SIGN_CD 
   AND  SIGN_DETAIL_USER_ID = @SIGN_DETAIL_USER_ID 

",
                "SEARCH_SIGN_INFO" => @"
SELECT	A.SIGN_DETAIL_NM
	,	B.USER_NAME			AS  SIGN_DETAIL_USER_NM
	,	A.SIGN_DETAIL_USER_ID
	,	A.SIGN_EMP_CD
	,	C.USER_NAME			AS  SIGN_EMP_NM
	,	A.SIGN_TIME
	,	A.SIGN_IMAGE
  FROM	SIGN_RECORD	A
					INNER JOIN USER_INFO	B
					ON B.USER_ID			= A.SIGN_DETAIL_USER_ID
					LEFT JOIN USER_INFO	C
					ON B.USER_ID			= A.SIGN_EMP_CD
 WHERE	SIGN_ID = @SIGN_ID 
   AND	SIGN_CD = @SIGN_CD 
",

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

                _ => string.Format("update SIGN_RECORD set {0} where {1}", setClause, whereClause)
            };

            return await DbHelper.ExecuteNonQueryAsync(sql, setParam, whereParam);
        }

    }
}
