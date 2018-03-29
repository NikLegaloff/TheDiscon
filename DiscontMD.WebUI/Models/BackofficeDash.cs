namespace DiscontMD.WebUI.Models
{
    public class BackofficeDash
    {
        public int ActiveCards { get; set; }
        public int AvailableCards { get; set; }
        public int Total => ActiveCards + AvailableCards;
    }
}