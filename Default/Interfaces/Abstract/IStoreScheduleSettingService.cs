using LCW.Core.Utilities.Results;
using LCW.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Interfaces.Abstract
{
    public interface IStoreScheduleSettingService
    {
        Task<IResult> RunStartDateProcess();

        Task<IResult> RunEndDateProcess();

        Task<SuccessResult> Update(StoreScheduleSetting storeScheduleSetting);

        Task<IResult> RunDailyStoreProcess();
    }
}
