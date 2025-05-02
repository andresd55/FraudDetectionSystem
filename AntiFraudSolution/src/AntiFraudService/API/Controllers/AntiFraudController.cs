using Microsoft.AspNetCore.Mvc;
using AntiFraudService.Application.Services;
using AntiFraudService.Domain.Entities;

namespace AntiFraudService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AntiFraudController : ControllerBase
{
    private readonly FraudDetectionService _fraudService;

    public AntiFraudController(FraudDetectionService fraudService)
    {
        _fraudService = fraudService;
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateTransaction([FromBody] TransactionValidation validation)
    {
        var result = await _fraudService.EvaluateAsync(validation);
        return Ok(result);
    }
}
