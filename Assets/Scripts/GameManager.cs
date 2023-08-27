using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

//ここでやること:敵の発生・照準の移動
public class GameManager : MonoBehaviour
{
    //敵の出現について
    public Transform _enemySpawnArea; //出現位置(ここから上下にランダムにずらす)
    public List<Transform> _enemyList; //敵の位置情報を格納しておくリスト
    public int _enemyAmount; //敵の数
    public Text _enemyAmountText; //敵の数を示すUIのテキスト
    public Transform _bossPanel; //ボスが出現した際、それを表現するパネル

    //照準について
    public Transform _targetTrans; //照準の位置情報
    public Transform _targetParentTrans; //照準を配置する親オブジェクト
    int targetNum = 0; //照準はデフォルトだと敵キャラの登場順になっている・そこで使う変数
    public Transform _expectedEffectSize; //技の予想される効果範囲

    //BGMについて
    public AudioSource _audioSource; //BGMやファンファーレなどを流す
    public AudioClip _StageBGM1; //ステージのBGMその1
    public AudioClip _clearSE; //クリア時のファンファーレ(本当はFFみたいにループするやつにしたい)

    //戦闘前での会話について
    public GameObject _stageTalkObject; //会話のテキストなどの親オブジェクト
    public Text _stageTalkText; 
    private string[] _stageTalk1 = { 
        "「敵がやってくるわ！私は魔術の詠唱に集中するから」\n「あなたはサポートをお願いね！」",
        "彼女は強力な魔法使いですが、呪文の詠唱中は他のことができません\nなので、あなたが彼女をサポートしてあげてください",
        "あなたがキーボードをガチャガチャしている間は彼女は呪文の詠唱を行います\n良い感じに詠唱が出来たらエンターキーで呪文の発動を指示しましょう"};

    private string[] _stageTalk2 = {
        "「転送陣は使ってるかしら？」\n「敵に近づかれた時にこれを使えば回避出来るの」",
        "「あなたでも分かるよう番号を書いてあげといたわ」\n「私ってばなんて優しいのかしら！」",
        "彼女が敵に襲われそうでピンチの時は\n転送陣を使って敵との距離を離してあげてください",
        "転送陣の番号はキーボードの数字キーに対応しています\n出来るだけ彼女を傷つけないよう頑張ってください"};

    //SpellManagerを一時的に無効化したりする
    public SpellManager _spellManager;

    [Header("以下、モンスターのプレハブを入れる")]
    public GameObject _slimePrefab;
    public GameObject _redSlimePrefab;

    public GameObject _gameOverPanel;
    public Text _gameOverText;

    // Start is called before the first frame update
    void Start()
    {
        //画面の邪魔になるものを一旦どかす
        _stageTalkObject.SetActive(false);
        _bossPanel.gameObject.SetActive(false);
        _spellManager.enabled = true;
        //BGM流す
        _audioSource.clip = _StageBGM1;
        _audioSource.Play();
        
        if (SceneManager.GetActiveScene().name == "Stage1")
        {
            StartCoroutine("Stage1");
        } else if (SceneManager.GetActiveScene().name == "Stage2")
        {
            StartCoroutine("Stage2");
        } else if (SceneManager.GetActiveScene().name == "Stage0")
        {
            StartCoroutine("Stage0");
        }

    }

    IEnumerator Stage1()
    {
        yield return StartCoroutine("Tutorial1");

        yield return StartCoroutine("Wave1");

        StartCoroutine("Clear");

    }

    //チュートリアル1
    IEnumerator Tutorial1()
    {
        _spellManager.enabled = false; //一時的に呪文を唱えられないようにする

        //チュートリアル1
        _stageTalkObject.SetActive(true);
        for (int talkNum = 0; talkNum < _stageTalk1.Length; talkNum++)
        {
            _stageTalkText.text = _stageTalk1[talkNum];
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return)); //なんでか分からないけどこうじゃないと動かない
        }
        _stageTalkObject.SetActive(false);
        _spellManager.enabled = true; //お話が終わったので操作可能にする
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
        bossEnemyDisplay.GetComponent<EnemyManager>()._EnemySlider.gameObject.SetActive(false);

        yield return new WaitUntil(() => bossEnemy.activeSelf == false);
    }


    IEnumerator Stage2()
    {
        yield return StartCoroutine("Tutorial2");

        yield return StartCoroutine("Wave2");

        StartCoroutine("Clear");

    }

    IEnumerator Tutorial2()
    {
        _spellManager.enabled = false; //一時的に呪文を唱えられないようにする

        //チュートリアル1
        _stageTalkObject.SetActive(true);
        for (int talkNum = 0; talkNum < _stageTalk2.Length; talkNum++)
        {
            _stageTalkText.text = _stageTalk2[talkNum];
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return)); //なんでか分からないけどこうじゃないと動かない
        }
        _stageTalkObject.SetActive(false);
        _spellManager.enabled = true; //お話が終わったので操作可能にする
    }

    IEnumerator Wave2()
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
        bossEnemyDisplay.GetComponent<EnemyManager>()._EnemySlider.gameObject.SetActive(false);

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

        for (int i = 0; i < _enemyAmount; i++)
        {
            newEnemy = Instantiate(_slimePrefab, _enemySpawnArea);

            if (i < 10)
            {
                newEnemy.GetComponent<EnemyManager>()._EnemySpeed *= 1f;
            } else if (i < 20)
            {
                newEnemy.GetComponent<EnemyManager>()._EnemySpeed *= 1.5f;
            } else if (i < 30)
            {
                newEnemy.GetComponent<EnemyManager>()._EnemySpeed *= 2.0f;
            } else if (i < 40)
            {
                newEnemy.GetComponent<EnemyManager>()._EnemySpeed *= 2.5f;
            } else if (i < 100)
            {
                newEnemy.GetComponent<EnemyManager>()._EnemySpeed *= 3.0f;
            }

            _enemyList.Add(newEnemy.transform);
            _enemyAmountText.text = "敵のこり" + (_enemyAmount - i - 1).ToString() + "体";
            yield return new WaitForSeconds(1f);
        }

        //ボス登場
        bossEnemy = Instantiate(_slimePrefab, _enemySpawnArea);
        bossEnemy.GetComponent<EnemyManager>()._bossText.gameObject.SetActive(true);
        bossEnemy.GetComponent<EnemyManager>()._EnemyHP = 100;
        bossEnemy.GetComponent<EnemyManager>()._EnemySpeed *= 3.0f;

        _enemyList.Add(bossEnemy.transform);

        //ボスが登場したことをカットインで知らせる
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
        _gameOverText.text = "ステージクリア！！！";
        _gameOverPanel.SetActive(true);
        _spellManager.enabled = false;
    }

   
    void Update()
    {
        //照準の移動
        if (_enemyList.Count != targetNum)
        {
            //重いかもしれない(要検討)
            //敵の出現順に照準が移動する
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

        //技の効果範囲を表示
        _expectedEffectSize.transform.localScale = new Vector3(SpellManager.s_effectSize, SpellManager.s_effectSize, 0);

        
    }

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
