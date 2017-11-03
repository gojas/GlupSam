﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

    public int playerDamage;

    private Animator animator;
    private Transform target;
    private bool skipMove;

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        // float.Epsilon number close to 0
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        animator.SetTrigger("enemyAttack");

        hitPlayer.LoseFood(playerDamage);
    }

    protected override void Start () {
        GameManager.instance.AddEnemyToList(this);

        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        base.Start();
	}

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            // every second time
            skipMove = false;
            return;
        }

        base.AttemptMove<Player>(xDir, yDir);

        // it moved
        skipMove = true;
    }
}
