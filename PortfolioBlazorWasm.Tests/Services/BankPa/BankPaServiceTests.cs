using FluentAssertions;
using Moq;
using PortfolioBlazorWasm.Models.UkBankPa;
using PortfolioBlazorWasm.Models.UkBankPa.ClassMaps;
using PortfolioBlazorWasm.Services.BankPa;
using PortfolioBlazorWasm.Services.CsvHelper;
using Xunit;

namespace PortfolioBlazorWasm.Tests.Services.BankPa;

public class BankPaServiceTests
{
    private readonly Mock<ICsvHelperService> _mockCsvHelperService;

    public BankPaServiceTests()
    {
        _mockCsvHelperService = new Mock<ICsvHelperService>();
    }

    private BankPaService CreateService()
    {
        return new BankPaService(_mockCsvHelperService.Object);
    }

    [Fact]
    public void BankPaService_UsingDirectoryInfo_FilesExist()
    {
        // Arrange
        DirectoryInfo? testProjectDirectoryInfo = (Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent) ?? throw new DirectoryNotFoundException("Could not find test project directory.");
        string testProjectDirectory = testProjectDirectoryInfo.FullName;
        string personalAllowanceFilePath = Path.Combine(testProjectDirectory, "..", "PortfolioBlazorWasm", "wwwroot", BankPaService.PersonalAllowanceFilePath);
        string bankInterestRateFilePath = Path.Combine(testProjectDirectory, "..", "PortfolioBlazorWasm", "wwwroot", BankPaService.BankInterestRateFilePath);

        // Act
        bool personalAllowanceFileExists = File.Exists(personalAllowanceFilePath);
        bool bankInterestRateFileExists = File.Exists(bankInterestRateFilePath);

        // Assert
        Assert.True(personalAllowanceFileExists);
        Assert.True(bankInterestRateFileExists);
    }

    [Fact]
    public void BankPaService_UsingRelativePath_FilesExist()
    {
        // Arrange
        string personalAllowanceFilePath = Path.Combine("..", "..", "..", "..", "PortfolioBlazorWasm", "wwwroot", BankPaService.PersonalAllowanceFilePath);
        string bankInterestRateFilePath = Path.Combine("..", "..", "..", "..", "PortfolioBlazorWasm", "wwwroot", BankPaService.BankInterestRateFilePath);

        // Act
        bool personalAllowanceFileExists = File.Exists(personalAllowanceFilePath);
        bool bankInterestRateFileExists = File.Exists(bankInterestRateFilePath);

        // Assert
        Assert.True(personalAllowanceFileExists);
        Assert.True(bankInterestRateFileExists);
    }

    [Fact]
    public async Task GetBankRateRecords_StateUnderTest_ExpectedBehavior()
    {
        // Arrange
        _mockCsvHelperService.Setup(x => x.GetDataFromCsv<BankRate, BankRateMap>(BankPaService.BankInterestRateFilePath)).ReturnsAsync(new List<BankRate> { new BankRate() });
        var service = CreateService();
        // Act
        var result = await service.GetBankRateRecords();

        // Assert
        _mockCsvHelperService.Verify(x => x.GetDataFromCsv<BankRate, BankRateMap>(BankPaService.BankInterestRateFilePath), Times.Once);
        result.Should().BeOfType<List<BankRate>>();
    }

    [Fact]
    public async Task GetPersonalAllowanceRecords_StateUnderTest_ExpectedBehavior()
    {
        // Arrange
        _mockCsvHelperService.Setup(x => x.GetDataFromCsv<PersonalAllowance, PersonalAllowanceMap>(BankPaService.PersonalAllowanceFilePath)).ReturnsAsync(new List<PersonalAllowance> { new PersonalAllowance() });
        var service = CreateService();
        // Act
        var result = await service.GetPersonalAllowanceRecords();

        // Assert
        _mockCsvHelperService.Verify(x => x.GetDataFromCsv<PersonalAllowance, PersonalAllowanceMap>(BankPaService.PersonalAllowanceFilePath), Times.Once);
        result.Should().BeOfType<List<PersonalAllowance>>();
    }
}
