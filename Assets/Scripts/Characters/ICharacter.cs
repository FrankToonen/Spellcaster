public interface ICharacter
{
    void Reset();
    void InitializeTurn();
    void HandleTurn();
    void PerformAttack(AttackInfo info);
    void ReceiveAttack(AttackInfo info);
    void Die();
    
    CharacterData Stats { get; }
    float Health { get; }
    bool IsDead { get; }
}
