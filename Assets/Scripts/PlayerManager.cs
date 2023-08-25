using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//この要素は本当に要るかは議論の余地があるかも
public class PlayerManager : MonoBehaviour
{
    public Transform[] _warpPoints = new Transform[3];

    [SerializeField] float _flashInterval;

    public bool _isHit;
    public int _loopCount;
    public Image _imageSelf;
    public BoxCollider2D _colliderSelf;

    public AudioSource _audioSource;
    public AudioClip _damaged;

    public Slider _hpSlider;
    public int _playerHP;
    int _playerMaxHP;

    public GameObject _gameOverPanel;

    public GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _playerMaxHP = _playerHP;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "EnemyAttack")
        {
            Debug.Log("攻撃を受けた！");
            StartCoroutine("Hit");
        }
    }

    IEnumerator Hit()
    {
        //当たりフラグをtrueに変更（当たっている状態）
        _isHit = true;

        _colliderSelf.enabled = false;
        _audioSource.PlayOneShot(_damaged);

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

    void CalculateHP()
    {
        _playerHP -= 1;
        if (_playerHP <= 0)
        {
            _playerHP = 0;
            _gameManager.GameOver();
            
        }
        _hpSlider.value = (float)_playerHP/_playerMaxHP;
    }

    //別の所に移した方が良さそう
    

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
                    break;
                default:
                    break;
            }
                
        }
        
    }
}
