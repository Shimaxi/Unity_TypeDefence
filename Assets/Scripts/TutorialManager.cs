using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    //戦闘前での会話について
    public GameObject _stageTalkPanel; //会話のテキストなどの親オブジェクト
    public Text _stageTalkText;

    private string[] _stageTalk1 = {
        "魔法少女「敵がやってくるわ！\nえーっと、確か魔法の使い方は...」",
        "「<color=#dc143c>キーボードをガチャガチャ</color>して呪文を詠唱して...\nある程度詠唱出来たら<color=#dc143c>Enterキー</color>を押して魔法を発動するんだったわね」",
        "「そして<color=#dc143c>敵に与えるダメージは詠唱した長さと同じ</color>...と\nよし、大丈夫！私なら出来るはず！」",
        "使い魔(猫)「ニャー」(大丈夫かにゃー...)",
        "基本的な操作は今言ったとおりです\n最初は難しいことを考えないで思った通りにやってみましょう！"};

    private string[] _stageTalk2 = {
        "魔法少女「呪文の詠唱には段階があって、<color=#dc143c>20字以上で炎上</color>、<color=#dc143c>40字以上で凍結</color>、\n<color=#dc143c>60字の完全詠唱でその両方の効果が発動</color>するんだったわね」",
        "「ってことは毎回完全詠唱すれば最強じゃない！簡単ね！」",
        "使い魔(猫)「ニャニャニャ」(この子はこう言ってるけど、\n敵のHPを見極めて必要十分のダメージ与えるのがオススメにゃ)",
        "『<color=#dc143c>炎上は一定時間ごとに10ダメージ</color>』『<color=#dc143c>凍結は敵を一定時間停止させる</color>』\nという効果があります。うまく使いこなしてクリアしましょう！"};

    private string[] _stageTalk3 = {
        "魔法少女「そういえば<color=#dc143c>転送陣</color>を作っていたのを忘れていたわ！\n敵に近づかれた時にこれを使えば回避出来るの」",
        "「<color=#dc143c>数字キーを押すと、対応した転送陣に移動</color>できるわ\n使わないで済むのが一番なんだけどね...」",
        "使い魔(猫)「ニャ！」(ドクロの敵に注意にゃ！)",
        "このステージでは<color=#dc143c>ドクロの敵</color>が登場します\nこいつには攻撃が効きません。転送陣を使って回避しましょう",
        "ドクロは一定時間経過すると<color=#dc143c>自爆攻撃</color>を行います\nどうやらこの爆発は敵にもダメージがあるようですよ...!"};

    private string[] _stageTalk4 = {
        "最後に発展テクニックを紹介します",
        "<color=#dc143c>Spaceキーを押すと、魔法の照準が一つ後に出てきた敵に移動</color>します\nどうしても今の照準が気に入らない時に使いましょう",
        "戻るボタンはないですが、最後まで照準を動かせば先頭に戻ってきます\nでもその分手が取られちゃうので、上手に使えるかどうかはあなたの腕次第です",
        "使い魔(猫)「ニャ？」(今何か聞こえた気がするにゃ？)"};

    private string[] _stageTalkDamage = {
        "ダメージコンテスト！\n10秒間でどれだガチャガチャ出来るかな？" };

    string[] stageTalk;

    public IEnumerator Tutorial(int stageNum)
    {

        switch (stageNum)
        {
            case 1:
                stageTalk = _stageTalk1;
                break;
            case 2:
                stageTalk = _stageTalk2;
                break;
            case 3:
                stageTalk = _stageTalk3;
                break;
            case 4:
                stageTalk = _stageTalk4;
                break;
            case 1000:
                stageTalk = _stageTalkDamage;
                break;
        }

        _stageTalkPanel.SetActive(true);
        for (int talkNum = 0; talkNum < stageTalk.Length; talkNum++)
        {
            _stageTalkText.text = stageTalk[talkNum];
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return)); 
        }
        _stageTalkPanel.SetActive(false);
    }
}
