using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.Data;
using TodoApp.Api.Filters;
using TodoApp.Api.Messages;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueryController : Controller
{
    private readonly IItemRepository _itemRepository;

    public QueryController(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    [HttpPost]
    [ModelValidation]
    public async Task<ActionResult<Item[]>> Post(QueryRequest request)
    {
        var result = await _itemRepository.FindByQuery(request);
        return result.Length == 0 ? NotFound() : Ok(result);
    }

    [HttpGet("priority/{priority}")]
    public async Task<ActionResult<Item[]>> Get(int priority, int? limit)
    {
        if (priority is < 1 or > 5)
        {
            return BadRequest("Priority has to be an integer between 1 and 5.");
        }

        if (limit <= 0)
        {
            return BadRequest("Limit cannot be less then 1.");
        }
        
        var result = await _itemRepository.FindByPriority(priority, limit);
        return result.Length == 0 ? NotFound() : Ok(result);
    }
}