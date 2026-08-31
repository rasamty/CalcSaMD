namespace PipelineSimulator;

// Thrown by PipelineStateMachine.Transition when the requested move does not
// appear in the machine's own table of permitted edges -- for example DEV
// straight to UAT, or any move at all out of PROD. This is the mechanism
// PipelineStateMachineTests uses to prove illegal transitions are actually
// rejected by running code, not merely undocumented in Part 2's prose.
public sealed class IllegalTransitionException : Exception
{
    public PipelineEnvironment From { get; }
    public PipelineEnvironment To { get; }

    public IllegalTransitionException(PipelineEnvironment from, PipelineEnvironment to)
        : base($"Illegal pipeline transition: {from} -> {to} is not a permitted move under Part 2's policy.")
    {
        From = from;
        To = to;
    }
}
