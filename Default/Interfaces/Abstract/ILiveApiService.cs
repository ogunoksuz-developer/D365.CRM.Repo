using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Interfaces.Abstract
{
    public interface ILiveApiService
    {
        Task ClearCache(string cacheKey);
    }
}
