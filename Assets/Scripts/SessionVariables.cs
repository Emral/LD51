using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    Thunder = 5
}

public enum Mechanics
{
    TakingRequests,
    TimeLimit,
    SellEarly,
    DaySlider,
    GetMoney,
    RushHour,
    Thunderstorms,
    Events,
    Colors
}

public class SessionVariables : ScriptableObject
{
    public static List<Expense> Expenses = new List<Expense>();

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

    public static ValidColors Colors;

    public static List<Texture2D> allDrawings;
    public static List<Texture2D> todaysDrawings;

    public static List<Weather> UpcomingWeathers = new List<Weather>();

    public static List<Mechanics> TutorialsDone = new List<Mechanics>();

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
        _savings = 50;
        Colors = ValidColors.Black;
        Day = 1;
        allDrawings = new List<Texture2D>();
        todaysDrawings = new List<Texture2D>();
        UpcomingWeathers = new List<Weather>();

        for (int i = 0; i < 6; i++)
        {
            CalculateWeather(i);
        }
    }

    public static void AddFirstColorSet()
    {
        Colors = Colors | ValidColors.Purple | ValidColors.Grey | ValidColors.Green | ValidColors.Orange;
    }

    public static void AddSecondColorSet()
    {
        Colors = Colors | ValidColors.Yellow | ValidColors.Grey | ValidColors.Blue | ValidColors.Red;
    }

    public static float CalculateExpenses()
    {
        float sum = 0;
        foreach (var expense in Expenses)
        {
            sum += expense.Value;
        }

        return Mathf.FloorToInt(sum * 100) / 100;
    }

    public static void CalculateWeather(int dayToCalculate)
    {
        if (dayToCalculate > 2 && dayToCalculate % 5 == 3)
        {
            UpcomingWeathers.Add(Weather.Rain);
        } else if (Experience > 50)
        {
            if (Random.Range(0, 1000) < Mathf.Min(Experience, 250))
            {
                UpcomingWeathers.Add(Weather.Thunder);
            }
        } else
        {
            UpcomingWeathers.Add((Weather)Random.Range(1, 3));
        }
    }

    public static void RemoveOldestWeather()
    {
        UpcomingWeathers.RemoveAt(0);
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
}
