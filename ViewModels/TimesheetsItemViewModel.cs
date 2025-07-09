namespace Timesheets_APP.ViewModels
{
    public class TimesheetsItemViewModel
    {
        public int TrId { get; set; }
        public string TimeFrom { get; set; } = "";
        public string TimeOut { get; set; } = "";
        public string? Description { get; set; }

        public string TimeFrom24 => DateTime.Parse(TimeFrom).ToString("HH:mm");
        public string TimeOut24 => DateTime.Parse(TimeOut).ToString("HH:mm");

    }
}
