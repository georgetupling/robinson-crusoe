using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScenario
{
    public int turns { get; set; }
    public bool CheckWinCondition();
}
