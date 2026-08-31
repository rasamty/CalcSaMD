namespace PipelineSimulator;

// The dial-board for one simulated run. Every field here corresponds to
// something a real developer, QA_1, or PO_1 actually controls or observes
// in the real pipeline -- nothing in here is a simulator-only concept.
public sealed class ScenarioConfig
{
    // A short label, used only in the printed timeline.
    public string Name { get; init; } = "Unnamed scenario";

    // How many feature branches merge into main in this scenario. Each one
    // is simulated as its own independent run through the pipeline --
    // release.yml triggers on every merge to main, not once per "batch" of
    // merges -- so this is also how many timeline sections ScenarioRunner
    // prints. Ignored when HotfixFires is true (a hotfix is always exactly
    // one branch, cut from a tag, not from main -- Part 2.3).
    public int FeatureBranchCount { get; init; } = 1;

    // How many bugs this batch of work is narratively described as
    // carrying. This is printed for context, so a reader can see *why* the
    // test gate is behaving the way TestPassesOnAttempt says it will -- it
    // does not, by itself, drive any pipeline decision. See the remark on
    // TestPassesOnAttempt below for why these two dials are kept separate
    // rather than one being computed from the other.
    public int InjectedBugCount { get; init; } = 0;

    // Which attempt at the automated test gate (Part 2.1's DEV -> QA gate)
    // first passes. 1 means it passes the first time; 3 means it fails
    // twice and passes on the third try; 0 (or negative) means it never
    // passes in this scenario, and the run never leaves DEV. This is the
    // dial that mechanically drives pass/fail -- InjectedBugCount only
    // narrates *why*, so a scenario author can, for example, describe three
    // separate bugs while still choosing exactly when the suite starts
    // passing, rather than the two numbers being forced to match.
    public int TestPassesOnAttempt { get; init; } = 1;

    // QA_1's decision at the QA -> UAT gate (or, for a hotfix, at the
    // combined pre-PROD gate -- Part 2.3).
    public ApprovalDecision Qa1Decision { get; init; } = ApprovalDecision.Approve;

    // PO_1's decision at the UAT -> PROD gate (or, for a hotfix, at the
    // combined pre-PROD gate). Never even asked if Qa1Decision rejected
    // first -- Part 2.3 is explicit that a hotfix rejection at either gate
    // sends the branch back to DEV without proceeding to the other one.
    public ApprovalDecision Po1Decision { get; init; } = ApprovalDecision.Approve;

    // When true, this scenario runs as a Part 2.3 hotfix instead of a
    // normal release: branched from the current PROD tag rather than main,
    // UAT is skipped, and a successful run ends by printing the automatic
    // back-merge Pull Request rather than a plain version tag.
    public bool HotfixFires { get; init; } = false;
}
