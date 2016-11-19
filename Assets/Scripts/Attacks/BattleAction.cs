using System;

/// <summary>
/// An Action which lasts a duration. 
/// Used to have the BattleManager wait while processing turns.
/// </summary>
public struct BattleAction
{
    public Action method;
    public float duration;

    public BattleAction(Action method, float duration)
    {
        this.method = method;
        this.duration = duration;
    }
}
