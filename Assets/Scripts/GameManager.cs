using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

//�����ł�邱��:�X�e�[�W�ł̉�b�E�G�̔����E�Ə��̈ړ�
public class GameManager : MonoBehaviour
{
    //�G�̏o���ɂ���
    [SerializeField] private Transform _enemySpawnArea; //�o����{�ʒu(��������㉺�Ƀ����_���ɂ��炷)
    [SerializeField] private List<Transform> _enemyList; //�G�̏����i�[���Ă������X�g
    public static List<Transform> s_deadEnemyList = new List<Transform>(); //�|�����G�̏����i�[���Ă������X�g
    public static int s_deadEnemyCount; //�y�ی��z
    [Header("�e�X�e�[�W�ŕύX����")]public int _enemyAmount; //�o������G�̐�
    [SerializeField] private Text _enemyAmountText; //�G�̐�������UI�̃e�L�X�g
    [SerializeField] private Transform _bossPanel; //�{�X���o�������ہA�����\������p�l��

    //�Ə��ɂ���(�X�N���v�g��������)
    public Transform _targetTrans; //�Ə��̈ʒu���
    public Transform _targetParentTrans; //�Ə���z�u����e�I�u�W�F�N�g
    private int _targetNum = 0; //�Ə��̓f�t�H���g���ƓG�L�����̓o�ꏇ�ɂȂ��Ă���E�����Ŏg���ϐ�
    //[SerializeField] private Transform _expectedEffectSize; //�G�t�F�N�g�̗\�z�������ʔ͈�

    //BGM�ɂ���
    public AudioSource _audioSource; //BGM��t�@���t�@�[���Ȃǂ𗬂�
    public AudioClip _StageBGM1; //�X�e�[�W��BGM����1
    public AudioClip _clearBGM; //�N���A���̃t�@���t�@�[��(�{����FF�݂����Ƀ��[�v�����ɂ�����)

    //SpellManager���ꎞ�I�ɖ����������肷��
    public SpellManager _spellManager;

    //Tutorial�̕\��
    public TutorialManager _tutorial;

    //�Q�[���I�[�o�[�E�Q�[���N���A�̃p�l��
    public GameObject _gameOverPanel;
    public Text _gameOverText;

    [Header("�ȉ��A�����X�^�[�̃v���n�u������")]
    public GameObject _slimePrefab;
    public GameObject _redSlimePrefab;
    public GameObject _redScullPrefab;
    public GameObject _kakashi;

    //�_���[�W�R���e�X�g���[�h�p�̕�
    [SerializeField] private Text _countDownText;

    //�Q�[���I�[�o�[�������ۂ�
    public bool _isGameOver = false;

    void Start()
    {
        //��ʂ̎ז��ɂȂ���̂���U�ǂ���
        _bossPanel.gameObject.SetActive(false);
        _spellManager.enabled = false;
        s_deadEnemyList.Clear();

        //BGM����
        _audioSource.clip = _StageBGM1;
        _audioSource.Play();
        
        //�X�e�[�W�ɂ���ĕύX
        if (SceneManager.GetActiveScene().name == "Stage1")
        {
            StartCoroutine(Stage(1));
        } else if (SceneManager.GetActiveScene().name == "Stage2")
        {
            StartCoroutine(Stage(2));
        } else if (SceneManager.GetActiveScene().name == "Stage3")
        {
            StartCoroutine(Stage(3));
        } else if (SceneManager.GetActiveScene().name == "Stage4")
        {
            StartCoroutine(Stage(4));
        } else if (SceneManager.GetActiveScene().name == "Stage0")
        {
            StartCoroutine("Stage0");
        } else if (SceneManager.GetActiveScene().name == "StageDamageCon")
        {
            StartCoroutine("StageDamageCon");
        }

    }

    IEnumerator Stage(int stageNum)
    {
        if(_tutorial != null)
        {
            yield return StartCoroutine(_tutorial.Tutorial(stageNum));
        }
        _spellManager.enabled = true;
        yield return StartCoroutine(Wave(stageNum));

        StartCoroutine("Clear");
    }

    //�X�e�[�W1�`4�̊Ǘ�
    IEnumerator Wave(int stageNum)
    {
        GameObject newEnemy;
        GameObject bossEnemy;
        GameObject bossEnemyDisplay;
        if(stageNum == 1)
        {
            for (int i = 0; i < _enemyAmount; i++)
            {   
                if(_isGameOver == false)
                {
                    newEnemy = Instantiate(_slimePrefab, _enemySpawnArea);
                    _enemyList.Add(newEnemy.transform);
                    _enemyAmountText.text = "�G�̂���" + (_enemyAmount - i - 1).ToString() + "��";
                    yield return new WaitForSeconds(1f);
                }
                
            }
        } else if(stageNum == 2)
        {
            for (int i = 0; i < _enemyAmount; i++)
            {
                if(_isGameOver == false)
                {
                    newEnemy = Instantiate(_redSlimePrefab, _enemySpawnArea);
                    _enemyList.Add(newEnemy.transform);
                    _enemyAmountText.text = "�G�̂���" + (_enemyAmount - i - 1).ToString() + "��";
                    yield return new WaitForSeconds(1f);
                }
            }
        } else if(stageNum == 3)
        {
            for (int i = 0; i < _enemyAmount; i++)
            {
                if(_isGameOver == false)
                {
                    if (i % 5 == 0)
                    {
                        newEnemy = Instantiate(_redScullPrefab, _enemySpawnArea);
                    }
                    else
                    {
                        newEnemy = Instantiate(_slimePrefab, _enemySpawnArea);
                        _enemyList.Add(newEnemy.transform);
                    }
                    _enemyAmountText.text = "�G�̂���" + (_enemyAmount - i - 1).ToString() + "��";

                    yield return new WaitForSeconds(1f);
                }
                
            }
        }else if(stageNum == 4)
        {
            for (int i = 0; i < _enemyAmount; i++)
            {
                if(_isGameOver == false)
                {
                    if (i % 5 == 0)
                    {
                        newEnemy = Instantiate(_redSlimePrefab, _enemySpawnArea);
                        _enemyList.Add(newEnemy.transform);
                    }
                    else
                    {
                        newEnemy = Instantiate(_slimePrefab, _enemySpawnArea);
                        _enemyList.Add(newEnemy.transform);
                    }
                    _enemyAmountText.text = "�G�̂���" + (_enemyAmount - i - 1).ToString() + "��";

                    yield return new WaitForSeconds(1f);
                }
                
            }
        }
        
        yield return new WaitUntil(() => s_deadEnemyList.Count >= _enemyAmount);
        yield return new WaitForSeconds(1f);

        //�{�X�o��
        bossEnemy = Instantiate(_slimePrefab, _enemySpawnArea);
        bossEnemy.GetComponent<Enemy>()._bossText.gameObject.SetActive(true);
        bossEnemy.GetComponent<Enemy>()._enemyHP = 100;
        bossEnemy.GetComponent<Transform>().localScale = new Vector3(2f, 2f, 1f);
        _enemyList.Add(bossEnemy.transform);

        //�{�X���o�ꂵ�����Ƃ��J�b�g�C���Œm�点��
        _bossPanel.gameObject.SetActive(true);
        bossEnemyDisplay = Instantiate(_slimePrefab, _bossPanel);
        bossEnemyDisplay.GetComponent<Enemy>().enabled = false;
        bossEnemyDisplay.GetComponent<BoxCollider2D>().enabled = false;
        bossEnemyDisplay.GetComponent<Enemy>()._enemyHPSlider.gameObject.SetActive(false);

        //�Ō�̃{�X���|�����܂ő҂�
        yield return new WaitUntil(() => bossEnemy.activeSelf == false);
    }

    //�X�e�[�W0(�`�������W���[�h)�̊Ǘ�
    IEnumerator Stage0()
    {
        _spellManager.enabled = true;
        yield return StartCoroutine("Wave0");

        StartCoroutine("Clear");
    }

    IEnumerator Wave0()
    {
        GameObject newEnemy;
        GameObject bossEnemy;
        GameObject bossEnemyDisplay;

        for (int i = 0; i < _enemyAmount; i++)
        {
            if(_isGameOver == false)
            {
                int randEnemy;
                randEnemy = UnityEngine.Random.Range(0, 5);
                switch (randEnemy)
                {
                    case 0:
                    case 1:
                    case 2:
                        newEnemy = Instantiate(_slimePrefab, _enemySpawnArea);
                        _enemyList.Add(newEnemy.transform);
                        break;
                    case 3:
                        newEnemy = Instantiate(_redSlimePrefab, _enemySpawnArea);
                        _enemyList.Add(newEnemy.transform);
                        break;
                    case 4:
                        newEnemy = Instantiate(_redScullPrefab, _enemySpawnArea);
                        break;
                }
                _enemyAmountText.text = "�G�̂���" + (_enemyAmount - i - 1).ToString() + "��";
                yield return new WaitForSeconds(1f);
            }
            
        }

        yield return new WaitUntil(() => s_deadEnemyList.Count >= _enemyAmount);

        //�{�X�o��
        bossEnemy = Instantiate(_slimePrefab, _enemySpawnArea);
        bossEnemy.GetComponent<Enemy>()._bossText.gameObject.SetActive(true);
        bossEnemy.GetComponent<Enemy>()._enemyHP = 100;
        bossEnemy.GetComponent<Enemy>()._enemySpeed *= 2.0f;
        bossEnemy.GetComponent<Transform>().localScale = new Vector3(2f, 2f, 1f);

        _enemyList.Add(bossEnemy.transform);

        //�{�X���o�ꂵ�����Ƃ��J�b�g�C���Œm�点��
        _bossPanel.gameObject.SetActive(true);
        bossEnemyDisplay = Instantiate(_slimePrefab, _bossPanel);
        bossEnemyDisplay.GetComponent<Enemy>().enabled = false;
        bossEnemyDisplay.GetComponent<Enemy>()._enemyHPSlider.gameObject.SetActive(false);
        bossEnemyDisplay.GetComponent<BoxCollider2D>().enabled = false;

        //�Ō�̃{�X���|�����܂ő҂�
        yield return new WaitUntil(() => bossEnemy.activeSelf == false);
    }

    IEnumerator Clear()
    {
        for (int i = 0; i < _enemyList.Count; i++)
        {
            _enemyList[i].gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(1f);
        _audioSource.Stop();
        _audioSource.PlayOneShot(_clearBGM);
        _gameOverText.text = "�X�e�[�W�N���A�I�I�I";
        _gameOverPanel.SetActive(true);
        _spellManager.enabled = false;
    }

    //�_���[�W�`�������W�X�e�[�W�̊Ǘ�
    IEnumerator StageDamageCon()
    {
        _enemyList.Add(_kakashi.transform);

        _spellManager.enabled = false;
        yield return new WaitForSeconds(1f);
        _countDownText.gameObject.SetActive(true);
        _countDownText.text = "3";
        yield return new WaitForSeconds(1f);
        _countDownText.text = "2";
        yield return new WaitForSeconds(1f);
        _countDownText.text = "1";
        yield return new WaitForSeconds(1f);
        _countDownText.text = "GO!!!!";
        _spellManager.enabled = true;
        yield return new WaitForSeconds(1f);
        _countDownText.gameObject.SetActive(false);

        yield return new WaitForSeconds(10f);
        _countDownText.text = "Enter�L�[\n�������I";
        _countDownText.gameObject.SetActive(true);
        SpellManager.s_isRestriction = true;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        _countDownText.gameObject.SetActive(false);
        _spellManager.enabled = false;

        StartCoroutine("Record");

    }

    IEnumerator Record()
    {
        yield return new WaitForSeconds(2f);
        _audioSource.Stop();
        _audioSource.PlayOneShot(_clearBGM);
        _gameOverText.text = "�L�^: " + SpellManager.s_spellLength + "�_���[�W";
        _gameOverPanel.SetActive(true);
        _spellManager.enabled = false;
    }

    void Update()
    {
        //�Ə��̈ړ��������ōs��
        //�X�y�[�X�L�[�������ƁA�Ə������Ɍ��ꂽ�G�Ɉڂ�(�[�܂ōs���Ɛ擪�Ƀ��Z�b�g)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_enemyList.Count != 0)
            {
                if(_targetNum + 1 < _enemyList.Count)
                {
                    _targetNum++;
                }
                else
                {
                    _targetNum = 0;
                }

            }
            
        }
        if (_enemyList.Count != 0)
        {
            while (_enemyList[_targetNum].gameObject.GetComponent<Enemy>()._enemyHP == 0)
            {
                _targetNum++;
                if (_targetNum  == _enemyList.Count)
                {
                    _targetNum = 0;
                    break;
                }
            }
            _targetTrans.position = _enemyList[_targetNum].position;
        }
    }

    //playerManager����Ăяo���ăQ�[���I�[�o�[�ɂ���
    public void GameOver()
    {
        _isGameOver = true;
        StartCoroutine("GameOverCor");
    }

    IEnumerator GameOverCor()
    {
        yield return new WaitForSeconds(1f);
        _gameOverText.text = "�Q�[���I�[�o�[";
        if(SceneManager.GetActiveScene().name == "Stage0")
        {
            _gameOverText.text += "\n�L�^: " + s_deadEnemyList.Count +"��";
        }
        _gameOverPanel.SetActive(true);
        _spellManager.enabled = false;
    }

    //�^�C�g���ɖ߂�{�^��
    public void OnClickBackToMainBtn()
    {
        SceneManager.LoadScene("Title");
    }
}
