using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedScull2 : MonoBehaviour
{
    private Transform _playerTrans;//主人公の位置
    private Vector3 _enemyGoal;//敵が向かう先
    public float _enemySpeed;//敵の移動速度
    public Animator _anim;//敵のアニメーション管理

    int _bombCount = 0;

    [SerializeField] private GameObject _bombPrefab;
    [SerializeField] private Transform _effectsTrans;

    void Start()
    {
        //出現位置を少し上下にバラつかせる
        float randomSpwan = Random.Range(-5, 5);
        this.transform.position += new Vector3(0.0f, randomSpwan, 0f);
        
        //出現時の主人公の場所を記録して、そこに向かって進むようにする
        _playerTrans = GameObject.Find("Player").GetComponent<Transform>();
        _enemyGoal = _playerTrans.position;
        _effectsTrans = GameObject.Find("Effects").GetComponent<Transform>();
    }
    void FixedUpdate()
    {
        //移動状態の際には主人公追尾
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Walking") == true)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, _enemyGoal, _enemySpeed * Time.deltaTime); //主人公追尾

            Vector2 distance;
            distance = this.transform.position - _enemyGoal;
            if (distance == new Vector2(0f, 0f))
            {
                _anim.SetTrigger("SucideAttack");
            }
        }else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("SucideAttack") == true)
        {
            _bombCount++;
            _anim.speed = _bombCount * 0.02f;
            //時間経過で大爆発
            if(_bombCount >= 200)
            {
                GameObject bomb = Instantiate(_bombPrefab, this.transform.position, Quaternion.identity);//エフェクト生成
                bomb.transform.SetParent(_effectsTrans);
                bomb.transform.localScale = new Vector3(3, 3, 0);
                StartCoroutine("Dead");
                this.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator Dead()
    {
        GameManager.s_deadEnemyList.Add(this.transform);
        yield return null;
    }
}
