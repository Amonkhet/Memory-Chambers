using UnityEngine;

public class HintManager : MonoBehaviour
{
    // Manager all hint event
    public static HintManager Instance {get; private set;}
    // Broadcast this event to ui
    public static event System.Action<string> OnDisplayHint;
    public static event System.Action OnClearHint;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        Hint.OnShowHint += HandleShowHint;
        Hint.OnHideHint += HandleHideHint;
    }

    void OnDisable()
    {
        Hint.OnShowHint -= HandleShowHint;
        Hint.OnHideHint -= HandleHideHint;
    }
    private void HandleShowHint(string text)
    {
        OnDisplayHint?.Invoke(text);
    }

    private void HandleHideHint()
    {
        OnClearHint?.Invoke();
    }
}
