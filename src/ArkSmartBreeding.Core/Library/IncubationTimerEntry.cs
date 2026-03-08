using Newtonsoft.Json;
using System;

namespace ARKBreedingStats.Library;

[JsonObject(MemberSerialization.OptIn)]
public class IncubationTimerEntry
{
    [JsonProperty]
    public bool timerIsRunning { get; set; }
    public TimeSpan incubationDuration { get; set; }
    [JsonProperty]
    public DateTime incubationEnd { get; set; }
    private Creature _mother;
    private Creature _father;
    [JsonProperty]
    public Guid motherGuid { get; set; }
    [JsonProperty]
    public Guid fatherGuid { get; set; }
    public string kind { get; set; } // contains "Egg" or "Gestation", depending on the species
    public bool expired { get; set; }
    public bool ShowInOverlay { get; set; }

    public IncubationTimerEntry() { }

    public IncubationTimerEntry(Creature mother, Creature father, TimeSpan incubationDuration, bool incubationStarted)
    {
        Mother = mother;
        Father = father;
        this.incubationDuration = incubationDuration;
        incubationEnd = new DateTime();
        if (incubationStarted)
        {
            StartTimer();
        }
    }

    private void StartTimer()
    {
        if (!timerIsRunning)
        {
            timerIsRunning = true;
            incubationEnd = DateTime.Now.Add(incubationDuration);
        }
    }

    private void PauseTimer()
    {
        if (timerIsRunning)
        {
            timerIsRunning = false;
            incubationDuration = incubationEnd.Subtract(DateTime.Now);
        }
    }

    public void StartStopTimer(bool start)
    {
        if (start)
        {
            StartTimer();
        }
        else
        {
            PauseTimer();
        }
    }

    public Creature Mother
    {
        get => _mother;
        set
        {
            motherGuid = value?.guid ?? Guid.Empty;
            _mother = value;
        }
    }

    public Creature Father
    {
        get => _father;
        set
        {
            fatherGuid = value?.guid ?? Guid.Empty;
            _father = value;
        }
    }

    // Serializer does not support TimeSpan directly, so use this property for serialization instead.
    [System.ComponentModel.Browsable(false)]
    [JsonProperty("incubationDuration")]
    public string incubationDurationString
    {
        get => System.Xml.XmlConvert.ToString(incubationDuration);
        set => incubationDuration = string.IsNullOrEmpty(value) ?
                TimeSpan.Zero : System.Xml.XmlConvert.ToTimeSpan(value);
    }
}
