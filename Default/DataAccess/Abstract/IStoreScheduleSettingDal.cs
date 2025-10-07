using LCW.Core.DataAccess;
using LCW.Core.Utilities.Results;
using LCW.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LCW.Core.Enums.Enumarations;

namespace LCW.DataAccess.Abstract
{
    public interface IStoreScheduleSettingDal : ICrmEntityRepository<StoreScheduleSetting>
    {
        Task<IList<StoreScheduleSetting>> StoreScheduleSettingList(DateTime? startDate, DateTime? endDate, int? statusCode = 1);

    }
}
