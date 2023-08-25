using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//���̗v�f�͖{���ɗv�邩�͋c�_�̗]�n�����邩��
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
            Debug.Log("�U�����󂯂��I");
            StartCoroutine("Hit");
        }
    }

    IEnumerator Hit()
    {
        //������t���O��true�ɕύX�i�������Ă����ԁj
        _isHit = true;

        _colliderSelf.enabled = false;
        _audioSource.PlayOneShot(_damaged);

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

    //�ʂ̏��Ɉڂ��������ǂ�����
    

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
                    break;
                default:
                    break;
            }
                
        }
        
    }
}
