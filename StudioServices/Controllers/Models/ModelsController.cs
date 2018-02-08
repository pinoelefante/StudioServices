using StudioServices.Data.Models;
using System;
using System.Collections.Generic;

namespace StudioServices.Controllers.Models
{
    public class ModelsController
    {
        private ModelDatabase db;
        public ModelsController()
        {
            db = new ModelDatabase();
        }
        public bool RequestModel(int person_id, int model_id, bool print, string note, out string message)
        {
            message = "";

            Model model = db.SelectModel(model_id);
            if(!model.Attivo)
            {
                message = "Non è più possibile richiedere il modello";
                return false;
            }

            // Verifica la presenza di vecchie richieste
            var old_requests = db.SelectModelRequestList(model_id, person_id);
            if(model.IsUnique && old_requests.Count > 0)
            {
                message = "Il modello è già stato richiesto in passato e non può più essere richiesto";
                return false;
            }

            ModelRequest request = new ModelRequest()
            {
                IsPrint = print,
                IsRequest = true,
                ModelId = model_id,
                PersonId = person_id,
                Note = note,
                Status = ModelRequestStatus.PENDING
            };

            return db.SaveItem(request);
        }
        public bool PrintModel(int person_id, int model_id, string note, out string message)
        {
            message = "";
            Model model = db.SelectModel(model_id);
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

            var old_requests = db.SelectModelRequestList(model_id, person_id);
            bool is_request = old_requests.Count == 0;
            ModelRequest request = new ModelRequest()
            {
                IsPrint = true,
                IsRequest = is_request,
                ModelId = model_id,
                Note = note,
                PersonId = person_id,
                Status = ModelRequestStatus.PENDING
            };
            return db.SaveItem(request);
        }
        public IEnumerable<ModelRequest> ListRequests(int person_id)
        {
            return db.SelectModelRequestList(person_id);
        }
        public bool DeleteRequest(int request_id, int person_id)
        {
            // Cancellabile solo se lo stato è PENDING
            ModelRequest request = db.SelectModelRequest(request_id, person_id);
            if (request == null)
                return true;
            if(request.Status == ModelRequestStatus.PENDING)
            {
                request.Status = ModelRequestStatus.DELETED;
                return db.SaveItem(request);
            }
            return false;
        }
        public bool ModifyNote(int request_id, int person_id, string note)
        {
            ModelRequest request = db.SelectModelRequest(request_id, person_id);
            if (request == null)
                return false;
            if (request.Status != ModelRequestStatus.PENDING)
                return false;
            request.Note = note;
            return db.SaveItem(request);
        }
    }
}