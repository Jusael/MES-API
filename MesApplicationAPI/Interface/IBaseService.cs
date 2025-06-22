using System.Data;
using MesApplicationAPI.Dto;
using MesApplicationAPI.Interface;

namespace MesApplicationAPI.Interface
{
    public interface IBaseService 
    {
        public Task<DataTable?> SelectAsync(string kind, IBaseDto dto);


        public Task<int?> UpdateAsync(string kind, IBaseDto dto);

    }
}
