using MesApplicationAPI.Dao;
using MesApplicationAPI.Dto;
using MesApplicationAPI.Interface;
using System.Data;

namespace MesApplicationAPI.Services
{
    public class UserService : IBaseService
    {

        //클래스끼리 결합도가 낮아져서 수정이 쉽다고 한다...
        // 해당 정보를 쓸려면 programs에 추가해줘야함
        // builder.Services.AddScoped<UserService>();, builder.Services.AddScoped<UserDao>();
        private readonly UserDao _userDao;

        public UserService(UserDao userDao)
        {
            _userDao = userDao;
        }

        #region - SELECT -


        public async Task<DataTable?> SelectAsync(string kind, IBaseDto dto)
        {
            //dto를 받아서 그대로 사용할경우
            var param = Utils.Uils.DtoToDictionary(dto);

            switch (kind)
            {
                case "SEARCH_USER_INFO":
                    if (dto is LoginDto SearchUserDto)
                    {
                        param.Clear();
                        param["USER_ID"] = SearchUserDto.UserId;
                        param["USER_PASSWORD"] = SearchUserDto.Password;
                        param["NOW_DATE"] = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                    break;

                case "SEARCH_EXPIRE_DAYS":
                    if (dto is JwtDto SearchExpireDto)
                    {
                        param.Clear();
                        param["USER_ID"] = SearchExpireDto.UserId;
                    }
                    break;
            }

            return await _userDao.ExecuteSelectAsync(kind, param);
        }

        #endregion

        #region - UPDATE -

        public async Task<int?> UpdateAsync(string kind, IBaseDto dto)
        {
            var setParam = new Dictionary<string, object>();
            var whereParam = new Dictionary<string, object>();

            switch (kind)
            {
                case "UPDATE_FCM_TOKEN":
                    if (dto is FcmDto setFcmDto)
                    {
                        setParam.Add("FCM_TOKEN", setFcmDto.FcmToken);
                        whereParam.Add("USER_ID", setFcmDto.UserId);
                    }
                    break;

                case "UPDATE_JWT_TOKEN":
                    if (dto is JwtDto getJwtDto)
                    {
                        setParam.Add("JWT_TOKEN", getJwtDto.JwtToken);
                        whereParam.Add("USER_ID", getJwtDto.UserId);
                    }
                    break;
            }

            return await _userDao.ExecuteUpdatetAsync(kind, setParam, whereParam);
        }
        #endregion


    }
}