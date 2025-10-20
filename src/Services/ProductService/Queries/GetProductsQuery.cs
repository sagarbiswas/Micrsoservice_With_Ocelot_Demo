using Common.Models;
using Common.Queries;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;

namespace ProductService.Queries;

public record GetProductsQuery : IQuery<List<Product>>;

public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, List<Product>>
{
    private readonly ProductDbContext _context;

    public GetProductsQueryHandler(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<Product>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _context.Products.ToListAsync(cancellationToken);
        return Result.Success(products);
    }
}