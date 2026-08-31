using PipelineSimulator;
using Xunit;

namespace PipelineSimulator.Tests;

// These tests are the whole point of Phase 5's second promise: not just
// that Part 2's policy is written down, but that a direct DEV -> UAT (or
// any other) illegal jump is actually rejected by running code, not merely
// absent from the documentation.
public class PipelineStateMachineTests
{
    [Theory]
    [InlineData(PipelineEnvironment.Dev, PipelineEnvironment.Uat)]
    [InlineData(PipelineEnvironment.Dev, PipelineEnvironment.Prod)]
    [InlineData(PipelineEnvironment.Qa, PipelineEnvironment.Prod)]
    public void NormalMachine_RejectsSkippedStageTransitions(PipelineEnvironment from, PipelineEnvironment to)
    {
        var machine = new PipelineStateMachine(isHotfix: false, startAt: from);

        Assert.Throws<IllegalTransitionException>(() => machine.Transition(to));
    }

    [Theory]
    [InlineData(PipelineEnvironment.Prod, PipelineEnvironment.Uat)]
    [InlineData(PipelineEnvironment.Prod, PipelineEnvironment.Dev)]
    [InlineData(PipelineEnvironment.Prod, PipelineEnvironment.Qa)]
    public void NormalMachine_RejectsAnyMoveOutOfProd(PipelineEnvironment from, PipelineEnvironment to)
    {
        var machine = new PipelineStateMachine(isHotfix: false, startAt: from);

        Assert.Throws<IllegalTransitionException>(() => machine.Transition(to));
    }

    [Fact]
    public void NormalMachine_AllowsTheFiveDocumentedEdges()
    {
        var devToQa = new PipelineStateMachine(startAt: PipelineEnvironment.Dev);
        devToQa.Transition(PipelineEnvironment.Qa);
        Assert.Equal(PipelineEnvironment.Qa, devToQa.Current);

        var qaToUat = new PipelineStateMachine(startAt: PipelineEnvironment.Qa);
        qaToUat.Transition(PipelineEnvironment.Uat);
        Assert.Equal(PipelineEnvironment.Uat, qaToUat.Current);

        var uatToProd = new PipelineStateMachine(startAt: PipelineEnvironment.Uat);
        uatToProd.Transition(PipelineEnvironment.Prod);
        Assert.Equal(PipelineEnvironment.Prod, uatToProd.Current);

        var qaRejectBackToDev = new PipelineStateMachine(startAt: PipelineEnvironment.Qa);
        qaRejectBackToDev.Transition(PipelineEnvironment.Dev);
        Assert.Equal(PipelineEnvironment.Dev, qaRejectBackToDev.Current);

        var uatRejectBackToDev = new PipelineStateMachine(startAt: PipelineEnvironment.Uat);
        uatRejectBackToDev.Transition(PipelineEnvironment.Dev);
        Assert.Equal(PipelineEnvironment.Dev, uatRejectBackToDev.Current);
    }

    [Fact]
    public void HotfixMachine_AllowsQaDirectlyToProd_ButNormalMachineDoesNot()
    {
        var hotfix = new PipelineStateMachine(isHotfix: true, startAt: PipelineEnvironment.Qa);
        hotfix.Transition(PipelineEnvironment.Prod);
        Assert.Equal(PipelineEnvironment.Prod, hotfix.Current);

        var normal = new PipelineStateMachine(isHotfix: false, startAt: PipelineEnvironment.Qa);
        Assert.Throws<IllegalTransitionException>(() => normal.Transition(PipelineEnvironment.Prod));
    }

    [Fact]
    public void HotfixMachine_StillRejectsDevDirectlyToUatOrProd()
    {
        var toUat = new PipelineStateMachine(isHotfix: true, startAt: PipelineEnvironment.Dev);
        Assert.Throws<IllegalTransitionException>(() => toUat.Transition(PipelineEnvironment.Uat));

        var toProd = new PipelineStateMachine(isHotfix: true, startAt: PipelineEnvironment.Dev);
        Assert.Throws<IllegalTransitionException>(() => toProd.Transition(PipelineEnvironment.Prod));
    }

    [Fact]
    public void IllegalTransitionException_NamesBothEnvironmentsInvolved()
    {
        var machine = new PipelineStateMachine(startAt: PipelineEnvironment.Dev);

        var ex = Assert.Throws<IllegalTransitionException>(() => machine.Transition(PipelineEnvironment.Uat));

        Assert.Equal(PipelineEnvironment.Dev, ex.From);
        Assert.Equal(PipelineEnvironment.Uat, ex.To);
    }

    [Fact]
    public void CanTransition_AgreesWithTransition_WithoutChangingState()
    {
        var machine = new PipelineStateMachine(startAt: PipelineEnvironment.Dev);

        Assert.True(machine.CanTransition(PipelineEnvironment.Qa));
        Assert.False(machine.CanTransition(PipelineEnvironment.Uat));
        // Calling CanTransition must not itself move the machine.
        Assert.Equal(PipelineEnvironment.Dev, machine.Current);
    }
}
