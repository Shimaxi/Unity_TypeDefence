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
    [SerializeField] private List<Transform> _enemyList; //�G�̈ʒu�����i�[���Ă������X�g
    [Header("�e�X�e�[�W�ŕύX����")]public int _enemyAmount; //�G�̐�
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
    public GameObject _redSlimePrefab;//������

    void Start()
    {
        //��ʂ̎ז��ɂȂ���̂���U�ǂ���
        _bossPanel.gameObject.SetActive(false);
        _spellManager.enabled = true;

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
        } else if (SceneManager.GetActiveScene().name == "Stage0")
        {
            StartCoroutine("Stage0");
        }

    }

    IEnumerator Stage(int stageNum)
    {
        if(_tutorial != null)
        {
            yield return StartCoroutine(_tutorial.Tutorial(stageNum));
        }

        yield return StartCoroutine(Wave());

        StartCoroutine("Clear");
    }

    IEnumerator Wave()
    {
        GameObject newEnemy;
        GameObject bossEnemy;
        GameObject bossEnemyDisplay;

        for (int i = 0; i < _enemyAmount; i++)
        {
            newEnemy = Instantiate(_slimePrefab, _enemySpawnArea);
            _enemyList.Add(newEnemy.transform);
            _enemyAmountText.text = "�G�̂���" + (_enemyAmount - i - 1).ToString() + "��";
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(3f);

        //�{�X�o��
        bossEnemy = Instantiate(_slimePrefab, _enemySpawnArea);
        bossEnemy.GetComponent<EnemyManager>()._bossText.gameObject.SetActive(true);
        _enemyList.Add(bossEnemy.transform);

        //�{�X���o�ꂵ�����Ƃ��J�b�g�C���Œm�点��
        _bossPanel.gameObject.SetActive(true);
        bossEnemyDisplay = Instantiate(_slimePrefab, _bossPanel);
        bossEnemyDisplay.GetComponent<EnemyManager>().enabled = false;
        bossEnemyDisplay.GetComponent<EnemyManager>()._enemyHPSlider.gameObject.SetActive(false);

        yield return new WaitUntil(() => bossEnemy.activeSelf == false);
    }
    

    IEnumerator Stage0()
    {
        yield return StartCoroutine("Wave0");

        StartCoroutine("Clear");
    }

    IEnumerator Wave0()
    {
        GameObject newEnemy;
        GameObject bossEnemy;
        GameObject bossEnemyDisplay;
        float reinforce = 1;

        for (int i = 0; i < _enemyAmount; i++)
        {
            newEnemy = Instantiate(_slimePrefab, _enemySpawnArea);


            if (i % 10 == 0 &&  i != 0)
            {
                reinforce += 0.2f;
            }

            newEnemy.GetComponent<EnemyManager>()._enemySpeed *= reinforce;
            newEnemy.GetComponent<EnemyManager>()._enemyHP = (int)(newEnemy.GetComponent<EnemyManager>()._enemyHP * reinforce);

            _enemyList.Add(newEnemy.transform);
            _enemyAmountText.text = "�G�̂���" + (_enemyAmount - i - 1).ToString() + "��";
            yield return new WaitForSeconds(1f);
        }

        //�{�X�o��
        bossEnemy = Instantiate(_slimePrefab, _enemySpawnArea);
        bossEnemy.GetComponent<EnemyManager>()._bossText.gameObject.SetActive(true);
        bossEnemy.GetComponent<EnemyManager>()._enemyHP = 100;
        bossEnemy.GetComponent<EnemyManager>()._enemySpeed *= 3.0f;

        _enemyList.Add(bossEnemy.transform);

        //�{�X���o�ꂵ�����Ƃ��J�b�g�C���Œm�点��
        _bossPanel.gameObject.SetActive(true);
        bossEnemyDisplay = Instantiate(_slimePrefab, _bossPanel);
        bossEnemyDisplay.GetComponent<EnemyManager>().enabled = false;
        bossEnemyDisplay.GetComponent<EnemyManager>()._enemyHPSlider.gameObject.SetActive(false);
        bossEnemyDisplay.GetComponent<BoxCollider2D>().enabled = false;

        //�Ō�̃{�X���|�����܂ő҂�
        yield return new WaitUntil(() => bossEnemy.activeSelf == false);
    }


    IEnumerator Clear()
    {
        yield return new WaitForSeconds(1f);
        _audioSource.Stop();
        _audioSource.PlayOneShot(_clearBGM);
        _gameOverText.text = "�X�e�[�W�N���A�I�I�I";
        _gameOverPanel.SetActive(true);
        _spellManager.enabled = false;
    }

   
    void Update()
    {
        //�Ə��̈ړ�(�V�o�[�W����)
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
            while (_enemyList[_targetNum].gameObject.GetComponent<EnemyManager>()._enemyHP == 0)
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

    //playerManager����Ăяo��
    public void GameOver()
    {
        StartCoroutine("GameOverCor");
    }

    IEnumerator GameOverCor()
    {
        yield return new WaitForSeconds(1f);
        _gameOverText.text = "�Q�[���I�[�o�[";
        _gameOverPanel.SetActive(true);
        _spellManager.enabled = false;
    }

    public void OnClickBackToMainBtn()
    {
        SceneManager.LoadScene("Title");
    }
}
