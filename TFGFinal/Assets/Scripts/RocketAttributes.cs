using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RocketAttributes
{
    public float thrustForce;
    public float scaleForceUser;
    public float groundRayDistance;
    public float PlanetGravity;
    public float windForce;
    public float propulsorActivationHeight;
    public float maxVelocityforLanding;
    public float windMinTime;
    public float windMaxTime;
    public float parachuteThrustMultiplier;
    public float parachuteGravityReduction;
    public float massPercentageDecrease;
    public float fuelConsumptionAmount;
    public float fuelAmount;
}

[System.Serializable]
public class RocketMass
{
    public float VikingLander;
    public float LunarModule;
    public float RandomShip;
}