using Xunit;
using AvalphaTechnologies.CommissionCalculator.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace AvalphaTechnologies.CommissionCalculator.Tests
{
    public class CommissionCalculationTests
    {
        [Fact]
        public void CalculatesCorrectly_ForTypicalInput()
        {
            var controller = new CommisionController();

            var request = new CommissionCalculationRequest
            {
                LocalSalesCount = 10,
                ForeignSalesCount = 10,
                AverageSaleAmount = 100m
            };

            var result = controller.Calculate(request) as OkObjectResult;
            Assert.NotNull(result);

            var response = Assert.IsType<CommissionCalculationResponse>(result.Value);

            // Avalpha: local 20% * 10 * 100 = 200
            Assert.Equal(200m, response.AvalphaTechnologiesLocal);
            // Avalpha: foreign 35% * 10 * 100 = 350
            Assert.Equal(350m, response.AvalphaTechnologiesForeign);
            Assert.Equal(550m, response.AvalphaTechnologiesCommissionAmount);

            // Competitor: local 2% * 10 * 100 = 20
            Assert.Equal(20m, response.CompetitorLocal);
            // Competitor: foreign 7.55% * 10 * 100 = 75.5
            Assert.Equal(75.5m, response.CompetitorForeign);
            Assert.Equal(95.5m, response.CompetitorCommissionAmount);
        }

        [Fact]
        public void ReturnsBadRequest_ForNegativeValues()
        {
            var controller = new CommisionController();

            var request = new CommissionCalculationRequest
            {
                LocalSalesCount = -1,
                ForeignSalesCount = 0,
                AverageSaleAmount = 10m
            };

            var result = controller.Calculate(request);
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
