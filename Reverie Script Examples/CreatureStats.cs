using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CreatureStats : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public int attack;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        Stun(damageAmount);
        if (health <= 0)
        {
            Die();
        }
    }
    public virtual void TakeDamage(int damageAmount, int stunAmount)
    {
        health -= damageAmount;
        Stun(stunAmount);
        if (health <= 0)
        {
            Die();
        }
    }
    public virtual void TakeDamage(int damageAmount, int stunAmount, GameObject damageSource)
    {
        health -= damageAmount;
        Stun(stunAmount);
        if (health <= 0)
        {
            Die();
        }
    }
    public virtual void TakeDamage(int damageAmount, GameObject damageSource)
    {
        health -= damageAmount;
        Stun(damageAmount);
        if (health <= 0)
        {
            Die();
        }
    }
    public abstract void Heal(int healAmount);
    public virtual void Stun(int stunRecieved)
    {

    }
    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
