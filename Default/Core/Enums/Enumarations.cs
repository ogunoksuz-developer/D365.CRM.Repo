using System;
using System.Collections.Generic;
using System.Text;

namespace LCW.Core.Enums
{
    public static class Enumarations
    {
        public enum FileOwnerShip
        {
            Owner = 0,
            User = 1
        }

        public enum CacheKey
        {
            Iller = 0,
            Kategoriler = 1
        }

        public enum Priority
        {
            Low = 0,
            Normal = 1,
            High = 2
        }

        public enum ChatStates
        {
            Active = 0,
            InActive = 1
        }

        public enum ChatStatus
        {
            Active = 1,
            InActive = 2
        }

        public enum EmailStatus
        {
            Sended = 1,
            Error = 2,
            Waiting = 3,
            Sending = 4
        }

        public enum EventViewerSources
        {
            EmailService
        }

        public enum EventViewerLogNames
        {
            EmailServiceLog = 1
        }

        public enum OperationType
        {
            Insert=0,
            Update=1,
            Delete=2,
            Select=3
        }

        public enum CustomerWarehouseImportStatus
        {
            Taslak = 0,
            Alindi = 1,
            Isleniyor = 2,
            Tamamlandi = 3,
            HataAlindi = 4
        }

        public enum EntityStates
        {
            Active = 0,
            InActive = 1
        }

        public enum CampaignStatus
        {
            Live = 862440002,
            PublishWaiting = 862440001
        }

        public enum StateCode
        {
            Active = 0,
            InActive = 1
        }

        public enum StoreScheduleSettingStatus
        {
            Active = 1,
            InActive = 2,
            InProgress = 100000000,
            Completed = 100000001
        }

        public enum StoreScheduleType
        {
            Temporary = 100000001,
            MainDay = 100000002,
            ClosedStore = 100000003,
            StoreDailyHours = 100000000
        }


    }
}
