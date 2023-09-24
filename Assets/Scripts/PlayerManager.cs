using System;
using System.Collections;
using System.Collections.Generic;
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

    //プレイヤーが出す音
    public AudioSource _audioSource;
    public AudioClip _damagedSE;
    public AudioClip _warpSE;

    //プレイヤーの体力
    public Slider _hpSlider;
    public int _playerHP;
    int _playerMaxHP;

    public GameManager _gameManager;
    public CameraManager _cameraManager;

    // Start is called before the first frame update
    void Start()
    {
        _playerMaxHP = _playerHP;
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
        _audioSource.PlayOneShot(_damagedSE);

        CalculateHP();

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
        yield return null;
    }

    //体力の計算(今のところ一律にダメージ1)
    void CalculateHP()
    {
        _playerHP -= 1;
        if (_playerHP <= 0)
        {
            _playerHP = 0;
            _gameManager.GameOver();
        }
        _hpSlider.value = (float)_playerHP/_playerMaxHP;
        _cameraManager.DamagedShake();
    }

   //転送陣を使って移動する
    void Update()
    {
        if (Input.anyKeyDown)
        {
            string keyStr = Input.inputString; // 入力されたキーの名前を取得

            
            switch (keyStr)
            {
                case ("1"):
                case ("2"):
                case ("3"):
                    this.transform.position = _warpPoints[int.Parse(keyStr) - 1].position;
                    _audioSource.PlayOneShot(_warpSE);
                    break;
                default:
                    break;
            }
                
        }
        
    }
}
