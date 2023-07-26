using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InventionCardUnprocessedData {
        public string inventionName;
        public List<string> itemRequirements;
        public List<string> resourceCosts;
        public string terrainTypeRequirement;
        public List<string> effectsOnBuild;
        public List<string> effectsOnLoss;
        public bool isDefaultInvention;
        public bool isPersonalInvention;
    };