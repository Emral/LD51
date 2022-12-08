using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Expense
{
    public string Name;
    public float Value;
    public string PerWhat = "";
    public int Multiplier = 1;
    public int DefaultMultiplier = 1;

    public Expense(int dm = 1)
    {
        Multiplier = dm;
        DefaultMultiplier = dm;
    }
}

public enum Weather
{
    Sunny = 1,
    Overcast = 2,
    Cloudy = 3,
    Rain = 4,
    Thunder = 5,
    Heat = 6,
    Wind = 7,
    Snow = 8,
    Snowstorm = 9,
    FestivalNight = 10,
}

public enum Season
{
    Spring = 1,
    Summer = 2,
    Autumn = 3,
    Winter = 4
}

public enum Mechanics
{
    HowToPlay,
    TimeLimit,
    SellEarly,
    SelectColor,
    DaySlider,
    MoneyExplanation,
    RushHour,
    Thunderstorms,
    Events,
    Followers,
    Rain
}

public class SessionVariables : ScriptableObject
{
    public static WeatherSettings weatherSettings;
    public static Seasons seasonSettings;

    public static GameValue<float> Experience;
    public static GameValue<int> Followers;
    public static GameValue<float> Reputation;
    public static GameValue<float> IncomeMultiplier;
    public static GameValue<float> MaxIncomeBase;
    public static ListGameValue<Expense> Expenses;
    public static GameValue<ValidColors> Colors;
    internal static ListGameValue<Weather> UpcomingWeathers;
    public static ListGameValue<Mechanics> TutorialsDone;
    public static ListGameValue<EventData> Events;
    public static GameValue<int> Day;
    internal static GameValue<float> _savings;

    public static UnityEvent<float> OnSavingsChanged = new UnityEvent<float>();
    public static float Savings
    {
        get => _savings.Value;
        set
        {
            _savings.Value = value;
            OnSavingsChanged.Invoke(value);
        }
    }
    public static float TodaysEarnings = 0;
    public static int LastDay = 0;

    public static List<Texture2D> todaysDrawings;

    public void ResetAll()
    {
        Expenses.ResetToDefault();

        Experience.ResetToDefault();
        Followers.ResetToDefault();
        Reputation.ResetToDefault();
        IncomeMultiplier.ResetToDefault();
        _savings.ResetToDefault();
        Colors.ResetToDefault();
        Day.ResetToDefault();
        MaxIncomeBase.ResetToDefault();
        todaysDrawings = new List<Texture2D>();
        UpcomingWeathers.ResetToDefault();
        Events.ResetToDefault();

        foreach (var e in MetaManager.instance.events.events)
        {
            Events.Add(e.data);
        }

        for (int i = 0; i < 32; i++)
        {
            CalculateWeather(Day.Value + i);
        }

        Initialize();
    }

    public void Initialize()
    {
        TodaysEarnings = 0;
        LastDay = Mathf.Max(Day.Value - 1, 0);
    }

    private static void CalculateWeather(int day)
    {
        var list = seasonSettings.springWeathersByDay;

        if (day % 32 >= 24)
        {
            list = seasonSettings.winterWeathersByDay;
        } else if (day % 32 >= 16)
        {
            list = seasonSettings.autumnWeathersByDay;
        } else if (day % 32 >= 8)
        {
            list = seasonSettings.summerWeathersByDay;
        }

        UpcomingWeathers.Add(weatherSettings.weathers[list[day % 8].choices[Random.Range(0, list[day % 8].choices.Count)]].weather);
    }

    public static WeatherConfig GetTodaysWeather()
    {
        return weatherSettings.weathers[UpcomingWeathers[(Day.Value - LastDay)]];
    }

    public static WeatherConfig GetUpcomingWeather(int i)
    {
        return weatherSettings.weathers[UpcomingWeathers[i]];
    }

    public static async void NewDayBegins()
    {
        TodaysEarnings = 0;
        foreach (var expense in Expenses)
        {
            expense.Multiplier = expense.DefaultMultiplier;
        }
        LastDay = Day.Value;
        SFX.DayAdvance.Play();
        Day.Value++;
        CalculateWeather(Day.Value + 5);

        var eData = MetaManager.instance.events.FindEligible();
        if (eData != null)
        {
            var e = MetaManager.instance.events.GetEvent(eData);
            e.Schedule();
            if (!e.Permanent)
            {
                await MetaManager.instance.DoTutorial(Mechanics.Events);
            }
        }

        for (int i = Events.Count-1; i >= 0; i--)
        {
            var ev = MetaManager.instance.events.GetEvent(Events[i]);
            if (ev.GetEndDay() == Day.Value)
            {
                ev.EndEvent();
            } else if (ev.GetStartDay() == Day.Value)
            {
                ev.StartEvent();
            }
        }

        if (UpcomingWeathers[Day.Value - LastDay] == Weather.Rain)
        {
            await MetaManager.instance.DoTutorial(Mechanics.Rain);
        }

        if (UpcomingWeathers[Day.Value - LastDay] == Weather.Thunder)
        {
            await MetaManager.instance.DoTutorial(Mechanics.Thunderstorms);
        }

        foreach (var e in Events) {
            var ev = MetaManager.instance.events.GetEvent(e);
            e.StartDay = ev.data.StartDay;
            e.Duration = ev.data.Duration;
        }
    }

    public static string GetMessageOfTheDay()
    {
        var weather = weatherSettings.weathers[UpcomingWeathers[Day.Value - LastDay]];
        var e = MetaManager.instance.events.events.FirstOrDefault(ev => ev.data.StartDay == Day.Value);
        if (Day.Value == 0)
        {
            return "Alright, let's get started! Today is the first day of my new art studio!";
        } else if (e != null)
        {
            return e.eventLogMessage;
        } else if (weather.sallyDialogue.Count > 0)
        {
            return weather.sallyDialogue[Random.Range(0, weather.sallyDialogue.Count)];
        } else
        {
            return "This message should not appear....";
        }
    }

    public static void AddNewColorSet()
    {
        var missingColors = new List<ValidColors>();
        for (int i = 1; i <= 8; i++)
        {
            var col = (ValidColors)(int)Mathf.Pow(2, i);
            if ((Colors.Value & col) == 0)
            {
                missingColors.Add(col);
            }
        }
        int idx2 = 0;
        ValidColors newCols = 0;
        while (idx2 < 4 && missingColors.Count > 0)
        {
            var idx = Random.Range(0, missingColors.Count);
            newCols |= missingColors[idx];
            missingColors.RemoveAt(idx);
            idx2++;
        }
        Colors.Value = Colors.Value | newCols;
    }

    public static float CalculateExpenses()
    {
        float sum = 0;
        foreach (var expense in Expenses)
        {
            sum += expense.Value * expense.Multiplier;
        }

        return sum.MakeDollars();
    }

    public static void RemoveOldestWeather()
    {
        UpcomingWeathers.RemoveAt(0);
        LastDay = Day.Value;
    }

    public static string GetDayStringShort(int d = -1)
    {
        if (d == -1) {
            d = Day.Value;
        }
        var s = "";
        d = d % 365;
        switch (d)
        {
            case < 30:
                s += "Jun ";
                break;
            case < 61:
                s += "Jul ";
                d -= 30;
                break;
            case < 92:
                s += "Aug ";
                d -= 61;
                break;
            case < 122:
                s += "Sep ";
                d -= 92;
                break;
            case < 153:
                s += "Oct ";
                d -= 122;
                break;
            case < 183:
                s += "Nov ";
                d -= 153;
                break;
            case < 214:
                s += "Dec ";
                d -= 183;
                break;
            case < 245:
                s += "Jan ";
                d -= 214;
                break;
            case < 273:
                s += "Feb ";
                d -= 245;
                break;
            case < 304:
                s += "Mar ";
                d -= 273;
                break;
            case < 334:
                s += "Apr ";
                d -= 304;
                break;
            case < 365:
                s += "May ";
                d -= 334;
                break;
        }

        return s += d;
    }

    public static string GetDayString()
    {
        var s = "";
        var d = Day.Value % 365;
        switch (d)
        {
            case < 30:
                s += "June ";
                break;
            case < 61:
                s += "July ";
                d -= 30;
                break;
            case < 92:
                s += "August ";
                d -= 61;
                break;
            case < 122:
                s += "September ";
                d -= 92;
                break;
            case < 153:
                s += "October ";
                d -= 122;
                break;
            case < 183:
                s += "November ";
                d -= 153;
                break;
            case < 214:
                s += "December ";
                d -= 183;
                break;
            case < 245:
                s += "January ";
                d -= 214;
                break;
            case < 273:
                s += "February ";
                d -= 245;
                break;
            case < 304:
                s += "March ";
                d -= 273;
                break;
            case < 334:
                s += "April ";
                d -= 304;
                break;
            case < 365:
                s += "May ";
                d -= 334;
                break;
        }

        return s += d;
    }

    public static Season GetSeason()
    {
        return GetSeason(Day.Value);
    }

    public static Season GetSeason(int day)
    {
        return (Season)(1 + (Mathf.Floor(day / 8.0f) % 4));
    }
}
