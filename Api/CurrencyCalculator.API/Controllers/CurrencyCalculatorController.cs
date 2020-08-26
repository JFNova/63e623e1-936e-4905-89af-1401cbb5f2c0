using System;
using System.Linq;
using System.Threading.Tasks;
using CurrencyCalculator.Core;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyCalculator.API.Controllers
{
    [ApiController]
    [EnableCors("AllowOrigin")]
    [Route("[controller]")]
    public class CurrencyCalculatorController : ControllerBase
    {
        private readonly ICurrencyConverter _currencyConverter;
        private readonly ICurrencyService _currencyService;

        public CurrencyCalculatorController(ICurrencyConverter currencyConverter, ICurrencyService currencyService)
        {
            _currencyConverter = currencyConverter;
            _currencyService = currencyService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("ListCurrenciesAsync")]
        public async Task<IActionResult> ListCurrenciesAsync()
        {
            var currencies = await _currencyService.Currencies();

			if (currencies == null && !currencies.Any())
			{
                return NotFound();
			}

			return Ok(currencies);
        }

        [HttpGet("ConvertAsync/{fromAmount}/{exchangeDate}/{fromCurrencyId}/{toCurrencyId}")]
        public async Task<IActionResult> ConvertAsync(double fromAmount, DateTime exchangeDate, string fromCurrencyId, string toCurrencyId)
        {
            var convertedAmount = await _currencyConverter.ConvertAmount(fromCurrencyId, toCurrencyId, exchangeDate, fromAmount);
            return Ok(convertedAmount);
        }

        [HttpPost("TotalAsync")]
        public async Task<IActionResult> TotalAsync([FromBody]TotalRequest request)
        {
            var totalAmount = await _currencyConverter.TotalAmount(request.ToCurrencyId, request.Amounts);
            return Ok(totalAmount);
        }
    }
}
