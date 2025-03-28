using JetBrains.Annotations;

namespace Frelance.Contracts.Requests.Invoices;

[UsedImplicitly]
public record CreateInvoiceRequest(string ProjectName, string ClientName, decimal Amount, string InvoiceFile);