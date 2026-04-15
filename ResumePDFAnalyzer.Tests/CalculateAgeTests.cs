using System.Globalization;
using ResumePDFAnalyzer.Services;

namespace ResumePDFAnalyzer.Tests;

public class CalculateAgeTests
{
    // ---- missing / invalid input ----

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("-")]
    public void CalculateAge_MissingInput_ReturnsDash(string? input)
    {
        Assert.Equal("-", ResumeAgentHelpers.CalculateAge(input!));
    }

    [Theory]
    [InlineData("not-a-date")]
    [InlineData("32/01/1990")]
    [InlineData("hello world")]
    public void CalculateAge_InvalidFormat_ReturnsDash(string input)
    {
        Assert.Equal("-", ResumeAgentHelpers.CalculateAge(input));
    }

    // ---- format parsing ----

    [Theory]
    [InlineData("1990-01-01")]   // yyyy-MM-dd
    [InlineData("01/01/1990")]   // dd/MM/yyyy
    [InlineData("1/1/1990")]     // d/M/yyyy
    [InlineData("01-01-1990")]   // dd-MM-yyyy
    [InlineData("1 Jan 1990")]   // d MMM yyyy
    [InlineData("01 Jan 1990")]  // dd MMM yyyy
    [InlineData("1 January 1990")]  // d MMMM yyyy
    [InlineData("01 January 1990")] // dd MMMM yyyy
    [InlineData("January 1, 1990")] // MMMM d, yyyy
    [InlineData("Jan 1, 1990")]     // MMM d, yyyy
    public void CalculateAge_AllSupportedFormats_ParsesWithoutError(string input)
    {
        var result = ResumeAgentHelpers.CalculateAge(input);
        Assert.NotEqual("-", result);
        Assert.True(int.TryParse(result, out _), $"Expected numeric age, got '{result}'");
    }

    // ---- age calculation correctness ----

    [Fact]
    public void CalculateAge_BornExactly30YearsAgo_Returns30()
    {
        var birthDate = DateOnly.FromDateTime(DateTime.Today).AddYears(-30);
        var result = ResumeAgentHelpers.CalculateAge(birthDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        Assert.Equal("30", result);
    }

    [Fact]
    public void CalculateAge_BirthdayTomorrow_NotYetIncremented()
    {
        // born exactly 25 years ago + 1 day → still 24
        var birthDate = DateOnly.FromDateTime(DateTime.Today).AddYears(-25).AddDays(1);
        var result = ResumeAgentHelpers.CalculateAge(birthDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        Assert.Equal("24", result);
    }

    [Fact]
    public void CalculateAge_BirthdayYesterday_AlreadyIncremented()
    {
        // born exactly 25 years ago - 1 day → already 25
        var birthDate = DateOnly.FromDateTime(DateTime.Today).AddYears(-25).AddDays(-1);
        var result = ResumeAgentHelpers.CalculateAge(birthDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        Assert.Equal("25", result);
    }

    [Fact]
    public void CalculateAge_BornToday_Returns0()
    {
        var birthDate = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        Assert.Equal("0", ResumeAgentHelpers.CalculateAge(birthDate));
    }
}
