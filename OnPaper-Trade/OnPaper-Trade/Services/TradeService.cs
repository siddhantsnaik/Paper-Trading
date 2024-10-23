using FirebaseAdmin.Auth;
using FirebaseAdmin;
using OnPaper_Trade.Abstractions;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OnPaper_Trade.Services;

public class TradeService
{
    private readonly FirebaseApp _firebaseApp;
    private readonly FirebaseAuth _firebaseAuth;
    public TradeService(FirebaseApp firebaseApp, FirebaseAuth firebaseAuth)
    {
        _firebaseApp = firebaseApp;
        _firebaseAuth = firebaseAuth;
    }

    private string FormatErrorMessage(string errorMessage)
    {
        return $@"
                {{
                    ""error"": {{
                        ""errors"": [
                            {{
                                ""domain"": ""global"",
                                ""source"": ""API"",
                                ""reason"": ""invalid"",
                                ""message"": ""{errorMessage}""
                            }}
                        ],
                        ""code"": 400,
                        ""message"": ""{errorMessage}""
                    }}
                }}";
    }

    public async Task<bool> IsValidUser(string authToken, string userID)
    {
        FirebaseToken token = await _firebaseAuth.VerifyIdTokenAsync(authToken);
        if (token is null || token.Uid != userID)
        {
            Console.WriteLine($"Firebase Token: {token.Uid} Passed Token: {userID} AreEqual?: {token.Uid == userID}");
            return false;
        }
        return true;
    }

    public async Task<string> CreateTradeAsync(string authToken, string userId, Trade trade)
    {
        if (!await IsValidUser(authToken, userId))
        {
            return FormatErrorMessage("Invalid User Session");
        }

        var endpoint = new FirebaseRealtimeDatabaseEndpointFactory(authToken).CreateEndpoint();
        try
        {
            return await endpoint.SendRequestAsync(FirebaseRealtimeDatabaseEndpointsEnum.CreateTrade, userId, trade);
        }
        catch (FirebaseAuthException e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
        catch (Exception e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
    }

    public async Task<string> ReadTradeAsync(string authToken, string userId, string tradeId)
    {
        if (!await IsValidUser(authToken, userId))
        {
            return FormatErrorMessage("Invalid User Session");
        }

        var endpoint = new FirebaseRealtimeDatabaseEndpointFactory(authToken).CreateEndpoint();
        try
        {
            var response = await endpoint.SendRequestAsync(FirebaseRealtimeDatabaseEndpointsEnum.ReadTrade, userId, tradeId);
            if (string.IsNullOrEmpty(response))
            {
                return FormatErrorMessage("Trade not found");
            }
            return response;
        }
        catch (FirebaseAuthException e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
        catch (Exception e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
    }

    private async Task<string> UpdateTradeAsync(string authToken, string userId, Trade trade)
    {
        if (!await IsValidUser(authToken, userId))
        {
            return FormatErrorMessage("Invalid User Session");
        }

        var endpoint = new FirebaseRealtimeDatabaseEndpointFactory(authToken).CreateEndpoint();
        try
        {
            return await endpoint.SendRequestAsync(FirebaseRealtimeDatabaseEndpointsEnum.UpdateTrade, userId, trade);
        }
        catch (FirebaseAuthException e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
        catch (Exception e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
    }

    public async Task<string> DeleteTradeAsync(string authToken, string userId, string tradeId)
    {
        if (!await IsValidUser(authToken, userId))
        {
            return FormatErrorMessage("Invalid User Session");
        }

        var checkTrade = JsonConvert.DeserializeObject<Trade>(await ReadTradeAsync(authToken, userId, tradeId));
        if (checkTrade is null)
        {
            return FormatErrorMessage("Trade not found");
        }
        if (checkTrade.IsActive)
        {
            return FormatErrorMessage("Trade is still Active, Please Exit it");
        }

        var endpoint = new FirebaseRealtimeDatabaseEndpointFactory(authToken).CreateEndpoint();
        try
        {
            return await endpoint.SendRequestAsync(FirebaseRealtimeDatabaseEndpointsEnum.DeleteTrade, userId, tradeId);
        }
        catch (FirebaseAuthException e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
        catch (Exception e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
    }

    public async Task<string> EnterTradeAsync(string authToken, string userId, Trade trade)
    {
        trade.ExitPrice = null;
        trade.ExitTime = null;
        trade.IsActive = true;
        trade.TakeProfit = null;
        trade.StopLoss = null;

        decimal points = await GetPointsAsync(authToken, userId);
        if (points == -999)
        {
            return FormatErrorMessage("Error getting points");
        }
        if (points < 100 || trade.EntryPrice > points)
        {
            return FormatErrorMessage("Insufficient Points");
        }

        await AddPointsAsync(authToken, userId, (decimal)-trade.EntryPrice);
        return await CreateTradeAsync(authToken, userId, trade);
    }

    public async Task<string> ExitTradeAsync(string authToken, string userId, string tradeId, decimal exitPrice, long exitTime)
    {
        var trade = JsonConvert.DeserializeObject<Trade>(await ReadTradeAsync(authToken, userId, tradeId));
        if (trade is null)
        {
            return FormatErrorMessage("Trade not found");
        }
        if (!trade.IsActive)
        {
            return FormatErrorMessage("Trade is not Active");
        }

        trade.ExitPrice = exitPrice;
        trade.ExitTime = exitTime;
        trade.IsActive = false;
        trade.TakeProfit = trade.ExitPrice - trade.EntryPrice;

        await AddPointsAsync(authToken, userId, (decimal)trade.ExitPrice);
        return await UpdateTradeAsync(authToken, userId, trade);
    }

    public async Task<string> CreateUserAsync(string authToken, string userId)
    {
        if (!await IsValidUser(authToken, userId))
        {
            return FormatErrorMessage("Invalid User Session");
        }

        User user = new User();
        user.Points = 100000;
        user.Watchlist = new Dictionary<string,WatchItem>();
        user.Trades = new Dictionary<string, Trade>();

        var endpoint = new FirebaseRealtimeDatabaseEndpointFactory(authToken).CreateEndpoint();
        try
        {
            return await endpoint.SendRequestAsync(FirebaseRealtimeDatabaseEndpointsEnum.CreateUser, userId, user);
        }
        catch (FirebaseAuthException e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
        catch (Exception e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
    }

    public async Task<string> ReadUserAsync(string authToken, string userId)
    {
        if (!await IsValidUser(authToken, userId))
        {
            return FormatErrorMessage("Invalid User Session");
        }

        var endpoint = new FirebaseRealtimeDatabaseEndpointFactory(authToken).CreateEndpoint();
        try
        {
            var response = await endpoint.SendRequestAsync(FirebaseRealtimeDatabaseEndpointsEnum.ReadUser, userId, new { });
            if (string.IsNullOrEmpty(response) || response == "null")
            {
                return await CreateUserAsync(authToken, userId);
            }
            return response;
        }
        catch (FirebaseAuthException e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
        catch (Exception e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
    }

    public async Task<decimal> GetPointsAsync(string authToken, string userId)
    {
        var endpoint = new FirebaseRealtimeDatabaseEndpointFactory(authToken).CreateEndpoint();
        try
        {
            var response = await endpoint.SendRequestAsync(FirebaseRealtimeDatabaseEndpointsEnum.GetPoints, userId, new { });
            return decimal.TryParse(response, out decimal points) ? points : -999;
        }
        catch (FirebaseAuthException)
        {
            return -999;
        }
        catch (Exception)
        {
            return -999;
        }
    }

    public async Task<string> AddPointsAsync(string authToken, string userId, decimal addPoints)
    {
        if (!await IsValidUser(authToken, userId))
        {
            return FormatErrorMessage("Invalid User Session");
        }

        var points = await GetPointsAsync(authToken, userId);
        if (points == -999)
        {
            return FormatErrorMessage("Error getting points");
        }

        points += addPoints;

        var endpoint = new FirebaseRealtimeDatabaseEndpointFactory(authToken).CreateEndpoint();
        try
        {
            return await endpoint.SendRequestAsync(FirebaseRealtimeDatabaseEndpointsEnum.AddPoints, userId, new { Points = points });
        }
        catch (FirebaseAuthException e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
        catch (Exception e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
    }

    public async Task<string> AddToWatchList(string authToken, string userId, WatchItem watch)
    {
        if (!await IsValidUser(authToken, userId))
        {
            return FormatErrorMessage("Invalid User Session");
        }

        var endpoint = new FirebaseRealtimeDatabaseEndpointFactory(authToken).CreateEndpoint();

        try
        {
            return await endpoint.SendRequestAsync(FirebaseRealtimeDatabaseEndpointsEnum.AddToWatchlist, userId, watch);
        }
        catch (FirebaseAuthException e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
        catch (Exception e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
    }
    public async Task<string> RemoveFromWatchList(string authToken, string userId, string watchItem)
    {
        if (!await IsValidUser(authToken, userId))
        {
            return FormatErrorMessage("Invalid User Session");
        }

        var endpoint = new FirebaseRealtimeDatabaseEndpointFactory(authToken).CreateEndpoint();

        try
        {
            return await endpoint.SendRequestAsync(FirebaseRealtimeDatabaseEndpointsEnum.RemoveFromWatchlist, userId, watchItem);
        }
        catch (FirebaseAuthException e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
        catch (Exception e)
        {
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
    }
}
