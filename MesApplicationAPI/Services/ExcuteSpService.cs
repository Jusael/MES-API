using MesApplicationAPI.Dao;
using MesApplicationAPI.Dto;
using MesApplicationAPI.Helpers.cs;
using MesApplicationAPI.Interface;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MesApplicationAPI.Services
{
    public class ExcuteSpService : IBaseService
    {
        private readonly EsSpMappingDao _mappingDao;
        private readonly SpExecLogDao _execLogDao;


        public ExcuteSpService(EsSpMappingDao esSpMappingDao, SpExecLogDao spExecLogDao)
        {
            _mappingDao = esSpMappingDao;
            _execLogDao = spExecLogDao;
        }

        private enum resultStatus
        {
            Request = 0,
            Fail = 1,
            Success = 2,
        }

        public Task<DataTable?> SelectAsync(string kind, IBaseDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<int?> UpdateAsync(string kind, IBaseDto dto)
        {
            throw new NotImplementedException();
        }

        //NOTE:
        //1. 전자서명 이후 파생되는 로직이 실행된다.
        //2. 파생되는 로직은 SP로 호출한다.
        //3. 전자서명 SIGN_CD별로 SP를 저장하는 테이블을 조회한다.
        //4. 특정 SP를 조회한뒤 SP를 실행한다.
        //5. sp 명과 sp 매개변수를 log에 기록한다.
        //6. log에 cnt 필드를 두어 스케쥴러를 통해 최대 3번 실행하도록 한다.
        //7. cnt가 3까지오르면, 무조건 오류나는 경우로 판단하여 사용자에게 알람을 보낸다.

        public async Task ExcuteStoredProcedure(SignDto signDto)
        {
            //catch에 넘겨주기위해 try문위에 작성
            DataTable dtSpInfo = new DataTable();
            Dictionary<string, object> gdSpParamter = new Dictionary<string, object>();

            try
            {
                //1. 실행할 sp 를 MAPPING 테이블 통해 찾는다.
                //2. SP 매개변수를 정제한다.
                //2.1 동적으로 진행해보려다. 키매핑이 절대안됨을 깨닫고 포기한다.
                //3. SP 실행를 실행한다.
                //4. 결과에 따른 LOG를 기록한다.

                Dictionary<string, object> gdSpname = new Dictionary<string, object>();
                gdSpname.Add("SIGN_CD", signDto.SignCd);
                dtSpInfo = await _mappingDao.ExecuteSelectAsync(string.Empty, gdSpname);

                if (dtSpInfo == null)
                    throw new Exception("FIND SP INFO ERROR");

                if (dtSpInfo.Rows.Count == 0)
                    //전자서명 이후 프로세스가 없는 경우
                    return;

                gdSpParamter = createKey(signDto.SignCd, signDto);

                string spName = dtSpInfo.Rows[0]["SP_SCHEMA"].ToString() + "." + dtSpInfo.Rows[0]["SP_NAME"].ToString();

                int? result = await DbHelper.CallSp(spName, gdSpParamter);

                if (result == null || result < 0)
                    throw new Exception("SP실행중 오류가 발생하였습니다.");


                await writeLog(signDto, dtSpInfo, gdSpParamter, resultStatus.Success, string.Empty);
            }
            catch (Exception ex)
            {
                await writeLog(signDto, dtSpInfo, gdSpParamter, resultStatus.Fail, ex.Message);
                throw ex;
            }
        }

        private async Task writeLog(SignDto signDto
            , DataTable dtSpInfo
            , Dictionary<string, object> gdSpParamter
            , Enum Status, string message)

        {
            Dictionary<string, object> gdLogValue = new Dictionary<string, object>();

            string StrExecParms = BuildSqlAssignments(gdSpParamter);

            gdLogValue.Add("SIGN_ID", signDto.SignId);
            gdLogValue.Add("APP_ALARM_ID", signDto.AppAlarmId);
            gdLogValue.Add("MES_ALARM_ID", signDto.MesAlarmId);
            gdLogValue.Add("SIGN_CD", signDto.SignCd);
            gdLogValue.Add("SP_SCHEMA", dtSpInfo.Rows[0]["SP_SCHEMA"]);
            gdLogValue.Add("SP_NAME", dtSpInfo.Rows[0]["SP_NAME"]);
            gdLogValue.Add("EXEC_PARAMS", StrExecParms);
            gdLogValue.Add("STATUS", Status.ToString());
            gdLogValue.Add("ERROR_MSG", message);

            int? result = await _execLogDao.ExecuteInsertAsync(string.Empty, gdLogValue);
        }

        private Dictionary<string, object> createKey(string signCd, SignDto signDto)
        {
            try
            {
                Dictionary<string, object> gdParameter = new Dictionary<string, object>();

                switch (signCd)
                {
                    case "ES_WORK_ORDER":
                        gdParameter.Add("ORDER_NO", signDto.Key1);
                        break;

                    case "ES_PACKING_ORDER":
                        gdParameter.Add("PACKING_ORDER_NO", signDto.Key1);
                        break;
                }

                return gdParameter;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string BuildSqlAssignments(Dictionary<string, object> gdSpParameter)
        {
            string separator = ", ";
            if (gdSpParameter == null || gdSpParameter.Count == 0)
                return string.Empty;

            // 각 Key-Value 쌍을 "key = 'value'" 형태로 포맷
            var assignments = gdSpParameter.Select(kv =>
            {
                string columnName = kv.Key;
                object value = kv.Value ?? DBNull.Value;

                // 문자열 타입인 경우는 작은따옴표로 감싸준다.
                // (여기선 단순 예제이므로, 내부에 작은따옴표(')가 들어오는 경우 이스케이프 처리는 생략함)
                if (value is string s)
                {
                    return $"@{columnName} = '{s}'";
                }
                // DateTime은 ISO8601 문자열로 바꿔서 작은따옴표로 감싼다.
                else if (value is DateTime dt)
                {
                    return $"@{columnName} = '{dt:yyyy-MM-ddTHH:mm:ss}'";
                }
                else if (value is int || value is long || value is decimal || value is float || value is double)
                {
                    return $"@{columnName} = {value}";
                }
                // 그 외 (예: byte[], GUID 등) 필요에 따라 형식 처리
                else
                {
                    // 기본적으로 ToString() 결과를 작은따옴표로 감싼다
                    string raw = value.ToString();
                    return $"@{columnName} = '{raw}'";
                }
            });

            // separator(예: ", " 또는 " AND ")로 조인
            return string.Join(separator, assignments);
        }

        public async Task RetryFailedSpExecutionsAsync()
        {

            Dictionary<string, object> gd = new Dictionary<string, object>();
            gd.Add("STATUS", "Fail");

            DataTable dt = await _execLogDao.ExecuteSelectAsync("SEARCH_EXCUTE_FAIL_SP", gd);

            if (dt.Rows.Count == 0)
                return;

            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    //시도 횟수를 1늘려준다.
                    long logId = Convert.ToInt64(dr["LOG_ID"]);

                    long cnt = Convert.ToInt64(dr["CNT"]);

                    //3회까지만 설정하고, 안되면 사용자 메세지처리나 아웃룩처리하는 방향
                    if (cnt >= 3)
                        continue;

                    cnt = cnt + 1;

                    Dictionary<string, object> gdCntValue = new Dictionary<string, object>();
                    gdCntValue.Add("CNT", cnt);

                    Dictionary<string, object> gdLogIdKey = new Dictionary<string, object>();
                    gdLogIdKey.Add("LOG_ID", logId);

                    int? result = await _execLogDao.ExecuteUpdatetAsync(string.Empty, gdCntValue, gdLogIdKey);

                    //DBO.SP_ORDER @ORDER_NO = 'M20210204-03' 형식으로 문자열 붙힘
                    string sqlText = "EXEC " + dr["SP_SCHEMA"].ToString() + "." + dr["SP_NAME"].ToString() + Environment.NewLine + dr["EXEC_PARAMS"].ToString();

                    int? reTryResult = await DbHelper.excuteStrSql(sqlText);

                    if (reTryResult == 1)
                    {
                        Dictionary<string, object> gdScuessValue = new Dictionary<string, object>();
                        gdScuessValue.Add("STATUS", "ReTryComplete");
                        await _execLogDao.ExecuteUpdatetAsync(string.Empty, gdScuessValue, gdLogIdKey);
                    }
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"RetryFailedSpExecutionsAsync ERROR {sqlEx.Message}");
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SP 실행 오류] {ex.Message}");
                }
            }

        }
    }
}
