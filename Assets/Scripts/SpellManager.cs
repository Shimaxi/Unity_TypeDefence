using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellManager : MonoBehaviour
{
    //スペルについて
    public Text _spell; //スペルのテキスト
    public Outline _spellOutline; //スペルの装飾
    public static int s_spellLength; //スペルの長さ(ダメージの計算にも使うのでstatic宣言)
    public static int s_spellDamage; //ダメージ量
    private bool _isSpellOver = false; //
    [SerializeField,Header("【要調整】Spellの長さに対するエフェクトの拡大率")]
    private float _spellSizeRate; //Spell長さに対するエフェクトの拡大率
    public static float s_effectSize; //エフェクトの大きさ(GameManagerでも使う)
    public Text _spellDamageText;

    //魔法のチャージ音について
    public AudioClip _magicChargeSound; //チャージ音本体
    [SerializeField] private AudioSource _audioSource;

    //カメラを揺らす演出
    public CameraManager _cameraManager;

    //魔法のエフェクトについて
    public GameObject _bombPrefab;
    public Transform _effectsTrans;

    //照準について
    public Transform _targetMarkTrans;



    void Start()
    {
        //スペル初期化
        _spell.text = "";
        s_spellLength = 0;
        s_spellDamage = 0;
        s_effectSize = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _spellDamageText.text = "<size=40>予測ダメージ</size>\n" + s_spellDamage.ToString();

        if (s_spellLength < 20)
        {
            s_spellDamage = s_spellLength;
        } else if (s_spellLength < 40)
        {
            s_spellDamage = (int)(s_spellLength * 1.5f);
        }
        else if (s_spellLength < 60)
        {
            s_spellDamage = (int)(s_spellLength * 2);
        } else if (s_spellLength == 60)
        {
            s_spellDamage = (int)(s_spellLength * 3);
        }
        //スペルについて

        //エンターで魔法発動 それ以外では詠唱
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //スペルの長さが0の時と魔法を打った直後は魔法は発動しない
            if (s_spellLength != 0 && _isSpellOver == false)
            {
                _spell.text = "";//詠唱リセット
                GameObject bomb = Instantiate(_bombPrefab, _targetMarkTrans.position, Quaternion.identity);//エフェクト生成
                bomb.transform.SetParent(_effectsTrans); //エフェクトはEffectsの子オブジェクトにする
                
                bomb.transform.localScale = new Vector3(s_effectSize, s_effectSize, 0);//詠唱の長さによってエフェクトのサイズ変更
                _isSpellOver = true;
                _cameraManager.Shake(); //カメラを揺らす演出
                s_effectSize = 0;
                //_audioSource.Stop();
            }

        }
        else if (Input.anyKeyDown && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2) 
            && !Input.GetKeyDown(KeyCode.Alpha1) && !Input.GetKeyDown(KeyCode.Alpha2) && !Input.GetKeyDown(KeyCode.Alpha3))
        {
            
            if (_isSpellOver == true)
            {
                s_spellLength = 0; //スペルの長さリセット(ここで入れるのはダメージ計算等にスペルの長さを使うから)
                _isSpellOver = false; 
            }

            s_effectSize = s_spellLength * _spellSizeRate + 1; //

            if(s_spellLength < 60)
            {
                //適当に詠唱をする
                int wordNum = UnityEngine.Random.Range(0, 4);
                switch (wordNum)
                {
                    case 0:
                        _spell.text += "a";
                        break;
                    case 1:
                        _spell.text += "f";
                        break;
                    case 2:
                        _spell.text += "l";
                        break;
                    case 3:
                        _spell.text += "p";
                        break;
                }

                s_spellLength = _spell.text.Length;//スペルの長さカウント

                //魔法のチャージ具合(spell長さ)により音変化
                if (s_spellLength == 1)
                {
                    _audioSource.pitch = 1f;
                    _audioSource.PlayOneShot(_magicChargeSound);
                    _spellOutline.effectColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                }
                else if (s_spellLength == 21)
                {
                    _audioSource.pitch = 1.2f;
                    _audioSource.PlayOneShot(_magicChargeSound);
                    _spellOutline.effectColor = new Color(0.0f, 0.0f, 1.0f, 0.4f);
                }
                else if (s_spellLength == 41)
                {
                    _audioSource.pitch = 1.4f;
                    _audioSource.PlayOneShot(_magicChargeSound);
                    _spellOutline.effectColor = new Color(1.0f, 0.0f, 1.0f, 0.6f);
                }
                else if (s_spellLength == 60)
                {
                    _audioSource.pitch = 1.6f;
                    _audioSource.PlayOneShot(_magicChargeSound);
                    _spellOutline.effectColor = new Color(1.0f, 0.0f, 0.0f, 0.8f);
                }
            }
            

            

        }

    }
}
