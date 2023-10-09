using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField, Header("U“®‚·‚éŠÔ")]
    private float _shakeTime;
    [SerializeField, Header("U“®‚Ì‘å‚«‚³")]
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

    //SpellManager‚©‚çŒÄ‚Î‚ê‚ÄƒJƒƒ‰‚ğ—h‚ç‚·
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

    //PlayerManager‚©‚çŒÄ‚Î‚ê‚Äã‚­ƒJƒƒ‰‚ğ—h‚ç‚·
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
