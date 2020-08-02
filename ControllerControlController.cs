using UnityEngine;
using UnityEngine.InputSystem;

/**
 * simulate VR devices with a game controller
 * 
 * consists of 2 scripts:
 * 
 * ControllerControlController (YOU ARE HERE!)
 *    Manages the inputs and maps them to the different devices to control: headset, left and right controller.
 *    
 * ControllerControl
 *    One for each device: headset, left and right controller.
 *    Manages the commands from the ControllerControlController to simulate position and rotation changes as well as button presses.
 * 
 */

namespace David.Controller.ControllerControl
{
    public enum WhatToControl { Headset = 0, LeftController = 1, RightController = 2 };


    public class ControllerControlController : MonoBehaviour
    {

        public GameObject headset;
        public GameObject leftController;
        public GameObject rightController;
        public GameObject baseController;
        public float moveSpeed = 5;
        public float rotationSpeed = 5;

        public WhatToControl whatToControl = 0;

        private Gamepad gamepad;

        private bool leftShoulderPressed = false;
        private bool rightShoulderPressed = false;


        // Start is called before the first frame update
        void Start()
        {
            // ToDo: Check if the ControllerControl script is attached and correctly configured OR add them automatically.
            gamepad = Gamepad.current;

            // move camera (and base) to "standing" position
            headset.transform.Translate(new Vector3(0, 1.75f, 0));
            baseController.transform.Translate(new Vector3(0, 1.75f, 0));

            // move controllers in front of camera
            rightController.transform.Translate(new Vector3(0.2f, -0.1f, 0.5f));
            leftController.transform.Translate(new Vector3(-0.2f, -0.1f, 0.5f));
        }

        // Update is called once per frame
        void Update()
        {
            if (gamepad == null)
                return; // No gamepad connected.


            // iterate whatToControl with shoulder buttons
            {

                if (gamepad.leftShoulder.wasPressedThisFrame)
                {
                    if (!leftShoulderPressed)
                    {
                        // previous WhatToControl
                        int wTCtmp = (((int)whatToControl) - 1);
                        if (wTCtmp < 0) { wTCtmp = 2; }
                        whatToControl = (WhatToControl)wTCtmp;
                        leftShoulderPressed = true;
                    }
                }

                if (gamepad.leftShoulder.wasReleasedThisFrame)
                {
                    leftShoulderPressed = false;
                }

                if (gamepad.rightShoulder.wasPressedThisFrame)
                {
                    if (!rightShoulderPressed)
                    {
                        // previous WhatToControl
                        int wTCtmp = (((int)whatToControl) + 1);
                        if (wTCtmp > 2) { wTCtmp = 0; }
                        whatToControl = (WhatToControl)wTCtmp;
                        rightShoulderPressed = true;
                    }
                }

                if (gamepad.rightShoulder.wasReleasedThisFrame)
                {
                    rightShoulderPressed = false;
                }
            }

            Transform transform = headset.transform;
            if (whatToControl == WhatToControl.LeftController) { transform = leftController.transform; }
            if (whatToControl == WhatToControl.RightController) { transform = rightController.transform; }


            Vector2 dpad = gamepad.dpad.ReadValue();
            Vector2 leftStick = gamepad.leftStick.ReadValue();
            Vector2 rightStick = gamepad.rightStick.ReadValue();

            float tilt = -dpad.x;
            float yMove = dpad.y;
            float xMove = leftStick.x;
            float zMove = leftStick.y;
            float rotateHorizontal = rightStick.x;
            float rotateVertical = -rightStick.y;

            Quaternion quaternion = Quaternion.AngleAxis(tilt, transform.right);

            transform.Translate(new Vector3(xMove, yMove, zMove) * moveSpeed * Time.deltaTime);
            transform.RotateAround(Vector3.up, rotateHorizontal * rotationSpeed * Time.deltaTime);
            transform.RotateAround(transform.right, rotateVertical * rotationSpeed * Time.deltaTime);
            transform.RotateAround(transform.forward, tilt * rotationSpeed * Time.deltaTime);

            // in case you move main, also move controllers (via base controller parent)
            if (whatToControl == 0)
            {
                transform = baseController.transform;
                transform.Translate(new Vector3(xMove, yMove, zMove) * moveSpeed * Time.deltaTime);
                transform.RotateAround(Vector3.up, rotateHorizontal * rotationSpeed * Time.deltaTime);
                transform.RotateAround(transform.right, rotateVertical * rotationSpeed * Time.deltaTime);
                transform.RotateAround(transform.forward, tilt * rotationSpeed * Time.deltaTime);
            }
        }
    }
}