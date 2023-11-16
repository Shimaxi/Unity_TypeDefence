using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedScull2 : MonoBehaviour
{
    private Transform _playerTrans;//��l���̈ʒu
    private Vector3 _enemyGoal;//�G����������
    public float _enemySpeed;//�G�̈ړ����x
    public Animator _anim;//�G�̃A�j���[�V�����Ǘ�

    int _bombCount = 0;

    [SerializeField] private GameObject _bombPrefab;
    [SerializeField] private Transform _effectsTrans;

    void Start()
    {
        //�o���ʒu�������㉺�Ƀo��������
        float randomSpwan = Random.Range(-5, 5);
        this.transform.position += new Vector3(0.0f, randomSpwan, 0f);
        
        //�o�����̎�l���̏ꏊ���L�^���āA�����Ɍ������Đi�ނ悤�ɂ���
        _playerTrans = GameObject.Find("Player").GetComponent<Transform>();
        _enemyGoal = _playerTrans.position;
        _effectsTrans = GameObject.Find("Effects").GetComponent<Transform>();
    }
    void FixedUpdate()
    {
        //�ړ���Ԃ̍ۂɂ͎�l���ǔ�
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Walking") == true)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, _enemyGoal, _enemySpeed * Time.deltaTime); //��l���ǔ�

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
            //���Ԍo�߂ő唚��
            if(_bombCount >= 200)
            {
                GameObject bomb = Instantiate(_bombPrefab, this.transform.position, Quaternion.identity);//�G�t�F�N�g����
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
