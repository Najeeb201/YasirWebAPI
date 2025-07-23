public class CustomerCard
{
    public int Id { get; set; }

    // هذا الحقل إلزامي
    public string CustomerName { get; set; } = string.Empty;

    // باقي الحقول اختيارية (nullable)
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    public float Balance { get; set; }
    public DateTime EntryDate { get; set; }
}
