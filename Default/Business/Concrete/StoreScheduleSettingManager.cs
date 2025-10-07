using LCW.Core.Common;
using LCW.Core.Utilities.Results;
using LCW.DataAccess.Abstract;
using LCW.Entities.Concrete;
using LCW.Interfaces.Abstract;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static LCW.Core.Enums.Enumarations;

namespace LCW.Business.Concrete
{
    public class StoreScheduleSettingManager : IStoreScheduleSettingService
    {
        private readonly IStoreScheduleSettingDal _storeScheduleSettingDal;
        private readonly IStoreScheduleANDal _storeScheduleANDal;
        private readonly IStoreService _storeService;
        private readonly ILiveApiService _liveApiService;
        private readonly string[] _cacheKeys;

        public StoreScheduleSettingManager(IConfiguration configuration,
            IStoreScheduleSettingDal storeScheduleSettingDal,
            IStoreService storeService,
            ILiveApiService liveApiService,
            IStoreScheduleANDal storeScheduleANDal)
        {
            _storeScheduleSettingDal = storeScheduleSettingDal;
            _storeService = storeService;
            _liveApiService = liveApiService;
            _cacheKeys = configuration.GetSection("CacheKeys:StoreKeys").Get<string[]>();
            _storeScheduleANDal = storeScheduleANDal;
        }

        /// <summary>
        /// Processes store schedule settings, updating store information and marking schedules as active or in progress.
        /// </summary>
        /// <returns>Result of the start date process.</returns>

        public async Task<IResult> RunStartDateProcess()
        {
            #region Get Store Schedule Setting List
            var storeScheduleSettingList = await _storeScheduleSettingDal.StoreScheduleSettingList(startDate: DateTime.Now, endDate: null, statusCode: (int)StoreScheduleSettingStatus.Active);
            #endregion

            if (!storeScheduleSettingList.Any())
                return new SuccessResult();

            var storeCodeList = storeScheduleSettingList.Select(x => x.StoreCode).Distinct().ToList();

            if (!storeCodeList.Any())
                return new SuccessResult();

            #region Get Store List
            var storeList = await _storeService.GetStoreListByCode(storeCodeList);
            #endregion

            if (!storeList.Success || !storeList.Data.Any()) return new SuccessResult();

            foreach (var item in storeList.Data)
            {
                var storeScheduleSetting = storeScheduleSettingList.FirstOrDefault(x => x.StoreCode.ToLowerInvariant() == item.Code.ToLowerInvariant());

                if (storeScheduleSetting == null)
                    continue;

                var isUpdated = false;

                #region Backup Store Data
                var backUpStore = JsonConvert.SerializeObject(item);
                #endregion

                #region Update Store Schedule Setting Dates
                storeScheduleSetting.StartDate = storeScheduleSetting.StartDate.AddHours(3);
                storeScheduleSetting.EndDate = storeScheduleSetting.EndDate.AddHours(3);
                #endregion

                switch (storeScheduleSetting.Type)
                {
                    case (int)StoreScheduleType.ClosedStore:
                        isUpdated = UpdateClosedStore(item, storeScheduleSetting);
                        break;
                    case (int)StoreScheduleType.Temporary:
                        isUpdated = HandleTemporaryStoreSchedule(item, storeScheduleSetting, backUpStore);
                        break;
                    case (int)StoreScheduleType.MainDay:
                        isUpdated = HandleMainDayStoreSchedule(item, storeScheduleSetting);
                        break;
                }

                storeScheduleSetting.Store = item.Id;

                if (isUpdated)
                    await _storeService.Update(item);

                await Update(storeScheduleSetting);
            }

            await ClearCacheKeys();

            return new SuccessResult();

        }

        /// <summary>
        /// Processes store schedule settings, updating store information and marking schedules as completed.
        /// </summary>
        /// <returns>Result of the end date process.</returns>
        public async Task<IResult> RunEndDateProcess()
        {

            #region Get Store Schedule Setting List
            var storeScheduleSettingList = await _storeScheduleSettingDal.StoreScheduleSettingList(startDate: null, endDate: DateTime.Now, statusCode: (int)StoreScheduleSettingStatus.InProgress);
            #endregion

            if (!storeScheduleSettingList.Any())
                return new SuccessResult();

            var storeCodeList = storeScheduleSettingList.Select(x => x.StoreCode).Distinct().ToList();

            if (!storeCodeList.Any())
                return new SuccessResult();

            #region Get Store List
            var storeList = await _storeService.GetStoreListByCode(storeCodeList);
            #endregion

            if (!storeList.Success || !storeList.Data.Any()) return new SuccessResult();

            foreach (var item in storeList.Data)
            {
                var storeScheduleSetting = storeScheduleSettingList.FirstOrDefault(x => x.StoreCode.ToLowerInvariant() == item.Code.ToLowerInvariant());

                if (storeScheduleSetting == null)
                    continue;

                #region Update Store Schedule Setting Dates
                storeScheduleSetting.StartDate = storeScheduleSetting.StartDate.AddHours(3);
                storeScheduleSetting.EndDate = storeScheduleSetting.EndDate.AddHours(3);
                #endregion

                switch (storeScheduleSetting.Type)
                {
                    case (int)StoreScheduleType.ClosedStore:
                        UpdateClosedStoreForEnd(item);
                        break;
                    case (int)StoreScheduleType.Temporary:
                        HandleTemporaryStoreScheduleForEnd(item, storeScheduleSetting);
                        break;
                }

                #region Finalize Store Schedule Setting
                storeScheduleSetting.StatusCode = (int)StoreScheduleSettingStatus.Completed;
                storeScheduleSetting.StateCode = (int)StateCode.InActive;
                #endregion

                await _storeService.Update(item);
                await Update(storeScheduleSetting);
            }

            await ClearCacheKeys();


            return new SuccessResult();

        }

        /// <summary>
        /// Updates the given store schedule setting in the database.
        /// </summary>
        /// <param name="storeScheduleSetting">The store schedule setting to update.</param>
        /// <returns>Result indicating the success of the update operation.</returns>
        public async Task<SuccessResult> Update(StoreScheduleSetting storeScheduleSetting)
        {
            await _storeScheduleSettingDal.Update(storeScheduleSetting);
            return new SuccessResult();
        }

        /// <summary>
        /// Processes store schedule settings, updating store information and marking schedules as active or in progress.
        /// </summary>
        /// <returns>Result of the start date process.</returns>

        public async Task<IResult> RunDailyStoreProcess()
        {
            try
            {
                #region Get Store List
                var storeList = await _storeService.GetStoreList();
                #endregion

                if (!storeList.Success || !storeList.Data.Any()) return new SuccessResult();

                var storeScheduleList = new List<DailyStoreSchedule>();

                foreach (var item in storeList.Data)
                {

                    TimeSpan ParseOrStore(string dayValue, string storeValue)
                    {
                        var valueToParse = !string.IsNullOrWhiteSpace(dayValue) ? dayValue : storeValue;
                        // Adjust the format string as needed, e.g., "c" for invariant culture TimeSpan ("hh:mm:ss")
                        return TimeSpan.TryParseExact(valueToParse, "c", CultureInfo.InvariantCulture, out var ts) ? ts : TimeSpan.Zero;
                    }


                    var today = DateTime.Today.DayOfWeek;
                    TimeSpan opening, closing;

                    switch (today)
                    {
                        case DayOfWeek.Monday:
                            opening = ParseOrStore(item.MondayOpeningTime, item.StoreOpeningTime);
                            closing = ParseOrStore(item.MondayClosingTime, item.StoreClosingTime);
                            break;
                        case DayOfWeek.Tuesday:
                            opening = ParseOrStore(item.TuesdayOpeningTime, item.StoreOpeningTime);
                            closing = ParseOrStore(item.TuesdayClosingTime, item.StoreClosingTime);
                            break;
                        case DayOfWeek.Wednesday:
                            opening = ParseOrStore(item.WednesdayOpeningTime, item.StoreOpeningTime);
                            closing = ParseOrStore(item.WednesdayClosingTime, item.StoreClosingTime);
                            break;
                        case DayOfWeek.Thursday:
                            opening = ParseOrStore(item.ThursdayOpeningTime, item.StoreOpeningTime);
                            closing = ParseOrStore(item.ThursdayClosingTime, item.StoreClosingTime);
                            break;
                        case DayOfWeek.Friday:
                            opening = ParseOrStore(item.FridayOpeningTime, item.StoreOpeningTime);
                            closing = ParseOrStore(item.FridayClosingTime, item.StoreClosingTime);
                            break;
                        case DayOfWeek.Saturday:
                            opening = ParseOrStore(item.SaturdayOpeningTime, item.StoreOpeningTime);
                            closing = ParseOrStore(item.SaturdayClosingTime, item.StoreClosingTime);
                            break;
                        case DayOfWeek.Sunday:
                            opening = ParseOrStore(item.SundayOpeningTime, item.StoreOpeningTime);
                            closing = ParseOrStore(item.SundayClosingTime, item.StoreClosingTime);
                            break;
                        default:
                            opening = TimeSpan.Zero;
                            closing = TimeSpan.Zero;
                            break;
                    }

                    var storeSchedule = new DailyStoreSchedule
                    {
                        StoreId = item.Id,
                        StoreCode = item.Code,
                        ScheduleDate = DateTime.Now,
                        OpeningTime = opening,
                        ClosingTime = closing,
                        IsClosed = GetIsClosed(item, today)
                    };


                    storeScheduleList.Add(storeSchedule);

                }

                await _storeScheduleANDal.AddBulk(storeScheduleList);

                return new SuccessResult();
            }
            catch (Exception ex)
            {
                return new ErrorResult(message: ex.Message);
            }

        }


        #region utility methods

        public static bool GetIsClosed(Store item, DayOfWeek day)
        {
            if (item.ClosedStore)
                return true;

            return day switch
            {
                DayOfWeek.Monday => item.IsClosedMonday,
                DayOfWeek.Tuesday => item.IsClosedTuesday,
                DayOfWeek.Wednesday => item.IsClosedWednesday,
                DayOfWeek.Thursday => item.IsClosedThursday,
                DayOfWeek.Friday => item.IsClosedFriday,
                DayOfWeek.Saturday => item.IsClosedSaturday,
                DayOfWeek.Sunday => item.IsClosedSunday,
                _ => false
            };
        }

        private async Task ClearCacheKeys()
        {
            foreach (var key in _cacheKeys)
            {
                await _liveApiService.ClearCache(key);
            }
        }

        private static bool UpdateClosedStore(Store item, StoreScheduleSetting storeScheduleSetting)
        {
            item.ClosedStore = true;
            storeScheduleSetting.StatusCode = (int)StoreScheduleSettingStatus.InProgress;
            return true;
        }

        private static void UpdateClosedStoreForEnd(Store item)
        {
            item.ClosedStore = false;
        }

        private static bool HandleTemporaryStoreSchedule(Store item, StoreScheduleSetting storeScheduleSetting, string backUpStore)
        {
            item.BackupTime = backUpStore;
            var days = CommonFunction.GetWeekdaysBetweenDates(storeScheduleSetting.StartDate, storeScheduleSetting.EndDate);

            var scheduleActions = new Dictionary<DayOfWeek, Action>
            {
                [DayOfWeek.Monday] = () => { item.MondayOpeningTime = storeScheduleSetting.MondayOpeningTime; item.MondayClosingTime = storeScheduleSetting.MondayClosingTime; },
                [DayOfWeek.Tuesday] = () => { item.TuesdayOpeningTime = storeScheduleSetting.TuesdayOpeningTime; item.TuesdayClosingTime = storeScheduleSetting.TuesdayClosingTime; },
                [DayOfWeek.Wednesday] = () => { item.WednesdayOpeningTime = storeScheduleSetting.WednesdayOpeningTime; item.WednesdayClosingTime = storeScheduleSetting.WednesdayClosingTime; },
                [DayOfWeek.Thursday] = () => { item.ThursdayOpeningTime = storeScheduleSetting.ThursdayOpeningTime; item.ThursdayClosingTime = storeScheduleSetting.ThursdayClosingTime; },
                [DayOfWeek.Friday] = () => { item.FridayOpeningTime = storeScheduleSetting.FridayOpeningTime; item.FridayClosingTime = storeScheduleSetting.FridayClosingTime; },
                [DayOfWeek.Saturday] = () => { item.SaturdayOpeningTime = storeScheduleSetting.SaturdayOpeningTime; item.SaturdayClosingTime = storeScheduleSetting.SaturdayClosingTime; },
                [DayOfWeek.Sunday] = () => { item.SundayOpeningTime = storeScheduleSetting.SundayOpeningTime; item.SundayClosingTime = storeScheduleSetting.SundayClosingTime; },
            };

            days.Distinct().ToList().ForEach(day =>
            {
                if (scheduleActions.ContainsKey(day))
                {
                    scheduleActions[day]();
                }
            });

            storeScheduleSetting.StatusCode = (int)StoreScheduleSettingStatus.InProgress;
            return true;
        }

        private static void HandleTemporaryStoreScheduleForEnd(Store item, StoreScheduleSetting storeScheduleSetting)
        {
            #region Handle Temporary Store Schedule
            var backUpStoreEntity = JsonConvert.DeserializeObject<Store>(item.BackupTime);
            var days = CommonFunction.GetWeekdaysBetweenDates(storeScheduleSetting.StartDate, storeScheduleSetting.EndDate);

            var scheduleActions = new Dictionary<DayOfWeek, Action>
            {
                [DayOfWeek.Monday] = () => { item.MondayOpeningTime = backUpStoreEntity.MondayOpeningTime; item.MondayClosingTime = backUpStoreEntity.MondayClosingTime; },
                [DayOfWeek.Tuesday] = () => { item.TuesdayOpeningTime = backUpStoreEntity.TuesdayOpeningTime; item.TuesdayClosingTime = backUpStoreEntity.TuesdayClosingTime; },
                [DayOfWeek.Wednesday] = () => { item.WednesdayOpeningTime = backUpStoreEntity.WednesdayOpeningTime; item.WednesdayClosingTime = backUpStoreEntity.WednesdayClosingTime; },
                [DayOfWeek.Thursday] = () => { item.ThursdayOpeningTime = backUpStoreEntity.ThursdayOpeningTime; item.ThursdayClosingTime = backUpStoreEntity.ThursdayClosingTime; },
                [DayOfWeek.Friday] = () => { item.FridayOpeningTime = backUpStoreEntity.FridayOpeningTime; item.FridayClosingTime = backUpStoreEntity.FridayClosingTime; },
                [DayOfWeek.Saturday] = () => { item.SaturdayOpeningTime = backUpStoreEntity.SaturdayOpeningTime; item.SaturdayClosingTime = backUpStoreEntity.SaturdayClosingTime; },
                [DayOfWeek.Sunday] = () => { item.SundayOpeningTime = backUpStoreEntity.SundayOpeningTime; item.SundayClosingTime = backUpStoreEntity.SundayClosingTime; },
            };

            days.Distinct().ToList().ForEach(day =>
            {
                if (scheduleActions.ContainsKey(day))
                {
                    scheduleActions[day]();
                }
            });

            item.BackupTime = null;
            #endregion
        }

        private static bool HandleMainDayStoreSchedule(Store item, StoreScheduleSetting storeScheduleSetting)
        {
            #region Handle Main Day Store Schedule
            item.MondayOpeningTime = storeScheduleSetting.MondayOpeningTime;
            item.MondayClosingTime = storeScheduleSetting.MondayClosingTime;

            item.TuesdayOpeningTime = storeScheduleSetting.TuesdayOpeningTime;
            item.TuesdayClosingTime = storeScheduleSetting.TuesdayClosingTime;

            item.WednesdayOpeningTime = storeScheduleSetting.WednesdayOpeningTime;
            item.WednesdayClosingTime = storeScheduleSetting.WednesdayClosingTime;

            item.ThursdayOpeningTime = storeScheduleSetting.ThursdayOpeningTime;
            item.ThursdayClosingTime = storeScheduleSetting.ThursdayClosingTime;

            item.FridayOpeningTime = storeScheduleSetting.FridayOpeningTime;
            item.FridayClosingTime = storeScheduleSetting.FridayClosingTime;

            item.SaturdayOpeningTime = storeScheduleSetting.SaturdayOpeningTime;
            item.SaturdayClosingTime = storeScheduleSetting.SaturdayClosingTime;

            item.SundayOpeningTime = storeScheduleSetting.SundayOpeningTime;
            item.SundayClosingTime = storeScheduleSetting.SundayClosingTime;

            item.IsClosedMonday = storeScheduleSetting.IsClosedMonday;
            item.IsClosedTuesday = storeScheduleSetting.IsClosedTuesday;
            item.IsClosedWednesday = storeScheduleSetting.IsClosedWednesday;
            item.IsClosedThursday = storeScheduleSetting.IsClosedThursday;
            item.IsClosedFriday = storeScheduleSetting.IsClosedFriday;
            item.IsClosedSaturday = storeScheduleSetting.IsClosedSaturday;
            item.IsClosedSunday = storeScheduleSetting.IsClosedSunday;

            storeScheduleSetting.StateCode = (int)StateCode.InActive;
            storeScheduleSetting.StatusCode = (int)StoreScheduleSettingStatus.InActive;
            storeScheduleSetting.EndDate = DateTime.Now;
            #endregion

            return true;
        }

        #endregion
    }
}
