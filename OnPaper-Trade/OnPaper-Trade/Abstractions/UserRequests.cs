namespace OnPaper_Trade.Abstractions;
public class EnterTradeRequest
{
    public required string AuthToken { get; set; }
    public required string UserID { get; set; }
    public required string StockCode { get; set; }
    public required string StockName { get; set; }
    public required string StockToken { get; set; }
    public required string ExchangeCode { get; set; }
    public required long EntryTime { get; set; }
    public required decimal EntryPrice { get; set; }
    public required int Quantity { get; set; }
}

public class ExitTradeRequest 
{
    public required string AuthToken { get; set; }
    public required string UserID { get; set; }
    public required string TradeID { get; set; }
    public required decimal ExitPrice { get; set; }
    public required long ExitTime { get; set; }
}

public class ReadUserRequest
{
    public required string AuthToken { get; set; }
    public required string UserID { get; set; }
}

public class CustomerDetailsResponse
{
    public SuccessData Success { get; set; }
    public int Status { get; set; }
    public object Error { get; set; }
}

public class SuccessData
{
    public Dictionary<string, string> exg_trade_date { get; set; }
    public Dictionary<string, string> exg_status { get; set; }
    public Dictionary<string, string> segments_allowed { get; set; }
    public string idirect_userid { get; set; }
    public string session_token { get; set; }
    public string idirect_user_name { get; set; }
    public string idirect_ORD_TYP { get; set; }
    public string idirect_lastlogin_time { get; set; }
    public string mf_holding_mode_popup_flg { get; set; }
    public string commodity_exchange_status { get; set; }
    public string commodity_trade_date { get; set; }
    public string commodity_allowed { get; set; }
}

public class WatchListRequest
{
    public required string AuthToken { get; set; }
    public required string UserID { get; set; }
    public string StockToken { get; set; }
    public string? StockName { get; set; }
    public string? StockCode { get; set; }
    public string? ExchangeCode { get; set; }
}