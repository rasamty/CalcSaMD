namespace PipelineSimulator;

// The four environments from Part 2.1 of the plan document. Declaration
// order here carries no meaning about which moves between them are legal --
// that is entirely down to PipelineStateMachine's own edge table, not the
// order these names happen to be listed in.
public enum PipelineEnvironment
{
    Dev,
    Qa,
    Uat,
    Prod,
}
