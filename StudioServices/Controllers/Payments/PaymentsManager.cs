using StudioServices.Controllers.Items;
using StudioServices.Data.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Controllers.Payments
{
    public class PaymentsManager
    {
        private PaymentDatabase db;
        private ItemsDatabase db_models;
        public PaymentsManager()
        {
            db = new PaymentDatabase();
            db_models = new ItemsDatabase();
        }
        public double GetToPay(int person_id, bool exclude_pending = false)
        {
            double payed = GetTotalPayed(person_id);
            var total = GetTotalToPay(person_id, exclude_pending);
            Console.WriteLine($"{person_id} - ToPay: {total} Payed: {payed} - TOTAL: {total-payed}");
            return total - payed;
        }
        public double GetTotalToPay(int person_id, bool exclude_pending = false)
        {
            var requests = db_models.SelectItemRequestsList(person_id);
            var models = db_models.SelectItemsList(false);
            var total = 0d;
            foreach (var item in requests)
            {
                var model = models.FirstOrDefault(x => x.Id == item.ItemId);
                if (model == null)
                    throw new ArgumentNullException();
                double total_request = 0;
                if (item.IsRequest)
                    total_request += model.RequestCost * item.RequestQuantity;
                if (item.IsPrint)
                    total_request += model.RequestPrintCost * item.PrintCopies;
                Console.WriteLine($"{person_id} - {model.Name}({model.Year}) - : {total_request}");
                total += total_request;
            }
            Console.WriteLine($"{person_id} - TotalToPay: {total}");
            return total;
        }
        public double GetTotalPayed(int person_id)
        {
            var history = db.SelectPaymentHistoryList(person_id);
            double total = 0;
            history.ForEach(x => total += x.Amount);
            return total;
        }
        public List<PaymentHistory> GetPaymentHistory(int person_id)
        {
            return db.SelectPaymentHistoryList(person_id);
        }
        public bool AddPayment(int person_id, double amount, PaymentMethod type, string transaction_id = "000000")
        {
            PaymentHistory payment = new PaymentHistory()
            {
                PersonId = person_id,
                Amount = amount,
                Type = type
            };
            return db.SaveItem(payment);
        }
    }
}
