using UnityEngine;
using System.Collections;

public static class ConstantsSpacerock{


    // ~ projectwide ~ //
    public static int RIGHT = 2100;
    public static int LEFT = 2101;

    // ~ FOR HANDSRAYCAST.CS ~ //
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

    // colors for hand gestures
    public static Color pinchColor = Color.red;
    public static Color fistColor = Color.blue;


    // ~ FOR GENERATEGRAPH.CS ~ //

    // graph position
    public static float ELEVATION_CONSTANT = 1.0f; // elevation of graph
    public static float NODE_SPREAD_Z = 1.0f;

    public static float DISTANCE_FROM_FACE = 17.0f; // distance of the graph from face

    public static float GRAPH_SCALE_CONSTANT = 0.005f;

    // the detailing / explosion interaction
    public static float EXPLOSION_TIME_1 = 3.0f;  // after this time (in seconds) show only the selected node
    public static float EXPLOSION_TIME_2 = 6.0f;  // after this time, show selected node and its relations
    public static float EXPLOSION_TIME_3 = 33.0f; // after this time, show relations of relations of the selected node

    // for the currently deprecated force calculations
    public static float CHARGE_CONSTANT = 0.5f;
    public static float SPRING_CONSTANT = 4.0f;

    public static float CHANCE_OF_CONNECTION = 0.09f;
    public static int NUMBER_NODES = 40;

    public static int NODES_PROCESSED_PER_FRAME = 40; // nodes processed per frame, for the currently deprecated force calculations

    public static float NODE_SPREAD_X = 1.0f;
    public static float NODE_SPREAD_Y = 0.8f;

    // ~ FOR LEAPRTS.CS ~ //
    public static float PULL_CONSTANT = 17.0f; // LeapRTS.cs uses this to determine the sensitivity of pulling objects towards you or pushing them away

}
