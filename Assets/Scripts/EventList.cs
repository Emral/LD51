using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Data/Event", fileName = "Event")]
public class EventList : ScriptableObject
{
    public List<Event> events;

    public EventData FindEligible()
    {
        var candidates = new List<Event>();
        foreach(Event e in events)
        {
            if (e.CanUnlock())
            {
                candidates.Add(e);

                if (e.FixedDay != -1)
                {
                    return e.data;
                }
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
    public int StartDay = -1;
    public int Duration = 0;
}

[System.Serializable]
public class Event
{
    public EventData data = new EventData();

    public string eventLogMessage;
    [MinMaxSlider(1, 10, showFields: true)]
    public Vector2Int minMaxDuration = Vector2Int.one;
    public bool Permanent = false;

    [MinMaxSlider(0, 1, showFields: true)]
    public Vector2 RushHourIncrease = Vector2.zero;

    [MinMaxSlider(0, 3, showFields: true)]
    public Vector2 CustomerGapDecrease = Vector2.zero;

    public bool CanRepeat;
    public int MinRepeatDayDifference;

    public float IncreaseMaxIncomeMultiplier = 0;
    public float IncreaseMaxIncome = 0;

    public bool UnlocksColors;
    public float FollowersRequired;
    public float ExperienceRequired;
    public float ReputationRequired;
    public float MoneyRequired;
    public int MinimumDay;
    public int FixedDay = -1;

    public List<Tag> tags;

    public List<Expense> newExpenses;

    public void Initialize()
    {
        var e = SessionVariables.Events.FirstOrDefault(ev => ev.idx == data.idx);
        if (e != null)
        {
            data.StartDay = e.StartDay;
            data.Duration = e.Duration;
        } else
        {
            data.StartDay = -1;
        }
    }

    public void Schedule()
    {
        data.StartDay = SessionVariables.Day.Value;
        data.Duration = Random.Range(minMaxDuration.x, minMaxDuration.y);
    }

    public int GetStartDay()
    {
        return data.StartDay;
    }

    public int GetEndDay()
    {
        if (Permanent || data.StartDay == -1)
        {
            return -1;
        }

        return data.StartDay + data.Duration;
    }

    public bool CanUnlock()
    {
        var e = SessionVariables.Events.FirstOrDefault(a => a.idx == data.idx);
        if (e != null)
        {
            data.StartDay = e.StartDay;
            data.Duration = e.Duration;
        }
        if (data.StartDay > 0 && (SessionVariables.Day.Value - (data.StartDay + data.Duration) < MinRepeatDayDifference || !CanRepeat))
        {
            return false;
        }

        if (FixedDay > 0)
        {
            return (SessionVariables.Day.Value + 1) % 32 == FixedDay && (SessionVariables.Day.Value + 1) >= MinimumDay;
        }

        if ((SessionVariables.Day.Value + 1) >= MinimumDay && SessionVariables.Followers.Value >= FollowersRequired && SessionVariables.Experience.Value >= ExperienceRequired && SessionVariables.Reputation.Value >= ReputationRequired && SessionVariables.Savings >= MoneyRequired)
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
            for (int j = SessionVariables.Expenses.Count - 1; j >= 0; j--)
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
        var increase = Random.Range(RushHourIncrease.x, RushHourIncrease.y);
        DayManager.Globals.RushHourStart -= increase;
        if (DayManager.Globals.RushHourStart < 0)
        {
            DayManager.Globals.RushHourEnd -= DayManager.Globals.RushHourStart;
            DayManager.Globals.RushHourStart = 0;
            DayManager.Globals.RushHourEnd = Mathf.Min(DayManager.Globals.RushHourEnd, 1);
        }
        DayManager.Globals.CustomerDelay = DayManager.Globals.CustomerDelay - CustomerGapDecrease;
    }
}
