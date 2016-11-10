public struct AttackInfo
{
    // TODO: Split damage in physical & elemental damage
    // TODO: Add 'Status effects' to this struct (as booleans? or a decorator pattern type stuff).
    public float damage;

    public AttackInfo(float damage)
    {
        this.damage = damage;
    }
}
