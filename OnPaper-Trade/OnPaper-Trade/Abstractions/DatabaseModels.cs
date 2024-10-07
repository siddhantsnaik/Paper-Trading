namespace OnPaper_Trade.Abstractions;

public class User
{
    public decimal Points { get; set; }
    public List<string> Watchlist { get; set; }
    public Dictionary<string, Trade> Trades { get; set; }
}


public class Trade
{
    public string? TradeId { get; set; }
    public string? StockCode { get; set; }
    public string? StockName { get; set; }
    public string? StockToken { get; set; }
    public string? ExchangeCode { get; set; }
    public decimal? EntryPrice { get; set; }
    public long? EntryTime { get; set; }
    public decimal? ExitPrice { get; set; }
    public long? ExitTime { get; set; }
    public bool IsActive { get; set; }
    public int? Quantity { get; set; }
    public string? EntryType { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }
}
