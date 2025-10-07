using LCW.Core.DataAccess;
using LCW.Core.Utilities.Results;
using LCW.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCW.DataAccess.Abstract
{
    public interface IStoreDal : ICrmEntityRepository<Store>
    {
        Task<IList<Store>> GetStoreListByCode(List<string> codes);

        Task<IList<Store>> GetStoreList();
    }
}
