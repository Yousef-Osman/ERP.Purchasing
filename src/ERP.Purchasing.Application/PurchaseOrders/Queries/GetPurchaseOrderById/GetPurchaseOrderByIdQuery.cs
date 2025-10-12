using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Purchasing.Application.Common.DTOs;
using MediatR;

namespace ERP.Purchasing.Application.PurchaseOrders.Queries.GetPurchaseOrderById;
public class GetPurchaseOrderByIdQuery : IRequest<PurchaseOrderDto>
{
    public Guid Id { get; set; }
}
