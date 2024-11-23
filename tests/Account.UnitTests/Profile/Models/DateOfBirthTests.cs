using System;
using System.Collections.Generic;
using Account.Profile.Models;
using ErrorOr;
using FluentAssertions;

namespace Account.UnitTests.Profile.Models;

[Trait("Category", "Unit")]
public class DateOfBirthTests
{
    [Theory]
    [Trait("Category", "Unit")]
    [MemberData(nameof(GetValidBirthDates))]
    public void Create_WithValidDate_ShouldReturnExpectedInstance(DateTime validDate)
    {
        // Act
        ErrorOr<DateOfBirth> result = DateOfBirth.Create(validDate);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(DateTime.SpecifyKind(validDate, DateTimeKind.Utc));
        result.Value.Value.Kind.Should().Be(DateTimeKind.Utc);
    }

    public static IEnumerable<object[]> GetValidBirthDates()
    {
        DateTime today = DateTime.UtcNow.Date;
        return new List<object[]>
        {
            new object[] { today.AddYears(-1) },
            new object[] { today.AddYears(-18) },
            new object[] { today.AddYears(-50) },
            new object[] { today.AddYears(-100) },
            new object[] { today.AddYears(-120).AddDays(1) },
        };
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Create_WithFutureDate_ShouldReturnError()
    {
        // Arrange
        DateTime futureDate = DateTime.UtcNow.AddDays(1);

        // Act
        ErrorOr<DateOfBirth> result = DateOfBirth.Create(futureDate);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be("BirthDate.FutureDate");
        result.FirstError.Description.Should().NotBeNullOrWhiteSpace();
        result.Errors.Should().HaveCount(1);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [MemberData(nameof(GetTooOldDates))]
    public void Create_WithAgeExceedingMaxAge_ShouldReturnError(DateTime tooOldDate)
    {
        // Act
        ErrorOr<DateOfBirth> result = DateOfBirth.Create(tooOldDate);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be("BirthDate.TooOld");
        result.FirstError.Description.Should().Contain("120");
        result.Errors.Should().HaveCount(1);
    }

    public static IEnumerable<object[]> GetTooOldDates()
    {
        DateTime today = DateTime.UtcNow.Date;
        return new List<object[]>
        {
            new object[] { today.AddYears(-121) },
            new object[] { today.AddYears(-150) },
            new object[] { today.AddYears(-200) },
        };
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Create_ShouldHandleBirthYearCalculationCorrectly()
    {
        // Arrange
        DateTime today = DateTime.UtcNow.Date;
        DateTime birthDate = today.AddYears(-50).AddDays(1);

        // Act
        ErrorOr<DateOfBirth> result = DateOfBirth.Create(birthDate);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(DateTime.SpecifyKind(birthDate, DateTimeKind.Utc));
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("1990-01-01", "01/01/1990")]
    [InlineData("2000-12-31", "31/12/2000")]
    public void ToString_ShouldReturnFrenchFormattedDate(string inputDate, string expected)
    {
        // Arrange
        DateTime date = DateTime.Parse(inputDate);
        ErrorOr<DateOfBirth> result = DateOfBirth.Create(date);
        result.IsError.Should().BeFalse();
        DateOfBirth dateOfBirth = result.Value;

        // Act
        string formatted = dateOfBirth.ToString();

        // Assert
        formatted.Should().Be(expected);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [MemberData(nameof(GetEqualityTestCases))]
    public void ValueObject_EqualityBehavior_ShouldBeCorrect(
        DateTime value1,
        DateTime value2,
        bool shouldBeEqual
    )
    {
        // Arrange
        ErrorOr<DateOfBirth> result1 = DateOfBirth.Create(value1);
        ErrorOr<DateOfBirth> result2 = DateOfBirth.Create(value2);

        result1.IsError.Should().BeFalse();
        result2.IsError.Should().BeFalse();

        DateOfBirth dateOfBirthDate1 = result1.Value;
        DateOfBirth dateOfBirthDate2 = result2.Value;

        // Act & Assert
        (dateOfBirthDate1 == dateOfBirthDate2)
            .Should()
            .Be(shouldBeEqual);
        (dateOfBirthDate1 != dateOfBirthDate2).Should().Be(!shouldBeEqual);
        dateOfBirthDate1.Equals(dateOfBirthDate2).Should().Be(shouldBeEqual);
        if (shouldBeEqual)
        {
            dateOfBirthDate1.GetHashCode().Should().Be(dateOfBirthDate2.GetHashCode());
        }
    }

    public static IEnumerable<object[]> GetEqualityTestCases()
    {
        DateTime date1 = new(1990, 1, 1);
        return new List<object[]>
        {
            new object[] { date1, date1, true },
            new object[] { date1, date1.AddDays(1), false },
            new object[] { date1, new DateTime(1990, 1, 1), true },
        };
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ImplicitConversion_ToDateTime_ShouldWork()
    {
        // Arrange
        DateTime date = DateTime.UtcNow.Date.AddYears(-20);
        DateOfBirth dateOfBirth = DateOfBirth.Create(date).Value;

        // Act
        DateTime converted = dateOfBirth;

        // Assert
        converted.Should().Be(DateTime.SpecifyKind(date, DateTimeKind.Utc));
        converted.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Create_ShouldAlwaysReturnUtcDateTime()
    {
        // Arrange
        DateTime localDate = new(1990, 1, 1, 0, 0, 0, DateTimeKind.Local);
        DateTime unspecifiedDate = new(1990, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        // Act
        ErrorOr<DateOfBirth> resultLocal = DateOfBirth.Create(localDate);
        ErrorOr<DateOfBirth> resultUnspecified = DateOfBirth.Create(unspecifiedDate);

        // Assert
        resultLocal.IsError.Should().BeFalse();
        resultUnspecified.IsError.Should().BeFalse();

        resultLocal.Value.Value.Kind.Should().Be(DateTimeKind.Utc);
        resultUnspecified.Value.Value.Kind.Should().Be(DateTimeKind.Utc);
    }
}
