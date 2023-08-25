using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public GameObject _warmUp;
    public Text _warmUpSpell;
    public AudioSource _audioSource;
    public AudioClip _magicChargeSound;
    public AudioClip _explosionSound;
    public AudioClip _decideSound;
    public Outline _spellOutline;

    public Fade _fade;

    public AudioSource _bgm;

    public Button _stg1Btn;

    public static bool s_isWarmUp;

    public Text _warmUpText;

    // Start is called before the first frame update
    void Start()
    {
        if(s_isWarmUp == true)
        {
            _warmUp.SetActive(false);
            _fade.gameObject.SetActive(false);
            _bgm.Play();
        }

        if (_warmUp != null && _warmUp.activeSelf == true)
        {
            _warmUp.SetActive(true);
            WarmUp();
            s_isWarmUp = true;
        }
    }

    void WarmUp()
    {
        _warmUpSpell.text = "";
        StartCoroutine("WarmUpSpell");
        _warmUpText.text = "適当にキーボードをガチャガチャして！";
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
        _audioSource.volume = 1f;
        _audioSource.PlayOneShot(_decideSound);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Main");
    }

    IEnumerator WarmUpSpell()
    {
        int spellLength = 0;
        

        bool isWarmUpEnd = false;

        while (isWarmUpEnd == false)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("ウォームアップ終了！");
                _audioSource.pitch = 1.0f;
                _audioSource.volume = 0.2f;
                _audioSource.PlayOneShot(_explosionSound);
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
                    _audioSource.pitch = 1f;
                    _audioSource.PlayOneShot(_magicChargeSound);
                    _spellOutline.effectColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                }
                else if (spellLength == 21)
                {
                    _audioSource.pitch = 1.2f;
                    _audioSource.PlayOneShot(_magicChargeSound);
                    _spellOutline.effectColor = new Color(0.0f, 0.0f, 1.0f, 0.4f);
                    _warmUpText.text = "もっと！！！";
                }
                else if (spellLength == 41)
                {
                    _audioSource.pitch = 1.4f;
                    _audioSource.PlayOneShot(_magicChargeSound);
                    _spellOutline.effectColor = new Color(1.0f, 0.0f, 1.0f, 0.6f);
                    _warmUpText.text = "もっと！！！！！！";
                }
                else if (spellLength == 60)
                {
                    _audioSource.pitch = 1.6f;
                    _audioSource.PlayOneShot(_magicChargeSound);
                    _spellOutline.effectColor = new Color(1.0f, 0.0f, 0.0f, 0.8f);
                    _warmUpText.text = "Enterキーを押して！";
                }

            }
            yield return null;
        }
        
    }

}
