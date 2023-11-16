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
        "���@�����u�G������Ă����I\n���[���ƁA�m�����@�̎g������...�v",
        "�u<color=#dc143c>�L�[�{�[�h���K�`���K�`��</color>���Ď������r������...\n������x�r���o������<color=#dc143c>Enter�L�[</color>�������Ė��@�𔭓�����񂾂�����ˁv",
        "�u������<color=#dc143c>�G�ɗ^����_���[�W�͉r�����������Ɠ���</color>...��\n�悵�A���v�I���Ȃ�o����͂��I�v",
        "�g����(�L)�u�j���[�v(���v���ɂ�[...)",
        "��{�I�ȑ���͍��������Ƃ���ł�\n�ŏ��͓�����Ƃ��l���Ȃ��Ŏv�����ʂ�ɂ���Ă݂܂��傤�I"};

    private string[] _stageTalk2 = {
        "���@�����u�����̉r���ɂ͒i�K�������āA<color=#dc143c>20���ȏ�ŉ���</color>�A<color=#dc143c>40���ȏ�œ���</color>�A\n<color=#dc143c>60���̊��S�r���ł��̗����̌��ʂ�����</color>����񂾂�����ˁv",
        "�u���Ă��Ƃ͖��񊮑S�r������΍ŋ�����Ȃ��I�ȒP�ˁI�v",
        "�g����(�L)�u�j���j���j���v(���̎q�͂��������Ă邯�ǁA\n�G��HP�����ɂ߂ĕK�v�\���̃_���[�W�^����̂��I�X�X���ɂ�)",
        "�w<color=#dc143c>����͈�莞�Ԃ��Ƃ�10�_���[�W</color>�x�w<color=#dc143c>�����͓G����莞�Ԓ�~������</color>�x\n�Ƃ������ʂ�����܂��B���܂��g�����Ȃ��ăN���A���܂��傤�I"};

    private string[] _stageTalk3 = {
        "���@�����u����������<color=#dc143c>�]���w</color>������Ă����̂�Y��Ă�����I\n�G�ɋ߂Â��ꂽ���ɂ�����g���Ή���o����́v",
        "�u<color=#dc143c>�����L�[�������ƁA�Ή������]���w�Ɉړ�</color>�ł����\n�g��Ȃ��ōςނ̂���ԂȂ񂾂��ǂ�...�v",
        "�g����(�L)�u�j���I�v(�h�N���̓G�ɒ��ӂɂ�I)",
        "���̃X�e�[�W�ł�<color=#dc143c>�h�N���̓G</color>���o�ꂵ�܂�\n�����ɂ͍U���������܂���B�]���w���g���ĉ�����܂��傤",
        "�h�N���͈�莞�Ԍo�߂����<color=#dc143c>�����U��</color>���s���܂�\n�ǂ���炱�̔����͓G�ɂ��_���[�W������悤�ł���...!"};

    private string[] _stageTalk4 = {
        "�Ō�ɔ��W�e�N�j�b�N���Љ�܂�",
        "<color=#dc143c>Space�L�[�������ƁA���@�̏Ə������ɏo�Ă����G�Ɉړ�</color>���܂�\n�ǂ����Ă����̏Ə����C�ɓ���Ȃ����Ɏg���܂��傤",
        "�߂�{�^���͂Ȃ��ł����A�Ō�܂ŏƏ��𓮂����ΐ擪�ɖ߂��Ă��܂�\n�ł����̕��肪���ꂿ�Ⴄ�̂ŁA���Ɏg���邩�ǂ����͂��Ȃ��̘r����ł�",
        "�g����(�L)�u�j���H�v(���������������C������ɂ�H)"};

    private string[] _stageTalkDamage = {
        "�_���[�W�R���e�X�g�I\n10�b�Ԃłǂꂾ�K�`���K�`���o���邩�ȁH" };

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
