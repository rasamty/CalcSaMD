using PipelineSimulator;
using Xunit;

namespace PipelineSimulator.Tests;

// These exercise ScenarioRunner's narrative layer -- the timeline text it
// produces on top of PipelineStateMachine -- separately from
// PipelineStateMachineTests, which exercises the state machine itself.
public class ScenarioRunnerTests
{
    private static string JoinMessages(IEnumerable<TimelineEntry> timeline) =>
        string.Join(" | ", timeline.Select(e => e.Message));

    [Fact]
    public void HappyPath_ReachesProdAndMentionsAllFourEnvironments()
    {
        var config = new ScenarioConfig
        {
            Name = "test-happy-path",
            FeatureBranchCount = 1,
            TestPassesOnAttempt = 1,
            Qa1Decision = ApprovalDecision.Approve,
            Po1Decision = ApprovalDecision.Approve,
        };

        string joined = JoinMessages(ScenarioRunner.Run(config));

        Assert.Contains("DEV", joined);
        Assert.Contains("QA", joined);
        Assert.Contains("UAT", joined);
        Assert.Contains("deployed to PROD", joined);
    }

    [Fact]
    public void FailingTests_NeverReachQa()
    {
        var config = new ScenarioConfig { Name = "test-tests-never-pass", TestPassesOnAttempt = 0 };

        string joined = JoinMessages(ScenarioRunner.Run(config));

        Assert.DoesNotContain("auto-deployed to QA", joined);
        Assert.Contains("stays at DEV", joined);
    }

    [Fact]
    public void FlakyTests_LogOneFailurePerAttemptBeforePassing()
    {
        var config = new ScenarioConfig { Name = "test-flaky", TestPassesOnAttempt = 3 };

        var timeline = ScenarioRunner.Run(config);
        int failureCount = timeline.Count(e => e.Message.Contains("unit tests FAIL"));
        int passCount = timeline.Count(e => e.Message.Contains("unit tests PASS"));

        Assert.Equal(2, failureCount);
        Assert.Equal(1, passCount);
    }

    [Fact]
    public void Qa1Rejection_ReturnsToDevAndNeverReachesUat()
    {
        var config = new ScenarioConfig
        {
            Name = "test-qa1-reject",
            TestPassesOnAttempt = 1,
            Qa1Decision = ApprovalDecision.Reject,
        };

        string joined = JoinMessages(ScenarioRunner.Run(config));

        Assert.Contains("QA_1 REJECTED", joined);
        Assert.DoesNotContain("deployed to UAT", joined);
    }

    [Fact]
    public void Po1Rejection_ReturnsToDevAndNeverReachesProd()
    {
        var config = new ScenarioConfig
        {
            Name = "test-po1-reject",
            TestPassesOnAttempt = 1,
            Qa1Decision = ApprovalDecision.Approve,
            Po1Decision = ApprovalDecision.Reject,
        };

        string joined = JoinMessages(ScenarioRunner.Run(config));

        Assert.Contains("PO_1 REJECTED", joined);
        Assert.DoesNotContain("deployed to PROD", joined);
    }

    [Fact]
    public void MultipleFeatureBranches_ProducesOneRunPerBranch()
    {
        var config = new ScenarioConfig { Name = "test-multi-branch", FeatureBranchCount = 3, TestPassesOnAttempt = 1 };

        var timeline = ScenarioRunner.Run(config);
        int mergeCount = timeline.Count(e => e.Message.Contains("merged into main"));

        Assert.Equal(3, mergeCount);
    }

    [Fact]
    public void Hotfix_SkipsUatButStillRequiresBothSignOffs()
    {
        var config = new ScenarioConfig
        {
            Name = "test-hotfix-happy",
            HotfixFires = true,
            TestPassesOnAttempt = 1,
            Qa1Decision = ApprovalDecision.Approve,
            Po1Decision = ApprovalDecision.Approve,
        };

        string joined = JoinMessages(ScenarioRunner.Run(config));

        Assert.DoesNotContain("deployed to UAT", joined);
        Assert.Contains("QA_1 APPROVED", joined);
        Assert.Contains("PO_1 APPROVED", joined);
        Assert.Contains("deployed straight to PROD", joined);
        Assert.Contains("back-merge", joined);
    }

    [Fact]
    public void HotfixWithQa1Rejection_NeverAsksPo1()
    {
        var config = new ScenarioConfig
        {
            Name = "test-hotfix-qa1-reject",
            HotfixFires = true,
            TestPassesOnAttempt = 1,
            Qa1Decision = ApprovalDecision.Reject,
        };

        string joined = JoinMessages(ScenarioRunner.Run(config));

        Assert.Contains("QA_1 REJECTED", joined);
        Assert.DoesNotContain("PO_1", joined);
    }
}
