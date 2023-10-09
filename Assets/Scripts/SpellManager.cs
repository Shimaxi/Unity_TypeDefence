using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellManager : MonoBehaviour
{
    //�X�y���ɂ���
    [SerializeField] private Text _spell; //�X�y���{�̂̃e�L�X�g
    [SerializeField] private Outline _spellOutline; //�X�y���̑���
    public static int s_spellLength; //�X�y���̒���(�_���[�W�̌v�Z�ɂ��g���̂�static�錾)
    public static int s_spellDamage; //�_���[�W��
    private bool _isSpellOver = false; //
    [SerializeField,Header("�y�v�����zSpell�̒����ɑ΂���G�t�F�N�g�̊g�嗦")]
    private float _spellSizeRate; //Spell�����ɑ΂���G�t�F�N�g�̊g�嗦
    public static float s_effectSize; //�G�t�F�N�g�̑傫��(GameManager�ł��g��)
    [SerializeField] private Text _spellDamageText;
    [SerializeField] private Text _spellAdditionalText;

    //���@�̃`���[�W���ɂ���
    [SerializeField] private AudioClip _magicChargeSound; //�`���[�W���{��
    [SerializeField] private AudioSource _audioSource;

    //�J������h�炷���o
    [SerializeField] private CameraManager _cameraManager;

    //���@�̃G�t�F�N�g�ɂ���
    [SerializeField] private GameObject _bombPrefab;
    [SerializeField] private Transform _effectsTrans;

    //�Ə��ɂ���
    [SerializeField] private Transform _targetMarkTrans;
    [SerializeField] private Transform _magicCircleTrans; //�G�t�F�N�g�̗\�z�������ʔ͈�
    [SerializeField] private Image _magicCircleImage; //�G�t�F�N�g�̗\�z�������ʔ͈�
    [SerializeField] private Sprite[] _magicCircleSprites = new Sprite[4];

    //���������@�\�@��d�r��
    private bool _isDualcaster = false;

    void Start()
    {
        //�X�y��������
        _spell.text = "";
        _spellAdditionalText.text = "";
        s_spellLength = 0;
        s_spellDamage = 0;
        s_effectSize = 0;
    }

    
    void Update()
    {
       
        //�X�y���ɂ���

        //�G���^�[�Ŗ��@���� ����ȊO�ł͉r��
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //�X�y���̒�����0�̎��Ɩ��@��ł�������͖��@�͔������Ȃ�
            if (s_spellLength != 0 && _isSpellOver == false)
            {
                _spell.text = "";//�r�����Z�b�g
                GameObject bomb = Instantiate(_bombPrefab, _targetMarkTrans.position, Quaternion.identity);//�G�t�F�N�g����
                bomb.transform.SetParent(_effectsTrans); //�G�t�F�N�g��Effects�̎q�I�u�W�F�N�g�ɂ���
                
                bomb.transform.localScale = new Vector3(s_effectSize, s_effectSize, 0);//�r���̒����ɂ���ăG�t�F�N�g�̃T�C�Y�ύX
                _magicCircleTrans.transform.localScale = new Vector3(0, 0, 0);
                _isSpellOver = true;
                _cameraManager.Shake(); //�J������h�炷���o
                s_effectSize = 0;
            }

        }
        else if (Input.anyKeyDown && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2) 
            && !Input.GetKeyDown(KeyCode.Alpha1) && !Input.GetKeyDown(KeyCode.Alpha2) && !Input.GetKeyDown(KeyCode.Alpha3)
            && !Input.GetKeyDown(KeyCode.Space))
        {
            
            if (_isSpellOver == true)
            {
                s_spellLength = 0; //�X�y���̒������Z�b�g(�����œ����̂̓_���[�W�v�Z���ɃX�y���̒������g������)
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
                    //�K���ɉr��������
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
                
                s_spellLength = _spell.text.Length;//�X�y���̒����J�E���g
                s_spellDamage = s_spellLength;

                _spellDamageText.text = "<size=40>�\���_���[�W</size>\n" + s_spellDamage.ToString();
                if (s_spellLength < 20)
                {
                    _spellAdditionalText.text = "";
                }
                else if (s_spellLength < 40)
                {
                    _spellAdditionalText.text = "����";
                    _spellAdditionalText.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                }
                else if (s_spellLength < 60)
                {
                    _spellAdditionalText.text = "����";
                    _spellAdditionalText.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
                }
                else if (s_spellLength == 60)
                {
                    _spellAdditionalText.text = "����\n+����";
                    _spellAdditionalText.color = new Color(1.0f, 0.0f, 1.0f, 1.0f);
                }

                //���@�̃`���[�W�(spell����)�ɂ�艹�ƐF�ω�
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

                //�Ō�ɏ��X�K���������@�w��\��
                _magicCircleTrans.transform.localScale = new Vector3(s_effectSize, s_effectSize, 0);

            }

        }
    }
}
