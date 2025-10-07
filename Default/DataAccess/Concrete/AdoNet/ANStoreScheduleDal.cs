using LCW.Core.DataAccess.AdoNet;
using LCW.DataAccess.Abstract;
using LCW.Entities.Concrete;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace LCW.DataAccess.Concrete.AdoNet
{
    public class ANStoreScheduleDal : AnRepositoryBase<DailyStoreSchedule>, IStoreScheduleANDal
    {
        public ANStoreScheduleDal() : base(Core.AppSettings.SettingFactory.GetCRMConnectionString)
        {

        }


        public async Task AddBulk(List<DailyStoreSchedule> dailyStoreSchedules)
        {
            var mappings = new List<SqlBulkCopyColumnMapping>();
            mappings.Add(new SqlBulkCopyColumnMapping(nameof(DailyStoreSchedule.StoreId), "StoreId"));
            mappings.Add(new SqlBulkCopyColumnMapping(nameof(DailyStoreSchedule.StoreCode), "StoreCode"));
            mappings.Add(new SqlBulkCopyColumnMapping(nameof(DailyStoreSchedule.ScheduleDate), "ScheduleDate"));
            mappings.Add(new SqlBulkCopyColumnMapping(nameof(DailyStoreSchedule.OpeningTime), "OpeningTime"));
            mappings.Add(new SqlBulkCopyColumnMapping(nameof(DailyStoreSchedule.ClosingTime), "ClosingTime"));
            mappings.Add(new SqlBulkCopyColumnMapping(nameof(DailyStoreSchedule.IsClosed), "IsClosed"));
          

            await BulkCopyInsert("dbo.StoreSchedule", mappings, dailyStoreSchedules);

        }
    }
}
