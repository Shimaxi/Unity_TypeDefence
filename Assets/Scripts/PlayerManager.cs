using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//���̗v�f�͖{���ɗv�邩�͋c�_�̗]�n�����邩��
public class PlayerManager : MonoBehaviour
{
    //�]���w�̈ʒu�ɂ���
    public Transform[] _warpPoints = new Transform[3];

    //�U�����󂯂���̖��G���Ԃɂ���
    public bool _isHit;
    public float _flashInterval;
    public int _loopCount;
    public Image _imageSelf;
    public BoxCollider2D _colliderSelf;

    //�v���C���[���o����
    public AudioSource _audioSource;
    public AudioClip _damagedSE;
    public AudioClip _warpSE;

    //�v���C���[�̗̑�
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
        //������t���O��true�ɕύX�i�������Ă����ԁj
        _isHit = true;

        _colliderSelf.enabled = false;
        _audioSource.PlayOneShot(_damagedSE);

        CalculateHP();

        //�_�Ń��[�v�J�n
        for (int i = 0; i < _loopCount; i++)
        {
            //flashInterval�҂��Ă���
            yield return new WaitForSeconds(_flashInterval);
            //spriteRenderer���I�t
            _imageSelf.enabled = false;

            //flashInterval�҂��Ă���
            yield return new WaitForSeconds(_flashInterval);
            //spriteRenderer���I��
            _imageSelf.enabled = true;
        }

        //�_�Ń��[�v���������瓖����t���O��false(�������ĂȂ����)
        _isHit = false;
        _colliderSelf.enabled = true;
        yield return null;
    }

    //�̗͂̌v�Z(���̂Ƃ���ꗥ�Ƀ_���[�W1)
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

   //�]���w���g���Ĉړ�����
    void Update()
    {
        if (Input.anyKeyDown)
        {
            string keyStr = Input.inputString; // ���͂��ꂽ�L�[�̖��O���擾

            
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
