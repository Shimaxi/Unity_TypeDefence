using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

//敵キャラクターの管理
public class Enemy : MonoBehaviour
{
    //敵の基本的なステータス情報
    public int _enemyHP;//敵のHP
    private int _enemyMaxHP;//敵のMAXHP
    public Slider _enemyHPSlider;//敵のHPバー
    [SerializeField] private Text _enemyHPText;//敵のHPの表示
    [SerializeField] private Sprite _EnemySprite; //敵の画像(確認用)
    public int _enemyAttack;//敵の攻撃力(未実装)
    public float _enemySpeed;//敵の移動速度
    
    private Transform _enemyGoal;//敵が向かう先
    public Animator _anim;//敵のアニメーション管理

    [SerializeField, Header("ノックバック時間")]
    private float _knockBackTime;
    [SerializeField, Header("ノックバックの大きさ")]
    private float _knockBackMagnitude;

    [SerializeField, Header("炎上状態かどうか")]
    private bool _isBurning = false;
    private float _burningCount;
    public Image _burningImage; //炎の画像
    float acolor = 1;//炎の画像の透明度

    [SerializeField]private float _additionalEffectCount;//ノックバック時間をカウントする際に使うやつ

    public Text _bossText;//ボスの表記
    [SerializeField] private GameObject _hitBox;//攻撃のヒットボックス

    

    void Start()
    {
        //不要なものが表示されてしまわないように
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

    void FixedUpdate()
    {
        //移動状態の際には主人公を追尾
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Walking") == true)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, _enemyGoal.position, _enemySpeed * Time.deltaTime); //プレイヤー追尾
        }
        
        //主人公の位置に来たら攻撃
        Vector2 distance;
        distance = this.transform.position - _enemyGoal.position;
        if(distance == new Vector2(0f,0f))
        {
            _anim.SetBool("AttackBool", true);
        }
        else
        {
            _anim.SetBool("AttackBool", false); //暗黙的に1フレーム挟めるので、2回繰り返すのを防げる(SetTriggerでは駄目)
        }

        //やけど状態の際の定数ダメージ
        if(_isBurning == true && _enemyHP > 0)
        {
            //定期的にダメージを与える
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
            //エフェクト(主人公の攻撃の爆発)に触れた際のダメージ計算
            _additionalEffectCount = 0;
            DamageCalculate(); //ダメージ計算
        }
        if (other.gameObject.tag == "Effect2")
        {
            //エフェクト(使い魔の攻撃・ドクロの爆風)に触れた際のダメージ計算
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

    //ダメージ計算+追加効果適応
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
                StartCoroutine("KnockBack");//呪文長さ19以下ではノックバック
            }
            else if (SpellManager.s_spellLength < 40)
            {
                _anim.SetTrigger("Damage");
                StartCoroutine("KnockBack");
                StartCoroutine("Burning");//呪文長さ39以下ではノックバック+炎上
            }
            else if (SpellManager.s_spellLength < 60)
            {
                StartCoroutine("Frozen");//呪文長さ59以下では凍結
            }else if (SpellManager.s_spellLength == 60)
            {
                StartCoroutine("Burning");
                StartCoroutine("Frozen");//呪文長さ60では炎上+凍結
            }
        }
    }

    //ノックバック
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

    //炎上
    IEnumerator Burning()
    {
        _isBurning = true;
        _burningCount = 0;
        yield return null;
    }
    //凍結
    IEnumerator Frozen()
    {
        _anim.SetTrigger("Frozen");
        this.GetComponent<BoxCollider2D>().enabled = true;
        yield return null;
    }


    //体力が0になったら物理演算で吹っ飛ぶ演出
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
