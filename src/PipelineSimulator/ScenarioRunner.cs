namespace PipelineSimulator;

// Drives a PipelineStateMachine using the knobs on a ScenarioConfig and
// records a human-readable timeline of every step -- exactly what Part 2's
// policy says the real pipeline would do for that scenario, with no GitHub
// or Azure call anywhere in this class.
public static class ScenarioRunner
{
    public static IReadOnlyList<TimelineEntry> Run(ScenarioConfig config)
    {
        var timeline = new List<TimelineEntry>();
        int step = 0;
        void Log(string message) => timeline.Add(new TimelineEntry(++step, message));

        Log($"=== Scenario: {config.Name} ===");

        if (config.HotfixFires)
        {
            RunHotfix(config, Log);
        }
        else
        {
            int branchCount = Math.Max(config.FeatureBranchCount, 1);
            for (int branch = 1; branch <= branchCount; branch++)
            {
                RunNormalRelease(config, branch, branchCount, Log);
            }
        }

        return timeline;
    }

    private static void RunNormalRelease(ScenarioConfig config, int branchNumber, int branchCount, Action<string> log)
    {
        var machine = new PipelineStateMachine(isHotfix: false);
        string label = branchCount > 1 ? $"feature/branch-{branchNumber}" : "feature/branch-1";

        log($"[{label}] merged into main -> DEV auto-deploys the latest merged code.");

        if (config.InjectedBugCount > 0)
        {
            log($"[{label}] this batch of work carries {config.InjectedBugCount} injected bug(s).");
        }

        if (!TryPassTestGate(config, log, label))
        {
            log($"[{label}] unit tests never passed in this scenario -> run stays at DEV. QA is untouched.");
            return;
        }

        machine.Transition(PipelineEnvironment.Qa);
        log($"[{label}] unit tests passed -> auto-deployed to QA.");

        if (config.Qa1Decision == ApprovalDecision.Reject)
        {
            machine.Transition(PipelineEnvironment.Dev);
            log($"[{label}] QA_1 REJECTED the QA -> UAT promotion -> sent back to DEV.");
            return;
        }

        machine.Transition(PipelineEnvironment.Uat);
        log($"[{label}] QA_1 APPROVED -> deployed to UAT.");

        if (config.Po1Decision == ApprovalDecision.Reject)
        {
            machine.Transition(PipelineEnvironment.Dev);
            log($"[{label}] PO_1 REJECTED the UAT -> PROD promotion -> sent back to DEV.");
            return;
        }

        machine.Transition(PipelineEnvironment.Prod);
        log($"[{label}] PO_1 APPROVED -> deployed to PROD. Tag v<majorMinorPatch> created and pushed.");
    }

    private static void RunHotfix(ScenarioConfig config, Action<string> log)
    {
        var machine = new PipelineStateMachine(isHotfix: true);
        const string label = "hotfix/branch";

        log($"[{label}] branched from the current PROD release tag (not main) -> DEV auto-deploys.");

        if (config.InjectedBugCount > 0)
        {
            log($"[{label}] this hotfix addresses {config.InjectedBugCount} confirmed production defect(s).");
        }

        if (!TryPassTestGate(config, log, label))
        {
            log($"[{label}] unit tests never passed in this scenario -> hotfix run stays at DEV.");
            return;
        }

        machine.Transition(PipelineEnvironment.Qa);
        log($"[{label}] unit tests passed -> auto-deployed to QA. UAT is skipped for hotfixes, but neither sign-off is.");

        if (config.Qa1Decision == ApprovalDecision.Reject)
        {
            machine.Transition(PipelineEnvironment.Dev);
            log($"[{label}] QA_1 REJECTED -> sent back to DEV. PO_1 is never asked.");
            return;
        }

        log($"[{label}] QA_1 APPROVED (reviewed against the UAT-environment reviewer rule; no UAT deployment occurs).");

        if (config.Po1Decision == ApprovalDecision.Reject)
        {
            machine.Transition(PipelineEnvironment.Dev);
            log($"[{label}] PO_1 REJECTED -> sent back to DEV.");
            return;
        }

        machine.Transition(PipelineEnvironment.Prod);
        log($"[{label}] PO_1 APPROVED -> deployed straight to PROD (UAT skipped). Tag v<majorMinorPatch> created and pushed.");
        log($"[{label}] pipeline automatically opens a Pull Request to back-merge {label} into main.");
    }

    private static bool TryPassTestGate(ScenarioConfig config, Action<string> log, string label)
    {
        if (config.TestPassesOnAttempt <= 0)
        {
            log($"[{label}] attempt 1: unit tests FAIL. (Configured to never pass in this scenario.)");
            return false;
        }

        for (int attempt = 1; attempt < config.TestPassesOnAttempt; attempt++)
        {
            log($"[{label}] attempt {attempt}: unit tests FAIL -> DEV keeps the failing build, QA untouched.");
        }

        log($"[{label}] attempt {config.TestPassesOnAttempt}: unit tests PASS.");
        return true;
    }
}
