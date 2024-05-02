using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.Data;
using TodoApp.Api.Extensions;
using TodoApp.Api.Filters;
using TodoApp.Api.Messages;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ItemsController : Controller
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<ItemsController> _logger;
    
    public ItemsController(IItemRepository itemRepository, ILogger<ItemsController> logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<Item[]>> Get()
    {
        _logger.LogInformation("Getting the items list.");
        try
        {
            var items = await _itemRepository.List();
            return items;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get the items list.");
            return StatusCode(500, "Error when getting the items list occured.");
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Item>> Get(int id)
    {
        _logger.LogInformation("Getting the item with ID: {itemId}.", id);
        try
        {
            var item = await _itemRepository.Find(id);
            return item is null ? NotFound() : item;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get the item with ID: {itemId}.", id);
            return StatusCode(500, "Error when getting the item occured.");
        }
    }
    
    [HttpPut]
    [ModelValidation]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> Put(Item item)
    {
        _logger.LogInformation("Updating the item with ID: {itemId}.", item.Id);
        try
        { 
            var updatedItem = await _itemRepository.Update(item);
            return updatedItem is null ? NotFound() : NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update the item with ID: {itemId}.", item.Id);
            return StatusCode(500, "Error when updating the item occured.");
        }
    }
    
    [HttpPost]
    [ModelValidation]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<Item>> Post(CreateItemRequest request)
    {
        _logger.LogInformation("Creating new item.");
        try
        {
            var item = await _itemRepository.Add(request.ToItem());
            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create new item based on the request: {CreateItemRequest}.", request);
            return StatusCode(500, "Error when creating new item occured.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<Item>> Delete(int id)
    {
        _logger.LogInformation("Finding the item with ID: {itemId} for deletion.", id);
        Item item;
        try
        {
            item = await _itemRepository.Find(id);
            if (item == null)
            {
                _logger.LogInformation("Item with ID: {itemId} not found for deletion.", id);
                return NotFound();
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to find the item with ID: {itemId} for deletion.", id);
            return StatusCode(500, "Error when finding the item for deletion occured.");
        }
        
        _logger.LogError("Deleting the item with ID: {itemId}.", id);
        try
        {
            await _itemRepository.Remove(item);
            return item;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to delete the item with ID: {itemId}.", id);
            return StatusCode(500, "Error when deleting the item occured.");
        }
    }
}