namespace CalcSaMD.Web.Models;

// Mirrors wwwroot/env-config.json exactly — property names match the JSON
// keys case-for-case on purpose, so there's no ambiguity about System.Text.Json's
// default casing rules.
//
// In Phase 1 (this phase), env-config.json is a hand-written placeholder
// checked into the project, and every environment looks identical: LOCAL.
// From Phase 3 onward, the pipeline generates and overwrites this file at
// build time with the real GitVersion-computed version, the real commit SHA,
// and whichever environment is actually being deployed to — the same
// "build once, promote unchanged" idea used throughout the main document
// (Part 4.3), just for a static file instead of a server response.
public class EnvironmentInfo
{
    public string EnvironmentName { get; set; } = "LOCAL";
    public string Version { get; set; } = "0.0.0-local";
    public string CommitSha { get; set; } = "unknown";
}
