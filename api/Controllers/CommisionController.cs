using Microsoft.AspNetCore.Mvc;

namespace AvalphaTechnologies.CommissionCalculator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommisionController : ControllerBase
    {
        [ProducesResponseType(typeof(CommissionCalculationResponse), 200)]
        [ProducesResponseType(400)]
        [HttpPost]
        public IActionResult Calculate([FromBody] CommissionCalculationRequest calculationRequest)
        {
            if (calculationRequest == null)
                return BadRequest(new { error = "Request body is required." });

            // Basic validation
            if (calculationRequest.LocalSalesCount < 0 || calculationRequest.ForeignSalesCount < 0 || calculationRequest.AverageSaleAmount < 0)
            {
                return BadRequest(new { error = "Values must be >= 0." });
            }

            // Sensible upper bounds to avoid overflow / abuse
            const int MaxSalesCount = 1_000_000;
            const decimal MaxAverageAmount = 1_000_000_000M;

            if (calculationRequest.LocalSalesCount > MaxSalesCount
                || calculationRequest.ForeignSalesCount > MaxSalesCount
                || calculationRequest.AverageSaleAmount > MaxAverageAmount)
            {
                return BadRequest(new { error = "One or more values exceed allowed limits." });
            }

            // Business rates
            const decimal AvalphaLocalRate = 0.20M;   // 20%
            const decimal AvalphaForeignRate = 0.35M; // 35%
            const decimal CompetitorLocalRate = 0.02M;    // 2%
            const decimal CompetitorForeignRate = 0.0755M; // 7.55%

            var localAvalpha = AvalphaLocalRate * calculationRequest.LocalSalesCount * calculationRequest.AverageSaleAmount;
            var foreignAvalpha = AvalphaForeignRate * calculationRequest.ForeignSalesCount * calculationRequest.AverageSaleAmount;
            var avalphaTotal = localAvalpha + foreignAvalpha;

            var localCompetitor = CompetitorLocalRate * calculationRequest.LocalSalesCount * calculationRequest.AverageSaleAmount;
            var foreignCompetitor = CompetitorForeignRate * calculationRequest.ForeignSalesCount * calculationRequest.AverageSaleAmount;
            var competitorTotal = localCompetitor + foreignCompetitor;

            var response = new CommissionCalculationResponse
            {
                AvalphaTechnologiesLocal = decimal.Round(localAvalpha, 2),
                AvalphaTechnologiesForeign = decimal.Round(foreignAvalpha, 2),
                AvalphaTechnologiesCommissionAmount = decimal.Round(avalphaTotal, 2),

                CompetitorLocal = decimal.Round(localCompetitor, 2),
                CompetitorForeign = decimal.Round(foreignCompetitor, 2),
                CompetitorCommissionAmount = decimal.Round(competitorTotal, 2)
            };

            return Ok(response);
        }
    }

    public class CommissionCalculationRequest
    {
        public int LocalSalesCount { get; set; }
        public int ForeignSalesCount { get; set; }
        public decimal AverageSaleAmount { get; set; }
    }

    public class CommissionCalculationResponse
    {
        // Avalpha breakdown
        public decimal AvalphaTechnologiesLocal { get; set; }
        public decimal AvalphaTechnologiesForeign { get; set; }
        public decimal AvalphaTechnologiesCommissionAmount { get; set; }

        // Competitor breakdown
        public decimal CompetitorLocal { get; set; }
        public decimal CompetitorForeign { get; set; }
        public decimal CompetitorCommissionAmount { get; set; }
    }
}
