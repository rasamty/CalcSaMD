using CalcSaMD.Core.Logic;
using Xunit;

namespace CalcSaMD.Tests;

// These tests are the entire automated gate between DEV and QA (Part 2.1 of
// the plan document). The Phase 3 pipeline will run "dotnet test" against
// this project after every push to main; if any test here fails, the
// pipeline stops and the build never reaches QA — there is no manual
// override. That is why CalculatorLogic.Add is a plain static method with
// no dependencies (Part 7 write-up explains this design choice): a method
// like that is trivial to call directly from a test, with no server, no
// browser, and no mocking required.
//
// The FEATURE_BUG_SIMULATION block further down is what the "simulator
// class" you asked for (Part 6, Phase 5) will eventually toggle to force a
// failing test on purpose, to prove the DEV→QA gate actually blocks a bad
// build rather than always passing by coincidence. It does nothing yet in
// Phase 1 — it is off, and left here only so the hook already exists when
// Phase 5 wires the simulator up to it.
public class CalculatorLogicTests
{
    [Theory]
    [InlineData(2, 3, 5)]
    [InlineData(-4, 4, 0)]
    [InlineData(0, 0, 0)]
    [InlineData(2.5, 2.5, 5.0)]
    [InlineData(-10.25, -0.75, -11.0)]
    public void Add_ReturnsSumOfTwoNumbers(double a, double b, double expected)
    {
        // Arrange happens implicitly via [InlineData] above: xUnit calls
        // this method once per line, substituting a, b and expected.

        // Act
        var actual = CalculatorLogic.Add(a, b);

        // Assert
        Assert.Equal(expected, actual, precision: 10);
    }

#if FEATURE_BUG_SIMULATION
    // This block is compiled in only when the FEATURE_BUG_SIMULATION MSBuild
    // constant is defined (Phase 5 will show exactly how the simulator class
    // sets that). With it defined, this test deliberately asserts a wrong
    // answer, so the whole suite fails on purpose and you can watch the
    // pipeline correctly push the build back to DEV instead of promoting it.
    [Fact]
    public void Add_SimulatedFailure_ForPipelineDemonstration()
    {
        Assert.Equal(999, CalculatorLogic.Add(2, 2));
    }
#endif
}
