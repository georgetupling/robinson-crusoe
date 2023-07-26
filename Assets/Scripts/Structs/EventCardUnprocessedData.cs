using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EventCardUnprocessedData {
        public string eventName;
        public string eventDescription;
        public string cardSymbol;
        public List<string> eventEffects;
        public string threatName;
        public List<string> threatItemRequirements;
        public int threatWeaponRequirement;
        public List<string> threatResourceCosts;
        public bool has1ActionThreat;
        public bool has2ActionThreat;
        public List<string> successEffects1Action;
        public List<string> successEffects2Action;
        public List<string> failureEffects;
        public string materialName;
        public bool isStartingEvent;
    };
