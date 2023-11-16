using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    //�]���w�̈ʒu�ɂ���
    public Transform[] _warpPoints = new Transform[3];
    private int _currentPosition = 1;

    //�U�����󂯂���̖��G���Ԃɂ���
    public bool _isHit;
    public float _flashInterval;
    public int _loopCount;
    public Image _imageSelf;
    public BoxCollider2D _colliderSelf;
    public Animator _damagedAnim;

    //�g�����̃w���v
    public Transform[] _barkEffects = new Transform[3];
    [SerializeField] private GameObject _catHelpPanel;


    //�v���C���[�}�l�[�W���[����o����
    public AudioSource _audioSource;
    public AudioClip _damagedSE;
    public AudioClip _barkSE;
    public AudioClip _warpSE;
    public AudioClip _downSE;

    //�v���C���[�̗̑͂�UI
    public int _playerHP;
    int _playerMaxHP;
    public Transform _hpTrans;
    public GameObject _heartPrefab;
    private GameObject[] _heart = new GameObject[4];

    //�v���C���[�̗����G(�A�j���[�V�����ŊǗ������ق����ǂ����H)
    public Image _playerTachie;
    [SerializeField] private Sprite _playerNormal;
    [SerializeField] private Sprite _playerChanting;
    [SerializeField] private Sprite _playerLose;


    public GameManager _gameManager;
    public CameraManager _cameraManager;

    void Start()
    {
        //�v���C���[�̗̑�UI�\��
        _playerMaxHP = _playerHP;
        _playerTachie.sprite = _playerNormal;

        for(int i = 0; i < _playerMaxHP; i++)
        {
            _heart[i] = Instantiate(_heartPrefab,_hpTrans);
        }
    }

    //�G�̍U���Ń_���[�W
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "EnemyAttack")
        {
            StartCoroutine("Hit");
        }
        if (other.gameObject.tag == "Effect2")
        {
            StartCoroutine("Hit");
        }
    }

    //�G�̍U�������������
    IEnumerator Hit()
    {
        //������t���O��true�ɕύX�i�������Ă����ԁj
        _isHit = true;
        _colliderSelf.enabled = false;

        //�̗͂̌v�Z(���̂Ƃ���ꗥ�Ƀ_���[�W1)
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
        

            _catHelpPanel.SetActive(false);
        }

        yield return null;
    }


    //�_���[�W���󂯂�Ǝg�������ꎞ�I�ɓG��ނ���
    IEnumerator Bark()
    {
        GameObject bark = _barkEffects[_currentPosition].gameObject;
        bark.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        bark.SetActive(false);
    }

    //�̗͂̕\�����s��
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

    //�]���w�̈ړ���K������
    void Update()
    {
        if (Input.anyKeyDown && _playerHP != 0)
        {
            string keyStr = Input.inputString; // ���͂��ꂽ�L�[�̖��O���擾

            
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
