using MesApplicationAPI.Dao;
using MesApplicationAPI.Dto;
using MesApplicationAPI.Interface;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace MesApplicationAPI.Services
{
    public class ReceiptService : IBaseService
    {

        //클래스끼리 결합도가 낮아져서 수정이 쉽다고 한다...
        // 해당 정보를 쓸려면 programs에 추가해줘야함
        // builder.Services.AddScoped<UserService>();, builder.Services.AddScoped<UserDao>();
        private readonly ReceiptDao _receiptDao;
        private readonly ReceiptPackDao _receiptpackDao;
        private readonly CommonService _commonService;

        public ReceiptService(ReceiptDao receiptDao, ReceiptPackDao receiptPackDao, CommonService commonService)
        {
            _receiptDao = receiptDao;
            _receiptpackDao = receiptPackDao;
            _commonService = commonService;
        }

        #region - SELECT -

        public async Task<DataTable?> SelectAsync(string kind, IBaseDto dto)
        {
            //dto를 받아서 그대로 사용할경우
            var param = Utils.Uils.DtoToDictionary(dto);

            switch (kind)
            {
                case "SEARCH_USER_AUTHORITY":
                    {
                        if (dto is SignDto signDto)
                        {
                            param.Clear();
                            param["SIGN_DETAIL_USER_ID"] = signDto.UserId;
                            param["SIGN_CD"] = signDto.SignCd;
                            param["SIGN_ID"] = Convert.ToInt64(signDto.SignId);
                        }
                        break;
                    }

                case "SEARCH_SIGN_INFO":
                    {
                        if (dto is SignDto signDto)
                        {
                            param.Clear();
                            param["SIGN_CD"] = signDto.SignCd;
                            param["SIGN_ID"] = Convert.ToInt64(signDto.SignId);
                        }
                        break;
                    }
            }

            return await _receiptDao.ExecuteSelectAsync(kind, param);
        }

        private async Task<DataTable?> SelectAsync(string kind, Dictionary<string, object> selectKey)
        {
            return await _receiptDao.ExecuteSelectAsync(kind, selectKey);
        }

        #endregion

        #region - UPDATE -

        public async Task<int?> UpdateAsync(string kind, IBaseDto dto)
        {
            var setParam = new Dictionary<string, object>();
            var whereParam = new Dictionary<string, object>();

            switch (kind)
            {
              

            }

            return await _receiptDao.ExecuteUpdatetAsync(kind, setParam, whereParam);
        }

        private async Task<DataTable?> UpdateAsync(string kind, Dictionary<string, object> selectValue, Dictionary<string, object> selectKey)
        {
            return await _receiptDao.ExecuteSelectAsync(kind, selectKey);
        }
        #endregion


    }
}