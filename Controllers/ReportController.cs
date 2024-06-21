using AccountingApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AccountingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("report")]
        public IActionResult GetReport(DateTime? startDate, DateTime? endDate, string currency, int? accountId)
        {
            // Проверяем, что идентичность не null и аутентифицирована
            if (HttpContext.User.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            {
                return Unauthorized("Пользователь не аутентифицирован");
            }

            // Преобразуем даты в UTC
            startDate = startDate.HasValue ? DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc) : (DateTime?)null;
            endDate = endDate.HasValue ? DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc) : (DateTime?)null;

            // Проверяем корректность даты
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return BadRequest("Неверно указана дата начала/окончания");
            }

            // Извлекаем имя пользователя
            var username = identity.Name;

            // Ищем пользователя в базе данных по имени пользователя
            var user = _context.Users.First(u => u.Username == username);
            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }

            // Фильтрация данных транзакций по счету и дате
            var query = _context.Transactions.AsQueryable();

            // Фильтрация транзакций по счетам текущего пользователя
            var userAccountIds = _context.Accounts
                .Where(a => a.UserId == user.Id)
                .Select(a => a.Id)
                .ToList();

            query = query.Where(t => userAccountIds.Contains(t.FromAccountId) || userAccountIds.Contains(t.ToAccountId));

            if (accountId.HasValue) //показывает только те транзакции, где From совпадает с AccountId
            {
                query = query.Where(t => t.FromAccountId == accountId.Value);
            }

            if (startDate.HasValue) //првоерка даты
            {
                query = query.Where(t => t.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.Date <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(currency)) //проверка валюты
            {
                currency = currency.ToUpper();
                if (currency == "RUB" || currency == "MNT")
                {
                    query = query.Where(t => t.Currency.ToUpper() == currency);
                }
                else
                {
                    return BadRequest("Неверно указана валюта");
                }
            }

            // Извлечение данных из бд
            var transactions = query.ToList();

            // Проверка наличия транзакций
            if (!transactions.Any())
            {
                return Ok(new { message = "Транзакций нет за указанный период" });
            }

            // Преобразование данных в формат JSON
            var reportData = transactions.Select(t => new
            {
                t.Date,
                t.Currency,
                t.Amount,
                t.Id,
                t.FromAccountId,
                t.ToAccountId
            }).ToList();

            // Возврат данных в формате JSON
            return Ok(reportData);
        }
    }
}