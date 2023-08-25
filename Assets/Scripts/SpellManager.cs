using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellManager : MonoBehaviour
{
    //�X�y���ɂ���
    public Text _spell; //�X�y���̃e�L�X�g
    public Outline _spellOutline; //�X�y���̑���
    public static int s_spellLength; //�X�y���̒���(�_���[�W�̌v�Z�ɂ��g���̂�static�錾)
    public static int s_spellDamage; //�_���[�W��
    private bool _isSpellOver = false; //
    [SerializeField,Header("�y�v�����zSpell�̒����ɑ΂���G�t�F�N�g�̊g�嗦")]
    private float _spellSizeRate; //Spell�����ɑ΂���G�t�F�N�g�̊g�嗦
    public static float s_effectSize; //�G�t�F�N�g�̑傫��(GameManager�ł��g��)
    public Text _spellDamageText;

    //���@�̃`���[�W���ɂ���
    public AudioClip _magicChargeSound; //�`���[�W���{��
    [SerializeField] private AudioSource _audioSource;

    //�J������h�炷���o
    public CameraManager _cameraManager;

    //���@�̃G�t�F�N�g�ɂ���
    public GameObject _bombPrefab;
    public Transform _effectsTrans;

    //�Ə��ɂ���
    public Transform _targetMarkTrans;



    void Start()
    {
        //�X�y��������
        _spell.text = "";
        s_spellLength = 0;
        s_spellDamage = 0;
        s_effectSize = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _spellDamageText.text = "<size=40>�\���_���[�W</size>\n" + s_spellDamage.ToString();

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
                _isSpellOver = true;
                _cameraManager.Shake(); //�J������h�炷���o
                s_effectSize = 0;
                //_audioSource.Stop();
            }

        }
        else if (Input.anyKeyDown && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2) 
            && !Input.GetKeyDown(KeyCode.Alpha1) && !Input.GetKeyDown(KeyCode.Alpha2) && !Input.GetKeyDown(KeyCode.Alpha3))
        {
            
            if (_isSpellOver == true)
            {
                s_spellLength = 0; //�X�y���̒������Z�b�g(�����œ����̂̓_���[�W�v�Z���ɃX�y���̒������g������)
                _isSpellOver = false; 
            }

            s_effectSize = s_spellLength * _spellSizeRate + 1; //

            if(s_spellLength < 60)
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

                s_spellLength = _spell.text.Length;//�X�y���̒����J�E���g

                //���@�̃`���[�W�(spell����)�ɂ�艹�ω�
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
