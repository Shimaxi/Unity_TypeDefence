using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    // ����N�����ɃE�H�[���A�b�v���s��
    public GameObject _warmUp;
    public Text _warmUpText;
    public Text _warmUpSpell;
    public Outline _spellOutline;
    public static bool s_isWarmUpDone;

    //�t�F�[�h�C�����o
    public Fade _fade;

    //�I�[�f�B�I�\�[�XSE��BGM
    public AudioSource _se;
    public AudioSource _se2;
    public AudioSource _bgm;
    public AudioMixer _audioMixer;

    //���ʐݒ�
    public GameObject _optionPanel;

    [Header("�ȉ��^�C�g����ʂŗp���鉹�̃f�[�^")]
    public AudioClip _magicChargeSE;
    public AudioClip _decideSE;
    public AudioClip _explosionSE;

    private int _seNum = 0;

    
    void Start()
    {
        //�N�����Ɉ�x�����E�H�[���A�b�v
        if (s_isWarmUpDone == true)
        {
            _warmUp.SetActive(false);
            _fade.gameObject.SetActive(false);
            _bgm.Play();
        }
        if (_warmUp != null && _warmUp.activeSelf == true)
        {
            _warmUp.SetActive(true);
            StartCoroutine("WarmUpSpell");
            s_isWarmUpDone = true;
        }
    }
    public void SetBGM(float volume)
    {
        _audioMixer.SetFloat("BGMVol", volume);
    }

    public void SetSE(float volume)
    {
        _audioMixer.SetFloat("SEVol", volume);
    }

    //SE�̉��ʊm�F�{�^��
    public void TrySEBtn()
    {
        if (_seNum == 0)
        {
            _se.PlayOneShot(_decideSE);
            _seNum++;
        } else if(_seNum == 1)
        {
            _se.PlayOneShot(_magicChargeSE);
            _seNum++;
        } else if(_seNum == 2)
        {
            _se.PlayOneShot(_explosionSE);
            _seNum = 0;
        }
        
    }

    //�ݒ�{�^��(���ʒ����{�^��)
    public void OnClickOptionBtn()
    {
        if (_optionPanel.activeSelf == false)
        {
            _optionPanel.SetActive(true);
        }else
        {
            _optionPanel.SetActive(false);
        }
        
    }

    IEnumerator Title()
    {
        yield return new WaitForSeconds(0.5f);
        _fade.FadeOut(2f);

        _bgm.Play();
    }

    public void OnclickStg1Btn()
    {
        StartCoroutine("Onclickstg1BtnCor");
    }
    public IEnumerator Onclickstg1BtnCor()
    {
        _se.PlayOneShot(_decideSE);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Stage1");
    }

    public void OnclickStg2Btn()
    {
        StartCoroutine("Onclickstg2BtnCor");
    }
    public IEnumerator Onclickstg2BtnCor()
    {
        _se.PlayOneShot(_decideSE);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Stage2");
    }
    public void OnclickStg3Btn()
    {
        StartCoroutine("Onclickstg3BtnCor");
    }
    public IEnumerator Onclickstg3BtnCor()
    {
        _se.PlayOneShot(_decideSE);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Stage3");
    }
    
    public void OnclickStg4Btn()
    {
        StartCoroutine("Onclickstg4BtnCor");
    }
    public IEnumerator Onclickstg4BtnCor()
    {
        _se.PlayOneShot(_decideSE);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Stage4");
    }

    public void OnclickStg0Btn()
    {
        StartCoroutine("Onclickstg0BtnCor");
    }
    public IEnumerator Onclickstg0BtnCor()
    {
        _se.PlayOneShot(_decideSE);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Stage0");
    }

    public void OnclickStgDamageConBtn()
    {
        StartCoroutine("OnclickstgDamageConBtnCor");
    }

    public IEnumerator OnclickstgDamageConBtnCor()
    {
        _se.PlayOneShot(_decideSE);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("StageDamageCon");
    }

    //����N�����ɍs����E�H�[���A�b�v
    IEnumerator WarmUpSpell()
    {
        _warmUpSpell.text = "";
        int spellLength = 0;
        _warmUpText.text = "�K���ɃL�[�{�[�h���K�`���K�`�����āI";

        bool isWarmUpEnd = false;

        while (isWarmUpEnd == false)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {

                //_se.volume = 0.2f;
                _se.PlayOneShot(_explosionSE);
                isWarmUpEnd = true;
                StartCoroutine("Title");
                Destroy(_warmUp.gameObject);

            }
            else if (Input.anyKeyDown && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
            {
                spellLength++;
                int wordNum = UnityEngine.Random.Range(0, 4);
                Debug.Log(spellLength);
                switch (wordNum)
                {
                    case 0:
                        _warmUpSpell.text += "a";
                        break;
                    case 1:
                        _warmUpSpell.text += "f";
                        break;
                    case 2:
                        _warmUpSpell.text += "l";
                        break;
                    case 3:
                        _warmUpSpell.text += "p";
                        break;
                }

                //���@�̃`���[�W�(spell����)�ɂ�艹�ω�
                if (spellLength == 1)
                {
                    _se2.pitch = 1f;
                    _se2.PlayOneShot(_magicChargeSE);
                    _spellOutline.effectColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                }
                else if (spellLength == 20)
                {
                    _se2.pitch = 1.2f;
                    _se2.PlayOneShot(_magicChargeSE);
                    _spellOutline.effectColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
                    _warmUpText.text = "�����ƁI�I�I";
                }
                else if (spellLength == 40)
                {
                    _se2.pitch = 1.4f;
                    _se2.PlayOneShot(_magicChargeSE);
                    _spellOutline.effectColor = new Color(0.0f, 0.0f, 1.0f, 0.5f);
                    _warmUpText.text = "�����ƁI�I�I�I�I�I";
                }
                else if (spellLength == 60)
                {
                    _se2.pitch = 1.6f;
                    _se2.PlayOneShot(_magicChargeSE);
                    _spellOutline.effectColor = new Color(1.0f, 0.0f, 1.0f, 0.5f);
                    _warmUpText.text = "Enter�L�[�������āI";
                }

            }
            yield return null;
        }
        
    }

}
