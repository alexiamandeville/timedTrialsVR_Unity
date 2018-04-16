using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class Trial : MonoBehaviour {

    // Data stuff
    string _userID;
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
    public GameObject endUI;
    public GameObject tryUI;

    // Data variables
    string[] myOrders = new String[3] {" ", " ", " "}; // Store the acquisition order in this
    public float[] myWidths;
    public GameObject[] myButtons;
    public GameObject[] myCanvases;

    // Temp variables
    string myAquisitionType;
    int myTrial = 0;
    float myTimer = 0.0f;
    float myWidth;
    string myButtonDistance;
    int randomButton = 0;
    int randomWidth = 0;
    bool mySelected;
    int myHapticStrength;
    GameObject tempB; // For shuffling the array
    float tempW;

    // Demo things
    bool isTrying;

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

    // A public UI method to not collect data, but just try
    public void TryButton()
    {
        isTrying = true;
    }

    // (Button) Used for the start button to set up all the user and order data
    public void EnterInfo()
    {
        isTrying = false;
        // Check that we have a valid User ID
        if (myUserUI.text != "")
        {
            _userID = myUserUI.text;
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

        StartTrial(); // Start our trialsf
    }

    // We need to shuffle the array order for every 3rd trial
    public void Shuffle()
    {
        for (int i = 0; i < myButtons.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(0, myButtons.Length);
            tempB = myButtons[rnd];
            myButtons[rnd] = myButtons[i];
            myButtons[i] = tempB;
        }

        for (int i = 0; i < myWidths.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(0, myWidths.Length);
            tempW = myWidths[rnd];
            myWidths[rnd] = myWidths[i];
            myWidths[i] = tempW;
        }
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

        // If we are real time
        if (!isTrying)
        {
            // Set up my acquisition type here
            if (myTrial <= 9)
            {
                myAquisitionType = myOrders[0];//my first order slot
            }
            else if (myTrial >= 10 && myTrial <= 18)
            {
                myAquisitionType = myOrders[1]; //my second order slot
            }
            else if (myTrial >= 19 && myTrial <= 27)
            {
                myAquisitionType = myOrders[2]; //my third order slot
            }
            else if (myTrial > 27)
            {
                endUI.SetActive(true);
            }
        } else
        {
            if (myTrial <= 3)
            {
                myAquisitionType = myOrders[0]; //my first order slot
            }

            if (myTrial == 4)
            {
                tryUI.SetActive(true); // Set UI up when we're in a demo
            }
        }

        FeedbackType(myAquisitionType); // Set up my hovering feedback functionality

        myWidth = myWidths[randomWidth]; // Set up with width
        myButtons[randomButton].GetComponent<RectTransform>().localScale = new Vector3(myWidth, 1.0f, 1.0f); // Change the scale of the button
        myButtons[randomButton].SetActive(true); // Pick a random button to show
        myCanvases[randomButton].SetActive(true);
        myButtonDistance = myButtons[randomButton].name; // Store which button was shown

    }

    // Used to determine if user has pressed button (UI button event)
    public void HasSelected()
    {
        mySelected = true;
        StopCoroutine(Wait());

    }

    // Check to see if user has pressed trigger (VRTK public event)
    public void TriggerEvent()
    {
        StartCoroutine(Wait());
        
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.3f);

        // Only send data when we're not testing this thing out
        if (!isTrying)
        {
            SetFinalData();
            StartCoroutine(Save()); // Save our data to a json file
        }

        ResetData();
        controlTarget.SetActive(true); // Turn on our control target
    }

    void FeedbackType(String myType)
    {
        switch (myType)
        {
            case "Haptic":
                SwitchHaptics(255);
                SwitchVisuals(Color.white);
                break;
            case "Visual":
                SwitchHaptics(0);
                SwitchVisuals(Color.yellow);
                break;
            case "HV":
                SwitchHaptics(255);
                SwitchVisuals(Color.yellow);
                break;
        }
    }

    // Method to turn on and off visuals for each button
    void SwitchVisuals(Color myColor)
    {
        foreach (GameObject button in myButtons)
        {
            ColorBlock colorTint = button.GetComponent<Button>().colors;
            colorTint.highlightedColor = myColor;
            colorTint.pressedColor = myColor;
            button.GetComponent<Button>().colors = colorTint;
        }
    }

    // Method to turn on and off haptics for each button
    void SwitchHaptics(int haptics)
    {
        myHapticStrength = haptics;
    }

    // Let's play haptics on a button when it's hovered (public UI)
    public void PlayHaptics()
    {
        print(myHapticStrength + "haptics");
        HapticHelper.instance.ProceduralTone(false, myHapticStrength, 20);
    }

    public void ResetTimer()
    {
        myTimer = 0.0f;
    }

    void ResetData()
    {
        myHapticStrength = 0;
        StopCoroutine(Save());
        foreach (GameObject canvas in myCanvases)
        {
            canvas.SetActive(false);
        }
        foreach (GameObject button in myButtons)
        {
            button.SetActive(false);
        }

        mySelected = false;

        // Starting our selection from 
        if (randomButton < 3)
        {
            randomButton += 1;
            randomWidth += 1;
        } else
        {
            Shuffle();
            randomButton = 0;
            randomWidth = 0;
        }

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
            string dataCSV = _timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "," + _userID+ "," + _trialID.ToString() + "," + _acquisitionType.ToString() + "," + _distance.ToString() + "," + _width.ToString() + "," + _selected.ToString() + "," + _taskTime.ToString();

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
