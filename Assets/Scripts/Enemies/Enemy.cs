using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

//�G�L�����N�^�[�̊Ǘ�
public class Enemy : MonoBehaviour
{
    //�G�̊�{�I�ȃX�e�[�^�X���
    public int _enemyHP;//�G��HP
    private int _enemyMaxHP;//�G��MAXHP
    public Slider _enemyHPSlider;//�G��HP�o�[
    [SerializeField] private Text _enemyHPText;//�G��HP�̕\��
    [SerializeField] private Sprite _EnemySprite; //�G�̉摜(�m�F�p)
    public int _enemyAttack;//�G�̍U����(������)
    public float _enemySpeed;//�G�̈ړ����x
    
    private Transform _enemyGoal;//�G����������
    public Animator _anim;//�G�̃A�j���[�V�����Ǘ�

    [SerializeField, Header("�m�b�N�o�b�N����")]
    private float _knockBackTime;
    [SerializeField, Header("�m�b�N�o�b�N�̑傫��")]
    private float _knockBackMagnitude;

    [SerializeField, Header("�����Ԃ��ǂ���")]
    private bool _isBurning = false;
    private float _burningCount;
    public Image _burningImage; //���̉摜
    float acolor = 1;//���̉摜�̓����x

    [SerializeField]private float _additionalEffectCount;//�m�b�N�o�b�N���Ԃ��J�E���g����ۂɎg�����

    public Text _bossText;//�{�X�̕\�L
    [SerializeField] private GameObject _hitBox;//�U���̃q�b�g�{�b�N�X

    

    void Start()
    {
        //�s�v�Ȃ��̂��\������Ă��܂�Ȃ��悤��
        _hitBox.SetActive(false);

        //�����̗͂�ݒ�
        _enemyMaxHP = _enemyHP;
        _enemyHPText.text = _enemyHP.ToString() + " / " + _enemyMaxHP.ToString();
        _enemyHPSlider.value = (float)_enemyHP / _enemyMaxHP;

        //�N���ʒu�������㉺�Ƀ����_���ɂȂ�悤�ݒ�
        float randomSpwan = Random.Range(-5, 5);
        this.transform.position += new Vector3(0.0f, randomSpwan, 0f);

        //��l���̈ʒu�����擾
        _enemyGoal = GameObject.Find("Player").GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        //�ړ���Ԃ̍ۂɂ͎�l����ǔ�
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Walking") == true)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, _enemyGoal.position, _enemySpeed * Time.deltaTime); //�v���C���[�ǔ�
        }
        
        //��l���̈ʒu�ɗ�����U��
        Vector2 distance;
        distance = this.transform.position - _enemyGoal.position;
        if(distance == new Vector2(0f,0f))
        {
            _anim.SetBool("AttackBool", true);
        }
        else
        {
            _anim.SetBool("AttackBool", false); //�ÖٓI��1�t���[�����߂�̂ŁA2��J��Ԃ��̂�h����(SetTrigger�ł͑ʖ�)
        }

        //�₯�Ǐ�Ԃ̍ۂ̒萔�_���[�W
        if(_isBurning == true && _enemyHP > 0)
        {
            //����I�Ƀ_���[�W��^����
            _burningCount++;
            _burningImage.color = new Color(1, 1, 1, acolor);
            acolor -= Time.deltaTime;
            if (_burningCount % 60 == 0)
            {
                acolor = 1;
                _burningImage.color = new Color(1, 1, 1, 1);
                _enemyHP -= 10;
                if (_enemyHP <= 0)
                {
                    _enemyHP = 0;
                    _enemyHPText.text = _enemyHP.ToString() + " / " + _enemyMaxHP.ToString();
                    _enemyHPSlider.value = (float)_enemyHP / _enemyMaxHP;
                    StartCoroutine("Dead");
                    _anim.SetTrigger("Damage");
                }
                else
                {
                    _enemyHPText.text = _enemyHP.ToString() + " / " + _enemyMaxHP.ToString();
                    _enemyHPSlider.value = (float)_enemyHP / _enemyMaxHP;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Effect")
        {
            //�G�t�F�N�g(��l���̍U���̔���)�ɐG�ꂽ�ۂ̃_���[�W�v�Z
            _additionalEffectCount = 0;
            DamageCalculate(); //�_���[�W�v�Z
        }
        if (other.gameObject.tag == "Effect2")
        {
            //�G�t�F�N�g(�g�����̍U���E�h�N���̔���)�ɐG�ꂽ�ۂ̃_���[�W�v�Z
            _additionalEffectCount = 0;
            _enemyHP -= 20;
            if (_enemyHP <= 0)
            {
                _enemyHP = 0;
                _enemyHPText.text = _enemyHP.ToString() + " / " + _enemyMaxHP.ToString();
                _enemyHPSlider.value = (float)_enemyHP / _enemyMaxHP;
                StartCoroutine("Dead");
                _anim.SetTrigger("Damage");
            }
            else
            {
                _enemyHPText.text = _enemyHP.ToString() + " / " + _enemyMaxHP.ToString();
                _enemyHPSlider.value = (float)_enemyHP / _enemyMaxHP;

                _anim.SetTrigger("Damage");
                StartCoroutine("KnockBack");
            }
        }
    }

    //�_���[�W�v�Z+�ǉ����ʓK��
    void DamageCalculate()
    {
        _enemyHP -= SpellManager.s_spellDamage;

        if (_enemyHP <= 0)
        {
            _enemyHP = 0;
            _enemyHPText.text = _enemyHP.ToString() + " / " + _enemyMaxHP.ToString();
            _enemyHPSlider.value = (float)_enemyHP / _enemyMaxHP;
            StartCoroutine("Dead");
            _anim.SetTrigger("Damage");
        }
        else
        {
            _enemyHPText.text = _enemyHP.ToString() + " / " + _enemyMaxHP.ToString();
            _enemyHPSlider.value = (float)_enemyHP / _enemyMaxHP;

            if(SpellManager.s_spellLength < 20)
            {
                _anim.SetTrigger("Damage");
                StartCoroutine("KnockBack");//��������19�ȉ��ł̓m�b�N�o�b�N
            }
            else if (SpellManager.s_spellLength < 40)
            {
                _anim.SetTrigger("Damage");
                StartCoroutine("KnockBack");
                StartCoroutine("Burning");//��������39�ȉ��ł̓m�b�N�o�b�N+����
            }
            else if (SpellManager.s_spellLength < 60)
            {
                StartCoroutine("Frozen");//��������59�ȉ��ł͓���
            }else if (SpellManager.s_spellLength == 60)
            {
                StartCoroutine("Burning");
                StartCoroutine("Frozen");//��������60�ł͉���+����
            }
        }
    }

    //�m�b�N�o�b�N
    IEnumerator KnockBack()
    {
        Vector3 initPos = this.transform.position;

        while (_additionalEffectCount < _knockBackTime)
        {
            float x = initPos.x + _knockBackMagnitude;
            this.transform.position = new Vector3(x, initPos.y, initPos.z);
            _additionalEffectCount += Time.deltaTime;
            this.GetComponent<BoxCollider2D>().enabled = false;
            yield return null;
        }
        this.GetComponent<BoxCollider2D>().enabled = true;
        _anim.SetTrigger("Walking");
    }

    //����
    IEnumerator Burning()
    {
        _isBurning = true;
        _burningCount = 0;
        yield return null;
    }
    //����
    IEnumerator Frozen()
    {
        _anim.SetTrigger("Frozen");
        this.GetComponent<BoxCollider2D>().enabled = true;
        yield return null;
    }


    //�̗͂�0�ɂȂ����畨�����Z�Ő�����ԉ��o
    IEnumerator Dead()
    {
        _isBurning = false;
        GameManager.s_deadEnemyList.Add(this.transform);
        Rigidbody2D rb = this.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(new Vector3(2.0f, 10.0f, 0f), ForceMode2D.Impulse);
        rb.AddTorque(10f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }
}
