using StudioServices.Data.Accounting;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudioServicesApp.Views.Framework
{
    public class CompanyComparerByName : IComparer<Company>
    {
        public int Compare(Company x, Company y)
        {
            return x.Name.ToLower().CompareTo(y.Name.ToLower());
        }
    }
}
