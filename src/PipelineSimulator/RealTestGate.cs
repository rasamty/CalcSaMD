using System.Diagnostics;

namespace PipelineSimulator;

// Phase 1's CalcSaMD.Tests project left one hook for this phase to wire up:
// a test guarded by "#if FEATURE_BUG_SIMULATION" that is compiled in, and
// deliberately fails, only when that MSBuild constant is defined. This
// class is that wiring -- it shells out to the real `dotnet test` command
// against the real CalcSaMD.Tests project, with that constant defined or
// not, so you can watch an actual local test run fail or pass on demand.
//
// This is deliberately kept separate from ScenarioRunner. Everything in
// ScenarioRunner is a pure in-memory "what would happen" prediction, with no
// dependency on the .NET SDK being installed or the real solution being
// present on disk -- exactly what Part 6/Part 11 promise this phase is.
// RealTestGate is an optional, genuinely-executing companion for when you
// want to see the DEV -> QA gate actually reject a real build, not just read
// a prediction of it doing so. It still makes no GitHub or Azure call of
// any kind -- the only thing it runs is a local `dotnet test`.
//
// CAUTION, stated plainly rather than glossed over: passing
// "/p:DefineConstants=FEATURE_BUG_SIMULATION" on the command line replaces
// MSBuild's DefineConstants property outright, rather than adding to
// whatever it would otherwise contain (TRACE, DEBUG, and so on, depending
// on configuration). For this project that is harmless -- the only
// conditional-compilation symbol anything here checks for is
// FEATURE_BUG_SIMULATION itself -- but it is not a generally safe way to
// toggle one symbol in a larger project, and this comment says so rather
// than implying the technique generalizes.
public static class RealTestGate
{
    // testProjectPath: path to CalcSaMD.Tests.csproj, relative to whatever
    // directory you run PipelineSimulator from. From the repository root,
    // that's src/CalcSaMD.Web/tests/CalcSaMD.Tests/CalcSaMD.Tests.csproj --
    // see Part 11.6 for the exact command.
    public static int RunRealTests(string testProjectPath, bool simulateBug)
    {
        var startInfo = new ProcessStartInfo("dotnet")
        {
            UseShellExecute = false,
        };
        startInfo.ArgumentList.Add("test");
        startInfo.ArgumentList.Add(testProjectPath);
        if (simulateBug)
        {
            startInfo.ArgumentList.Add("/p:DefineConstants=FEATURE_BUG_SIMULATION");
        }

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Could not start the dotnet CLI. Is the .NET SDK installed and on PATH?");
        process.WaitForExit();
        return process.ExitCode;
    }
}
