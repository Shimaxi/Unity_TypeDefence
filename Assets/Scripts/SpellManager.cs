using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellManager : MonoBehaviour
{
    //スペルについて
    [SerializeField] private Text _spell; //スペル本体のテキスト
    [SerializeField] private Outline _spellOutline; //スペルの装飾
    public static int s_spellLength; //スペルの長さ(ダメージの計算にも使うのでstatic宣言)
    public static int s_spellDamage; //ダメージ量
    private bool _isSpellOver = false; //
    [SerializeField,Header("【要調整】Spellの長さに対するエフェクトの拡大率")]
    private float _spellSizeRate; //Spell長さに対するエフェクトの拡大率
    public static float s_effectSize; //エフェクトの大きさ(GameManagerでも使う)
    [SerializeField] private Text _spellDamageText;
    [SerializeField] private Text _spellAdditionalText;

    //魔法のチャージ音について
    [SerializeField] private AudioClip _magicChargeSound; //チャージ音本体
    [SerializeField] private AudioSource _audioSource;

    //カメラを揺らす演出
    [SerializeField] private CameraManager _cameraManager;

    //魔法のエフェクトについて
    [SerializeField] private GameObject _bombPrefab;
    [SerializeField] private Transform _effectsTrans;

    //照準について
    [SerializeField] private Transform _targetMarkTrans;
    [SerializeField] private Transform _magicCircleTrans; //エフェクトの予想される効果範囲
    [SerializeField] private Image _magicCircleImage; //エフェクトの予想される効果範囲
    [SerializeField] private Sprite[] _magicCircleSprites = new Sprite[4];

    //実装検討機能　二重詠唱
    private bool _isDualcaster = false;

    void Start()
    {
        //スペル初期化
        _spell.text = "";
        _spellAdditionalText.text = "";
        s_spellLength = 0;
        s_spellDamage = 0;
        s_effectSize = 0;
    }

    
    void Update()
    {
       
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
                _magicCircleTrans.transform.localScale = new Vector3(0, 0, 0);
                _isSpellOver = true;
                _cameraManager.Shake(); //カメラを揺らす演出
                s_effectSize = 0;
            }

        }
        else if (Input.anyKeyDown && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2) 
            && !Input.GetKeyDown(KeyCode.Alpha1) && !Input.GetKeyDown(KeyCode.Alpha2) && !Input.GetKeyDown(KeyCode.Alpha3)
            && !Input.GetKeyDown(KeyCode.Space))
        {
            
            if (_isSpellOver == true)
            {
                s_spellLength = 0; //スペルの長さリセット(ここで入れるのはダメージ計算等にスペルの長さを使うから)
                _isSpellOver = false; 
            }

            s_effectSize = s_spellLength * _spellSizeRate + 1; 
            if(s_spellLength < 60)
            {
                int count = 1;
                if(_isDualcaster == true)
                {
                    count = 2;
                }

                for(int i = 0; i < count; i++)
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
                }
                
                s_spellLength = _spell.text.Length;//スペルの長さカウント
                s_spellDamage = s_spellLength;

                _spellDamageText.text = "<size=40>予測ダメージ</size>\n" + s_spellDamage.ToString();
                if (s_spellLength < 20)
                {
                    _spellAdditionalText.text = "";
                }
                else if (s_spellLength < 40)
                {
                    _spellAdditionalText.text = "炎上";
                    _spellAdditionalText.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                }
                else if (s_spellLength < 60)
                {
                    _spellAdditionalText.text = "凍結";
                    _spellAdditionalText.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
                }
                else if (s_spellLength == 60)
                {
                    _spellAdditionalText.text = "炎上\n+凍結";
                    _spellAdditionalText.color = new Color(1.0f, 0.0f, 1.0f, 1.0f);
                }

                //魔法のチャージ具合(spell長さ)により音と色変化
                if (s_spellLength == 1)
                {
                    _audioSource.pitch = 1f;
                    _audioSource.PlayOneShot(_magicChargeSound);
                    _spellOutline.effectColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    _magicCircleImage.sprite = _magicCircleSprites[0];
                }
                else if (s_spellLength == 20)
                {
                    _audioSource.pitch = 1.2f;
                    _audioSource.PlayOneShot(_magicChargeSound);
                    _spellOutline.effectColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
                    _magicCircleImage.sprite = _magicCircleSprites[1];

                }
                else if (s_spellLength == 40)
                {
                    _audioSource.pitch = 1.4f;
                    _audioSource.PlayOneShot(_magicChargeSound);
                    _spellOutline.effectColor = new Color(0.0f, 0.0f, 1.0f, 0.5f);
                    _magicCircleImage.sprite = _magicCircleSprites[2];

                }
                else if (s_spellLength == 60)
                {
                    _audioSource.pitch = 1.6f;
                    _audioSource.PlayOneShot(_magicChargeSound);
                    _spellOutline.effectColor = new Color(1.0f, 0.0f, 1.0f, 0.5f);
                    _magicCircleImage.sprite = _magicCircleSprites[3];

                }

                //最後に諸々適応した魔法陣を表示
                _magicCircleTrans.transform.localScale = new Vector3(s_effectSize, s_effectSize, 0);

            }

        }
    }
}
