namespace SubmitClaims.Models;

public class Invoice
{
    public int Id { get; set; }
    public DateTime GeneratedDate { get; set; }
    public List<LecturerClaim> Claims { get; set; } // Approved claims in the invoice
    public decimal TotalAmount => Claims.Sum(c => c.FinalPayment);
}