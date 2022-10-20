using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class PatchCord : MonoBehaviour
{
    [SerializeField] private float minLedDist = 0.5f;
    [SerializeField] private LED ledPrefab;
    [SerializeField] private LineRenderer wire;

    [SerializeField] private float ledSize = .3f;
    [SerializeField] private float wireSize = .25f;

    [SerializeField] private SwitchboardPlug startPlug, endPlug;
    [SerializeField] private Transform start, end;

    private List<LED> _leds = new List<LED>();
    private bool[] _power;

    private float _timeOfLastTick = 0f;
    [SerializeField] private float tickRate = 0.2f;

    [Button()]
    private void RegenerateCord()
    {
        foreach (var led in _leds)
        {
            if(led != null)
                DestroyImmediate(led.gameObject);
        }
        _leds.Clear();
        GenerateCord();
    }

    private void GenerateCord()
    {
        if (start == null || end == null) return;

        Vector3 offset = Vector3.forward * -0.25f;
        Vector3 startPos = start.transform.position + offset, endPos = end.transform.position + offset;
        var dist = Vector3.Distance(startPos, endPos);

        var ledCount = Mathf.FloorToInt(dist / minLedDist) - 1;

        if (ledCount < _leds.Count)
        {
            for (int ii = ledCount; ii < _leds.Count; ++ii)
                DestroyImmediate(_leds[ii].gameObject);
            
            _leds.RemoveRange(ledCount, _leds.Count - ledCount);
        }

        while (_leds.Count < ledCount)
        {
            LED led = Instantiate(ledPrefab, transform);
            _leds.Add(led);
        }

        wire.startWidth = wire.endWidth = wireSize;
        wire.positionCount = ledCount + 2;
        wire.SetPosition(0, startPos);
        
        for (int ii = 0; ii < _leds.Count; ++ii)
        {
            var pos = _leds[ii].transform.position = Vector3.Lerp(startPos, endPos, (ii + 1) / (ledCount + 1f));
            wire.SetPosition(ii + 1, pos);

            _leds[ii].transform.localScale = Vector3.one * ledSize;
        }
        
        wire.SetPosition(ledCount + 1, endPos);

        _power = new bool[ledCount + 2];
    }

    private void Start()
    {
        RegenerateCord();
        Debug.Log(_power.Length);
        Debug.Log(_power);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SendPacket();

        var diff = Time.time - _timeOfLastTick;
        if (Time.time - _timeOfLastTick < tickRate) return;
        
        _timeOfLastTick = Time.time - diff + tickRate;
        ForwardPackets();
    }

    private void SendPacket()
    {
        _power[0] = true;
        startPlug.Led.Power();
    }

    private void ForwardPackets()
    {
#if DEBUG_PACKET_STATE
        Debug.Log("Tick");

        string bools = "";
        foreach (var b in _power)
        {
            bools += b ? "1 " : "0 ";
        }
        Debug.Log(bools);
#endif
        
        if (_power[^2]) endPlug.Led.Power();
        else if (_power[^1]) endPlug.Led.Unpower();
        _power[^1] = _power[^2];
        
        for (int ii = _leds.Count - 1; ii >= 0; --ii)
        {
            if (_power[ii + 1] && !_power[ii]) _leds[ii].Unpower();
            if (!_power[ii + 1] && _power[ii]) _leds[ii].Power();
            _power[ii + 1] = _power[ii];
        }
        
        if (_power[0])
        {
            _power[0] = false;
            startPlug.Led.Unpower();
        }

#if DEBUG_PACKET_STATE
        bools = "";
        foreach (var b in _power)
        {
            bools += b ? "1 " : "0 ";
        }
        Debug.Log(bools);
        
        Debug.Log("End Tick");
#endif
    }
    
}
