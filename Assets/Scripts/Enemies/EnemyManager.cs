using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class EnemyManager : MonoBehaviour
{
    public int _EnemyHP;
    private int _EnemyMaxHP;
    public Sprite _EnemySprite;
    public Text _EnemyHPText;
    public int _EnemyAttack;
    public float _EnemySpeed;
    public Slider _EnemySlider;

    public Transform _enemyGoal;

    public Animator _anim;

    [SerializeField, Header("ノックバック時間")]
    private float _knockBackTime;
    [SerializeField, Header("ノックバックの大きさ")]
    private float _knockBackMagnitude;

    private float _knockBackCount;

    public Text _bossText;

    public GameObject _hitBox;


    // Start is called before the first frame update
    void Start()
    {
        _enemyGoal = GameObject.Find("Player").GetComponent<Transform>();
        _EnemyMaxHP = _EnemyHP;
        this.GetComponent<Image>().sprite = _EnemySprite;
        _EnemyHPText.text = _EnemyHP.ToString() + " / " + _EnemyMaxHP.ToString();
        _EnemySlider.value = (float)_EnemyHP / _EnemyMaxHP;

        float randomSpwan = Random.Range(-5, 5);
        this.transform.position += new Vector3(0.0f, randomSpwan, 0f);

        _hitBox.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Vector2.MoveTowards(this.transform.position, _enemyGoal.position, _EnemySpeed * Time.deltaTime);
        Vector2 distance;
        distance = this.transform.position - _enemyGoal.position;
        if(distance == new Vector2(0f,0f))
        {
            StartCoroutine("Attack");
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Effect")
        {
            _knockBackCount = 0;
            Calculate(other.transform);
        }
    }

    void Calculate(Transform collisionTrans)
    {
        //_EnemyHP -= Mathf.CeilToInt(SpellManager.s_spellLength * (1 - (Vector2.Distance(this.transform.position ,collisionTrans.position))/10));
        _EnemyHP -= SpellManager.s_spellDamage;

        if (_EnemyHP <= 0)
        {
            _EnemyHP = 0;
            _EnemyHPText.text = _EnemyHP.ToString() + " / " + _EnemyMaxHP.ToString();
            _EnemySlider.value = (float)_EnemyHP / _EnemyMaxHP;
            StartCoroutine("Dead");
            _anim.SetTrigger("Damage");
            //Destroy(this.gameObject);
            //this.gameObject.SetActive(false);
        }
        else
        {
            _EnemyHPText.text = _EnemyHP.ToString() + " / " + _EnemyMaxHP.ToString();
            _EnemySlider.value = (float)_EnemyHP / _EnemyMaxHP;
            StartCoroutine("KnockBack");
            _anim.SetTrigger("Damage");
        }
    }

    IEnumerator KnockBack()
    {
        Vector3 initPos = this.transform.position;

        while (_knockBackCount < _knockBackTime)
        {
            //float x = initPos.x + _knockBackMagnitude;
            //this.transform.position = new Vector3(x, initPos.y, initPos.z);
            this.transform.position = new Vector3(initPos.x, initPos.y, initPos.z);
            _knockBackCount += Time.deltaTime;

            yield return null;
        }
        
    }

    IEnumerator Attack()
    {
        _anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.2f);
        _hitBox.SetActive(true);
        yield return null;
        _hitBox.SetActive(false);
        yield return new WaitForSeconds(1f);
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
