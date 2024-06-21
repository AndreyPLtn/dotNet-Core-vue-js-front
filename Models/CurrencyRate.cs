namespace AccountingApp.Models
{
    public class CurrencyRate
    {
        public int Id { get; set; }
        public string? FromCurrency { get; set; }
        public string? ToCurrency { get; set; }
        public decimal Rate { get; set; }
    }
}
