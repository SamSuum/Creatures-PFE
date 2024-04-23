using UnityEngine;

[System.Serializable]
public struct DialogBranchPoint
{
    [TextArea]
    public string question;
    public Answers[] answers;

}