namespace RoomHub.Api.Models;

public enum BookingStatus
{
    Active,
    Cancelled
}

public class Booking
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public string BookedBy { get; set; } = string.Empty;
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public int Attendees { get; set; }
    public BookingStatus Status { get; set; }
}
