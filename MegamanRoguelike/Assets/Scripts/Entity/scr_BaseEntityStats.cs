using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class scr_BaseEntityStats : MonoBehaviour
{
    [Header("Data")]
    public data_Entity_PlayableCharacters dataPlayer;
    public data_Entity_Enemies dataEnemy;

    [Header("Stats")]
    public HealthClass health;
    public AttackClass attack;
    public DeffenseClass deffense;
    public SpeedClass speed;

    private bool knockback = false;

    private void Start() 
    {
        if(dataPlayer != null)
        {
            health.Max = dataPlayer.Health_Max;
            health.Base = dataPlayer.Health_Base;
            health.Current = dataPlayer.Health_Current;

            attack.Base = dataPlayer.Attack_Base;

            deffense.Base = dataPlayer.Deffense_Base;

            speed.Dash = dataPlayer.DashSpeed;
            speed.Move = dataPlayer.MoveSpeed;
        }
        else if (dataEnemy != null)
        {
            health.Max = dataEnemy.Health_Max;
            health.Base = dataEnemy.Health_Base;
            health.Current = dataEnemy.Health_Current;

            attack.Base = dataEnemy.Attack_Base;

            deffense.Base = dataEnemy.Deffense_Base;
        }
        //else if (dataEnemy != null)
        //{
        //    health.Max = data.Health_Max;
        //    health.Base = data.Health_Base;
        //    health.Current = data.Health_Current;

        //    attack.Base = data.Attack_Base;

        //    deffense.Base = data.Deffense_Base;

        //    speed.Dash = data.DashSpeed;
        //    speed.Move = data.MoveSpeed;
        //}
    }

     void Update() 
    {
        AttackControl();
        DeffenseControl();

        Die();
    }

    #region Health
    public void TakeDamage(int _amount, bool _knockback)
    {
        knockback = _knockback;
        int damage = _amount - deffense.Base;
        if (damage < 0)
        {
            damage = 0;
        }

        health.Current -= damage;
        Debug.Log(this.name + " received " + damage + " damage, remain " + health.Current + "/" + health.Max);
    }

    #endregion

    #region Attack
    void AttackControl()
    {
        attack.Current = attack.Base;
    }
    #endregion

    #region Deffense

    void DeffenseControl()
    {
        deffense.Current = deffense.Base;
    }
    #endregion

    void Die()
    {
        if(health.Current <= 0)
        {
            Destroy(this.gameObject);
        }
    }

}

[System.Serializable]
public class HealthClass
{
    public int Max;
    public int Base;
    public int Current;
}

[System.Serializable]
public class AttackClass
{
    public int Base;
    public int Current;
}

[System.Serializable]
public class DeffenseClass
{
    public int Base;
    public int Current;
}

[System.Serializable]
public class SpeedClass
{
    public float Dash;
    public float Move;
    public float Current;
}