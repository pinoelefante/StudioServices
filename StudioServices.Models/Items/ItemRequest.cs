namespace StudioServices.Data.Items
{
    public class ItemRequest : PersonReference
    {
        public int ItemId { get; set; }
        public bool IsRequest { get; set; }
        public bool IsPrint { get; set; }
        public string Note { get; set; }
        public ItemRequestStatus Status { get; set; }
        public string Filename { get; set; }
        public int RequestQuantity { get; set; } = 1;
        public int PrintCopies { get; set; } = 1;
        public string Description { get; set; }
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