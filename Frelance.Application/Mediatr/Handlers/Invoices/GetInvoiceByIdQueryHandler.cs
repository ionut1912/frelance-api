using Frelance.Application.Mediatr.Queries.Invoices;
using Frelance.Application.Repositories;
using Frelance.Contracts.Dtos;
using MediatR;

namespace Frelance.Application.Mediatr.Handlers.Invoices;

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoicesDto>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoiceByIdQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<InvoicesDto> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        return await _invoiceRepository.GetInvoiceByIdAsync(request, cancellationToken);
    }
}