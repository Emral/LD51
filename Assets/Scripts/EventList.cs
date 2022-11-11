using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Event", fileName = "Event")]
public class EventList : ScriptableObject
{
    public List<Event> events;

    private void Awake()
    {
        int i = 0;
        foreach(var e in events)
        {
            e.data.idx = i;
        }
    }

    public EventData FindEligible()
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

        return candidates[Random.Range(0, candidates.Count)].data;
    }

    public Event GetEvent(EventData data)
    {
        return events[data.idx];
    }
}

[System.Serializable]
public class EventData
{
    public int idx;
    public bool eventLogMessageSeen;
    public int StartDay;
    public int Duration;
}

[System.Serializable]
public class Event
{
    public EventData data = new EventData();

    public string eventLogMessage;
    public int minDuration;
    public int maxDuration;
    public bool Permanent = false;

    public int Delay = 1;

    public float IncreasesRushHourMin = 0;
    public float IncreasesRushHourMax = 0;

    public bool CanRepeat;
    public int MinRepeatDayDifference;


    public float IncreaseMaxIncomeMultiplier = 0;
    public float IncreaseMaxIncome = 0;

    public bool UnlocksColors;
    public float FollowersRequired;
    public float ExperienceRequired;
    public float ReputationRequired;
    public float MoneyRequired;
    public float DayRequired;

    public List<Tag> tags;

    public List<Expense> newExpenses;

    public bool EventLogSeen()
    {
        return data.eventLogMessageSeen;
    }

    public void Initialize()
    {
        data.StartDay = 0;
        data.eventLogMessageSeen = false;
    }

    public void Schedule()
    {
        data.eventLogMessageSeen = false;
        data.StartDay = SessionVariables.Day.Value + Delay;
        data.Duration = Random.Range(minDuration, maxDuration + 1);
        SessionVariables.Events.Add(data);
    }

    public void SetMessageSeen()
    {
        data.eventLogMessageSeen = true;
    }

    public int GetStartDay()
    {
        return data.StartDay;
    }

    public int GetEndDay()
    {
        if (Permanent)
        {
            return -1;
        }

        return data.StartDay + data.Duration;
    }

    public bool CanUnlock()
    {
        if (data.StartDay != 0 && (SessionVariables.Day.Value - (data.StartDay + data.Duration) < MinRepeatDayDifference || !CanRepeat))
        {
            return false;
        }

        if (SessionVariables.Day.Value >= DayRequired && SessionVariables.Followers.Value >= FollowersRequired && SessionVariables.Experience.Value >= ExperienceRequired && SessionVariables.Reputation.Value >= ReputationRequired && SessionVariables.Savings >= MoneyRequired)
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

        SessionVariables.IncomeMultiplier.Value += IncreaseMaxIncomeMultiplier;
        SessionVariables.MaxIncomeBase.Value += IncreaseMaxIncome;

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
        SessionVariables.IncomeMultiplier.Value -= IncreaseMaxIncomeMultiplier;
        SessionVariables.MaxIncomeBase.Value -= IncreaseMaxIncome;

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
        foreach (var tag in tags)
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
