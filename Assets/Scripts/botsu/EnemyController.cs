using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyModel enemyModel; 
    public EnemyView enemyView; 

    public void Init(int enemyID)
    {
        enemyModel = new EnemyModel(enemyID);
        enemyView.Show(enemyModel);
    }

}
