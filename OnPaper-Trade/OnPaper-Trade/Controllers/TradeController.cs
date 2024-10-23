using Microsoft.AspNetCore.Mvc;
using System;
using OnPaper_Trade.Abstractions;
using OnPaper_Trade.Services;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using FirebaseAdmin.Auth;
using System.Diagnostics.Contracts;


namespace OnPaper_Trade.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class tradeController : Controller
{
    private readonly TradeService _tradeService;
    private readonly HttpClient _httpClient;

    public tradeController(TradeService tradeService, IHttpClientFactory httpClientFactory)
    {
        _tradeService = tradeService;
        _httpClient = httpClientFactory.CreateClient();
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

    [HttpPut]
    public async Task<IActionResult> AddToWatchList([FromBody] WatchListRequest request)
    {
        WatchItem watchItem = new WatchItem();
        watchItem.ExchangeCode = request.ExchangeCode;
        watchItem.StockCode = request.StockCode;
        watchItem.StockToken = request.StockToken;
        watchItem.StockName = request.StockName;
        var response = await _tradeService.AddToWatchList(request.AuthToken, request.UserID, watchItem);

        if (response.StartsWith("{ \"error\":"))
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveFromWatch([FromBody] WatchListRequest request)
    {
        string AuthToken = request.AuthToken;
        string UserId = request.UserID;
        string StockToken = request.StockToken;

        var response = await _tradeService.RemoveFromWatchList(AuthToken, UserId, StockToken);

        if (response.StartsWith("{ \"error\":"))
        {
            return BadRequest(response);
        }
        return Ok(response);

    }

    [HttpGet]
    public async Task<IActionResult> ReadApiSession([FromQuery]  string apisession)
    {
        //?apisession = 47990536
        if (string.IsNullOrEmpty(apisession))
        {
            return BadRequest("Session token is missing");
        }

        string hostname = "https://api.icicidirect.com/breezeapi/api/v1/";
        string endpoint = "customerdetails";
        string url = $"{hostname}{endpoint}";


        string jsonPayload = $"{{" +
            $"\"SessionToken\": \"{apisession}\"," +
            $"\"AppKey\": \"+9352295AT1652ns9X364f81F298A115\"" +
            $"}}";

        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        _httpClient.BaseAddress = new Uri("https://api.icicidirect.com");
        //var response = await _httpClient.GetAsync("/breezeapi/api/v1/customerdetails", content);

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Content = content;

        HttpResponseMessage response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Failed to fetch customer details");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<CustomerDetailsResponse>(responseBody);

        if (data?.Success?.session_token == null)
        {
            return BadRequest("Invalid response from ICICI Direct API "+responseBody);
        }

        var decodedSessionKey = Encoding.ASCII.GetString(Convert.FromBase64String(data.Success.session_token));
        var parts = decodedSessionKey.Split(':');

        if (parts.Length != 2)
        {
            return BadRequest("Invalid session key format");
        }

        var userId = parts[0];
        var sessionTokenDecoded = parts[1];
        var issueDateTime = DateTime.UtcNow; // Capture the current UTC time

        // Store in Firebase
        var storeResult = await StoreInFirebase(userId, sessionTokenDecoded, issueDateTime);
        if (!storeResult)
        {
            return StatusCode(500, "Failed to store data in Firebase");
        }

        return Ok(new { Message = "Session info processed and stored successfully", UserId = userId });

    }

    [HttpGet]
    public async Task<IActionResult> GetAPIAsync()
    {
        var firebaseUrl = "https://onpaper-auth-default-rtdb.asia-southeast1.firebasedatabase.app/apicredentials.json";

        var response = await _httpClient.GetAsync(firebaseUrl);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
            return Ok(data);
        }
        else
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            return BadRequest(errorResponse);
        }
    }

    private async Task<bool> StoreInFirebase(string userId, string sessionToken, DateTime issueDateTime)
    {
        var firebaseUrl = "https://onpaper-auth-default-rtdb.asia-southeast1.firebasedatabase.app/apicredentials.json";

        var payload = new
        {
            UserId = userId,
            SessionToken = sessionToken,
            IssueDate = issueDateTime.ToString("yyyy-MM-dd"),
            IssueTime = issueDateTime.ToString("HH:mm:ss"),
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync(firebaseUrl, content);

        return response.IsSuccessStatusCode;
    }

}



