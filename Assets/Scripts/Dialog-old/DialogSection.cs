

using UnityEngine;

[System.Serializable]
public struct DialogSection
{
    [TextArea]
    public string[] dialogue;
    public bool endAfterDialogue;
    public DialogBranchPoint branchPoint;
}