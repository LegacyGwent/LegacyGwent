using Cynthia.Card;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShowWeather : MonoBehaviour
{
    public Image[] Row;
    public RowPosition[] RowIndex;
    public Color[] WeatherColor;
    public RowStatus[] WeatherIndex;
    private WeatherRowEffect[] _effects;
    // A weather card can hit several rows at once; its sound should only play once.
    private const float SoundCooldown = 0.5f;
    private readonly Dictionary<RowStatus, float> _lastSound = new Dictionary<RowStatus, float>();

    public void SetWeather(RowPosition row,RowStatus weather)
    {
        if (_effects == null) _effects = Row.Select(WeatherRowEffect.Attach).ToArray();
        var rowIndex = RowIndex.Select((item, index) => (item, index)).Single(x => x.item == row).index;
        var color = WeatherColor[WeatherIndex.Select((item, index) => (item, index)).Single(x=>x.item==weather).index];
        if (_effects[rowIndex].Current != weather) PlaySound(weather);
        _effects[rowIndex].SetWeather(weather, color);
    }

    private void PlaySound(RowStatus weather)
    {
        var sound = WeatherMaterials.Sound(weather);
        if (sound == null) return;
        if (_lastSound.TryGetValue(weather, out var last) && Time.time - last < SoundCooldown) return;
        _lastSound[weather] = Time.time;
        AudioManager.Instance.PlayAudio(sound, AudioType.Effect, AudioPlayMode.PlayOneShoot);
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    // Debug: F9 cycles every row through all weathers, to preview the effects without a match setup.
    private int _debugWeather;

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.F9)) return;
        var weathers = (RowStatus[])Enum.GetValues(typeof(RowStatus));
        _debugWeather = (_debugWeather + 1) % weathers.Length;
        Debug.Log($"Weather preview: {weathers[_debugWeather]}");
        foreach (var row in RowIndex) SetWeather(row, weathers[_debugWeather]);
    }
#endif
}
