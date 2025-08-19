using MediatR;
using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.Features.Properties.Application.Commands;
using RealEstateAPI.Features.Properties.Application.Queries;

namespace RealEstateAPI.Features.Properties.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PropertiesController> _logger;

        public PropertiesController(IMediator mediator, ILogger<PropertiesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get all properties with optional filtering and pagination
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(GetAllPropertiesResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetAllPropertiesResult>> GetAll([FromQuery] GetAllPropertiesQuery query)
        {
            _logger.LogInformation("Getting all properties with filters: {@Query}", query);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get a specific property by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PropertyDto>> GetById(int id)
        {
            _logger.LogInformation("Getting property with ID: {Id}", id);
            var result = await _mediator.Send(new GetPropertyByIdQuery(id));

            if (result == null)
            {
                _logger.LogWarning("Property with ID {Id} not found", id);
                return NotFound(new { message = $"Property with ID {id} not found" });
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a new property
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CreatePropertyResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CreatePropertyResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreatePropertyResult>> Create([FromBody] CreatePropertyCommand command)
        {
            _logger.LogInformation("Creating new property at {Address}, {City}", command.Address, command.City);
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to create property: {Message}", result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Property created successfully with ID: {Id}", result.Id);
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result
            );
        }

        /// <summary>
        /// Update an existing property
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(UpdatePropertyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UpdatePropertyResult>> Update(int id, [FromBody] UpdatePropertyCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(new { message = "ID in URL does not match ID in request body" });
            }

            _logger.LogInformation("Updating property with ID: {Id}", id);
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to update property {Id}: {Message}", id, result.Message);

                if (result.Message.Contains("not found"))
                    return NotFound(result);

                return BadRequest(result);
            }

            _logger.LogInformation("Property {Id} updated successfully", id);
            return Ok(result);
        }

        /// <summary>
        /// Delete a property
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(DeletePropertyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeletePropertyResult>> Delete(int id)
        {
            _logger.LogInformation("Deleting property with ID: {Id}", id);
            var result = await _mediator.Send(new DeletePropertyCommand(id));

            if (!result.Success)
            {
                _logger.LogWarning("Failed to delete property {Id}: {Message}", id, result.Message);
                return NotFound(result);
            }

            _logger.LogInformation("Property {Id} deleted successfully", id);
            return Ok(result);
        }

        /// <summary>
        /// Get available properties
        /// </summary>
        [HttpGet("available")]
        [ProducesResponseType(typeof(GetAllPropertiesResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetAllPropertiesResult>> GetAvailable([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting available properties");
            var query = new GetAllPropertiesQuery(
                IsAvailable: true,
                PageNumber: pageNumber,
                PageSize: pageSize
            );
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Search properties by city
        /// </summary>
        [HttpGet("city/{city}")]
        [ProducesResponseType(typeof(GetAllPropertiesResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetAllPropertiesResult>> GetByCity(string city, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting properties in city: {City}", city);
            var query = new GetAllPropertiesQuery(
                City: city,
                PageNumber: pageNumber,
                PageSize: pageSize
            );
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}