using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LED : MonoBehaviour
{
    [SerializeField] private float decayTime = 1f;
    [SerializeField] private float intensity = 1f;
    
    private bool _powered = false;
    private float _timeSincePower = 0f;

    private Material _mat;
    private Color _offColor;
    private Color _emission;
    private float _intensity;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    public void Power()
    {
        Power(Color.white);
    }
    
    public void Power(Color color)
    {
        _powered = true;
        _timeSincePower = Time.time;
        _emission = color;
        _intensity = intensity;
        _mat.EnableKeyword("_EMISSION");
        _mat.SetColor(EmissionColor, _emission * Mathf.GammaToLinearSpace(_intensity));
    }

    public void Unpower()
    {
        _powered = false;
        _timeSincePower = Time.time;
    }

    private void FullOff()
    {
        _mat.color = _offColor;
        _mat.DisableKeyword("_EMISSION");
    }

    private void Start()
    {
        _mat = GetComponent<Renderer>().material;
        _offColor = _mat.color;
    }

    private void Update()
    {
        if (_powered) return;
        
        var time = Time.time - _timeSincePower;

        if (time > decayTime) FullOff();
        else
        {
            _intensity = Mathf.Lerp(_intensity, .01f, time / decayTime);
            _mat.SetColor(EmissionColor, _emission * Mathf.GammaToLinearSpace(_intensity));
        }
    }

    private void OnDrawGizmos()
    {
        if (_powered)
        {
            Gizmos.DrawWireSphere(transform.position, transform.localScale.x * 0.5f);
        }
    }
}
