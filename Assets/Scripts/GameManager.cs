using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

//ここでやること:ステージでの会話・敵の発生・照準の移動
public class GameManager : MonoBehaviour
{
    //敵の出現について
    [SerializeField] private Transform _enemySpawnArea; //出現基本位置(ここから上下にランダムにずらす)
    [SerializeField] private List<Transform> _enemyList; //敵の情報を格納しておくリスト
    public static List<Transform> s_deadEnemyList = new List<Transform>(); //倒した敵の情報を格納しておくリスト
    public static int s_deadEnemyCount; //【保険】
    [Header("各ステージで変更する")]public int _enemyAmount; //出現する敵の数
    [SerializeField] private Text _enemyAmountText; //敵の数を示すUIのテキスト
    [SerializeField] private Transform _bossPanel; //ボスが出現した際、それを表示するパネル

    //照準について(スクリプト分けたい)
    public Transform _targetTrans; //照準の位置情報
    public Transform _targetParentTrans; //照準を配置する親オブジェクト
    private int _targetNum = 0; //照準はデフォルトだと敵キャラの登場順になっている・そこで使う変数
    //[SerializeField] private Transform _expectedEffectSize; //エフェクトの予想される効果範囲

    //BGMについて
    public AudioSource _audioSource; //BGMやファンファーレなどを流す
    public AudioClip _StageBGM1; //ステージのBGMその1
    public AudioClip _clearBGM; //クリア時のファンファーレ(本当はFFみたいにループするやつにしたい)

    //SpellManagerを一時的に無効化したりする
    public SpellManager _spellManager;

    //Tutorialの表示
    public TutorialManager _tutorial;

    //ゲームオーバー・ゲームクリアのパネル
    public GameObject _gameOverPanel;
    public Text _gameOverText;

    [Header("以下、モンスターのプレハブを入れる")]
    public GameObject _slimePrefab;
    public GameObject _redSlimePrefab;
    public GameObject _redScullPrefab;
    public GameObject _kakashi;

    //ダメージコンテストモード用の物
    [SerializeField] private Text _countDownText;

    //ゲームオーバーしたか否か
    public bool _isGameOver = false;

    void Start()
    {
        //画面の邪魔になるものを一旦どかす
        _bossPanel.gameObject.SetActive(false);
        _spellManager.enabled = false;
        s_deadEnemyList.Clear();

        //BGM流す
        _audioSource.clip = _StageBGM1;
        _audioSource.Play();
        
        //ステージによって変更
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

    //ステージ1～4の管理
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
                    _enemyAmountText.text = "敵のこり" + (_enemyAmount - i - 1).ToString() + "体";
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
                    _enemyAmountText.text = "敵のこり" + (_enemyAmount - i - 1).ToString() + "体";
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
                    _enemyAmountText.text = "敵のこり" + (_enemyAmount - i - 1).ToString() + "体";

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
                    _enemyAmountText.text = "敵のこり" + (_enemyAmount - i - 1).ToString() + "体";

                    yield return new WaitForSeconds(1f);
                }
                
            }
        }
        
        yield return new WaitUntil(() => s_deadEnemyList.Count >= _enemyAmount);
        yield return new WaitForSeconds(1f);

        //ボス登場
        bossEnemy = Instantiate(_slimePrefab, _enemySpawnArea);
        bossEnemy.GetComponent<Enemy>()._bossText.gameObject.SetActive(true);
        bossEnemy.GetComponent<Enemy>()._enemyHP = 100;
        bossEnemy.GetComponent<Transform>().localScale = new Vector3(2f, 2f, 1f);
        _enemyList.Add(bossEnemy.transform);

        //ボスが登場したことをカットインで知らせる
        _bossPanel.gameObject.SetActive(true);
        bossEnemyDisplay = Instantiate(_slimePrefab, _bossPanel);
        bossEnemyDisplay.GetComponent<Enemy>().enabled = false;
        bossEnemyDisplay.GetComponent<BoxCollider2D>().enabled = false;
        bossEnemyDisplay.GetComponent<Enemy>()._enemyHPSlider.gameObject.SetActive(false);

        //最後のボスが倒されるまで待つ
        yield return new WaitUntil(() => bossEnemy.activeSelf == false);
    }

    //ステージ0(チャレンジモード)の管理
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
                _enemyAmountText.text = "敵のこり" + (_enemyAmount - i - 1).ToString() + "体";
                yield return new WaitForSeconds(1f);
            }
            
        }

        yield return new WaitUntil(() => s_deadEnemyList.Count >= _enemyAmount);

        //ボス登場
        bossEnemy = Instantiate(_slimePrefab, _enemySpawnArea);
        bossEnemy.GetComponent<Enemy>()._bossText.gameObject.SetActive(true);
        bossEnemy.GetComponent<Enemy>()._enemyHP = 100;
        bossEnemy.GetComponent<Enemy>()._enemySpeed *= 2.0f;
        bossEnemy.GetComponent<Transform>().localScale = new Vector3(2f, 2f, 1f);

        _enemyList.Add(bossEnemy.transform);

        //ボスが登場したことをカットインで知らせる
        _bossPanel.gameObject.SetActive(true);
        bossEnemyDisplay = Instantiate(_slimePrefab, _bossPanel);
        bossEnemyDisplay.GetComponent<Enemy>().enabled = false;
        bossEnemyDisplay.GetComponent<Enemy>()._enemyHPSlider.gameObject.SetActive(false);
        bossEnemyDisplay.GetComponent<BoxCollider2D>().enabled = false;

        //最後のボスが倒されるまで待つ
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
        _gameOverText.text = "ステージクリア！！！";
        _gameOverPanel.SetActive(true);
        _spellManager.enabled = false;
    }

    //ダメージチャレンジステージの管理
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
        _countDownText.text = "Enterキー\nを押せ！";
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
        _gameOverText.text = "記録: " + SpellManager.s_spellLength + "ダメージ";
        _gameOverPanel.SetActive(true);
        _spellManager.enabled = false;
    }

    void Update()
    {
        //照準の移動をここで行う
        //スペースキーを押すと、照準が次に現れた敵に移る(端まで行くと先頭にリセット)
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

    //playerManagerから呼び出してゲームオーバーにする
    public void GameOver()
    {
        _isGameOver = true;
        StartCoroutine("GameOverCor");
    }

    IEnumerator GameOverCor()
    {
        yield return new WaitForSeconds(1f);
        _gameOverText.text = "ゲームオーバー";
        if(SceneManager.GetActiveScene().name == "Stage0")
        {
            _gameOverText.text += "\n記録: " + s_deadEnemyList.Count +"体";
        }
        _gameOverPanel.SetActive(true);
        _spellManager.enabled = false;
    }

    //タイトルに戻るボタン
    public void OnClickBackToMainBtn()
    {
        SceneManager.LoadScene("Title");
    }
}
