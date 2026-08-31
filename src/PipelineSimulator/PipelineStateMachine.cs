namespace PipelineSimulator;

// A pure, in-memory implementation of the state machine defined in Part 2 of
// the plan document -- no GitHub, no Azure, no network call of any kind.
// It knows nothing about *why* a transition should happen (that is
// ScenarioRunner's job below); it only knows which (from, to) pairs are
// permitted for the kind of run it represents, and refuses everything else.
public sealed class PipelineStateMachine
{
    // Part 2.2, stated precisely:
    //   DEV -> QA   automatic, but only if the unit tests pass (Part 2.1)
    //   QA -> UAT   manual, QA_1 approves
    //   QA -> DEV   QA_1 rejects -> sent back to DEV
    //   UAT -> PROD manual, PO_1 approves
    //   UAT -> DEV  PO_1 rejects -> sent back to DEV (Part 2.2's callout:
    //               same rule as a QA_1 rejection, for consistency)
    // DEV -> UAT, DEV -> PROD, QA -> PROD, and every move out of PROD are
    // deliberately absent -- Part 2.2 states each of those is never allowed.
    private static readonly HashSet<(PipelineEnvironment From, PipelineEnvironment To)> NormalEdges = new()
    {
        (PipelineEnvironment.Dev, PipelineEnvironment.Qa),
        (PipelineEnvironment.Qa, PipelineEnvironment.Uat),
        (PipelineEnvironment.Qa, PipelineEnvironment.Dev),
        (PipelineEnvironment.Uat, PipelineEnvironment.Prod),
        (PipelineEnvironment.Uat, PipelineEnvironment.Dev),
    };

    // Part 2.3: a hotfix goes DEV -> QA exactly like a normal release, still
    // gated by the same unit tests, but then jumps straight from QA to PROD
    // instead of visiting UAT -- that one extra edge is the only structural
    // difference a hotfix run is allowed. Both QA_1 and PO_1 still have to
    // sign off before that jump (ScenarioRunner enforces that; this table
    // only says the jump itself is a legal *shape* once they have).
    private static readonly HashSet<(PipelineEnvironment From, PipelineEnvironment To)> HotfixEdges = new()
    {
        (PipelineEnvironment.Dev, PipelineEnvironment.Qa),
        (PipelineEnvironment.Qa, PipelineEnvironment.Prod),
        (PipelineEnvironment.Qa, PipelineEnvironment.Dev),
    };

    public bool IsHotfix { get; }
    public PipelineEnvironment Current { get; private set; }

    public PipelineStateMachine(bool isHotfix = false, PipelineEnvironment startAt = PipelineEnvironment.Dev)
    {
        IsHotfix = isHotfix;
        Current = startAt;
    }

    public bool CanTransition(PipelineEnvironment to)
    {
        var table = IsHotfix ? HotfixEdges : NormalEdges;
        return table.Contains((Current, to));
    }

    // Throws rather than silently ignoring or clamping to "the nearest
    // legal state" -- a real pipeline has no such concept, so neither does
    // this one. Every ScenarioRunner call site below only ever calls this
    // once it already knows the move is legal; PipelineStateMachineTests is
    // what deliberately calls it with illegal moves, to prove they fail.
    public void Transition(PipelineEnvironment to)
    {
        if (!CanTransition(to))
        {
            throw new IllegalTransitionException(Current, to);
        }

        Current = to;
    }
}
