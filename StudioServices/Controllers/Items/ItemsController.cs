using StudioServices.Controllers.Utils;
using StudioServices.Data.Items;
using System;
using System.Collections.Generic;

namespace StudioServices.Controllers.Items
{
    public class ItemsController
    {
        private ItemsDatabase db;
        public ItemsController()
        {
            db = new ItemsDatabase();
        }
        /* Spostare la parte admin in una nuova classe dedicata */
        public bool RequestModel(int person_id, int model_id, bool print, string note, out string message, int request_count = 1, int print_count = 1, bool admin = false)
        {
            message = "";
            PayableItem item = db.SelectItem(model_id);
            if(!admin && !item.IsRequestable)
            {
                message = "Il modello non è richiedibile";
                return false;
            }
            if(!admin && !item.Attivo)
            {
                message = "Non è più possibile richiedere il modello";
                return false;
            }

            // Verifica la presenza di vecchie richieste
            var old_requests = db.SelectItemRequestsList(model_id, person_id);
            if(item.IsUnique && old_requests.Count > 0)
            {
                message = "Il modello è già stato richiesto in passato e non può più essere richiesto";
                return false;
            }

            ItemRequest request = new ItemRequest()
            {
                IsPrint = print,
                IsRequest = true,
                ItemId = model_id,
                PersonId = person_id,
                Note = note,
                Status = ItemRequestStatus.PENDING,
                RequestQuantity = (request_count <= 0 ? 1 : request_count),
                PrintCopies = (print && print_count <= 0 ? 1 : print_count)
            };

            return db.SaveItem(request);
        }
        public bool PrintModel(int person_id, int model_id, int print_count, string note, out string message)
        {
            message = "";
            PayableItem model = db.SelectItem(model_id);
            if(!model.IsPrintable)
            {
                message = "Il modello non è stampabile";
                return false;
            }
            if(!model.Attivo)
            {
                message = "Il modello non è attivo. Contatta lo studio per ulteriori informazioni";
                return false;
            }

            var old_requests = db.SelectItemRequestsList(model_id, person_id);
            bool is_request = old_requests.Count == 0;
            ItemRequest request = new ItemRequest()
            {
                IsPrint = true,
                IsRequest = is_request,
                ItemId = model_id,
                Note = note,
                PersonId = person_id,
                Status = ItemRequestStatus.PENDING,
                RequestQuantity = (is_request ? 1 : 0),
                PrintCopies = (print_count <= 0 ? 1 : print_count)
            };
            return db.SaveItem(request);
        }
        public IEnumerable<ItemRequest> ListRequests(int person_id)
        {
            return db.SelectItemRequestsList(person_id);
        }
        public bool DeleteRequest(int request_id, int person_id, bool admin = false)
        {
            // Cancellabile solo se lo stato è PENDING
            ItemRequest request = db.SelectItemRequest(request_id, person_id);
            if (request == null)
                return true;
            if(admin || request.Status == ItemRequestStatus.PENDING)
            {
                request.Status = ItemRequestStatus.DELETED;
                return db.SaveItem(request);
            }
            return false;
        }
        public bool ModifyNote(int request_id, int person_id, string note)
        {
            ItemRequest request = db.SelectItemRequest(request_id, person_id);
            if (request == null)
                return false;
            if (request.Status != ItemRequestStatus.PENDING)
                return false;
            request.Note = note;
            return db.SaveItem(request);
        }
        /* Spostare in una classe dedicata all'admin */
        public bool CreateModel(string name, string code, int year, double request_cost, double print_cost, bool unique, bool printable, bool requestable, string description)
        {
            // TODO verifica amministratore
            PayableItem model = new PayableItem()
            {
                Code = code,
                Description = description,
                IsPrintable = printable,
                IsRequestable = requestable,
                IsUnique = unique,
                Name = name,
                RequestCost = request_cost,
                RequestPrintCost = print_cost,
                Year = year
            };
            return db.SaveItem(model);
        }
        /* Spostare in una classe dedicata all'admin */
        public bool AddOtherRequest(int person_id, double amount, string description, out string message)
        {
            // TODO verifica admin
            message = string.Empty;
            int base_length = 2;
            PayableItem model = new PayableItem()
            {
                Name = "Altro",
                Code = "OTHER_" + StringUtils.RandomString(base_length),
                Year = DateTime.Now.Year,
                RequestCost = amount,
                Description = description,
                IsPrintable = false,
                IsRequestable = false,
                IsUnique = true,
                IsOther = true,
                RequestPrintCost = 0
            };
            int count = 0;
            while (count <= 3)
            {
                if (db.SaveItem(model))
                    break;
                else
                {
                    model.Code = "OTHER_" + StringUtils.RandomString(base_length + count);
                    count++;
                }
            }
            if (model.Id <= 0) // non è stato possibile salvare nel database
                return false;
            return RequestModel(person_id, model.Id, false, description, out message, 1, 0, true);
        }
    }
}