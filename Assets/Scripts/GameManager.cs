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
    [SerializeField] private List<Transform> _enemyList; //敵の位置情報を格納しておくリスト
    [Header("各ステージで変更する")]public int _enemyAmount; //敵の数
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
    public GameObject _redSlimePrefab;//未実装

    void Start()
    {
        //画面の邪魔になるものを一旦どかす
        _bossPanel.gameObject.SetActive(false);
        _spellManager.enabled = true;

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
            _enemyAmountText.text = "敵のこり" + (_enemyAmount - i - 1).ToString() + "体";
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(3f);

        //ボス登場
        bossEnemy = Instantiate(_slimePrefab, _enemySpawnArea);
        bossEnemy.GetComponent<EnemyManager>()._bossText.gameObject.SetActive(true);
        _enemyList.Add(bossEnemy.transform);

        //ボスが登場したことをカットインで知らせる
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
            _enemyAmountText.text = "敵のこり" + (_enemyAmount - i - 1).ToString() + "体";
            yield return new WaitForSeconds(1f);
        }

        //ボス登場
        bossEnemy = Instantiate(_slimePrefab, _enemySpawnArea);
        bossEnemy.GetComponent<EnemyManager>()._bossText.gameObject.SetActive(true);
        bossEnemy.GetComponent<EnemyManager>()._enemyHP = 100;
        bossEnemy.GetComponent<EnemyManager>()._enemySpeed *= 3.0f;

        _enemyList.Add(bossEnemy.transform);

        //ボスが登場したことをカットインで知らせる
        _bossPanel.gameObject.SetActive(true);
        bossEnemyDisplay = Instantiate(_slimePrefab, _bossPanel);
        bossEnemyDisplay.GetComponent<EnemyManager>().enabled = false;
        bossEnemyDisplay.GetComponent<EnemyManager>()._enemyHPSlider.gameObject.SetActive(false);
        bossEnemyDisplay.GetComponent<BoxCollider2D>().enabled = false;

        //最後のボスが倒されるまで待つ
        yield return new WaitUntil(() => bossEnemy.activeSelf == false);
    }


    IEnumerator Clear()
    {
        yield return new WaitForSeconds(1f);
        _audioSource.Stop();
        _audioSource.PlayOneShot(_clearBGM);
        _gameOverText.text = "ステージクリア！！！";
        _gameOverPanel.SetActive(true);
        _spellManager.enabled = false;
    }

   
    void Update()
    {
        //照準の移動(新バージョン)
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

    //playerManagerから呼び出し
    public void GameOver()
    {
        StartCoroutine("GameOverCor");
    }

    IEnumerator GameOverCor()
    {
        yield return new WaitForSeconds(1f);
        _gameOverText.text = "ゲームオーバー";
        _gameOverPanel.SetActive(true);
        _spellManager.enabled = false;
    }

    public void OnClickBackToMainBtn()
    {
        SceneManager.LoadScene("Title");
    }
}
