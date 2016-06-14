using UnityEngine;
using System.Collections;

public static class ConstantsSpacerock{


    // ~ projectwide ~ //
    public static int RIGHT = 2100;
    public static int LEFT = 2101;


    // ~ For HandsRaycast.cs ~ //
    public static float SLIDER_MOVE_SPEED = 0.004f; // The speed at which slider bars scroll

    // For the viewpanel code inside HandsRaycast.cs
    public static float VIEWPANEL_EULER_X_LOWER_THRESHHOLD = 14.0f; // higher values here make the head need to incline lower to show the panel
    public static float VIEWPANEL_EULER_X_UPPER_THRESHHOLD = 100.0f;

    public static float PANEL_ON_TIMER_CONSTANT = 0.5f;
    public static float PANEL_OFF_TIMER_CONSTANT = 1.5f;

    // two-handed actions
    public static float PALM_SELECTION_TIME_THRESHHOLD = 1.0f;
    public static float PALM_DESELECTION_TIME_THRESHHOLD = 2.0f;

    public static float TWO_HAND_PROXIMITY_CONSTANT = 0.10f;


    // ~ For LeapRTS.cs ~ //
    public static float PULL_CONSTANT = 17.0f; // LeapRTS.cs uses this to determine the sensitivity of pulling objects towards you or pushing them away

}
