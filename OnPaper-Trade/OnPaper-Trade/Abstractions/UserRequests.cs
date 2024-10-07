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
    public required int EntryPrice { get; set; }
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