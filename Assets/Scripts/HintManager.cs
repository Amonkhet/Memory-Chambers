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
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        Hint.OnShowHint += (text) => OnDisplayHint?.Invoke(text);
        Hint.OnHideHint += () => OnClearHint?.Invoke();
    }

    void OnDisable()
    {
        Hint.OnShowHint -= text => OnDisplayHint?.Invoke(text);
        Hint.OnHideHint -= () => OnClearHint?.Invoke();
    }
}
