using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class AddRebuildFunctionality {
 
   // Add a menu item named "Do Something with a Shortcut Key" to MyMenu in the menu bar
// and give it a shortcut (ctrl-g on Windows, cmd-g on macOS).
[MenuItem("IMMATERIA/Rebuid Scene %b")]
static void RebuildScene()
{
  GameObject.Find("God").GetComponent<God>().Rebuild();
}

 
   // Add a menu item named "Do Something with a Shortcut Key" to MyMenu in the menu bar
// and give it a shortcut (ctrl-g on Windows, cmd-g on macOS).
[MenuItem("IMMATERIA/Fake Swipe Right %w")]
static void FakeSwipeLeft()
{
//  UnityEngine.Debug.Log("huhh");
  GameObject.Find("Input").GetComponent<InputEvents>().fakeSwipeRight = true;
}


   // Add a menu item named "Do Something with a Shortcut Key" to MyMenu in the menu bar
// and give it a shortcut (ctrl-g on Windows, cmd-g on macOS).
[MenuItem("IMMATERIA/Fake Swipe Left %e")]
static void FakeSwipeRight()
{
  GameObject.Find("Input").GetComponent<InputEvents>().fakeSwipeLeft = true;
}





}
