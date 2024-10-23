using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace OnPaper_Trade.Abstractions;

public enum FirebaseRealtimeDatabaseEndpointsEnum
{
    CreateUser,
    ReadUser,
    UpdateUser,
    GetPoints,
    AddPoints,
    GetWatchlist,
    AddToWatchlist,
    RemoveFromWatchlist,
    CreateTrade,
    ReadTrade,
    UpdateTrade,
    DeleteTrade
}

public class FirebaseRealtimeDatabaseEndpoint
{
    private readonly string _baseUrl;
    private readonly string _authToken;
    private readonly HttpClient _httpClient;

    public FirebaseRealtimeDatabaseEndpoint(string baseUrl, string authToken)
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _authToken = authToken;
        _httpClient = new HttpClient();
        //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken );
    }

    private string FormatErrorMessage(string errorMessage)
    {
        return $@"
            {{
                ""error"": {{
                    ""errors"": [
                        {{
                            ""domain"": ""global"",
                            ""reason"": ""invalid"",
                            ""source"": ""API EndPointFactory"",
                            ""message"": ""{errorMessage}""
                        }}
                    ],
                    ""code"": 400,
                    ""message"": ""{errorMessage}""
                }}
            }}";
    }
    public async Task<string> SendRequestAsync(FirebaseRealtimeDatabaseEndpointsEnum endpointType, string userId, object payload)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return FormatErrorMessage($"User ID cannot be null or empty {nameof(userId)}");
        }

        var url = $"{_baseUrl}/users/{userId}";
        HttpResponseMessage response;

        try
        {
            switch (endpointType)
            {
                case FirebaseRealtimeDatabaseEndpointsEnum.CreateUser:                 
                    url += ".json";
                    if (payload is not User)
                    {
                        throw new ArgumentException("Payload must be of type User for CreateUser operation");
                    }
                    var userContent = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
                    response = await _httpClient.PutAsync(AppendAuth(url), userContent);
                    break;

                case FirebaseRealtimeDatabaseEndpointsEnum.ReadUser:
                    url += ".json";
                    response = await _httpClient.GetAsync(AppendAuth(url));
                    break;

                case FirebaseRealtimeDatabaseEndpointsEnum.UpdateUser:
                    url += ".json";
                    var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
                    response = await _httpClient.PatchAsync(AppendAuth(url), content);
                    break;

                case FirebaseRealtimeDatabaseEndpointsEnum.GetPoints:
                    url += "/Points.json";
                    response = await _httpClient.GetAsync(AppendAuth(url));
                    break;

                case FirebaseRealtimeDatabaseEndpointsEnum.AddPoints:
                    url += ".json";
                    Console.WriteLine(url);
                    content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
                    Console.WriteLine(payload);
                    response = await _httpClient.PatchAsync(AppendAuth(url), content);
                    Console.WriteLine($"Headers: {response.Headers} \n Response: {response.Content}");
                    break;

                case FirebaseRealtimeDatabaseEndpointsEnum.GetWatchlist:
                    url += "/watchlist.json";
                    response = await _httpClient.GetAsync(AppendAuth(url));
                    break;

                case FirebaseRealtimeDatabaseEndpointsEnum.AddToWatchlist:
                    var watchItem = (WatchItem)payload;
                    var watchSymbol = watchItem.StockToken;
                    url += $"/watchlist/{watchSymbol}.json";
                    content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
                    response = await _httpClient.PutAsync(AppendAuth(url), content);
                    break;
                case FirebaseRealtimeDatabaseEndpointsEnum.RemoveFromWatchlist:
                    if(payload is string watchID)
                    { 
                        url += $"/watchlist/{watchID}.json"; 
                        content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
                        response = await _httpClient.DeleteAsync(AppendAuth(url));
                    }
                    else
                    {
                        throw new ArgumentException("Payload must be a string (watchID) for Delete Watch Item operation");
                    }
                    break;

                case FirebaseRealtimeDatabaseEndpointsEnum.CreateTrade:
                    if(payload is not Trade)
                    {
                        throw new ArgumentException("Payload must be of type Trade for CreateTrade operation");
                    }

                    var newTrade = (Trade)payload;
                    var newTradeId = newTrade.TradeId;
                    url += $"/trades/{newTradeId}.json";
                    content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
                    response = await _httpClient.PutAsync(AppendAuth(url), content);
                    break;

                case FirebaseRealtimeDatabaseEndpointsEnum.ReadTrade:
                    if (payload is string tradeId)
                    {
                        url += $"/trades/{tradeId}.json";
                        response = await _httpClient.GetAsync(AppendAuth(url));
                    }
                    else
                    {
                        throw new ArgumentException("Payload must be a string (tradeId) for ReadTrade operation");
                    }
                    break;

                case FirebaseRealtimeDatabaseEndpointsEnum.UpdateTrade:
                    if (payload is Trade trade)
                    {
                        url += $"/trades/{trade.TradeId}.json";
                        content = new StringContent(JsonSerializer.Serialize(trade), System.Text.Encoding.UTF8, "application/json");
                        response = await _httpClient.PatchAsync(AppendAuth(url), content);
                    }
                    else
                    {
                        throw new ArgumentException("Payload must be of type Trade for UpdateTrade operation");
                    }
                    break;

                case FirebaseRealtimeDatabaseEndpointsEnum.DeleteTrade:
                    if (payload is string deltradeId)
                    {
                        url += $"/trades/{deltradeId}.json";
                        response = await _httpClient.DeleteAsync(AppendAuth(url));
                    }
                    else
                    {
                        throw new ArgumentException("Payload must be a string (tradeId) for DeleteTrade operation");
                    }
                    break;

                default:
                    throw new ArgumentException("Invalid endpoint type", nameof(endpointType));
            }

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"HTTP request error: {e.ToString()}");
            return FormatErrorMessage($"HTTP request error: {e.Message.Split('\n')[0]}" );
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error sending request: {e.Message}");
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
    }


    private string AppendAuth(string url)
    {
        return $"{url}?auth={_authToken}";
    }
}

public class FirebaseRealtimeDatabaseEndpointFactory
{
    private readonly string _baseUrl;
    private readonly string _authToken;

    public FirebaseRealtimeDatabaseEndpointFactory(string authToken)
    {
        _baseUrl = "https://onpaper-auth-default-rtdb.asia-southeast1.firebasedatabase.app/";
        _authToken = authToken ?? throw new ArgumentNullException(nameof(authToken));
    }

    public FirebaseRealtimeDatabaseEndpoint CreateEndpoint()
    {
        return new FirebaseRealtimeDatabaseEndpoint(_baseUrl, _authToken);
    }
}

