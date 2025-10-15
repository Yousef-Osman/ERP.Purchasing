using ERP.Purchasing.API.Moddels;
using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Application.Common.Requests;
using ERP.Purchasing.Application.PurchaseOrders.Commands.AddItemToPurchaseOrder;
using ERP.Purchasing.Application.PurchaseOrders.Commands.ApprovePurchaseOrder;
using ERP.Purchasing.Application.PurchaseOrders.Commands.CreateMultiplePurchaseOrders;
using ERP.Purchasing.Application.PurchaseOrders.Commands.CreatePurchaseOrder;
using ERP.Purchasing.Application.PurchaseOrders.Commands.DeactivatePurchaseOrder;
using ERP.Purchasing.Application.PurchaseOrders.Commands.UpdateItemPrice;
using ERP.Purchasing.Application.PurchaseOrders.Queries.GetAllPurchaseOrders;
using ERP.Purchasing.Application.PurchaseOrders.Queries.GetPurchaseOrderById;
using ERP.Purchasing.Application.PurchaseOrders.Queries.GetPurchaseOrderByNumber;
using ERP.Purchasing.Application.PurchaseOrders.Queries.GetRecentPurchaseOrders;
using ERP.SharedKernel.Exceptions;
using ERP.SharedKernel.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Purchasing.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PurchaseOrdersController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ILogger<PurchaseOrdersController> _logger;

    public PurchaseOrdersController(ISender mediator, ILogger<PurchaseOrdersController> logger)
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
            var query = new GetPurchaseOrderByIdQuery(id);
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
            var query = new GetPurchaseOrderByNumberQuery(number);
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
    [ProducesResponseType(typeof(PagedResult<PurchaseOrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PurchaseOrderDto>>> GetAll([FromQuery] PurchaseOrderQueryRequest request)
    {
        try
        {
            var query = new GetAllPurchaseOrdersQuery(request);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting purchase orders");
            return StatusCode(500, "An error occurred while retrieving purchase orders");
        }
    }

    [HttpGet("recent")]
    [ProducesResponseType(typeof(List<PurchaseOrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PurchaseOrderDto>>> GetRecent(int count = 7)
    {
        try
        {
            var query = new GetRecentPurchaseOrdersQuery { Count = count };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent purchase orders");
            return StatusCode(500, "An error occurred while retrieving recent purchase orders");
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PurchaseOrderDto>> Create(CreatePurchaseOrderCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation error creating purchase order");
            return BadRequest(new { error = ex.Message, code = ex.ErrorCode });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase order");
            return StatusCode(500, "An error occurred while creating the purchase order");
        }
    }

    [HttpPost("batch")]
    [ProducesResponseType(typeof(List<PurchaseOrderDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<PurchaseOrderDto>>> CreateMultiple(CreateMultiplePurchaseOrdersCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Created("", result);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation error creating multiple purchase orders");
            return BadRequest(new { error = ex.Message, code = ex.ErrorCode });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating multiple purchase orders");
            return StatusCode(500, "An error occurred while creating purchase orders");
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
            var command = new ApprovePurchaseOrderCommand(id);

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Purchase order not found: {Id}", id);
            return NotFound(new { error = ex.Message });
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
            var command = new DeactivatePurchaseOrderCommand(id);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Purchase order not found: {Id}", id);
            return NotFound(new { error = ex.Message });
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
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Purchase order not found: {Id}", id);
            return NotFound(new { error = ex.Message });
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
    public async Task<ActionResult<PurchaseOrderDto>> UpdateItemPrice(Guid id,string goodCode,[FromBody] UpdatePriceRequest request)
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
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Purchase order not found: {Id}", id);
            return NotFound(new { error = ex.Message });
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
