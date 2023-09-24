using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class EnemyManager : MonoBehaviour
{
    //敵の基本的なステータス情報
    public int _enemyHP;//敵のHP
    private int _enemyMaxHP;//敵のMAXHP
    public Slider _enemyHPSlider;//敵のHPバー
    [SerializeField] private Text _enemyHPText;//敵のHPの表示
    [SerializeField] private Sprite _EnemySprite; //本当は要らないんだけど、パッと見で分かりやすくするため
    public int _enemyAttack;//敵の攻撃力(未実装)
    public float _enemySpeed;//敵の移動速度
    
    private Transform _enemyGoal;//敵が向かう先
    public Animator _anim;//敵のアニメーション管理

    [SerializeField, Header("ノックバック時間")]
    private float _knockBackTime;
    [SerializeField, Header("ノックバックの大きさ")]
    private float _knockBackMagnitude;

    [SerializeField, Header("氷結時間")]
    private float _frozenTime;
    [SerializeField] private Material _frozenMaterial;//氷結時にかぶせておくマテリアル

    [SerializeField, Header("炎上状態かどうか")]
    private bool _isBurning = false;
    private float _burningCount;
    public Image _burningImage; //炎の画像

    private float _additionalEffectCount;//ノックバックや氷結時間をカウントする際に使うやつ

    public Text _bossText;//ボスの表記
    [SerializeField] private GameObject _hitBox;//攻撃判定

    void Start()
    {
        //不要なものが表示されてしまわないようにする
        _hitBox.SetActive(false);

        //初期体力を設定
        _enemyMaxHP = _enemyHP;
        _enemyHPText.text = _enemyHP.ToString() + " / " + _enemyMaxHP.ToString();
        _enemyHPSlider.value = (float)_enemyHP / _enemyMaxHP;

        //湧き位置を少し上下にランダムになるよう設定
        float randomSpwan = Random.Range(-5, 5);
        this.transform.position += new Vector3(0.0f, randomSpwan, 0f);

        //主人公の位置情報を取得
        _enemyGoal = GameObject.Find("Player").GetComponent<Transform>();
    }

    float acolor = 1;
    void FixedUpdate()
    {
        //移動状態の際には
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Walking") == true)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, _enemyGoal.position, _enemySpeed * Time.deltaTime);
        }
        
        Vector2 distance;
        distance = this.transform.position - _enemyGoal.position;
        if(distance == new Vector2(0f,0f))
        {
            _anim.SetBool("Attack2", true);
        }
        else
        {
            _anim.SetBool("Attack2", false); //暗黙的に1フレーム挟めるので、2回繰り返すのを防げる(SetTriggerでは駄目)
        }

        if(_isBurning == true)
        {
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
                    
                    //_anim.SetTrigger("Damage"); //【要確認】
                    //StartCoroutine("KnockBack");
                }
            }
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Effect")
        {
            _additionalEffectCount = 0;
            DamageCalculate();
        }
    }

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
                StartCoroutine("KnockBack");
            }
            else if (SpellManager.s_spellLength < 40)
            {
                StartCoroutine("KnockBack");
                StartCoroutine("Burning");
            }
            else if (SpellManager.s_spellLength <= 60)
            {
                StartCoroutine("Frozen");
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
        _anim.speed = 0;
        while (_additionalEffectCount < _frozenTime)
        {
            this.transform.position = initPos;
            
            _additionalEffectCount += Time.deltaTime;
            yield return null;
        }
        this.GetComponent<Image>().material = null;
        
        _anim.speed = 1;
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
