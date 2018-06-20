using StudioServices.Controllers.Utils;
using StudioServices.Data.EntityFramework.Items;
using StudioServicesWeb.DataControllers;
using System;
using System.Collections.Generic;

namespace StudioServices.Controllers.Items
{
    public class ItemsManager
    {
        private DatabaseEF db;
        public ItemsManager(DatabaseEF d)
        {
            db = d;
        }
        public List<PayableItem> ListItems()
        {
            return db.GetAll<PayableItem>();
        }
        public bool DeleteItem(int item_id)
        {
            // TODO implement this
            throw new NotImplementedException();
        }
        /* Spostare la parte admin in una nuova classe dedicata */
        public bool RequestModel(ItemRequest request, out string message, bool admin = false)
        {
            
            message = "";
            /*
            // PayableItem item = db.SelectItem(model_id);
            if(!admin && !request.Item.IsRequestable)
            {
                message = "Il modello non è richiedibile";
                return false;
            }
            if(!admin && !request.Item.Enabled)
            {
                message = "Non è più possibile richiedere il modello";
                return false;
            }

            // Verifica la presenza di vecchie richieste
            var old_requests = db.Requests_GetList(request.Item.Id, request.PersonId);
            if(request.Item.IsUnique && old_requests.Count > 0)
            {
                message = "Il modello è già stato richiesto in passato e non può più essere richiesto";
                return false;
            }
            return db.Save(request);
            */
            return false;
        }
        /*
        public bool PrintModel(int person_id, int model_id, int print_count, string note, out string message)
        {
            message = "";
            PayableItem model = db.SelectItem(model_id);
            if(!model.IsPrintable)
            {
                message = "Il modello non è stampabile";
                return false;
            }
            if(!model.Enabled)
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
        */
        public List<ItemRequest> ListRequests(int person_id)
        {
            return db.GetList<ItemRequest>(person_id);
        }
        public bool DeleteRequest(int request_id, int person_id, bool admin = false)
        {
            // Cancellabile solo se lo stato è PENDING
            ItemRequest request = db.Get<ItemRequest>(request_id);
            if (request == null || request.PersonId != person_id)
                return true;
            if(admin || request.Status == ItemRequestStatus.PENDING)
            {
                request.Status = ItemRequestStatus.DELETED;
                return db.Save(request);
            }
            return false;
        }
        public bool ModifyNote(int request_id, int person_id, string note)
        {
            ItemRequest request = db.Get<ItemRequest>(request_id);
            if (request == null || request.PersonId != person_id)
                return false;
            if (request.Status != ItemRequestStatus.PENDING)
                return false;
            request.Note = note;
            return db.Save(request);
        }
        /* Spostare in una classe dedicata all'admin */
        public bool CreateModel(PayableItem model)
        {
            // TODO verifica amministratore
            return db.Save(model);
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
                if (db.Save(model))
                    break;
                else
                {
                    model.Code = "OTHER_" + StringUtils.RandomString(base_length + count);
                    count++;
                }
            }
            if (model.Id <= 0) // non è stato possibile salvare nel database
                return false;
            ItemRequest request = new ItemRequest()
            {
                PersonId = person_id,
                Item = model,
                ItemId = model.Id,
                Note = description,
                IsPrint = false,
                IsRequest = true,
                PrintCopies = 0,
                RequestQuantity = 1,
            };
            return RequestModel(request, out message, true);
        }
    }
}