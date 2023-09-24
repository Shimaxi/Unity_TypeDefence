using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{
    public float _fadeTime = 0.5f;
    [SerializeField] private AudioClip _explosionSE;
    [SerializeField] private AudioClip _flameSE;
    [SerializeField] private AudioClip _freezeSE;
    [SerializeField] private AudioSource _audioSource;

    void Start()
    {
        _audioSource.PlayOneShot(_explosionSE);
        StartCoroutine(BombFin());
    }

    IEnumerator BombFin()
    {
        yield return new WaitForSeconds(_fadeTime/10);
        this.GetComponent<CircleCollider2D>().enabled = false;

        //’Ç‰Á‚ÅSE—¬‚·‚±‚ÆŒŸ“¢‚µ‚½‚¯‚Ç”÷–­‚©‚à
        /*
        yield return new WaitForSeconds(_fadeTime);
        if (SpellManager.s_spellLength < 40)
        {
            _audioSource.PlayOneShot(_flameSE);
        } else if (SpellManager.s_spellLength < 60)
        {
            _audioSource.PlayOneShot(_freezeSE);
        }
        */
        //–Â‚èI‚í‚é‚Ü‚Å‘Ò‹@
        yield return new WaitWhile(() => _audioSource.isPlaying);
        Destroy(this.gameObject);
    }

   
}
