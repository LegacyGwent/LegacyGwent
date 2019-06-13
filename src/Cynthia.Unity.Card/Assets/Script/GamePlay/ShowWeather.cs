using Cynthia.Card;
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
    public void SetWeather(RowPosition row,RowStatus weather)
    {
        Row[RowIndex.Select((item, index) => (item, index)).Single(x => x.item == row).index]
            .color = WeatherColor[WeatherIndex.Select((item, index) => (item, index)).Single(x=>x.item==weather).index];
    }
}
