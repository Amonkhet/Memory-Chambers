using System;

public static class Hint
{
    // Broadcast this event to manager
    public static event Action<string> OnShowHint;
    public static event Action OnHideHint;
    
    public static void ShowHint(string text) => OnShowHint?.Invoke(text);
    public static void HideHint() => OnHideHint?.Invoke();
}
