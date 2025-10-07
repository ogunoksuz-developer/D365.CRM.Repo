using LCW.Core.Utilities.Results;
using LCW.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Interfaces.Abstract
{
    public interface IStoreService
    {
        Task<IDataResult<IList<Store>>> GetStoreListByCode(List<string> codes);

        Task<IDataResult<IList<Store>>> GetStoreList();

        Task<SuccessResult> Update(Store store);
    }
}
