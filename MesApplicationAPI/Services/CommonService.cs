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
    public class CommonService : IBaseService
    {

        //클래스끼리 결합도가 낮아져서 수정이 쉽다고 한다...
        // 해당 정보를 쓸려면 programs에 추가해줘야함
        // builder.Services.AddScoped<UserService>();, builder.Services.AddScoped<UserDao>();
        private readonly CommonLocationDao _commonDao;

        public CommonService(CommonLocationDao commonDao)
        {
            _commonDao = commonDao;
        }

        #region - SELECT -


        public async Task<DataTable?> SelectAsync(string kind, IBaseDto dto)
        {
            //dto를 받아서 그대로 사용할경우
            var param = Utils.Uils.DtoToDictionary(dto);

            switch (kind)
            {
                case "SEARCH_LOCATION_BARCODE_INFO":
                    {
                        if (dto is BarcodeDto barcodeDto)
                        {
                            param.Clear();
                            param["CELL_CD"] = barcodeDto.BarCode;
                        }
                        break;
                    }
            }

            return await _commonDao.ExecuteSelectAsync(kind, param);
        }

        private async Task<DataTable?> SelectAsync(string kind, Dictionary<string, object> selectKey)
        {
            return await _commonDao.ExecuteSelectAsync(kind, selectKey);
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

            return await _commonDao.ExecuteUpdatetAsync(kind, setParam, whereParam);
        }

        private async Task<DataTable?> UpdateAsync(string kind, Dictionary<string, object> selectValue, Dictionary<string, object> selectKey)
        {
            return await _commonDao.ExecuteSelectAsync(kind, selectKey);
        }
        #endregion


    }
}