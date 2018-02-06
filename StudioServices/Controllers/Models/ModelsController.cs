using System;

namespace StudioServices.Controllers.Models
{
    public class ModelsController
    {
        private ModelsDatabase db;
        public ModelsController()
        {
            db = new ModelsDatabase();
        }
        public bool RequestModel(int person_id, int model_id, bool print)
        {
            
            throw new NotImplementedException();
        }
    }
}