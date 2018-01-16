﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using ZenFulcrum.EmbeddedBrowser;

class ViveBrowserUI : MonoBehaviour, IBrowserUI
{
    private bool click = false;
    private bool justclicked = false; // for the next update after the click
    private Vector2 clickCoord = Vector2.zero;
    public int intersectingLeapBones = 0;
    public float maxraycast;
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("LeapHands"))
        {
            Debug.Log("Hooray!");
            if (intersectingLeapBones == 0)
            {
                RaycastHit hit = new RaycastHit();
                int mask = 1 << gameObject.layer; // the goal is to fire a raycast at this object
                Transform otherObject = col.gameObject.transform;

                // get the point of impact by averaging all contact points
                Vector3 impactPoint = Vector3.zero;
                foreach (ContactPoint cpt in col.contacts)
                {
                    Debug.Log("Impact Point " + cpt.point);
                    impactPoint += cpt.point;
                }
                impactPoint /= col.contacts.Length;

                Vector3 rayDirection = impactPoint - otherObject.position;
                Ray ray = new Ray(otherObject.position, rayDirection);
                if (Physics.Raycast(ray, out hit, maxraycast, mask)) // && hit.collider.gameObject.Equals(gameObject))
                {
                    clickCoord = hit.textureCoord;
                    Debug.Log("Texture Coord: " + hit.textureCoord);
                    click = true;
                }
                else
                {
                    Debug.Log("Failed Raycast from " + otherObject.position + " To " + impactPoint);
                    GameObject a = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    GameObject b = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    a.transform.localScale = new Vector3(.01f, .01f, .01f);
                    b.transform.localScale = new Vector3(.01f, .01f, .01f);
                    a.transform.position = impactPoint;
                    b.transform.position = otherObject.position;
                }
            }
                
            intersectingLeapBones += 1;
        }
    }
    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("LeapHands"))
        {
            intersectingLeapBones -= 1;
        }
    }

    private Browser browser;

    private CursorInput input;

    void Awake()
    {
        // tell the browser that this object controls the UI
        browser = GetComponent<Browser>();
        browser.UIHandler = this;

        inputSettings = new BrowserInputSettings(); //TODO:  set special options?

        browserCursor = new BrowserCursor(); // do we care about anything special?
        browserCursor.SetActiveCursor(BrowserNative.CursorType.Pointer);
        //browserCursor.cursorChange += SetCursor;
        
        keyEvents = new List<Event>(); // TODO: POPULATE?
    }
    
    /** Called once per frame by the browser before fetching properties. */
    public void InputUpdate()
    {
        Debug.Log("InputUpdate");
        // create new cursorinput object:
        input = new CursorInput();
        input.LeftClick = click;
        if (click)
        {
            input.MouseHasFocus = true;
            input.MousePosition = clickCoord;
        }
        else if (justclicked)
        {
            input.MouseHasFocus = true;
            input.MousePosition = clickCoord;
        }
        // clear any state variables:
        if (click) justclicked = true;
        else justclicked = false;
        click = false;

        //input = //ControllerInputManager.RequestInputStatus(gameObject);

        //// extra functionality that I packed in the struct, but that the browser doesn't read
        //if (input.BackPage)
        //{
        //    browser.GoBack();
        //}
        //if (input.ForwardPage)
        //{
        //    browser.GoForward();
        //}
    }

    /**
	 * Returns true if the browser will be getting mouse events. Typically this is true when the mouse if over the browser.
	 * 
	 * If this is false, the Mouse* properties will be ignored.
	 */
    public bool MouseHasFocus { get
        {
            return input.MouseHasFocus;
        }
    }

    /**
	 * Current mouse position.
	 * 
	 * Returns the current position of the mouse with (0, 0) in the bottom-left corner and (1, 1) in the 
	 * top-right corner.
	 */
    public Vector2 MousePosition { get
        {
            return input.MousePosition;
        }
    }

    /** Bitmask of currently depressed mouse buttons */
    public MouseButton MouseButtons {
        get
        {
            MouseButton buttons = 0;
            if (input.LeftClick)
            {
                buttons = buttons | MouseButton.Left;
            }
            if (input.RightClick) {
                buttons = buttons | MouseButton.Right;
            }
            return buttons;
        }
    }

    /**
	 * Delta X and Y scroll values since the last time InputUpdate() was called.
	 * 
	 * Return 1 for every "click" of the scroll wheel.
	 * 
	 * Return only integers.
	 */
     //TODO: set
    private Vector2 mouseScroll;
    public Vector2 MouseScroll { get { return input.MouseScroll; } }

    /**
	 * Returns true when the browser will receive keyboard events.
	 * 
	 * In the simplest case, return the same value as MouseHasFocus, but you can track focus yourself if desired.
	 * 
	 * If this is false, the Key* properties will be ignored.
	 */
    public bool KeyboardHasFocus { get { return true; } }

    /**
	 * List of key up/down events that have happened since the last InputUpdate() call.
	 * 
	 * The returned list is not to be altered or retained.
	 */
     //TODO: set
    private List<Event> keyEvents;
    public List<Event> KeyEvents { get { return KeyboardInputManager.FlushEvents(); } }

    /**
	 * Returns a BrowserCursor instance. The Browser will update the current cursor to reflect the
	 * mouse's position on the page.
	 * 
	 * The IBrowserUI is responsible for changing the actual cursor, be it the mouse cursor or some in-game display.
	 */
    private BrowserCursor browserCursor;
    public BrowserCursor BrowserCursor { get { return browserCursor; } }

    /**
	 * These settings are used to interpret the input data.
	 */
    private BrowserInputSettings inputSettings;
    public BrowserInputSettings InputSettings { get { return inputSettings; } }
    

}