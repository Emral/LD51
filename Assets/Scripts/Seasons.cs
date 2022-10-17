using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Seasons", menuName = "Data/Seasons")]
public class Seasons : ScriptableObject
{
    [System.Serializable]
    public class DayWeathers
    {
        public List<Weather> choices = new List<Weather>();
    }
    [SerializeField]
    public List<DayWeathers> springWeathersByDay;
    public List<DayWeathers> summerWeathersByDay;
    public List<DayWeathers> autumnWeathersByDay;
    public List<DayWeathers> winterWeathersByDay;
}
