using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyView : MonoBehaviour
{
    [SerializeField] Image enemyImage;
    [SerializeField] Text enemyHP;
    [SerializeField] int enemyMaxHP;

    public void Show(EnemyModel enemyModel)
    {
        enemyMaxHP = enemyModel.enemyHP;
        //���ꂼ��Prefab�̒��̗v�f���C���X�y�N�^�[�Ƀh���b�O���h���b�v
        enemyImage.sprite = enemyModel.enemyImage;
        enemyHP.text = enemyModel.enemyHP.ToString() + " / " + enemyMaxHP.ToString();
    }

    public void Damage(EnemyModel enemyModel)
    {
        enemyHP.text = enemyModel.enemyHP.ToString() + " / " + enemyMaxHP.ToString();
    }
}
