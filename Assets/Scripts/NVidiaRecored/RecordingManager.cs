using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA;

public class RecordingManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetupHightlight();
    }

    public void SetupHightlight()
    {
        Highlights.HighlightScope[] RequiredScopes = new Highlights.HighlightScope[3]
        {
            Highlights.HighlightScope.Highlights,
            Highlights.HighlightScope.HighlightsRecordVideo,
            Highlights.HighlightScope.HighlightsRecordScreenshot,
        };

        System.String AppName = "DCT";
        if (Highlights.CreateHighlightsSDK(AppName, RequiredScopes) != Highlights.ReturnCode.SUCCESS)
        {
            Debug.LogError("Failed to initialize highlights");
            return;
        }
    
        Highlights.HighlightDefinition[] hlDefinitions = new Highlights.HighlightDefinition[1];
        hlDefinitions[0].Id = "Record";
        hlDefinitions[0].HighlightTags = Highlights.HighlightType.Achievement;
        hlDefinitions[0].Significance = Highlights.HighlightSignificance.Good;
        hlDefinitions[0].UserDefaultInterest = true;
        hlDefinitions[0].NameTranslationTable = new Highlights.TranslationEntry[]
        {
                new Highlights.TranslationEntry ("en-US", "Record"),
                new Highlights.TranslationEntry ("ko-KR", "녹화")
        };

        Highlights.ConfigureHighlights(hlDefinitions, "ko-KR", Highlights.DefaultConfigureCallback);
        Highlights.GetUserSettings(Highlights.DefaultGetUserSettingsCallback);
        Highlights.RequestPermissions(Highlights.DefaultRequestPermissionsCallback);

        Highlights.OpenGroupParams param = new Highlights.OpenGroupParams();
        param.Id = "Recorded_Group";
        param.GroupDescriptionTable = new Highlights.TranslationEntry[]
        {
                new Highlights.TranslationEntry ("en-US", "Recorded"),
                new Highlights.TranslationEntry ("ko-KR", "DCT녹화")
        };
        Highlights.OpenGroup(param, Highlights.DefaultOpenGroupCallback);
    }

    public void Record()
    {
        Highlights.VideoHighlightParams param = new Highlights.VideoHighlightParams();
        param.highlightId = "Record";
        param.groupId = "Recorded_Group";
        param.startDelta = -(int)(Time.realtimeSinceStartup * 1000);
        //param.startDelta = -10000;
        param.endDelta = 1000;

        Highlights.SetVideoHighlight(param, Highlights.DefaultSetVideoCallback);
    }

    public void ShowHighlightSummaryWindows()
    {
        Highlights.GroupView[] groupViews = new Highlights.GroupView[1];
        groupViews[0] = new Highlights.GroupView();
        groupViews[0].GroupId = "Recorded_Group";
        groupViews[0].SignificanceFilter = Highlights.HighlightSignificance.Good;
        groupViews[0].TagFilter = Highlights.HighlightType.Achievement;

        Highlights.OpenSummary(groupViews, Highlights.DefaultOpenSummaryCallback);
    }

    private void Update()
    {
        Highlights.UpdateLog();
    }

    private void OnDestroy()
    {
        Highlights.ReleaseHighlightsSDK();
    }
}
