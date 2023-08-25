using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField, Header("U“®‚·‚éŽžŠÔ")]
    private float _shakeTime;
    [SerializeField, Header("U“®‚Ì‘å‚«‚³")]
    private float _shakeMagnitude;

    private float _initShakeMagnitude;
    private float _initShakeTime;

    private float _shakeCount;



    void Start()
    {
        _initShakeMagnitude = _shakeMagnitude;
        _initShakeTime = _shakeTime;
    }

    //SpellManager‚©‚çŒÄ‚Î‚ê‚ÄƒJƒƒ‰‚ð—h‚ç‚·
    public void Shake()
    {
        _shakeCount = 0;
        _shakeMagnitude = _initShakeMagnitude;
        _shakeTime = _initShakeTime;
        if (SpellManager.s_spellLength >= 21)
        {
            _shakeMagnitude *= 2;
            _shakeTime *= 2;
        }
        else if (SpellManager.s_spellLength >= 41)
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
