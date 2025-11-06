namespace AppointmentApp.API.Models.Entity;

public sealed class WaTemplateButton
{
    public string Type { get; set; } = "URL";  // URL | PHONE | QUICK
    public string Text { get; set; } = default!;
    public string? UrlTemplate { get; set; }
    public string? Phone { get; set; }
}
