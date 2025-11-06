namespace AppointmentApp.API.Models.Enums;

public enum MessageStatus : short
{
    Queued    = 0,
    Sent      = 1,
    Delivered = 2,
    Failed    = 3
}