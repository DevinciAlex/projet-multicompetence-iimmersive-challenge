using UnityEngine;

/// <summary>
/// Etat minimal partage par les interfaces du projet.
/// L'ancien projet referencait ce composant sans conserver son script.
/// </summary>
public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsMenuOpen { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void OpenMenu()
    {
        IsMenuOpen = true;
    }

    public void CloseMenu()
    {
        IsMenuOpen = false;
    }
}
