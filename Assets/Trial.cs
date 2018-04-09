using System.Collections;
using System.Collections.Generic;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Trial : MonoBehaviour {

    // Data stuff
    int _userID;
    int _trialID;
    string _acquisitionType; //my feedback type
    float _width;
    int _distance;
    bool _selected;
    float _taskTime;
    DateTime _timestamp;

    // UI data
    public Text myUserUI;
    public Dropdown[] myOrderUI;
    public GameObject myStartMenu;
    public GameObject controlTarget;

    // Data variables
    string[] myOrders = new String[3] {" ", " ", " "}; // Store the acquisition order in this
    public float[] myWidths;
    public GameObject[] myButtons;

    // Temp variables
    string myAquisitionType;
    int myTrial = 1;
    float myTimer = 0.0f;
    float myWidth;
    string myButtonDistance;
    int randomButton;
    int randomWidth;

    private void Start()
    {
        ResetData();
        controlTarget.SetActive(true); // Turn on our control target
    }

    // (Button) Used for the start button to set up all the user and order data
    public void EnterInfo()
    {
        // Check that we have a valid User ID
        if (myUserUI.text != "")
        {
            _userID = int.Parse(myUserUI.text);
        } else
        {
            print("Please enter a User ID to start");
            return; // Do nothing
        }

        myStartMenu.SetActive(false); // Close start menu

        // Store our ordering for acquisition type
        myOrders[0] = myOrderUI[0].options[myOrderUI[0].value].text;
        myOrders[1] = myOrderUI[1].options[myOrderUI[1].value].text;
        myOrders[2] = myOrderUI[2].options[myOrderUI[2].value].text;

    }

    // We will show a control between every trial
    public void ClickedControl()
    {
        controlTarget.SetActive(false); // Turn off our control target

        StartTrial(); // Start our trials
    }

    void Update()
    {
        myTimer += Time.deltaTime; // Run timer

        if (Input.GetKeyDown(KeyCode.R)) // Reset timer button
            ResetTimer();

        if (Input.GetKeyDown(KeyCode.C)) // Testing
            ClickedControl();

        if (Input.GetKeyDown(KeyCode.T)) // Testing
            TriggerEvent();
    }

    // Things that happen when I start my trial
    void StartTrial()
    {
        ResetTimer();
        ResetData(); // Make sure everything is reset

        _timestamp = DateTime.Now; // Set the timestamp of the trial
        myTrial += 1; // Increment my trial each time for this user on start

        // Set up my acquisition type here
        if (myTrial <= 9)
        {
            myAquisitionType = myOrders[0];//my first order slot
        } else if (myTrial >= 10 && myTrial <= 19)
        {
            myAquisitionType = myOrders[1]; //my second order slot
        } else if (myTrial >= 20 && myTrial <= 27)
        {
            myAquisitionType = myOrders[2]; //my third order slot
        }

        // TODO Alexia hasn't set up the feedback stuff yet
        FeedbackType(myAquisitionType); // Set up my hovering feedback functionality

        myWidth = myWidths[randomWidth]; // Set up with width
        myButtons[randomButton].GetComponent<RectTransform>().localScale = new Vector3(myWidth, 1.0f, 1.0f); // Change the scale of the button

        myButtons[randomButton].SetActive(true); // Pick a random button to show
        myButtonDistance = myButtons[randomButton].name; // Store which button was shown

    }

    // Check to see if user has pressed trigger (VRTK public event)
    public void TriggerEvent()
    {
        // Check to see if target was hit

        SetFinalData();
        Save(); // Save our data to a json file
        ResetData();
        controlTarget.SetActive(true); // Turn on our control target

    }

    void FeedbackType(String myType)
    {
        switch (myType)
        {
            case "Haptics":
                print("Haptics"); // TODO
                break;
            case "Visuals":
                print("Visuals"); // TODO
                break;
            case "HV":
                print("HV"); // TODO
                break;
        }
    }

    void ResetTimer()
    {
        myTimer = 0.0f;
    }

    void ResetData()
    {
        foreach(GameObject button in myButtons)
        {
            button.SetActive(false);
        }


        randomButton = UnityEngine.Random.Range(0, 2);
        randomWidth = UnityEngine.Random.Range(0, 2);
    }

    void SetFinalData()
    {
        _trialID = myTrial;
        _taskTime = myTimer;
        _acquisitionType = myAquisitionType;
        _width = myWidth;
        _distance = int.Parse(myButtonDistance);
    }

    public void Save()
    {
        QuickSaveWriter.Create("Inputs")
            .Write("Timestamp", _timestamp)
            .Write("User", _userID)
            .Write("Trial", _trialID)
            .Write("AcquisitionType", _acquisitionType)
            .Write("TargetDistance", _distance)
            .Write("TargetWidth", _width)
            .Write("TargetSelection", _selected)
            .Write("TargetSelectionTie", _taskTime)
            .Commit();

        // ToDo where does this save?
        QuickSaveRaw.LoadString("Inputs.json"); // Save to file
    }

}
