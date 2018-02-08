namespace StudioServices.Data.Models
{
    public class ModelRequest : PersonReference
    {
        public int ModelId { get; set; }
        public bool IsRequest { get; set; }
        public bool IsPrint { get; set; }
        public string Note { get; set; }
        public ModelRequestStatus Status { get; set; }
        public string Filename { get; set; }
    }
    public enum ModelRequestStatus
    {
        DELETED = -1,
        PENDING = 0,
        WORKING = 1,
        COMPLETE = 2,
        DENIED = 3,
        PAYMENT_REQUIRED = 4
    }
}