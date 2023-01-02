using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/PlayerStats", order = 1)]
public class PlayerStats : ScriptableObject
{
    public float jumpHeight;
    public float jumpForgivenessTime;
    public float timeToJumpApex;
    public int maxJumpCount;
    public float antiGravityApexVelocity;
    public float antiGravityApexAmount;

    public float moveSpeed;
    public float accelerationTimeAirborne;
    public float accelerationTimeGrounded;

    public bool showDebug;
}
