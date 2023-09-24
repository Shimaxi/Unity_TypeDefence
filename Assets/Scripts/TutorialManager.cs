using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    //�퓬�O�ł̉�b�ɂ���
    public GameObject _stageTalkPanel; //��b�̃e�L�X�g�Ȃǂ̐e�I�u�W�F�N�g
    public Text _stageTalkText;

    private string[] _stageTalk1 = {
        "�u�G������Ă����I���͖��p�̉r���ɏW�����邩��\n���Ȃ��̓T�|�[�g�����肢�ˁI�v",
        "�ޏ��͋��͂Ȗ��@�g���ł����A�����̉r�����͑��̂��Ƃ��ł��܂���\n�Ȃ̂ŁA���Ȃ����ޏ����T�|�[�g���Ă����Ă�������",
        "���Ȃ����L�[�{�[�h���K�`���K�`�����Ă���Ԃ͔ޏ��͎����̉r�����s���܂�\n�ǂ������ɉr�����o������G���^�[�L�[�Ŏ����̔������w�����܂��傤"};

    private string[] _stageTalk2 = {
        "�u�]���w�͎g���Ă邩����H\n�G�ɋ߂Â��ꂽ���ɂ�����g���Ή���o����́v",
        "�u���Ȃ��ł�������悤�ԍ��������Ă����Ƃ�����\n�����Ă΂Ȃ�ėD�����̂�����I�v",
        "�E�E�E",
        "�ޏ����G�ɏP��ꂻ���Ńs���`�̎���\n�]���w���g���ēG�Ƃ̋����𗣂��Ă����Ă�������",
        "�]���w�̔ԍ��̓L�[�{�[�h�̐����L�[�ɑΉ����Ă��܂�\n�o���邾���ޏ��������Ȃ��悤�撣���Ă�������"};

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
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return)); //�Ȃ�ł�������Ȃ����ǂ�������Ȃ��Ɠ����Ȃ�
        }
        _stageTalkPanel.SetActive(false);
    }
}
