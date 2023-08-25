using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

//�����ł�邱��:�G�̔����E�Ə��̈ړ�
public class GameManager : MonoBehaviour
{
    //�G�̏o���ɂ���
    public Transform _enemySpawnArea; //�o���ʒu(��������㉺�Ƀ����_���ɂ��炷)
    public List<Transform> _enemyList; //�G�̈ʒu�����i�[���Ă������X�g
    public int _enemyAmount; //�G�̐�
    public Text _enemyAmountText; //�G�̐�������UI�̃e�L�X�g
    public Transform _bossPanel; //�{�X���o�������ہA�����\������p�l��

    //�Ə��ɂ���
    public Transform _targetTrans; //�Ə��̈ʒu���
    public Transform _targetParentTrans; //�Ə���z�u����e�I�u�W�F�N�g
    int targetNum = 0; //�Ə��̓f�t�H���g���ƓG�L�����̓o�ꏇ�ɂȂ��Ă���E�����Ŏg���ϐ�
    public Transform _expectedEffectSize; //�Z�̗\�z�������ʔ͈�

    //BGM�ɂ���
    public AudioSource _audioSource; //BGM��t�@���t�@�[���Ȃǂ𗬂�
    public AudioClip _StageBGM1; //�X�e�[�W��BGM����1
    public AudioClip _clearSE; //�N���A���̃t�@���t�@�[��(�{����FF�݂����Ƀ��[�v�����ɂ�����)

    //�퓬�O�ł̉�b�ɂ���
    public GameObject _stageTalkObject; //��b�̃e�L�X�g�Ȃǂ̐e�I�u�W�F�N�g
    public Text _stageTalkText; 
    private string[] _stageTalk1 = { 
        "�u�G������Ă����I���͎����̉r���ɏW�����邩��v\n�u���Ȃ��̓T�|�[�g�����肢�ˁI�v",
        "�ޏ��͋��͂Ȗ��@�g���ł����A�����̉r�����͑��̂��Ƃ��ł��܂���\n�Ȃ̂ŁA���Ȃ����ޏ����T�|�[�g���Ă����Ă�������",
        "���Ȃ����L�[�{�[�h���K�`���K�`�����Ă���Ԃ͔ޏ��͎����̉r�����s���܂�\n�ǂ������ɉr�����o������G���^�[�L�[�Ŏ����̔������w�����܂��傤"};

    //SpellManager���ꎞ�I�ɖ����������肷��
    public SpellManager _spellManager;

    [Header("�ȉ��A�����X�^�[�̃v���n�u������")]
    public GameObject _slimePrefab;
    public GameObject _redSlimePrefab;

    public GameObject _gameOverPanel;
    public Text _gameOverText;

    // Start is called before the first frame update
    void Start()
    {
        //��ʂ̎ז��ɂȂ���̂���U�ǂ���
        _stageTalkObject.SetActive(false);
        _bossPanel.gameObject.SetActive(false);
        _spellManager.enabled = true;
        //BGM����
        _audioSource.clip = _StageBGM1;
        _audioSource.Play();
        
        StartCoroutine("Stage1");
    }

    IEnumerator Stage1()
    {
        yield return StartCoroutine("Tutorial1");

        yield return StartCoroutine("Wave1");

        StartCoroutine("Clear");

    }
    //�`���[�g���A��1
    IEnumerator Tutorial1()
    {
        _spellManager.enabled = false; //�ꎞ�I�Ɏ������������Ȃ��悤�ɂ���

        //�`���[�g���A��1
        _stageTalkObject.SetActive(true);
        for (int talkNum = 0; talkNum < _stageTalk1.Length; talkNum++)
        {
            _stageTalkText.text = _stageTalk1[talkNum];
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return)); //�Ȃ�ł�������Ȃ����ǂ�������Ȃ��Ɠ����Ȃ�
        }
        _stageTalkObject.SetActive(false);
        _spellManager.enabled = true; //���b���I������̂ő���\�ɂ���
    }

    IEnumerator Wave1()
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
        bossEnemyDisplay.GetComponent<EnemyManager>()._EnemySlider.gameObject.SetActive(false);

        yield return new WaitUntil(() => bossEnemy.activeSelf == false);
    }

    IEnumerator Clear()
    {
        yield return new WaitForSeconds(1f);

        /*
        _stageTalkObject.SetActive(true);
        _stageTalkText.text = "Clear!!!";
        _spellManager.enabled = false;
        _audioSource.Stop();
        _audioSource.PlayOneShot(_clearSE);

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        */

        _audioSource.Stop();
        _audioSource.PlayOneShot(_clearSE);
        _gameOverText.text = "�X�e�[�W�N���A�I�I�I";
        _gameOverPanel.SetActive(true);
        _spellManager.enabled = false;
    }

   
    void Update()
    {
        //�Ə��̈ړ�
        if (_enemyList.Count != targetNum)
        {
            //�d����������Ȃ�(�v����)
            //�G�̏o�����ɏƏ����ړ�����
            if (_enemyList[targetNum] != null && _enemyList[targetNum].gameObject.activeSelf == true && _enemyList[targetNum].gameObject.GetComponent<EnemyManager>()._EnemyHP != 0)
            {
                _targetTrans.position = _enemyList[targetNum].position;
            }
            else
            {
                targetNum++;
            }
        }
        else
        {
            _targetTrans.position = Vector3.zero;
        }

        //�Z�̌��ʔ͈͂�\��
        _expectedEffectSize.transform.localScale = new Vector3(SpellManager.s_effectSize, SpellManager.s_effectSize, 0);

        
    }

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
