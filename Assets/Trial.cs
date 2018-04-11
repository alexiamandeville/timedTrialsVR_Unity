using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class Trial : MonoBehaviour {

    // Data stuff
    int _userID;
    int _trialID;
    string _acquisitionType; //my feedback type
    float _width;
    string _distance;
    bool _selected;
    float _taskTime;
    DateTime _timestamp;

    string file;
    StreamWriter line;

    // UI data
    public Text myUserUI;
    public Dropdown[] myOrderUI;
    public GameObject myStartMenu;
    public GameObject controlTarget;
    public Text myFilePath;

    // Data variables
    string[] myOrders = new String[3] {" ", " ", " "}; // Store the acquisition order in this
    public float[] myWidths;
    public GameObject[] myButtons;

    // Temp variables
    string myAquisitionType;
    int myTrial = 0;
    float myTimer = 0.0f;
    float myWidth;
    string myButtonDistance;
    int randomButton;
    int randomWidth;
    bool mySelected;

    private void Start()
    {
        file = Application.persistentDataPath + "/" + "FittsData.csv";
        myFilePath.text = file;
        // Does the file already exist?
        if (!File.Exists(file))
        {
            line = File.CreateText(file);
        }

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

        print(myTrial);
        print(myAquisitionType);

    }

    // Used to determine if user has pressed button (UI button event)
    public void HasSelected()
    {
        mySelected = true;
    }

    // Check to see if user has pressed trigger (VRTK public event)
    public void TriggerEvent()
    {

        SetFinalData();
        StartCoroutine(Save()); // Save our data to a json file
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
        StopCoroutine(Save());
        foreach (GameObject button in myButtons)
        {
            button.SetActive(false);
        }
        mySelected = false;
        randomButton = UnityEngine.Random.Range(0, 2);
        randomWidth = UnityEngine.Random.Range(0, 2);
    }

    void SetFinalData()
    {
        _trialID = myTrial;
        _taskTime = myTimer;
        _acquisitionType = myAquisitionType;
        _width = myWidth;
        _distance = myButtonDistance;
        _selected = mySelected;
    }

    IEnumerator Save()
    {

        // Does the file already exist?
        if (File.Exists(file))
        {
            // string dataCSV = "Timestamp,UserID,Trial,AcquisitionType,TargetDistance,TargetWidth,TargetSelect,TaskTime";
            string dataCSV = _timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "," + _userID.ToString() + "," + _trialID.ToString() + "," + _acquisitionType.ToString() + "," + _distance.ToString() + "," + _width.ToString() + "," + _selected.ToString() + "," + _taskTime.ToString();

            line = File.AppendText(file);
            
            line.WriteLine(dataCSV);
            

            line.Close();

        }

        yield return new WaitForSeconds(0.5f); // Wait to make sure we've written to the file

    }

    // Let's take a look at the data (UI button)
    public void OpenDataFile()
    {
        // Does the file already exist?
        if (File.Exists(file))
        {
            Application.OpenURL(file);
        } else
        {
            print("No file exists. Start a trial to generate a file.");
        }
    }

}
