using UnityEditor;
using UnityEngine;

/// <summary>
/// This class should change for release of each asset so the correct package name and url are uncommented and the rest are commented.
/// Rating key is what we use to store our stuff in prefs.
/// It has some garbage at the end intentionally.
/// </summary>
//[InitializeOnLoad]
public class RatePackage : EditorWindow
{
    #region BB
    //private const string RATING_KEY = "RateDen.RatingStatus18";
    //private const string ASSET_URL = "https://assetstore.unity.com/packages/slug/260366";
    //private const string ASSET_NAME = "Wise Feline Den";
    #endregion

    #region Remembrance
    //private const string RATING_KEY = "RateRemembrance.RatingStatus20";
    //private const string ASSET_URL = "https://assetstore.unity.com/packages/slug/259098";
    //private const string ASSET_NAME = "Wise Feline Remembrance";
    #endregion

    #region IMAP
    private const string RATING_KEY = "RateIMaps.RatingStatus18";
    private const string ASSET_URL = "https://assetstore.unity.com/packages/slug/258596";
    private const string ASSET_NAME = "Wise Feline Influence Maps";
    #endregion

    #region Lite
    //private const string RATING_KEY = "RateUAILite.RatingStatus18";
    //private const string ASSET_URL = "https://assetstore.unity.com/packages/slug/249840";
    //private const string ASSET_NAME = "Wise Feline Utility AI (Lite Version)";
    #endregion

    #region Ultimate
    //private const string RATING_KEY = "RateUAIUltimate.RatingStatus18";
    //private const string ASSET_URL = "https://assetstore.unity.com/packages/slug/235906";
    //private const string ASSET_NAME = "Wise Feline Utility AI (Ultimate Version)";
    #endregion

    #region SmartObjects
    //private const string RATING_KEY = "RateSmartObjects.RatingStatus18";
    //private const string ASSET_URL = "https://assetstore.unity.com/packages/slug/235906";
    //private const string ASSET_NAME = "Wise Feline Smart Objects";
    #endregion

    // A flag to indicate if the user has rated the package or not
    private static bool HasRated => EditorPrefs.GetBool(RATING_KEY, false);

    // A method to show the rating window
    public static void ShowWindow()
    {
        // Check if the user has already rated the package
        if (HasRated) return;

        // Get the existing window or create a new one
        var window = GetWindow<RatePackage>("Rate Package");

        // Set the window size and position
        window.minSize = new Vector2(500, 350);
        window.maxSize = new Vector2(500, 350);
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 350);

        // Show the window
        window.Show();
    }

    // A method to draw the window content
    private void OnGUI()
    {
        // Draw a label with the message
        EditorGUILayout.LabelField("Thank you for using NoOpArmy's packages. We hope you find them useful and enjoyable.", EditorStyles.wordWrappedLabel);

        EditorGUILayout.Space();

        // Draw a label with the rating request
        EditorGUILayout.LabelField($"If you like {ASSET_NAME} package, please consider leaving a rating and a review on the Asset Store. It would help us a lot and motivates us to keep improving it even more.", EditorStyles.wordWrappedLabel);

        EditorGUILayout.Space();

        if (GUILayout.Button("Rate Now"))
        {
            // Open the Asset Store link in the browser
            Application.OpenURL(ASSET_URL);

            // Set the rating status to true and close the window
            EditorPrefs.SetBool(RATING_KEY, true);
            Close();
        }

        if (GUILayout.Button("Other Packages"))
        {
            // Open the Asset Store link in the browser
            Application.OpenURL("https://assetstore.unity.com/publishers/5532");
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Maybe Later"))
        {
            Close();
        }
        if (GUILayout.Button("Don't show this again"))
        {
            EditorPrefs.SetBool(RATING_KEY, true);
            Close();
        }
    }

    private static bool isShown = false;
    
    // A method to show the window at editor startup
    [InitializeOnLoadMethod]
    private static void OnEditorStartup()
    {
        if (isShown)
            return;
        isShown = true;
        // Show the window after a delay of 5 seconds
        EditorApplication.delayCall += () => ShowWindow();
    }
}