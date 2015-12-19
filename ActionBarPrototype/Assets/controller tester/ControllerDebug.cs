using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.com
/// 
/// Description: NewBehaviourScript 
/// </summary>
public class ControllerDebug : MonoBehaviour {
    #region Fields

    //22 images in total
    public Image aButton, bButton, xButton, yButton;
    public Image startButton, backButton;
    public Image rightBumper, leftBumper, rightTrigger, leftTrigger;
    public Image lsUp, lsDown, lsRight, lsLeft;
    public Image rsUp, rsDown, rsRight, rsLeft;
    public Image dpadUp, dpadDown, dpadRight, dpadLeft;

    #endregion

    void Start() { }

    void Update() {
        //buttons
        aButton.color = (Input.GetButton("Button_A")) ? Color.red : Color.white;
        bButton.color = ( Input.GetButton( "Button_B" ) ) ? Color.red : Color.white;
        xButton.color = ( Input.GetButton( "Button_X" ) ) ? Color.red : Color.white;
        yButton.color = ( Input.GetButton( "Button_Y" ) ) ? Color.red : Color.white;

        //triggers and specials
        startButton.color = ( Input.GetButton( "Button_Start" ) ) ? Color.red : Color.white;
        backButton.color = ( Input.GetButton( "Button_Back" ) ) ? Color.red : Color.white;
        rightBumper.color = ( Input.GetButton( "Bumper_Right" ) ) ? Color.red : Color.white;
        leftBumper.color = ( Input.GetButton( "Bumper_Left" ) ) ? Color.red : Color.white;

        //left stick
        lsRight.color = CalculateColor( Input.GetAxis( "LeftJoystick_Horizontal" ) );
        lsLeft.color = CalculateColor( Input.GetAxis( "LeftJoystick_Horizontal" ) / -1 );
        lsUp.color = CalculateColor( Input.GetAxis( "LeftJoystick_Vertical" ) / -1 );
        lsDown.color = CalculateColor( Input.GetAxis( "LeftJoystick_Vertical" ) );

        //right stick
        rsRight.color = CalculateColor( Input.GetAxis( "RightJoystick_Horizontal" ) );
        rsLeft.color = CalculateColor( Input.GetAxis( "RightJoystick_Horizontal" ) / -1 );
        rsUp.color = CalculateColor( Input.GetAxis( "RightJoystick_Vertical" ) / -1 );
        rsDown.color = CalculateColor( Input.GetAxis( "RightJoystick_Vertical" ) );

        //dpad
        dpadRight.color = CalculateColor( Input.GetAxis( "DPad_Horizontal" ) );
        dpadLeft.color = CalculateColor( Input.GetAxis( "DPad_Horizontal" ) / -1 );
        dpadUp.color = CalculateColor( Input.GetAxis( "DPad_Vertical" ));
        dpadDown.color = CalculateColor( Input.GetAxis( "DPad_Vertical" ) / -1 );

        //triggers
        rightTrigger.color = CalculateColor( Input.GetAxis( "Triggers" ) / -1 );
        leftTrigger.color = CalculateColor( Input.GetAxis( "Triggers" )  );
    }

    Color CalculateColor(float input) {
       return Vector4.Lerp( Color.white, Color.red, input);
    }

}