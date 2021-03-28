using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTypes 
{



[System.Serializable]
public class Vector2Event : UnityEvent<Vector2>{}

[System.Serializable]
public class Vector3Event : UnityEvent<Vector3>{}

[System.Serializable]
public class Vector2FloatEvent : UnityEvent<Vector2,float>{}

[System.Serializable]
public class FloatEvent : UnityEvent<float>{}


[System.Serializable]
public class RayEvent : UnityEvent<Ray>{}

[System.Serializable]
public class BaseEvent : UnityEvent{}



}
