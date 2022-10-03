using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Event", fileName = "Event")]
public class EventList : ScriptableObject
{
    public List<Event> events;

    public Event FindEligible()
    {
        var candidates = new List<Event>();
        foreach(Event e in events)
        {
            if (e.CanUnlock())
            {
                candidates.Add(e);
            }
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        return candidates[Random.Range(0, candidates.Count)];
    }
}

[System.Serializable]
public class Event
{
    public string eventLogMessage;
    public int minDuration;
    public int maxDuration;
    public bool Permanent = false;

    public int Delay = 1;

    public float IncreasesRushHourMin = 0;
    public float IncreasesRushHourMax = 0;

    private int StartDay;
    private int Duration;

    public bool CanRepeat;
    public int MinRepeatDayDifference;

    private bool eventLogMessageSeen = false;

    public float IncreaseMaxIncomeMultiplier = 0;
    public float IncreaseMaxIncome = 0;

    public bool UnlocksColors;
    public float FollowersRequired;
    public float ExperienceRequired;
    public float ReputationRequired;
    public float MoneyRequired;
    public float DayRequired;

    public List<string> eventTags;

    public List<Expense> newExpenses;

    public bool EventLogSeen()
    {
        return eventLogMessageSeen;
    }

    public void Initialize()
    {
        StartDay = 0;
        eventLogMessageSeen = false;
    }

    public void Schedule()
    {
        eventLogMessageSeen = false;
        SessionVariables.Events.Add(this);
        StartDay = SessionVariables.Day + Delay;
        Duration = Random.Range(minDuration, maxDuration + 1);
    }

    public void SetMessageSeen()
    {
        eventLogMessageSeen = true;
    }

    public int GetStartDay()
    {
        return StartDay;
    }

    public int GetEndDay()
    {
        if (Permanent)
        {
            return -1;
        }

        return StartDay + Duration;
    }

    public bool CanUnlock()
    {
        if (StartDay != 0 && (SessionVariables.Day - (StartDay + Duration) < MinRepeatDayDifference || !CanRepeat))
        {
            return false;
        }

        if (SessionVariables.Day >= DayRequired && SessionVariables.Followers >= FollowersRequired && SessionVariables.Experience >= ExperienceRequired && SessionVariables.Reputation >= ReputationRequired && SessionVariables.Savings >= MoneyRequired)
        {
            return true;
        }

        return false;
    }

    public void StartEvent()
    {
        if (UnlocksColors)
        {
            SessionVariables.AddNewColorSet();
        }

        SessionVariables.IncomeMultiplier += IncreaseMaxIncomeMultiplier;
        SessionVariables.MaxIncomeBase += IncreaseMaxIncome;

        for (int i = 0; i < newExpenses.Count; i++)
        {
            bool modified = false;
            foreach (var e in SessionVariables.Expenses)
            {
                if (e.Name == newExpenses[i].Name)
                {
                    modified = true;
                    e.Value = e.Value + newExpenses[i].Value;
                    break;
                }
            }

            if (!modified)
            {
                SessionVariables.Expenses.Add(newExpenses[i]);
            }
        }
    }

    public void EndEvent()
    {
        SessionVariables.IncomeMultiplier -= IncreaseMaxIncomeMultiplier;
        SessionVariables.MaxIncomeBase -= IncreaseMaxIncome;

        for (int i = 0; i < newExpenses.Count; i++)
        {
            for (int j = SessionVariables.Expenses.Count; j >= 0; j--)
            {
                var e = SessionVariables.Expenses[j];
                if (e.Name == newExpenses[i].Name)
                {
                    e.Value = e.Value - newExpenses[i].Value;
                    if (e.Value == 0)
                    {
                        SessionVariables.Expenses.Remove(e);
                    }
                    break;
                }
            }
        }
    }

    public void DailyExecute()
    {
        foreach (var tag in eventTags)
        {
            if (!DayManager.Globals.tagBiases.Contains(tag))
            {
                DayManager.Globals.tagBiases.Add(tag);
            }
        }
        var increase = Random.Range(IncreasesRushHourMin, IncreasesRushHourMax);
        DayManager.Globals.RushHourStart -= increase;
        if (DayManager.Globals.RushHourStart < 0)
        {
            DayManager.Globals.RushHourEnd -= DayManager.Globals.RushHourStart;
            DayManager.Globals.RushHourStart = 0;
            DayManager.Globals.RushHourEnd = Mathf.Min(DayManager.Globals.RushHourEnd, 1);
        }
    }
}
