namespace AccountingApp.Models
{
    public class Account
    {
        public int Id { get; set; } //id первичного ключа
        public string? Currency { get; set; } //валюта
        public decimal Balance { get; set; } // баланс рубли
        public int UserId { get; set; } //принадлежность счета
        public User? User { get; set; } //связь с пользователем
    }
}