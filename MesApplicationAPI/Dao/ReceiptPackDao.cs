using MesApplicationAPI.Helpers.cs;
using MesApplicationAPI.Interface;
using System.Data;

namespace MesApplicationAPI.Dao
{
    public class ReceiptPackDao : IBaseDao
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

                "SEARCH_BARCODE_INFO" => @"
SELECT	C.ITEM_NM
	,	C.ITEM_CD
	,	B.RECEIPT_LOT_NO
	,	B.RECEIPT_VALID_DATE
	,	A.RECEIPT_PACK_QTY
	,	A.RECEIPT_PACK_REMAIN_QTY
	,	CASE WHEN  B.RECEIPT_STATUS	= 'RECEIPT' THEN '입고'
			 WHEN  B.RECEIPT_STATUS = 'TEST_REQUEST' THEN '시험중'
			 END   AS RECEIPT_STATUS
  FROM	RECEIPT_PACK			A
								INNER JOIN RECEIPT		B
								ON	B.RECEIPT_NO		= A.RECEIPT_NO
								AND B.RECEIPT_SEQ		= A.RECEIPT_SEQ
								INNER JOIN MATERIAL_INFO C
								ON C.ITEM_CD			= B.ITEM_CD
 WHERE	RECEIPT_PACK_BARCODE_NO = @RECEIPT_PACK_BARCODE_NO ",

                "SEARCH_INVENTORY_LIST" => @"
 SELECT	C.ITEM_NM
	,	C.ITEM_CD	
	,	B.RECEIPT_LOT_NO
	,	a.RECEIPT_PACK_BARCODE_NO
	
	,	A.RECEIPT_PACK_QTY
	,	A.RECEIPT_PACK_REMAIN_QTY
  FROM	RECEIPT_PACK			A
								INNER JOIN RECEIPT		B
								ON	B.RECEIPT_NO		= A.RECEIPT_NO
								AND B.RECEIPT_SEQ		= A.RECEIPT_SEQ
								INNER JOIN MATERIAL_INFO C
								ON C.ITEM_CD			= B.ITEM_CD
 WHERE	CELL_CD = @CELL_CD
",

                _ => string.Format("SELECT * FROM RECEIPT_PACK WHERE {0}", whereClause)
            };

            return await DbHelper.ExecuteScalarAsync(sql, whereParam); ;
        }

        public async Task<int?> ExecuteUpdatetAsync(string caseKey, Dictionary<string, object> setParam, Dictionary<string, object> whereParam)
        {
            string setClause = string.Join(", ", setParam.Keys.Select(k => $"{k} = @{k}"));
            string whereClause = string.Join(" AND ", whereParam.Keys.Select(k => $"{k} = @w_{k}"));


            string sql = caseKey switch
            {

                _ => string.Format("update RECEIPT_PACK set {0} where {1}", setClause, whereClause)
            };

            return await DbHelper.ExecuteNonQueryAsync(sql, setParam, whereParam);
        }


    }

}
