using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyEntity", menuName = "Create EnemyEntity")]
public class EnemyEntity : ScriptableObject
{
    public int enemyID;
    public string enemyName;
    public Sprite enemyImage;
    public int enemyHP;
    public int enemyAttack;
    public int enemySpeed;
}
