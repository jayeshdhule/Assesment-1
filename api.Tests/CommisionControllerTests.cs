using Xunit;
using Microsoft.AspNetCore.Mvc;
using AvalphaTechnologies.CommissionCalculator.Controllers;

namespace AvalphaTechnologies.CommissionCalculator.Tests
{
    public class CommisionControllerTests
    {
        private readonly CommisionController _controller = new();

        [Fact]
        public void Calculate_WithValidInput_ReturnsOkResult()
        {
            // Arrange
            var request = new CommissionCalculationRequest
            {
                LocalSalesCount = 10,
                ForeignSalesCount = 5,
                AverageSaleAmount = 1000
            };

            // Act
            var result = _controller.Calculate(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<CommissionCalculationResponse>(okResult.Value);
            
            // Validate calculations
            Assert.Equal(2000m, response.AvalphaTechnologiesLocal);        // 0.20 * 10 * 1000
            Assert.Equal(1750m, response.AvalphaTechnologiesForeign);      // 0.35 * 5 * 1000
            Assert.Equal(3750m, response.AvalphaTechnologiesCommissionAmount); // 2000 + 1750
            
            Assert.Equal(200m, response.CompetitorLocal);                  // 0.02 * 10 * 1000
            Assert.Equal(377.5m, response.CompetitorForeign);             // 0.0755 * 5 * 1000
            Assert.Equal(577.5m, response.CompetitorCommissionAmount);    // 200 + 377.5
        }

        [Fact]
        public void Calculate_WithNullRequest_ReturnsBadRequest()
        {
            // Act
            var result = _controller.Calculate(null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult.Value);
        }

        [Theory]
        [InlineData(-1, 5, 1000)]
        [InlineData(5, -1, 1000)]
        [InlineData(5, 5, -1000)]
        public void Calculate_WithNegativeValues_ReturnsBadRequest(int localCount, int foreignCount, decimal avgAmount)
        {
            // Arrange
            var request = new CommissionCalculationRequest
            {
                LocalSalesCount = localCount,
                ForeignSalesCount = foreignCount,
                AverageSaleAmount = avgAmount
            };

            // Act
            var result = _controller.Calculate(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData(1_000_001, 5, 1000)]
        [InlineData(5, 1_000_001, 1000)]
        [InlineData(5, 5, 1_000_000_001)]
        public void Calculate_WithValuesExceedingLimits_ReturnsBadRequest(int localCount, int foreignCount, decimal avgAmount)
        {
            // Arrange
            var request = new CommissionCalculationRequest
            {
                LocalSalesCount = localCount,
                ForeignSalesCount = foreignCount,
                AverageSaleAmount = avgAmount
            };

            // Act
            var result = _controller.Calculate(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Calculate_RoundsResultsToTwoDecimalPlaces()
        {
            // Arrange
            var request = new CommissionCalculationRequest
            {
                LocalSalesCount = 3,
                ForeignSalesCount = 2,
                AverageSaleAmount = 100.33m
            };

            // Act
            var result = _controller.Calculate(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<CommissionCalculationResponse>(okResult.Value);

            // All values should have at most 2 decimal places
            Assert.Equal(response.AvalphaTechnologiesLocal, decimal.Round(response.AvalphaTechnologiesLocal, 2));
            Assert.Equal(response.AvalphaTechnologiesForeign, decimal.Round(response.AvalphaTechnologiesForeign, 2));
            Assert.Equal(response.CompetitorLocal, decimal.Round(response.CompetitorLocal, 2));
            Assert.Equal(response.CompetitorForeign, decimal.Round(response.CompetitorForeign, 2));
        }

        [Fact]
        public void Calculate_WithZeroValues_ReturnsZeroCommissions()
        {
            // Arrange
            var request = new CommissionCalculationRequest
            {
                LocalSalesCount = 0,
                ForeignSalesCount = 0,
                AverageSaleAmount = 0
            };

            // Act
            var result = _controller.Calculate(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<CommissionCalculationResponse>(okResult.Value);

            Assert.Equal(0m, response.AvalphaTechnologiesCommissionAmount);
            Assert.Equal(0m, response.CompetitorCommissionAmount);
        }
    }
}
