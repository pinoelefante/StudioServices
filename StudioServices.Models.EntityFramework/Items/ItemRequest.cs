using StudioServices.Data.EntityFramework;

namespace StudioServices.Data.EntityFramework.Items
{
    public class ItemRequest : PersonReference
    {
        // [ForeignKey(typeof(PayableItem))]
        public int ItemId { get; set; }

        // [OneToOne]
        public PayableItem Item { get; set; }

        public bool IsRequest { get; set; }
        public bool IsPrint { get; set; }
        public string Note { get; set; }
        public ItemRequestStatus Status { get; set; }
        public string Filename { get; set; }
        public int RequestQuantity { get; set; } = 1;
        public int PrintCopies { get; set; } = 1;
        public string Description { get; set; }

        protected override void Validate()
        {
            RequestQuantity = RequestQuantity < 0 && IsRequest ? 1 : 0;
            PrintCopies = PrintCopies < 0 && IsPrint ? 1 : 0;
        }
    }
    public enum ItemRequestStatus
    {
        DELETED = -1,
        PENDING = 0,
        WORKING = 1,
        COMPLETE = 2,
        DENIED = 3,
        PAYMENT_REQUIRED = 4
    }
}