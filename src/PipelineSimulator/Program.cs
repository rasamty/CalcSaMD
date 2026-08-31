using PipelineSimulator;

// Two modes, chosen by the command line:
//
//   dotnet run --project src/PipelineSimulator
//     Runs five built-in demo scenarios entirely in memory and prints each
//     one's timeline. This is the default, and it needs nothing beyond
//     this one project -- no GitHub, no Azure, no other project on disk.
//
//   dotnet run --project src/PipelineSimulator -- --real-test-gate <path-to-CalcSaMD.Tests.csproj> [--simulate-bug]
//     Skips the demo scenarios and instead shells out to a real
//     `dotnet test` against the real CalcSaMD.Tests project -- see
//     RealTestGate.cs and Part 11.6 for exactly what this does and why.

if (args.Length > 0 && args[0] == "--real-test-gate")
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: --real-test-gate <path-to-CalcSaMD.Tests.csproj> [--simulate-bug]");
        return 1;
    }

    string testProjectPath = args[1];
    bool simulateBug = args.Contains("--simulate-bug");

    Console.WriteLine(simulateBug
        ? "Running the REAL CalcSaMD.Tests suite with FEATURE_BUG_SIMULATION defined -- expect a FAILURE."
        : "Running the REAL CalcSaMD.Tests suite normally -- expect a PASS.");
    Console.WriteLine($"Project: {testProjectPath}");
    Console.WriteLine();

    try
    {
        int exitCode = RealTestGate.RunRealTests(testProjectPath, simulateBug);
        Console.WriteLine();
        Console.WriteLine($"dotnet test exited with code {exitCode} ({(exitCode == 0 ? "PASSED" : "FAILED")}).");
        return exitCode;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not run the real test gate: {ex.Message}");
        return 1;
    }
}

RunDemoScenarios();
return 0;

void RunDemoScenarios()
{
    var scenarios = new[]
    {
        new ScenarioConfig
        {
            Name = "Happy path -- one feature branch, everyone approves",
            FeatureBranchCount = 1,
            TestPassesOnAttempt = 1,
            Qa1Decision = ApprovalDecision.Approve,
            Po1Decision = ApprovalDecision.Approve,
        },
        new ScenarioConfig
        {
            Name = "A flaky test -- two feature branches, one bug found and fixed before the tests pass",
            FeatureBranchCount = 2,
            InjectedBugCount = 1,
            TestPassesOnAttempt = 2,
            Qa1Decision = ApprovalDecision.Approve,
            Po1Decision = ApprovalDecision.Approve,
        },
        new ScenarioConfig
        {
            Name = "QA_1 rejects the promotion to UAT",
            FeatureBranchCount = 1,
            TestPassesOnAttempt = 1,
            Qa1Decision = ApprovalDecision.Reject,
        },
        new ScenarioConfig
        {
            Name = "PO_1 rejects the promotion to PROD",
            FeatureBranchCount = 1,
            TestPassesOnAttempt = 1,
            Qa1Decision = ApprovalDecision.Approve,
            Po1Decision = ApprovalDecision.Reject,
        },
        new ScenarioConfig
        {
            Name = "Emergency hotfix -- both sign-offs given, UAT skipped",
            HotfixFires = true,
            InjectedBugCount = 1,
            TestPassesOnAttempt = 1,
            Qa1Decision = ApprovalDecision.Approve,
            Po1Decision = ApprovalDecision.Approve,
        },

        // --- additional regulated / real-life cases (do not change the five above) ---

        new ScenarioConfig
        {
            Name = "Tests never pass -- change is blocked at DEV, QA never receives it",
            FeatureBranchCount = 1,
            InjectedBugCount = 3,
            TestPassesOnAttempt = 0,
            Qa1Decision = ApprovalDecision.Approve,
            Po1Decision = ApprovalDecision.Approve,
        },
        new ScenarioConfig
        {
            Name = "Remediation -- suite fails twice, passes on the third attempt, then both approve",
            FeatureBranchCount = 1,
            InjectedBugCount = 2,
            TestPassesOnAttempt = 3,
            Qa1Decision = ApprovalDecision.Approve,
            Po1Decision = ApprovalDecision.Approve,
        },
        new ScenarioConfig
        {
            Name = "Tests eventually pass but QA_1 still rejects -- automated green is not UAT sign-off",
            FeatureBranchCount = 1,
            InjectedBugCount = 1,
            TestPassesOnAttempt = 2,
            Qa1Decision = ApprovalDecision.Reject,
            Po1Decision = ApprovalDecision.Approve,
        },
        new ScenarioConfig
        {
            Name = "Fix reaches UAT then PO_1 rejects -- business sign-off is independent of QA_1",
            FeatureBranchCount = 1,
            InjectedBugCount = 1,
            TestPassesOnAttempt = 2,
            Qa1Decision = ApprovalDecision.Approve,
            Po1Decision = ApprovalDecision.Reject,
        },
        new ScenarioConfig
        {
            Name = "Three independent merges to main -- three separate release walks",
            FeatureBranchCount = 3,
            TestPassesOnAttempt = 1,
            Qa1Decision = ApprovalDecision.Approve,
            Po1Decision = ApprovalDecision.Approve,
        },
        new ScenarioConfig
        {
            Name = "Hotfix tests never pass -- emergency does not waive the unit-test gate",
            HotfixFires = true,
            InjectedBugCount = 1,
            TestPassesOnAttempt = 0,
            Qa1Decision = ApprovalDecision.Approve,
            Po1Decision = ApprovalDecision.Approve,
        },
        new ScenarioConfig
        {
            Name = "Hotfix QA_1 rejects -- UAT site is skipped, QA_1 sign-off is not",
            HotfixFires = true,
            InjectedBugCount = 1,
            TestPassesOnAttempt = 1,
            Qa1Decision = ApprovalDecision.Reject,
            Po1Decision = ApprovalDecision.Approve,
        },
        new ScenarioConfig
        {
            Name = "Hotfix PO_1 rejects -- QA_1 approved, PROD still blocked, back to DEV",
            HotfixFires = true,
            InjectedBugCount = 1,
            TestPassesOnAttempt = 1,
            Qa1Decision = ApprovalDecision.Approve,
            Po1Decision = ApprovalDecision.Reject,
        },
        new ScenarioConfig
        {
            Name = "Hotfix flaky tests then both sign off -- still no UAT deployment",
            HotfixFires = true,
            InjectedBugCount = 1,
            TestPassesOnAttempt = 2,
            Qa1Decision = ApprovalDecision.Approve,
            Po1Decision = ApprovalDecision.Approve,
        },
        new ScenarioConfig
        {
            Name = "Two merges, tests never pass on either -- each release independently stuck at DEV",
            FeatureBranchCount = 2,
            InjectedBugCount = 4,
            TestPassesOnAttempt = 0,
            Qa1Decision = ApprovalDecision.Approve,
            Po1Decision = ApprovalDecision.Approve,
        },
    };

    foreach (var scenario in scenarios)
    {
        var timeline = ScenarioRunner.Run(scenario);
        foreach (var entry in timeline)
        {
            Console.WriteLine($"{entry.Step,3}. {entry.Message}");
        }

        Console.WriteLine();
    }
}