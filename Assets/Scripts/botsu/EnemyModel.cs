using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel
{
    public int enemyID;
    public string enemyName;
    public Sprite enemyImage;
    public int enemyHP;
    public int enemyAttack;
    public int enemySpeed;

    public EnemyModel(int cardID)
    {
        EnemyEntity enemyEntity = Resources.Load<EnemyEntity>("EnemyEntityList/EnemyEntity" + enemyID);

        enemyID = enemyEntity.enemyID;
        enemyName = enemyEntity.enemyName;
        enemyImage = enemyEntity.enemyImage;
        enemyHP = enemyEntity.enemyHP;
        enemyAttack = enemyEntity.enemyAttack;
        enemySpeed = enemyEntity.enemySpeed;

    }
}
