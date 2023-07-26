using UnityEngine;
using UnityEngine.Events;

public class BuildInventionEvent : UnityEvent<string, int, int, Invention>
{
    public const string Build = "Build";
    public const string Success = "Success";
}
