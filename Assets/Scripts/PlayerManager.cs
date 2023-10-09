using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//この要素は本当に要るかは議論の余地があるかも
public class PlayerManager : MonoBehaviour
{
    //転送陣の位置について
    public Transform[] _warpPoints = new Transform[3];

    //攻撃を受けた後の無敵時間について
    public bool _isHit;
    public float _flashInterval;
    public int _loopCount;
    public Image _imageSelf;
    public BoxCollider2D _colliderSelf;

    public Animator _damagedAnim;

    private int _currentPosition = 1;

    //使い魔の行動
    public Transform[] _barkEffects = new Transform[3];
    [SerializeField] private GameObject _catHelpPanel;


    //プレイヤーが出す音
    public AudioSource _audioSource;
    public AudioClip _damagedSE;
    public AudioClip _barkSE;
    public AudioClip _warpSE;
    public AudioClip _downSE;

    //プレイヤーの体力
    public int _playerHP;
    int _playerMaxHP;
    public Transform _hpTrans;
    public GameObject _heartPrefab;
    private GameObject[] _heart = new GameObject[4];

    //プレイヤーの立ち絵(アニメーションで管理したほうが良いか？)
    public Image _playerTachie;
    [SerializeField] private Sprite _playerNormal;
    [SerializeField] private Sprite _playerChanting;
    [SerializeField] private Sprite _playerLose;


    public GameManager _gameManager;
    public CameraManager _cameraManager;

    // Start is called before the first frame update
    void Start()
    {
        _playerMaxHP = _playerHP;
        _playerTachie.sprite = _playerNormal;

        for(int i = 0; i < _playerMaxHP; i++)
        {
            _heart[i] = Instantiate(_heartPrefab,_hpTrans);
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "EnemyAttack")
        {
            StartCoroutine("Hit");
        }
    }

    IEnumerator Hit()
    {
        //当たりフラグをtrueに変更（当たっている状態）
        _isHit = true;
        _colliderSelf.enabled = false;

        //体力の計算(今のところ一律にダメージ1)
        _playerHP -= 1;

        if (_playerHP <= 0)
        {
            _audioSource.PlayOneShot(_damagedSE);
            _audioSource.PlayOneShot(_downSE);
            _playerTachie.sprite = _playerLose;
            _damagedAnim.SetTrigger("Damaged");
            _playerHP = 0;
            ShowHP();
            _gameManager.GameOver();
        }
        else
        {
            
            _audioSource.PlayOneShot(_damagedSE);
            _audioSource.PlayOneShot(_barkSE);
            ShowHP();
            _cameraManager.DamagedShake();
            _catHelpPanel.SetActive(true);
            StartCoroutine(Bark());
            _damagedAnim.SetTrigger("Damaged");

         //点滅ループ開始
            for (int i = 0; i < _loopCount; i++)
            {
                //flashInterval待ってから
                yield return new WaitForSeconds(_flashInterval);
                //spriteRendererをオフ
                _imageSelf.enabled = false;

                //flashInterval待ってから
                yield return new WaitForSeconds(_flashInterval);
                //spriteRendererをオン
                _imageSelf.enabled = true;
            }
        
            //点滅ループが抜けたら当たりフラグをfalse(当たってない状態)
            _isHit = false;
            _colliderSelf.enabled = true;
        

            _catHelpPanel.SetActive(false);
        }

        yield return null;
    }


    //ダメージを受けると使い魔が一時的に敵を退ける
    IEnumerator Bark()
    {
        
        GameObject bark = _barkEffects[_currentPosition].gameObject;
        bark.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        bark.SetActive(false);
        
    }

    //体力の表示を行う
    void ShowHP()
    {
        for (int i = 0; i < _playerHP; i++)
        {
            _heart[i].SetActive(true);
        }
        for (int i = _playerHP; i < _playerMaxHP; i++)
        {
            _heart[i].SetActive(false);
        }
    }

    //転送陣の移動を適応する
    void Update()
    {
        if (Input.anyKeyDown && _playerHP != 0)
        {
            string keyStr = Input.inputString; // 入力されたキーの名前を取得

            
            switch (keyStr)
            {
                case ("1"):
                case ("2"):
                case ("3"):
                    this.transform.position = _warpPoints[int.Parse(keyStr) - 1].position;
                    _currentPosition = int.Parse(keyStr) - 1;
                    _audioSource.PlayOneShot(_warpSE);
                    break;
                default:
                    break;
            }
                
        }
    }
}
