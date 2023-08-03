using UnityEngine;

public class OngoingCardEffect : CardEffect, IEffectEndable
{
    public Trigger endTrigger { get; protected set; }
    public Trigger effectTrigger { get; protected set; }
    public bool hasEnded { get; protected set; }

    // Used by wound-related ongoing effects
    protected bool medicineBuilt;

    protected override void Initialize()
    {
        base.Initialize();
        endTrigger = Trigger.None; // Default value
        effectTrigger = Trigger.None; // Default value
    }

    public override void ApplyEffect()
    {
        if (hasBeenApplied)
        {
            Debug.LogError("OngoingCardEffect effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseStartOngoingEffectEvent(this);
        hasBeenApplied = true;
        Debug.LogError("OngoingCardEffect started with no effect. Consider using a child class instead.");
    }

    public virtual void EndEffect()
    {
        if (hasEnded)
        {
            Debug.LogError("OngoingCardEffect effect has already ended.");
            return;
        }
        hasEnded = true;
        Debug.LogError("OngoingCardEffect ended with no effect. Consider using a child class instead.");
    }

    public virtual void ApplyEffectTrigger()
    {
        // This method exists to be overridden by child classes
    }

    public void SetMedicineBuilt(bool medicineBuilt)
    {
        this.medicineBuilt = medicineBuilt;
    }
}
