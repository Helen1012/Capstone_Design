using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    private Animator animator;

    private Transform playerTr;

    private Transform enemyTr;
    private SC2PlayerDamage playerDamage;
    private readonly int hashMeleeAttack = Animator.StringToHash("MeleeAttack");
    private readonly int hashMeleeAttackIdx = Animator.StringToHash("MeleeAttackIdx");

    private float nextAttack = 0.0f;
    private readonly float AttackRate = 0.3f;

    public bool isMeleeAttack = false;
    // Start is called before the first frame update
    void Start()
    {
        playerDamage = GameObject.Find("Handgun_01_FPSController").GetComponent<SC2PlayerDamage>();
        playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        enemyTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMeleeAttack)
        {
            if (Time.time >= nextAttack)
            {
                Attack();
                playerDamage.AttackedByMelee();
                nextAttack = Time.time + AttackRate + 2f;  // 공속딜레이
            }

        }
    }

    private void Attack()
    {
        animator.SetTrigger(hashMeleeAttack);
        animator.SetInteger(hashMeleeAttackIdx, (Random.Range(1, 4)));
    }

}