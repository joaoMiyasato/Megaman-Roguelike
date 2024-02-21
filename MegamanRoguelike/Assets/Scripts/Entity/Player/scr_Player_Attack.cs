using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class scr_Player_Attack : MonoBehaviour
{
    [HideInInspector] public scr_Player_Control player;

    [Header("Damage Settings")]
    public int damage;

    [Header("Weapon Setting")]
    public List<data_Weapon> weapons = new List<data_Weapon>();
    private float attackSpeedTime;
    private float time;
    private Vector2 direction;

    [Header("Gun Setting")]

    [Header("Test")]
    public Vector2 testPosition, testSize;
    public bool resetPositions;

    void Start()
    {
        player = GetComponent<scr_Player_Control>();

        foreach (var weapon in weapons)
        {
            if(weapon.IsGun)
            {
                if (weapon.Positions.Count == 0 || resetPositions)
                {
                    weapon.Positions.Add(GameObject.Find("GunPosition1").transform);
                    weapon.Positions.Add(GameObject.Find("GunPosition2").transform);
                }
            }
        }
    }

    void Update()
    {
        if(attackSpeedTime > 0)
        {
            attackSpeedTime -= Time.deltaTime;
        }
        if (player.equipedWeapon != null)
        {
            AttackTrigger();
        }
    }

    int DamageCalculation()
    {
        damage = player.stats.attack.Current + player.equipedWeapon.Attack_Base;

        //Debug.Log("player dmg = "+ player.stats.attack.Current + " Weapon dmg = "+ player.equipedWeapon.Attack_Base + " Total = " + damage);

        return damage;
    }

    void SetMoveAttack(float _time, Vector2 _direction)
    {
        time = _time;
        direction = _direction;
    }

    public void AttackTrigger()
    {
        if(Input.GetKeyDown(player.bt.attack1) && attackSpeedTime <= 0)
        {
            attackSpeedTime = player.equipedWeapon.attackSpeed_Base;
            if(player.equipedWeapon.IsGun)
            {
                ShotAttack1();
            }
        }
        if (Input.GetKeyDown(player.bt.attack2) && attackSpeedTime <= 0)
        {
            attackSpeedTime = player.equipedWeapon.attackSpeed_Base;
            if (player.equipedWeapon.IsGun)
            {
                ShotAttack2();
            }
        }
    }

    public void ShotAttack1()
    {
        GameObject shot = player.equipedWeapon.Projectile;
        shot.GetComponent<NormalShot>().damage = DamageCalculation();
        shot.GetComponent<NormalShot>().speed = player.equipedWeapon.shotSpeed;
        shot.GetComponent<NormalShot>().destructionTime = 1f;
        if (player.facingRight)
        {
            shot.GetComponent<NormalShot>().rightDirection = true;
        }
        else
        {
            shot.GetComponent<NormalShot>().rightDirection = false;
        }
        if (!player.isCrouching)
        {
            Instantiate(shot, GameObject.Find("GunPosition1").transform.position, GameObject.Find("GunPosition1").transform.rotation);
        }
        else
        {
            Instantiate(shot, GameObject.Find("GunPosition2").transform.position, GameObject.Find("GunPosition2").transform.rotation);
        }
    }

    public void ShotAttack2()
    {
        GameObject shot = player.equipedWeapon.Projectile;
        shot.GetComponent<NormalShot>().damage = DamageCalculation();
        shot.GetComponent<NormalShot>().speed = player.equipedWeapon.shotSpeed;
        shot.GetComponent<NormalShot>().destructionTime = 1f;
        if (player.facingRight)
        {
            shot.GetComponent<NormalShot>().rightDirection = true;
        }
        else
        {
            shot.GetComponent<NormalShot>().rightDirection = false;
        }
        if (!player.isCrouching)
        {
            Instantiate(shot, GameObject.Find("GunPosition1").transform.position, GameObject.Find("GunPosition1").transform.rotation);
        }
        else
        {
            Instantiate(shot, GameObject.Find("GunPosition2").transform.position, GameObject.Find("GunPosition2").transform.rotation);
        }
    }

    public void SaberAttackHit1()
    {
        //Collider2D[] enemiesHit =  Physics2D.OverlapBoxAll(spearPos1.position, player.equipedWeapon.Size1, 0);
        //foreach (Collider2D enemy in enemiesHit)
        //{
        //    if(enemy.gameObject.tag == "Enemy" && enemy.gameObject.GetComponentInParent<scr_BaseEntityStats>() != null)
        //    {
        //        enemy.gameObject.GetComponentInParent<scr_BaseEntityStats>().TakeDamage(DamageCalculation(), false);
        //    }
        //}
        //player.isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(spearPos1.position, player.equipedWeapon.Size1);
        //Gizmos.DrawWireCube(spearPos2.position, player.equipedWeapon.Size2);
    }
}
