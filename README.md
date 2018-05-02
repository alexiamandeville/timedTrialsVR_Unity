# FittsLawVR_Unity
An experiment measuring the optimal distance and angle at which a world-space UI object can be interacted with in VR.
Developed in Unity 20173.0f3.

The goal of the project is to run participants through trials in VR to select buttons with different feedback types. Our goal is to identify the optimal width and feedback type in VR applications for interactive user interface components.

This project includes a trial system in VR, for testing interactions based on time. It records data based on a user ID including the following:

Timestamp<br>
User ID<br>
Feedback type (haptic, visual, both)<br>
Target distance<br>
Target width<br>
User success type<br>
Task selection time<br>

The application collects it locally into a comma-delimited text file accessible from the UI all your data collection dreams.

Want to know more about Fitts's law?: https://en.wikipedia.org/wiki/Fitts%27s_law
It predicts that the time required to rapidly move to a target area is a function of the ratio between the distance to the target and the width of the target.

Using:<br>
VRTK for Oculus Touch interactions: https://assetstore.unity.com/packages/tools/vrtk-virtual-reality-toolkit-vr-toolkit-64131
Haptic Helper: https://assetstore.unity.com/packages/tools/input-management/haptics-helper-for-oculus-touch-74044
