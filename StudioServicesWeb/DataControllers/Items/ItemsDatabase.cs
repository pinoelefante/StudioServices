using StudioServices.Controllers.Utils;
using StudioServices.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Controllers.Items
{
    public class ItemsDatabase : Database
    {
        public ItemRequest SelectItemRequest(int request_id)
        {
            using (var con = GetConnection())
            {
                return con.Get<ItemRequest>(request_id);
            }
        }
        public ItemRequest SelectItemRequest(int request_id, int person_id)
        {
            var request = SelectItemRequest(request_id);
            return request != null && request.PersonId == person_id ? request : null;
        }
        public List<ItemRequest> SelectItemRequestsList(int item_id, int person_id)
        {
            using (var con = GetConnection())
            {
                return con.Table<ItemRequest>().Where(x => x.PersonId == person_id && x.ItemId == item_id).ToList();
            }
        }
        public List<ItemRequest> SelectItemRequestsList(int person_id, bool all = false)
        {
            using (var con = GetConnection())
            {
                return con.Table<ItemRequest>().Where(x => x.PersonId == person_id && (all ? true : x.Status!=ItemRequestStatus.DELETED)).OrderByDescending(x => x.Id).ToList();
            }
        }
        public List<PayableItem> SelectItemsList(bool only_active = true)
        {
            using (var con = GetConnection())
            {
                return con.Table<PayableItem>().Where(x => only_active ? x.Enabled : true).OrderBy(x => x.Year).ToList();
            }
        }
        public PayableItem SelectItem(int item_id)
        {
            using (var con = GetConnection())
            {
                return con.Get<PayableItem>(item_id);
            }
        }
    }
}
