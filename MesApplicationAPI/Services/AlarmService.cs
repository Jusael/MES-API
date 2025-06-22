namespace MesApplicationAPI.Services
{
    using System.Data;
    using global::MesApplicationAPI.Dto;
    using global::MesApplicationAPI.Interface;
    using MesApplicationAPI.Dao;


    public class AlarmService : IAlarmService
    {
        //반환 데이터가 5~6컬럼 이상, 또는 테이블처럼 반복 되면 dto로 변환이 유리하다.
        private readonly AlarmDao _alarmDao;

        public AlarmService(AlarmDao alarmDao)
        {
            _alarmDao = alarmDao;
        }

        // 전송되지 않은 알람 목록 조회
        public async Task<List<AlarmDto>> Select()
        {
            try
            {
                var whereParam = new Dictionary<string, object>();
                whereParam.Add("SEND_YN", "N");

                Task<DataTable?> dtAlarm = _alarmDao.ExecuteSelectAsync("SEARCH_NON_SEND_ALARM", whereParam);

                return await this.ConvertToAlarmDtoList(dtAlarm);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<DataTable?> SelectAsync(string kind, IBaseDto dto)
        {
            //dto를 받아서 그대로 사용할경우
            var param = Utils.Uils.DtoToDictionary(dto);

            switch (kind)
            {
                case "SEARCH_USER_AUTHORITY":
                    {
                        if (dto is AlarmDto alarmDto)
                        {
                            param.Clear();
                            param["SIGN_DETAIL_USER_ID"] = alarmDto.UserId;
                            param["SIGN_CD"] = alarmDto.SignCd;
                            param["SIGN_ID"] = Convert.ToInt64(alarmDto.SignId);
                        }
                        break;
                    }
            }

            return await _alarmDao.ExecuteSelectAsync(kind, param);
        }


        public async Task<List<AlarmDto>> SelectDtoAsync(string kind, IBaseDto dto)
        {
            //dto를 받아서 그대로 사용할경우
            var param = Utils.Uils.DtoToDictionary(dto);

            switch (kind)
            {
              
                case "GET_UNREAD_LIST":
                    {
                        if (dto is AlarmControlDto alarmControlDto)
                        {
                            param.Clear();
                            param["USER_ID"] = alarmControlDto.UserId;
                            param["SEND_YN"] = "Y";
                            ;

                        }
                        break;
                    }

            }

            Task<DataTable?> dtAlarm = _alarmDao.ExecuteSelectAsync(kind, param);

            return await this.ConvertToAlarmDtoList(dtAlarm);
        }

        //? 변수는 task형식으로 변환해줘야한다.
        private async Task<List<AlarmDto>> ConvertToAlarmDtoList(Task<DataTable?> task)
        {
            try
            {

                var list = new List<AlarmDto>();

                DataTable dt = await task;

                foreach (DataRow row in dt.Rows)
                {
                    var dto = new AlarmDto
                    {
                        AppAlarmId = row["APP_ALARM_ID"] != DBNull.Value ? Convert.ToInt32(row["APP_ALARM_ID"]) : 0,
                        FcmToken = row["FCM_TOKEN"]?.ToString() ?? string.Empty,
                        UserId = row["USER_ID"]?.ToString() ?? string.Empty,
                        UserNm = row["USER_NM"]?.ToString() ?? string.Empty,
                        Title = row["TITLE"]?.ToString() ?? string.Empty,
                        Content1 = row["CONTENT1"]?.ToString() ?? string.Empty,
                        Content2 = row["CONTENT2"]?.ToString() ?? string.Empty,
                        Content3 = row["CONTENT3"]?.ToString() ?? string.Empty,
                        Content4 = row["CONTENT4"]?.ToString() ?? string.Empty,
                        Content5 = row["CONTENT5"]?.ToString() ?? string.Empty,
                        SignCd = row["SIGN_CD"]?.ToString() ?? string.Empty,
                        SignId = row["SIGN_ID"]?.ToString() ?? string.Empty,
                        Key1 = row["KEY1"]?.ToString() ?? string.Empty,
                        Key2 = row["KEY2"]?.ToString() ?? string.Empty,
                        Key3 = row["KEY3"]?.ToString() ?? string.Empty,
                        Key4 = row["KEY4"]?.ToString() ?? string.Empty,
                        Key5 = row["KEY5"]?.ToString() ?? string.Empty,
                        CreateTime = row["CREATE_TIME"]?.ToString() ?? string.Empty
                    };

                    list.Add(dto);
                }

                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ConvertToAlarmDtoList ERROR " + ex.Message);
                return null;
            }

        }


        public async Task<int?> UpdateAsync(string kind, IBaseDto dto)
        {
            var setParam = new Dictionary<string, object>();
            var whereParam = new Dictionary<string, object>();

            switch (kind)
            {
                case "UPDATE_SEND_YN":
                    { 
                    if (dto is AlarmDto alarmDto)
                    {
                        
                        setParam.Add("SEND_YN", "Y");
                        setParam.Add("SEND_TIME", DateTime.Now);

                        whereParam.Add("APP_ALARM_ID",  alarmDto.AppAlarmId);
                    }
                    break;
                    }
                case "UPDATE_ALARM_STATUS":
                    {
                        if (dto is AlarmControlDto alarmControlDto)
                        {
                            setParam.Add("ALARM_STATUS", alarmControlDto.AlarmStatus);
                            whereParam.Add("APP_ALARM_ID", Int64.Parse(alarmControlDto.AppAlarmId));
                        }
                        break;
                    }
            }

            return await _alarmDao.ExecuteUpdatetAsync(kind, setParam, whereParam);
        }
    }

}
