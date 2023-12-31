using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//敵に攻撃した時や敵に攻撃された時にカメラを揺らす
public class CameraManager : MonoBehaviour
{
    [SerializeField, Header("振動する時間")]
    private float _shakeTime;
    [SerializeField, Header("振動の大きさ")]
    private float _shakeMagnitude;

    private float _initShakeMagnitude;
    private float _initShakeTime;

    private float _shakeCount;

    private Vector3 _initPos;

    void Start()
    {
        _initShakeMagnitude = _shakeMagnitude;
        _initShakeTime = _shakeTime;

        _initPos = this.transform.position;
    }

    //魔法発動時SpellManagerから呼ばれてカメラを揺らす
    public void Shake()
    {
        _shakeCount = 0;
        _shakeMagnitude = _initShakeMagnitude;
        _shakeTime = _initShakeTime;
        if (SpellManager.s_spellLength >= 20)
        {
            _shakeMagnitude *= 2;
            _shakeTime *= 2;
        }
        else if (SpellManager.s_spellLength >= 40)
        {
            _shakeMagnitude *= 4;
            _shakeTime *= 3;
        }
        else if (SpellManager.s_spellLength >= 60)
        {
            _shakeMagnitude *= 6;
            _shakeTime *= 4;
        }
            StartCoroutine("ShakeCor");
        
    }

    //ダメージを受けた時PlayerManagerから呼ばれて弱くカメラを揺らす
    public void DamagedShake()
    {
        _shakeCount = 0;
        _shakeMagnitude = _initShakeMagnitude/2;
        _shakeTime = _initShakeTime/2;
        StartCoroutine("ShakeCor");
    }

    IEnumerator ShakeCor()
    {
        while (_shakeCount < _shakeTime)
        {
            float x = _initPos.x + UnityEngine.Random.Range(-_shakeMagnitude, _shakeMagnitude);
            float y = _initPos.y + UnityEngine.Random.Range(-_shakeMagnitude, _shakeMagnitude);
            this.transform.position = new Vector3(x, y, _initPos.z);
            _shakeCount += Time.deltaTime;
            yield return null;
        }
        this.transform.position = _initPos;
    }
}
