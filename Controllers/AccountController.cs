using AccountingApp.Data;
using AccountingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AccountingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext? _context;
        private readonly HashSet<string> _supportedCurrencies = new HashSet<string> { "RUB", "MNT" }; // MNT - код тугриков

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        private ClaimsIdentity? GetIdentity(HttpContext httpContext)
        { //вспомогательный метод для получения идентичности пользователя
            return httpContext.User.Identity as ClaimsIdentity;
        }

        private IActionResult ValidateUser(out User user) //вспомогательный метод валидации пользователя
        {
            user = null;
            var identity = GetIdentity(HttpContext);
            if (identity == null || !identity.IsAuthenticated)
            {
                return Unauthorized("Пользователь не аутентифицирован");
            }

            var username = identity.Name;
            user = _context.Users.First(u => u.Username == username);
            if (user == null)
            {
                return NotFound("Пользователя не существует");
            }
            return null;
        }
        private IActionResult ValidateAccount(int accountId, User user, out Account account)
        { //вспомогательный метод валидации счета
            account = _context.Accounts.First(a => a.Id == accountId && a.UserId == user.Id);
            if (account == null) //проверка существования-принадлежности счета
            {
                return NotFound("Счет не существует или не принадлежит пользователю");
            }
            return null;
        }

        [Authorize]
        [HttpPost("account/create")]
        public IActionResult CreateAccount(string currency)
        {
            if (string.IsNullOrEmpty(currency)) //проверка указания валюты
            {
                return BadRequest("Валюта не введена");
            }

            if (!_supportedCurrencies.Contains(currency.ToUpper())) 
            {
                return BadRequest("Валюта введена неверно");
            }

            var validationResult = ValidateUser(out var user);
            if (validationResult != null) //валидация пользователя
            {
                return validationResult;
            }

            var accountCounter = _context.Accounts.Count(a => a.UserId == user.Id);
            if (accountCounter >= 5) //проверка количества счетов
            {
                return BadRequest("Не больше 5 аккаунтов");
            }

            var newAccount = new Account //создание аккаунта
            {
                UserId = user.Id,
                Currency = currency.ToUpper(),
                Balance = 5000
            };
            _context.Accounts.Add(newAccount);
            _context.SaveChanges();

            return Ok(new { user.Username, AccountId = newAccount.Id, Currency = currency });
        }

        [Authorize]
        [HttpPost("account/convert")]
        public IActionResult ConvertCurrency(int accountId)
        {
            var validationResult = ValidateUser(out var user);
            if (validationResult != null) //валидация пользователя
            {
                return validationResult;
            }

            validationResult = ValidateAccount(accountId, user, out var account);
            if (validationResult != null) //валидация аккаунта
            {
                return validationResult;
            }
            if (!_supportedCurrencies.Contains(account.Currency)) //проверка валюты
            {
                return BadRequest("Некорректная валюта счета");
            }

            string targetCurrency = account.Currency == "RUB" ? "MNT" : "RUB";
            var currencyRate = _context.CurrencyRates.FirstOrDefault(
                                cr => cr.FromCurrency == account.Currency &&
                                cr.ToCurrency == targetCurrency);

            if (currencyRate == null) //проверка курса
            {
                return BadRequest("Курс конвертации для данной валюты не найден");
            }

            account.Balance *= currencyRate.Rate;
            account.Currency = targetCurrency;

            _context.SaveChanges();

            return Ok(new { AccountId = account.Id, NewCurrency = account.Currency, NewBalance = account.Balance });
        }

        [Authorize]
        [HttpPost("account/transaction")]
        public IActionResult TransactionMoney(int fromAccountId, int toAccountId, decimal amount)
        {
            // валидация пользователя(отправителя)
            var validationResult = ValidateUser(out var user);
            if (validationResult != null)
            {
                return validationResult;
            }

            // валидация счета отправителя
            validationResult = ValidateAccount(fromAccountId, user, out var fromAccount);
            if (validationResult != null)
            {
                return validationResult;
            }

            // проверка существования счета (не принадлежности)
            var toAccount = _context.Accounts.First(a => a.Id == toAccountId);
            if (toAccount == null)
            {
                return NotFound("Целевой счет не существует");
            }

            // проверка наличия средств у отправителя
            if (fromAccount.Balance < amount)
            {
                return BadRequest("Недостаточно средств на исходном счете");
            }

            // транзакция
            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            var transaction = new Transaction
            {
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                Amount = amount,
                Date = DateTime.UtcNow,
                Currency = fromAccount.Currency
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return Ok(new { transaction });
        }

        [Authorize]
        [HttpDelete("account/delete/{accountId}")]
        public IActionResult DeleteAccount(int accountId)
        {
            var validationResult = ValidateUser(out var user);
            if (validationResult != null) // валидация пользователя
            {
                return validationResult;
            }

            validationResult = ValidateAccount(accountId, user, out var account);
            if (validationResult != null) // валидация аккаунта
            {
                return validationResult;
            }

            _context.Accounts.Remove(account);
            _context.SaveChanges();

            return Ok(new { message = "Аккаунт удален" });
        }
    }
}