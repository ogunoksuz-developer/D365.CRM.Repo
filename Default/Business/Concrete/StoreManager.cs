using LCW.Core.Utilities.Results;
using LCW.DataAccess.Abstract;
using LCW.Entities.Concrete;
using LCW.Interfaces.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LCW.Business.Concrete
{
    public class StoreManager : IStoreService
    {
        public readonly IStoreDal _storeDal;
        public StoreManager(IStoreDal storeDal)
        {
            _storeDal = storeDal;
        }

        /// <summary>
        /// Retrieves a list of stores by their codes.
        /// </summary>
        /// <param name="codes">The list of store codes to retrieve.</param>
        /// <returns>A result containing the list of stores.</returns>
        public async Task<IDataResult<IList<Store>>> GetStoreListByCode(List<string> codes)
        {
           

            if (!codes.Any())
            {
                return new SuccessDataResult<List<Store>>(Enumerable.Empty<Store>().ToList());
            }

            #region Get Stores By Codes
            var entities = await _storeDal.GetStoreListByCode(codes);
            #endregion

            return new SuccessDataResult<List<Store>>(entities.ToList());
        }

        /// <summary>
        /// Retrieves a list of stores
        /// </summary>
        /// <returns>A result containing the list of stores.</returns>
        public async Task<IDataResult<IList<Store>>> GetStoreList()
        {
            #region Get Stores
            var entities = await _storeDal.GetStoreList();
            #endregion

            return new SuccessDataResult<List<Store>>(entities.ToList());
        }

        /// <summary>
        /// Updates the given store in the database.
        /// </summary>
        /// <param name="store">The store to update.</param>
        /// <returns>Result indicating the success of the update operation.</returns>
        public async Task<SuccessResult> Update(Store store)
        {
            #region Update Store
            await _storeDal.Update(store);
            #endregion

            return new SuccessResult();
        }

    }
}
