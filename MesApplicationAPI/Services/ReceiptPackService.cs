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
    public class ReceiptPackService : IBaseService
    {

        //클래스끼리 결합도가 낮아져서 수정이 쉽다고 한다...
        // 해당 정보를 쓸려면 programs에 추가해줘야함
        // builder.Services.AddScoped<UserService>();, builder.Services.AddScoped<UserDao>();
        private readonly ReceiptPackDao _receiptPackDao;

        public ReceiptPackService(ReceiptPackDao receiptPackDao)
        {
            _receiptPackDao = receiptPackDao;
        }

        #region - SELECT -

        public async Task<DataTable?> SelectAsync(string kind, IBaseDto dto)
        {
            //dto를 받아서 그대로 사용할경우
            var param = Utils.Uils.DtoToDictionary(dto);

            switch (kind)
            {
                case "SEARCH_BARCODE_INFO":
                    {
                        if (dto is BarcodeDto barcodeDto)
                        {
                            param.Clear();
                            param["RECEIPT_PACK_BARCODE_NO"] = barcodeDto.BarCode;
                        }
                        break;
                    }

            }

            return await _receiptPackDao.ExecuteSelectAsync(kind, param);
        }

        private async Task<DataTable?> SelectAsync(string kind, Dictionary<string, object> selectKey)
        {
            return await _receiptPackDao.ExecuteSelectAsync(kind, selectKey);
        }



        //NOTE : 
        // 값을 ADTO로 받고 반환을 BDTO로 할경우에 사용한다.
        // 조회대상과 반환대상은 인터페이스로 무조건 등록해야한다.
        public async Task<List<TOut>> SelectAsyncDto<TIn, TOut>(string kind, TIn inputDto)
    where TIn : IBaseDto
    where TOut : IBaseDto, new()
        {
            var param = Utils.Uils.DtoToDictionary(inputDto);

            switch (kind)
            {
                case "SEARCH_INVENTORY_LIST":
                    if (inputDto is BarcodeDto barcodeDto)
                    {
                        param.Clear();
                        param["CELL_CD"] = barcodeDto.BarCode;
                    }
                    break;
            }

            DataTable dt = await _receiptPackDao.ExecuteSelectAsync(kind, param);

            return ConvertToDtoList<TOut>(dt);
        }

        #endregion

        #region - UPDATE -

        public async Task<int?> UpdateAsync(string kind, IBaseDto dto)
        {
            var setParam = new Dictionary<string, object>();
            var whereParam = new Dictionary<string, object>();

            switch (kind)
            {
                case "UPDATE_BARCODE_INFO":
                    {
                        if (dto is PutAwayBarcodeDto putAwayBarcodeDto)
                        {
                            setParam.Add("CELL_CD", putAwayBarcodeDto.Location);
                            setParam.Add("UPDATE_TIME", DateTime.Now);

                            whereParam.Add("RECEIPT_PACK_BARCODE_NO", putAwayBarcodeDto.BarCode);
                        }
                    }
                    break;

                case "UPDATE_PICKING_BARCODE":
                    {
                        if (dto is BarcodeDto barcodeDto)
                        {
                            setParam.Add("CELL_CD", string.Empty);
                            setParam.Add("UPDATE_TIME", DateTime.Now);

                            whereParam.Add("RECEIPT_PACK_BARCODE_NO", barcodeDto.BarCode);
                        }
                    }
                    break;
            }


            return await _receiptPackDao.ExecuteUpdatetAsync(kind, setParam, whereParam);
        }

        private async Task<DataTable?> UpdateAsync(string kind, Dictionary<string, object> selectValue, Dictionary<string, object> selectKey)
        {
            return await _receiptPackDao.ExecuteSelectAsync(kind, selectKey);
        }
        #endregion


        public List<T> ConvertToDtoList<T>(DataTable dt) where T : IBaseDto
        {
            try
            {

                var result = new List<T>();

                foreach (DataRow row in dt.Rows)
                {
                    object? dto = null;

                    switch (typeof(T).Name)
                    {
                        case nameof(InventoryListDto):
                            dto = new InventoryListDto
                            {
                                ItemCd = row["ITEM_CD"]?.ToString() ?? "",
                                ItemNm = row["ITEM_NM"]?.ToString() ?? "",
                                LotNo = row["RECEIPT_LOT_NO"]?.ToString() ?? "",
                                PackBarCode = row["RECEIPT_PACK_BARCODE_NO"]?.ToString() ?? "",
                                ReceiptQty = row["RECEIPT_PACK_QTY"] == DBNull.Value ? "0" : row["RECEIPT_PACK_QTY"].ToString(),
                                RemainQty = row["RECEIPT_PACK_REMAIN_QTY"] == DBNull.Value ? "0" : row["RECEIPT_PACK_REMAIN_QTY"].ToString()
                            };
                            break;

                        default:
                            throw new InvalidOperationException($"매핑되지 않은 DTO 타입: {typeof(T).Name}");
                    }

                    result.Add((T)dto!);
                }

                return result;

            }
            catch (Exception ex)
            {
                Console.WriteLine("ConvertToDtoList" + ex.Message);
                throw ex;
            }
        }


    }
}