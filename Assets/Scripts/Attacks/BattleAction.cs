using System;

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
