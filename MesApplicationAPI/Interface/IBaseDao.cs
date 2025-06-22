using System.Data;

namespace MesApplicationAPI.Interface
{
    public interface IBaseDao
    {
        public Task<DataTable?> ExecuteSelectAsync(string caseKey, Dictionary<string, object> param);

        public Task<int?> ExecuteInsertAsync(string caseKey, Dictionary<string, object> valueParam);

        public Task<int?> ExecuteUpdatetAsync(string caseKey, Dictionary<string, object> setParam, Dictionary<string, object> whereParam);


    }
}
