using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCW.Entities.Concrete;

namespace LCW.DataAccess.Abstract
{
    public interface IStoreScheduleANDal
    {
        Task AddBulk(List<DailyStoreSchedule> dailyStoreSchedules);
    }
}
