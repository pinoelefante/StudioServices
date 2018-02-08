using StudioServices.Data.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Controllers.Payments
{
    public class PaymentsController
    {
        private PaymentDatabase db;
        public PaymentsController()
        {
            db = new PaymentDatabase();
        }
        public double GetTotalToPay(int person_id)
        {
            throw new NotImplementedException();
        }
        public double GetTotalPayed(int person_id)
        {
            throw new NotImplementedException();
        }
        public bool AddPayment(int person_id, double amount, PaymentMethod type, string transaction_id = "000000")
        {
            PaymentHistory payment = new PaymentHistory()
            {
                PersonId = person_id,
                Quantity = amount,
                Type = type
            };
            return db.SaveItem(payment);
        }
    }
}
