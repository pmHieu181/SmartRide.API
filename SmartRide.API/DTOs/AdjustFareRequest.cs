namespace SmartRide.API.DTOs
{
    public class AdjustFareRequest
    {
        public decimal AdjustmentAmount { get; set; }
        public required string Reason { get; set; }
    }
}