using FluentAssertions;
using Moq;
using Moq.Protected;
using PortfolioBlazorWasm.Models.UkBankPa;
using PortfolioBlazorWasm.Models.UkBankPa.ClassMaps;
using PortfolioBlazorWasm.Services.CsvHelper;
using System.Net;
using System.Text;
using Xunit;

namespace PortfolioBlazorWasm.Tests.Services.CsvHelper;

public class CsvHelperServiceTests
{

    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _mockHttpClient;

    public CsvHelperServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _mockHttpClient = new HttpClient(_mockHttpMessageHandler.Object);
    }

    private CsvHelperService CreateService()
    {
        return new CsvHelperService(_mockHttpClient);
    }

    [Fact]
    public async Task GetDataFromCsv_ReturnsExpectedData()
    {
        // Arrange
        var testDataPath = Path.Combine(AppContext.BaseDirectory, "TestData", "TestBankRatesData.csv");
        var testCsvContent = File.ReadAllText(testDataPath, Encoding.UTF8);
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(testCsvContent)
            })
            .Verifiable();

        var csvHelperService = CreateService();

        // Act
        var result = await csvHelperService.GetDataFromCsv<BankRate, BankRateMap>("http://example.com/test.csv");

        // Assert
        result.Should().HaveCount(2);
        result[0].DateChanged.Should().Be(new DateOnly(2023, 3, 23));
        result[0].Rate.Should().Be(4.25);
        result[0].PercentageChanged.Should().Be(6.25m);
        result[1].DateChanged.Should().Be(new DateOnly(2023, 2, 2));
        result[1].Rate.Should().Be(4.00);
        result[1].PercentageChanged.Should().Be(14.29m);
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }
    [Fact]
    public async Task GetDataFromCsv_ReturnsExpectedPersonalAllowanceData()
    {
        // Arrange
        var testDataPath = Path.Combine(AppContext.BaseDirectory, "TestData", "TestUKPaData.csv");
        var testCsvContent = File.ReadAllText(testDataPath, Encoding.UTF8);
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(testCsvContent)
            })
            .Verifiable();
        var csvHelperService = CreateService();

        // Act
        var result = await csvHelperService.GetDataFromCsv<PersonalAllowance, PersonalAllowanceMap>("http://example.com/test.csv");

        // Assert
        result.Should().HaveCount(2);
        result[0].ToDate.Should().Be(new DateOnly(2023, 4, 6));
        result[0].AllowanceAmountGBP.Should().Be(12570m);
        result[0].PercentageChanged.Should().Be(0.00m);
        result[1].ToDate.Should().Be(new DateOnly(1976, 4, 6));
        result[1].AllowanceAmountGBP.Should().Be(675m);
        result[1].PercentageChanged.Should().Be(8.00m);
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }
}