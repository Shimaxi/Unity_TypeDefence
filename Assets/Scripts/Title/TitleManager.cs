using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    // 初回起動時にウォームアップを行う
    public GameObject _warmUp;
    public Text _warmUpText;
    public Text _warmUpSpell;
    public Outline _spellOutline;
    public static bool s_isWarmUpDone;

    

    //フェードイン演出
    public Fade _fade;

    //オーディオソースSEとBGM
    public AudioSource _se;
    public AudioSource _se2;
    public AudioSource _bgm;
    public AudioMixer _audioMixer;

    //音量設定
    public GameObject _optionPanel;

    [Header("以下タイトル画面で用いる音のデータ")]
    public AudioClip _magicChargeSE;
    public AudioClip _decideSE;
    public AudioClip _explosionSE;

    private int _seNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        //起動時に一度だけ
        if(s_isWarmUpDone == true)
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

    public void OnClickOptionBtn()
    {
        _optionPanel.SetActive(true);
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
        SceneManager.LoadScene("Main");
    }

    IEnumerator WarmUpSpell()
    {
        _warmUpSpell.text = "";
        int spellLength = 0;
        _warmUpText.text = "適当にキーボードをガチャガチャして！";

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

                //魔法のチャージ具合(spell長さ)により音変化
                if (spellLength == 1)
                {
                    _se2.pitch = 1f;
                    _se2.PlayOneShot(_magicChargeSE);
                    _spellOutline.effectColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                }
                else if (spellLength == 21)
                {
                    _se2.pitch = 1.2f;
                    _se2.PlayOneShot(_magicChargeSE);
                    _spellOutline.effectColor = new Color(0.0f, 0.0f, 1.0f, 0.4f);
                    _warmUpText.text = "もっと！！！";
                }
                else if (spellLength == 41)
                {
                    _se2.pitch = 1.4f;
                    _se2.PlayOneShot(_magicChargeSE);
                    _spellOutline.effectColor = new Color(1.0f, 0.0f, 1.0f, 0.6f);
                    _warmUpText.text = "もっと！！！！！！";
                }
                else if (spellLength == 60)
                {
                    _se2.pitch = 1.6f;
                    _se2.PlayOneShot(_magicChargeSE);
                    _spellOutline.effectColor = new Color(1.0f, 0.0f, 0.0f, 0.8f);
                    _warmUpText.text = "Enterキーを押して！";
                }

            }
            yield return null;
        }
        
    }

}
