using UnityEngine;

public class LocalisationManager : Module
{
    [Tooltip("Defines the path, of where the localisation data is read from.")]
    [SerializeField] private string _assetPathInProject;

    private LocalisationLanguage _language;
    public LocalisationLanguage Language
    {
        get { return _language; }
        set { _language = value; }
    }

    private Localizer _localizer;

    public override void Awake()
    {
        base.Awake();
        _localizer.AssetPathInProject = _assetPathInProject;
    }

    public string GetLocalizedString(LocalisationID ID)
    {
        return _localizer.LocalisationData[ID][_language];
    }
}
