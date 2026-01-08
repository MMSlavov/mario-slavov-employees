namespace SirmaTask.Models
{
    public class PairInfo
    {
        public int TotalDays { get; set; }
        public List<(int ProjectID, int Days)> Projects { get; set; } = new();
    }
}