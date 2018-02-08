using StudioServices.Controllers.Utils;
using StudioServices.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Controllers.Models
{
    public class ModelDatabase : Database
    {
        public ModelRequest SelectModelRequest(int request_id)
        {
            using (var con = GetConnection())
            {
                return con.Get<ModelRequest>(request_id);
            }
        }
        public ModelRequest SelectModelRequest(int request_id, int person_id)
        {
            var request = SelectModelRequest(request_id);
            return request != null && request.PersonId == person_id ? request : null;
        }
        public List<ModelRequest> SelectModelRequestList(int model_id, int person_id)
        {
            using (var con = GetConnection())
            {
                return con.Table<ModelRequest>().Where(x => x.PersonId == person_id && x.ModelId == model_id).ToList();
            }
        }
        public IEnumerable<ModelRequest> SelectModelRequestList(int person_id, bool all = false)
        {
            using (var con = GetConnection())
            {
                return con.Table<ModelRequest>().Where(x => x.PersonId == person_id && (all ? true : x.Status!=ModelRequestStatus.DELETED)).OrderByDescending(x => x.Id).AsEnumerable();
            }
        }
        public IEnumerable<Model> SelectModelList(bool only_active = true)
        {
            using (var con = GetConnection())
            {
                return con.Table<Model>().Where(x => only_active ? x.Attivo : true).OrderBy(x => x.Year).AsEnumerable();
            }
        }
        public Model SelectModel(int model_id)
        {
            using (var con = GetConnection())
            {
                return con.Get<Model>(model_id);
            }
        }
    }
}
