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
        "「敵がやってくるわ！私は魔術の詠唱に集中するから\nあなたはサポートをお願いね！」",
        "彼女は強力な魔法使いですが、呪文の詠唱中は他のことができません\nなので、あなたが彼女をサポートしてあげてください",
        "あなたがキーボードをガチャガチャしている間は彼女は呪文の詠唱を行います\n良い感じに詠唱が出来たらエンターキーで呪文の発動を指示しましょう"};

    private string[] _stageTalk2 = {
        "「転送陣は使ってるかしら？\n敵に近づかれた時にこれを使えば回避出来るの」",
        "「あなたでも分かるよう番号を書いてあげといたわ\n私ってばなんて優しいのかしら！」",
        "・・・",
        "彼女が敵に襲われそうでピンチの時は\n転送陣を使って敵との距離を離してあげてください",
        "転送陣の番号はキーボードの数字キーに対応しています\n出来るだけ彼女を傷つけないよう頑張ってください"};

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
        }

        _stageTalkPanel.SetActive(true);
        for (int talkNum = 0; talkNum < stageTalk.Length; talkNum++)
        {
            _stageTalkText.text = stageTalk[talkNum];
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return)); //なんでか分からないけどこうじゃないと動かない
        }
        _stageTalkPanel.SetActive(false);
    }
}
