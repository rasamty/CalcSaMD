namespace CalcSaMD.Core.Logic;

// The one piece of "business logic" this whole project has, and the only
// class the automated DEV -> QA test gate (Part 2.1) actually exercises.
//
// This lives in its own plain class library — CalcSaMD.Core — rather than
// inside CalcSaMD.Web, for a very concrete reason: CalcSaMD.Web targets the
// Microsoft.NET.Sdk.BlazorWebAssembly SDK, which brings a WebAssembly build
// pipeline (asset manifests, the WASM runtime, browser-specific packaging)
// along with it. A plain xUnit test project referencing that project
// directly can hit friction from that extra machinery — which is exactly
// what happened building this project for real, and is the reason this
// class was pulled out into its own ordinary Microsoft.NET.Sdk class
// library. CalcSaMD.Web now references CalcSaMD.Core, and so does the test
// project — both depend on the same plain, dependency-free code; neither
// depends on the other. This is also just good general practice: shared
// logic belongs in its own library, independent of any one app that
// happens to consume it.
public static class CalculatorLogic
{
    public static double Add(double a, double b) => a + b;
}
