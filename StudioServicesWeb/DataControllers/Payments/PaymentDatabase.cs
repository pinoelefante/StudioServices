using StudioServices.Controllers.Utils;
using StudioServices.Data.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Controllers.Payments
{
    public class PaymentDatabase : Database
    {
        public List<PaymentHistory> SelectPaymentHistoryList(int person_id)
        {
            using (var con = GetConnection())
            {
                return con.Table<PaymentHistory>().Where(x => x.PersonId == person_id).ToList();
            }
        }
    }
}
