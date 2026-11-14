using AppCanzoniere.Application.Display;

namespace AppCanzoniere.Application.Tests.Display;

public class LanguagePickerTests
{
    [Theory]
    [InlineData(null, 0)]
    [InlineData("", 0)]
    [InlineData("  ", 0)]
    [InlineData("en", 1)]
    [InlineData("it", 2)]
    [InlineData("IT", 2)]
    [InlineData("fr", 0)]
    public void AStoredLanguage_SelectsItsEntry_AndAnythingUnknownSelectsAutomatic(string? stored, int expected)
    {
        Assert.Equal(expected, LanguagePicker.ToIndex(stored));
    }

    [Theory]
    [InlineData(0, "")]
    [InlineData(1, "en")]
    [InlineData(2, "it")]
    public void AnEntry_IsStoredAsItsCode_AndAutomaticIsEmpty(int index, string expected)
    {
        Assert.True(LanguagePicker.TryToCode(index, out var code));
        Assert.Equal(expected, code);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    public void APickerBeingResetOrPastTheList_IsNotAChoice(int index)
    {
        Assert.False(LanguagePicker.TryToCode(index, out _));
    }

    [Fact]
    public void ThereIsOneLabelForEveryCode()
    {
        Assert.Equal(LanguagePicker.Codes.Count, LanguagePicker.Labels.Count);
    }
}
