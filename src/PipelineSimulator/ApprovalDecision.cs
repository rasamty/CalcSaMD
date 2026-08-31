namespace PipelineSimulator;

// What a human reviewer -- QA_1 or PO_1 -- does at one of Part 2.1's manual
// gates (QA -> UAT and UAT -> PROD respectively; both gates on a hotfix, per
// Part 2.3).
public enum ApprovalDecision
{
    Approve,
    Reject,
}
