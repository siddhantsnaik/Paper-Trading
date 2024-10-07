using Microsoft.AspNetCore.Mvc;
using System;
using OnPaper_Trade.Abstractions;
using OnPaper_Trade.Services;
using FirebaseAdmin.Auth;


namespace OnPaper_Trade.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class tradeController : Controller
{
    private readonly TradeService _tradeService;

    public tradeController(TradeService tradeService)
    {
        _tradeService = tradeService;
    }

    [HttpPost(Name = "entertrade")]
    public async Task<IActionResult> EnterTrade([FromBody] EnterTradeRequest request)
    {
        Trade trade = new Trade();
        trade.TradeId = Guid.NewGuid().ToString();
        trade.StockCode = request.StockCode;
        trade.StockName = request.StockName; 
        trade.StockToken = request.StockToken;
        trade.ExchangeCode = request.ExchangeCode; 
        trade.EntryTime = request.EntryTime; 
        trade.Quantity = request.Quantity;
        trade.EntryPrice = request.EntryPrice;

        var response = await _tradeService.EnterTradeAsync(request.AuthToken, request.UserID, trade);
        if (response.StartsWith("{ \"error\":"))
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPost(Name = "exittrade")]
    public async Task<IActionResult> ExitTrade([FromBody] ExitTradeRequest request)
    {
        var response = await _tradeService.ExitTradeAsync(request.AuthToken, request.UserID, request.TradeID, request.ExitPrice, request.ExitTime);
        if (response.StartsWith("{ \"error\":"))
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpGet(Name = "readuser")]
    public async Task<IActionResult> ReadUser([FromQuery] ReadUserRequest request)
    {
        var response = await _tradeService.ReadUserAsync(request.AuthToken, request.UserID);
        if (response.StartsWith("{ \"error\":"))
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}
