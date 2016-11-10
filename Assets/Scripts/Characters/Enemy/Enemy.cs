public class Enemy : Character
{
    public override void Reset()
    {
        Stats = LoadData("Enemy.caster");
        base.Reset();
    }

    public override void InitializeTurn()
    {
        PerformAttack(new AttackInfo(1));
    }

    public override void HandleTurn()
    {
    }

    public override void PerformAttack(AttackInfo info)
    {
        BattleManager.instance.EnqueueAction(new BattleAction(
            () =>
            {
                DebugHelper.instance.AddMessage(string.Format("Enemy attacked for {0} damage", info.damage), 5);
                BattleManager.instance.AttackPlayer(info);
                BattleManager.instance.DequeueCharacter();
            },
            2));
    }

    public override void Die()
    {
        base.Die();
        
        gameObject.SetActive(false);

        DebugHelper.instance.AddMessage("Enemy killed", 5);
    }
}
