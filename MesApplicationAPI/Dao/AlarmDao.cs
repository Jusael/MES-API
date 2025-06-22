using MesApplicationAPI.Helpers.cs;
using MesApplicationAPI.Interface;
using System.Data;

namespace MesApplicationAPI.Dao
{
    public class AlarmDao : IBaseDao
    {

        public  Task<int?> ExecuteInsertAsync(string caseKey, Dictionary<string, object> valueParam)
        {
          throw new NotImplementedException();
        }

        public async Task<DataTable?> ExecuteSelectAsync(string caseKey, Dictionary<string, object> param)
        {
            string sql = caseKey switch
            {
                "SEARCH_NON_SEND_ALARM" => @"
SELECT APP_ALARM_ID
    ,   MES_ALARM_ID
	,	B.FCM_TOKEN
	,	A.USER_ID
	,	A.USER_NM
	,	A.TITLE
	,	A.CONTENT1
	,	A.CONTENT2
	,	A.CONTENT3
    ,	A.CONTENT4
    ,	A.CONTENT5
	,	SIGN_CD
	,	SIGN_ID
	,	KEY1
	,	KEY2
	,	KEY3
	,	KEY4
	,	KEY5
    ,   CREATE_TIME
  FROM PUSH_NOTIFICATION	A
							INNER JOIN USER_INFO	B
							ON	B.USER_ID			= A.USER_ID
 WHERE SEND_YN = @SEND_YN
ORDER BY A.APP_ALARM_ID DESC
",

                "GET_UNREAD_LIST" => @"
SELECT APP_ALARM_ID
    ,   MES_ALARM_ID
	,	B.FCM_TOKEN
	,	A.USER_ID
	,	A.USER_NM
	,	A.TITLE
	,	A.CONTENT1
	,	A.CONTENT2
	,	A.CONTENT3
    ,	A.CONTENT4
    ,	A.CONTENT5
	,	SIGN_CD
	,	SIGN_ID
	,	KEY1
	,	KEY2
	,	KEY3
	,	KEY4
	,	KEY5
    ,   CREATE_TIME
  FROM PUSH_NOTIFICATION	A
							INNER JOIN USER_INFO	B
							ON	B.USER_ID			= A.USER_ID
 WHERE SEND_YN = @SEND_YN
   AND A.USER_ID  = @USER_ID
   AND ISNULL(A.ALARM_STATUS, '') = ''
",



                               _ => "SELECT * FROM PUSH_NOTIFICATION WHERE User_Id = @USER_ID"
            };

            return await DbHelper.ExecuteScalarAsync(sql, param); ;
        }

        public async Task<int?> ExecuteUpdatetAsync(string caseKey, Dictionary<string, object> setParam, Dictionary<string, object> whereParam)
        {
            string setClause = string.Join(", ", setParam.Keys.Select(k => $"{k} = @{k}"));
            string whereClause = string.Join(" AND ", whereParam.Keys.Select(k => $"{k} = @w_{k}"));


            string sql = caseKey switch
            {

                _ => string.Format("update PUSH_NOTIFICATION set {0} where {1}", setClause, whereClause)
            };

            return await DbHelper.ExecuteNonQueryAsync(sql, setParam, whereParam);
        }


    }

}
