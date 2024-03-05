using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.Data;
using TodoApp.Api.Extensions;
using TodoApp.Api.Filters;
using TodoApp.Api.Messages;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : Controller
{
    private readonly IItemRepository _itemRepository;

    public ItemsController(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    [HttpGet]
    public async Task<ActionResult<Item[]>> Get()
    {
        return await _itemRepository.List();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Item>> Get(int id)
    {
        var item = await _itemRepository.Find(id);
        return item is null ? NotFound() : item;
    }

    [HttpPut]
    [ModelValidation]
    public async Task<IActionResult> Put(Item item)
    {
        item = await _itemRepository.Update(item);
        return item is null ? NotFound() : NoContent();
    }

    [HttpPost]
    [ModelValidation]
    public async Task<ActionResult<Item>> Post(CreateItemRequest request)
    {
        var item = await _itemRepository.Add(request.ToItem());
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Item>> Delete(int id)
    {
        var item = await _itemRepository.Find(id);
        if (item == null)
        {
            return NotFound();
        }

        await _itemRepository.Remove(item);
        return item;
    }
}