using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Expense
{
    public string Name;
    public float Value;
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
    public static List<Expense> Expenses = new List<Expense>();

    public static WeatherSettings weatherSettings;
    public static Seasons seasonSettings;

    public static float Experience = 0;
    public static float Followers = 0;
    public static float Reputation = 0;
    public static float IncomeMultiplier = 1;

    public static UnityEvent<float> OnSavingsChanged = new UnityEvent<float>();
    private static float _savings = 50;
    public static float Savings
    {
        get => _savings;
        set
        {
            _savings = value;
            OnSavingsChanged.Invoke(value);
        }
    }
    public static float MaxIncomeBase = 10;
    public static float TodaysEarnings = 0;
    public static int Day = 1;
    public static int LastDay = 1;

    public static ValidColors Colors;

    public static List<Texture2D> allDrawings;
    public static List<Texture2D> todaysDrawings;

    private static List<WeatherConfig> UpcomingWeathers = new List<WeatherConfig>();

    public static List<Mechanics> TutorialsDone = new List<Mechanics>();

    public static List<Event> Events = new List<Event>();

    public void Initialize()
    {
        Expenses = new List<Expense>()
        {
            new Expense() { Name = "Rent", Value = 25 },
            new Expense() { Name = "Food", Value = 25 },
            new Expense() { Name = "Utensils", Value = 10 }
        };

        Experience = 0;
        Followers = 0;
        Reputation = 0;
        MaxIncomeBase = 10;
        IncomeMultiplier = 1;
        LastDay = 1;
        Day = 1;
        _savings = 50;
        Colors = ValidColors.Black;
        TodaysEarnings = 0;
        allDrawings = new List<Texture2D>();
        todaysDrawings = new List<Texture2D>();
        UpcomingWeathers = new List<WeatherConfig>();
        Events = new List<Event>();

        for (int i = 0; i < 5; i++)
        {
            CalculateWeather(Day + i);
        }
    }

    private static void CalculateWeather(int day)
    {
        day = day - 1;
        var list = seasonSettings.springWeathersByDay;

        if (day % 40 >= 30)
        {
            list = seasonSettings.winterWeathersByDay;
        }
        if (day % 40 >= 20)
        {
            list = seasonSettings.autumnWeathersByDay;
        }
        if (day % 40 >= 10)
        {
            list = seasonSettings.summerWeathersByDay;
        }

        UpcomingWeathers.Add(weatherSettings.weathers[list[day % 10].choices[Random.Range(0, list[day % 10].choices.Count)]]);
    }

    public static WeatherConfig GetTodaysWeather()
    {
        return UpcomingWeathers[(Day - LastDay)];
    }

    public static WeatherConfig GetUpcomingWeather(int i)
    {
        return UpcomingWeathers[(i)];
    }

    public static async void NewDayBegins()
    {
        TodaysEarnings = 0;
        LastDay = Day;
        SFX.DayAdvance.Play();
        Day++;
        CalculateWeather(Day + 5);

        var e = MetaManager.instance.events.FindEligible();

        if (e != null)
        {
            e.Schedule();
            if (!e.Permanent)
            {
                await MetaManager.instance.DoTutorial(Mechanics.Events);
            }
        }

        for (int i = Events.Count-1; i >= 0; i--)
        {
            var ev = Events[i];
            if (ev.GetStartDay() == Day)
            {
                ev.StartEvent();
            }
            else if (ev.GetEndDay() == Day)
            {
                ev.EndEvent();
                Events.Remove(e);
            }
        }

        if (UpcomingWeathers[Day - LastDay].weather == Weather.Rain)
        {
            await MetaManager.instance.DoTutorial(Mechanics.Rain);
        }

        if (Followers >= 25 && Day > 3)
        {
            await MetaManager.instance.DoTutorial(Mechanics.Followers);
        }
    }

    public static string GetMessageOfTheDay()
    {
        if (Day == 1)
        {
            return "Alright, let's get started! Today is the first day of my new art studio!";
        } else if (Events.Count > 0 && !Events[Events.Count - 1].EventLogSeen())
        {
            Events[Events.Count - 1].SetMessageSeen();
            return Events[Events.Count - 1].eventLogMessage;
        } else if (UpcomingWeathers[Day - LastDay].sallyDialogue.Count > 0)
        {
            return UpcomingWeathers[Day - LastDay].sallyDialogue[Random.Range(0, UpcomingWeathers[Day - LastDay].sallyDialogue.Count)];
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
            if ((Colors & col) == 0)
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
        Colors = Colors | newCols;
    }

    public static float CalculateExpenses()
    {
        float sum = 0;
        foreach (var expense in Expenses)
        {
            sum += expense.Value;
        }

        return sum.MakeDollars();
    }

    public static void RemoveOldestWeather()
    {
        UpcomingWeathers.RemoveAt(0);
        LastDay = Day;
    }

    public static string GetDayStringShort(int d = -1)
    {
        if (d == -1) {
            d = Day;
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
        var d = Day % 365;
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
        return (Season) (1 + (Mathf.Min((Day-1) / 10.0f) % 4));
    }
}
