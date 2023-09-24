using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField, Header("振動する時間")]
    private float _shakeTime;
    [SerializeField, Header("振動の大きさ")]
    private float _shakeMagnitude;

    private float _initShakeMagnitude;
    private float _initShakeTime;

    private float _shakeCount;

    void Start()
    {
        _initShakeMagnitude = _shakeMagnitude;
        _initShakeTime = _shakeTime;
    }

    //SpellManagerから呼ばれてカメラを揺らす
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

    public void DamagedShake()
    {
        _shakeCount = 0;
        _shakeMagnitude = _initShakeMagnitude/2;
        _shakeTime = _initShakeTime/2;
        StartCoroutine("ShakeCor");
    }

    IEnumerator ShakeCor()
    {
        Vector3 initPos = this.transform.position;

        while (_shakeCount < _shakeTime)
        {
            float x = initPos.x + UnityEngine.Random.Range(-_shakeMagnitude, _shakeMagnitude);
            float y = initPos.y + UnityEngine.Random.Range(-_shakeMagnitude, _shakeMagnitude);
            this.transform.position = new Vector3(x, y, initPos.z);
            _shakeCount += Time.deltaTime;
            yield return null;
        }
        this.transform.position = initPos;
    }
}
