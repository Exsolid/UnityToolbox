using UnityEngine;

/// <summary>
/// This manager soley initializes the Localizer, which is used for every localisation.
/// </summary>
public class LocalisationManager : Module
{
    [Tooltip("Defines the path, of where the localisation data is read from.")]
    [SerializeField] private string _assetPathInProject;

    private Localizer _localizer;

    public override void Awake()
    {
        base.Awake();
        _localizer.AssetPathInProject = _assetPathInProject;
    }
}
