using ERP.Purchasing.API.Moddels;
using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Application.PurchaseOrders.Commands.AddItemToPurchaseOrder;
using ERP.Purchasing.Application.PurchaseOrders.Commands.ApprovePurchaseOrder;
using ERP.Purchasing.Application.PurchaseOrders.Commands.CreatePurchaseOrder;
using ERP.Purchasing.Application.PurchaseOrders.Commands.DeactivatePurchaseOrder;
using ERP.Purchasing.Application.PurchaseOrders.Commands.UpdateItemPrice;
using ERP.Purchasing.Application.PurchaseOrders.Queries.GetAllPurchaseOrders;
using ERP.Purchasing.Application.PurchaseOrders.Queries.GetPurchaseOrderById;
using ERP.Purchasing.Application.PurchaseOrders.Queries.GetPurchaseOrderByNumber;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Purchasing.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PurchaseOrdersController> _logger;

    public PurchaseOrdersController(IMediator mediator, ILogger<PurchaseOrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseOrderDto>> GetById(Guid id)
    {
        try
        {
            var query = new GetPurchaseOrderByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound($"Purchase order with ID {id} not found");

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchase order {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the purchase order");
        }
    }

    [HttpGet("by-number/{number}")]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseOrderDto>> GetByNumber(string number)
    {
        try
        {
            var query = new GetPurchaseOrderByNumberQuery { Number = number };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound($"Purchase order {number} not found");

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchase order {Number}", number);
            return StatusCode(500, "An error occurred while retrieving the purchase order");
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<PurchaseOrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PurchaseOrderDto>>> GetAll(
        [FromQuery] PurchaseOrderState? state = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int? pageNumber = null,
        [FromQuery] int? pageSize = null)
    {
        try
        {
            var query = new GetAllPurchaseOrdersQuery
            {
                State = state,
                IsActive = isActive,
                FromDate = fromDate,
                ToDate = toDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchase orders");
            return StatusCode(500, "An error occurred while retrieving purchase orders");
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PurchaseOrderDto>> Create([FromBody] CreatePurchaseOrderCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase order");
            return StatusCode(500, "An error occurred while creating the purchase order");
        }
    }

    [HttpPost("{id}/approve")]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseOrderDto>> Approve(Guid id)
    {
        try
        {
            var command = new ApprovePurchaseOrderCommand { PurchaseOrderId = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot approve purchase order {Id}", id);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving purchase order {Id}", id);
            return StatusCode(500, "An error occurred while approving the purchase order");
        }
    }

    [HttpPost("{id}/deactivate")]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseOrderDto>> Deactivate(Guid id)
    {
        try
        {
            var command = new DeactivatePurchaseOrderCommand { PurchaseOrderId = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating purchase order {Id}", id);
            return StatusCode(500, "An error occurred while deactivating the purchase order");
        }
    }

    [HttpPost("{id}/items")]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseOrderDto>> AddItem(Guid id, [FromBody] AddItemRequest request)
    {
        try
        {
            var command = new AddItemToPurchaseOrderCommand
            {
                PurchaseOrderId = id,
                GoodCode = request.GoodCode,
                Price = request.Price,
                Currency = request.Currency
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot add item to purchase order {Id}", id);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to purchase order {Id}", id);
            return StatusCode(500, "An error occurred while adding the item");
        }
    }

    [HttpPut("{id}/items/{goodCode}/price")]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseOrderDto>> UpdateItemPrice(
        Guid id,
        string goodCode,
        [FromBody] UpdatePriceRequest request)
    {
        try
        {
            var command = new UpdateItemPriceCommand
            {
                PurchaseOrderId = id,
                GoodCode = goodCode,
                NewPrice = request.NewPrice,
                Currency = request.Currency
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot update item price in purchase order {Id}", id);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating item price in purchase order {Id}", id);
            return StatusCode(500, "An error occurred while updating the item price");
        }
    }
}