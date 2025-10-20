using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Queries;

namespace ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet] //https://localhost:7000/api/Products
    public async Task<IActionResult> GetProducts()
    {
        var result = await _mediator.Send(new GetProductsQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}