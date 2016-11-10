using System;
using System.Collections;
using UnityEngine;

public abstract class Character : MonoBehaviour, ICharacter
{
    [SerializeField]
    private Vector3 healthbarOffset;
    public CharacterData Stats { get; protected set; }

    private Healthbar healthbar;
    [SerializeField] private float health;

    protected virtual void Awake()
    {
        healthbar = gameObject.AddComponent<Healthbar>();
        healthbar.Initialize(transform.position + healthbarOffset);

        Reset();
    }

    protected virtual CharacterData LoadData(string dataName)
    {
        CharacterData data = GameManager.LoadFile<CharacterData>(dataName);

        if (data != null)
        {
            return data;
        }

        Debug.Log(dataName + " has not been found!");

        // Create data with standard stats. This should never happen!
        return new CharacterData(100, 10, 10, 50);
    }

    public virtual void Reset()
    {
        Health = Stats.MaxHealth;
    }

    public abstract void InitializeTurn();

    public abstract void HandleTurn();

    public abstract void PerformAttack(AttackInfo info);

    public void ReceiveAttack(AttackInfo info)
    {
        Health -= info.damage;
    }

    public virtual void Die()
    {
    }

    public float Health
    {
        get { return health; }
        protected set
        {
            health = Mathf.Clamp(value, 0, Stats.MaxHealth);
            healthbar.SetHealthBar(health / Stats.MaxHealth);

            if (health <= 0)
            {
                Die();
            }
        }
    }

    public bool IsDead
    {
        get { return health <= 0; }
    }
}
