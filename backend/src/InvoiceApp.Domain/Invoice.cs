using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("InvoiceApp.Api")]
[assembly: InternalsVisibleTo("InvoiceApp.Tests")]

namespace InvoiceApp.Domain;

public class Invoice
{
    public string Id { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public InvoiceStatus Status { get; internal set; } = InvoiceStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? DecidedBy { get; set; }
    public DateTime? DecidedAt { get; set; }
    public string? DecisionNote { get; set; }

    public void Submit()
    {
        if (Status != InvoiceStatus.Draft)
        {
            throw new InvalidOperationException(
                $"Invoice {Id} is {Status}; only drafts can be submitted.");
        }

        Status = InvoiceStatus.PendingApproval;
    }

    public void Approve(string approver)
    {
        EnsurePending();

        Status = InvoiceStatus.Approved;
        DecidedBy = approver;
        DecidedAt = DateTime.UtcNow;
    }

    public void Reject(string approver, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("A reason is required to reject an invoice.", nameof(reason));
        }

        EnsurePending();

        Status = InvoiceStatus.Rejected;
        DecidedBy = approver;
        DecidedAt = DateTime.UtcNow;
        DecisionNote = reason;
    }

    private void EnsurePending()
    {
        if (Status != InvoiceStatus.PendingApproval)
        {
            throw new InvalidOperationException(
                $"Invoice {Id} is {Status}; only pending invoices can be decided.");
        }
    }
}