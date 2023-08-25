using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{
    public float _fadeTime = 0.5f;
    private float _time;
    private Image _render;

    public AudioClip _explosionSound;
    [SerializeField] private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _render = GetComponent<Image>();
        _audioSource.PlayOneShot(_explosionSound);
        StartCoroutine(BombFin());
        //fadeTime *= this.gameObject.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        _time += Time.deltaTime;
        if (_time <= _fadeTime)
        {
            float alpha = 1.0f - _time / _fadeTime;
            Color color = _render.color;
            color.a = alpha;
            _render.color = color;
        }
        */

    }

    IEnumerator BombFin()
    {
        yield return new WaitForSeconds(_fadeTime/10);
        this.GetComponent<CircleCollider2D>().enabled = false;
        //–Â‚èI‚í‚é‚Ü‚Å‘Ò‹@
        yield return new WaitWhile(() => _audioSource.isPlaying);

        Destroy(this.gameObject);
    }

   
}
