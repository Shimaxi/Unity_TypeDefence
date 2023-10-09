using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class EnemyManager : MonoBehaviour
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

    [SerializeField, Header("�X������")]
    private float _frozenTime;
    [SerializeField] private Material _frozenMaterial;//�X�����ɂ��Ԃ��Ă����}�e���A��

    [SerializeField, Header("�����Ԃ��ǂ���")]
    private bool _isBurning = false;
    private float _burningCount;
    public Image _burningImage; //���̉摜

    private float _additionalEffectCount;//�m�b�N�o�b�N��X�����Ԃ��J�E���g����ۂɎg�����

    public Text _bossText;//�{�X�̕\�L
    [SerializeField] private GameObject _hitBox;//�U������

    void Start()
    {
        //�s�v�Ȃ��̂��\������Ă��܂�Ȃ��悤�ɂ���
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

    float acolor = 1;
    void FixedUpdate()
    {
        //�ړ���Ԃ̍ۂɂ�
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Walking") == true)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, _enemyGoal.position, _enemySpeed * Time.deltaTime); //�v���C���[�ǔ�
        }
        
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

        //�₯�Ǐ�Ԃ̍ۂ�
        if(_isBurning == true)
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
                    _additionalEffectCount = 0;
                }
            }
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Effect")
        {
            //�G�t�F�N�g(����)�ɐG�ꂽ��
            _additionalEffectCount = 0;
            DamageCalculate(); //�_���[�W�v�Z
        }
        if (other.gameObject.tag == "Effect2")
        {
            //�G�t�F�N�g(�g�����̍U��)�ɐG�ꂽ��
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
                _knockBackMagnitude = 2.0f;
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

            _anim.SetTrigger("Damage");
            if(SpellManager.s_spellLength < 20)
            {
                StartCoroutine("KnockBack");//��������19�ȉ��ł̓m�b�N�o�b�N
            }
            else if (SpellManager.s_spellLength < 40)
            {
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

    IEnumerator KnockBack()
    {
        Vector3 initPos = this.transform.position;

        while (_additionalEffectCount < _knockBackTime)
        {
            float x = initPos.x + _knockBackMagnitude;
            this.transform.position = new Vector3(x, initPos.y, initPos.z);
            //this.transform.position = new Vector3(initPos.x, initPos.y, initPos.z);
            _additionalEffectCount += Time.deltaTime;

            yield return null;
        }
        _anim.SetTrigger("Walking");
    }

    IEnumerator Frozen()
    {
        Vector3 initPos = this.transform.position;
        this.GetComponent<Image>().material = _frozenMaterial;
        while (_additionalEffectCount < _frozenTime)
        {
            if (_enemyHP != 0)
            {
                this.transform.position = initPos;
            }
            _additionalEffectCount += Time.deltaTime;
            yield return null;
        }
        this.GetComponent<Image>().material = null;
        _anim.SetTrigger("Walking");
        
    }

    IEnumerator Burning()
    {
        _isBurning = true;
        _burningCount = 0;
        yield return null;
    }

    IEnumerator Dead()
    {
        Rigidbody2D rb = this.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(new Vector3(2.0f, 10.0f, 0f), ForceMode2D.Impulse);
        rb.AddTorque(10f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }
}
