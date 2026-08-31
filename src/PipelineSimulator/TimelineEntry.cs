namespace PipelineSimulator;

// One printable line of a scenario's timeline, in the order it happened.
// Kept as data (not printed directly by ScenarioRunner) so tests can assert
// on the sequence of events without capturing Console output.
public sealed record TimelineEntry(int Step, string Message);
