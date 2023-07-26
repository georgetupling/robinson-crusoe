using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MoveComponentEvent : UnityEvent<int, Transform, Vector3, MoveStyle> { }
