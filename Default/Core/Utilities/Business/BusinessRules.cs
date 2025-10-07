using LCW.Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace LCW.Core.Utilities.LCW.Business
{
    public static class BusinessRules
    {
        public static IResult Run(params IResult[] logics)
        {
            foreach (var result in logics)
            {
                if (result.Success)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
