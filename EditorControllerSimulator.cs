using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;


public class EditorControllerSimulator : MonoBehaviour
{
#if UNITY_EDITOR

    private XRController m_xrController;
    private float m_distance = 0;

    private Gamepad gamepad;

    private Type GetNestedType(object obj, string typeName)
    {
        foreach (var type in m_xrController.GetType().GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public))
        {
            if (type.Name == typeName)
            {
                return type;
            }
        }
        return null;
    }

    private Dictionary<string, object> GetEnumValues(Type enumType)
    {
        Debug.Assert(enumType.IsEnum == true);
        Dictionary<string, object> enumValues = new Dictionary<string, object>();
        foreach (object value in Enum.GetValues(enumType))
        {
            enumValues[Enum.GetName(enumType, value)] = value;
        }
        return enumValues;
    }

    void Start()
    {
        // ToDo: Check if the ControllerControl script is attached and correctly configured OR add them automatically.
        gamepad = Gamepad.current;
    }


    private bool GetButtonPressed(InputHelpers.Button button)
    {
        switch (button)
        {
            case InputHelpers.Button.Trigger:
                // return Input.GetKey(triggerKey); 
                return gamepad.xButton.wasPressedThisFrame;
            case InputHelpers.Button.Grip:
                // return Input.GetKey(gripKey);
                return gamepad.yButton.wasPressedThisFrame;
            default:
                return false;
        }
    }

    private void LateUpdate()
    {

        // select with button 'a' on gamepad
        {
            // Update interaction state
            //bool state = Input.GetKey(inputKey); 
            Type interactionTypes = GetNestedType(m_xrController, "InteractionTypes");
            Dictionary<string, object> interactionTypesEnum = GetEnumValues(interactionTypes);
            MethodInfo updateInteractionType = m_xrController.GetType().GetMethod("UpdateInteractionType", BindingFlags.NonPublic | BindingFlags.Instance);
            updateInteractionType.Invoke(m_xrController, new object[] { interactionTypesEnum["select"], (object) gamepad.aButton.wasPressedThisFrame});
        }

        // activate with button 'b' on gamepad
        {
            // Update interaction state
            //bool state = Input.GetKey(inputKey); 
            Type interactionTypes = GetNestedType(m_xrController, "InteractionTypes");
            Dictionary<string, object> interactionTypesEnum = GetEnumValues(interactionTypes);
            MethodInfo updateInteractionType = m_xrController.GetType().GetMethod("UpdateInteractionType", BindingFlags.NonPublic | BindingFlags.Instance);
            updateInteractionType.Invoke(m_xrController, new object[] { interactionTypesEnum["activate"], (object)gamepad.bButton.wasPressedThisFrame });
        }
    }

    private void Awake()
    {
        m_xrController = GetComponent<XRController>();
        if (m_xrController == null)
        {
            m_xrController = FindObjectOfType<XRController>();
        }
    }
#endif
}
