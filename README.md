# CalcSaMD — Phase 1 reference solution

This is a **reference checkpoint**, not something to unzip and run. Per the
plan, you build this yourself in Visual Studio 2026 by following Part 7 of
`CalcSaMD_Plan_and_Design.docx` step by step. Use this zip only to compare
against what you typed, if something doesn't match or doesn't compile.

## Solution file: `CalcSaMD.slnx`, not `CalcSaMD.sln`

Visual Studio 2026 uses the newer XML-based `.slnx` solution format. This
reference solution only ships `CalcSaMD.slnx`. If your own project also has
a leftover `CalcSaMD.sln` sitting next to it (VS sometimes generates both
while migrating), it's safe to delete once you've confirmed `.slnx` opens
and builds correctly — keeping both around invites exactly the kind of
"which one is actually being built?" confusion this whole project is
trying to teach you to avoid.

## Why there are three projects, not two

`CalcSaMD.Core` is a plain class library holding nothing but
`CalculatorLogic`. It exists as its own project — separate from
`CalcSaMD.Web` — because a test project referencing a
`Microsoft.NET.Sdk.BlazorWebAssembly` project directly (i.e. referencing
`CalcSaMD.Web` itself) can run into real build friction from the WebAssembly
tooling that SDK pulls in. Pulling the shared logic into an ordinary
`Microsoft.NET.Sdk` class library sidesteps that entirely: `CalcSaMD.Web`
references `CalcSaMD.Core`, the test project also references
`CalcSaMD.Core`, and neither of the other two references each other. This
is also just good general practice — shared logic belongs in its own
library, not fused into the one app that happens to consume it first.

## Where the test project lives

`CalcSaMD.Tests` is nested inside `src/CalcSaMD.Web/tests/`, not at the
repository root next to `src/`. That's an unusual location for a test
project, but it works, and `CalcSaMD.Web.csproj` has an explicit
`<Compile Remove="tests\**\*.cs" />` block (see the comment inside it) so
the Web project's own build never tries to swallow the test project's files
as if they were its own.

## Files the default templates generate that this solution deliberately
## does NOT include

If you scaffold this with `dotnet new blazorwasm`, your project briefly
contains a few files this reference solution does not:

- `Pages/Counter.razor`, `Pages/Weather.razor` (+ `wwwroot/sample-data/weather.json`) — demo pages. Delete them; CalcSaMD has exactly one page.
- `Layout/NavMenu.razor` + `.razor.css` — a sidebar for navigating between them. Delete it; `MainLayout.razor` here never references it.
- `tests/UnitTest1.cs` — the empty xUnit placeholder. Replaced by `CalculatorLogicTests.cs`.

**If you instead used Visual Studio 2026's own "Blazor WebAssembly
Standalone App" project wizard**, you'll see a *different* set of generated
files, because the wizard's built-in template is not the same as the
`dotnet new blazorwasm` CLI template:

- `Pages/Home.razor` — a demo page declaring **`@page "/"`**. This is the
  one you must delete, and it's not just tidiness: `Pages/Index.razor` in
  this solution also declares `@page "/"`. Two components claiming the same
  route is not caught by the C# compiler — it's Blazor's `<Router>` that
  discovers it, only at runtime, the moment it builds its route table on
  first render. The router throws an unhandled exception right there, before
  anything paints — which is why the symptom is not "the badge looks wrong,"
  it's "nothing renders at all," even though `dotnet build` succeeded
  perfectly. If you hit a blank page or a stuck "Loading…" screen with a
  clean build, open the browser DevTools console (F12) — an ambiguous-route
  exception logged there is the signature of exactly this problem.
- `Pages/NotFound.razor` — harmless (a different route, `/not-found`); no
  conflict either way, safe to keep or delete.
- `wwwroot/lib/bootstrap/` — a full Bootstrap CSS/JS library (~8MB),
  unreferenced by this solution's `index.html`. Safe to delete.

## Folder layout

```
CalcSaMD/
  CalcSaMD.slnx
  GitVersion.yml                          (inert until Phase 3 — see comments inside)
  src/
    CalcSaMD.Core/
      CalcSaMD.Core.csproj
      Logic/
        CalculatorLogic.cs
    CalcSaMD.Web/
      CalcSaMD.Web.csproj
      App.razor
      Program.cs
      _Imports.razor
      Layout/
        MainLayout.razor(.css)
      Pages/
        Index.razor(.css)
      Components/
        EnvironmentBadge.razor(.css)
      Models/
        EnvironmentInfo.cs
      wwwroot/
        index.html
        env-config.json
        css/app.css
      tests/
        CalcSaMD.Tests/
          CalcSaMD.Tests.csproj
          CalculatorLogicTests.cs
```

## A note on `Properties/launchSettings.json`

This file's exact port numbers (`5170`/`7170` here) are whatever Visual
Studio happened to assign when the project was scaffolded — yours will
likely be different numbers, and that's fine. This file only controls your
local `dotnet run` / F5 experience; it is never published to Azure and has
no effect on DEV/QA/UAT/PROD.
