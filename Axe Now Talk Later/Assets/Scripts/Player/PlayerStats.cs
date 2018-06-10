using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CREATED BY SONNY
/// Stores all values that are editable for tuning purposes.
/// PlayerMovement Inherits from this class.
/// </summary>
public class PlayerStats : MonoBehaviour {

    public float armorAmount;
    public float attackDamage;
    public float jumpForce;
    public float moveSpeed;
    public float dashForce;

    public float m_FallMultiplier;
    public float m_LowJumpMultiplier;

    public void InitialValues()
    {
        attackDamage = 25;
        armorAmount = 100;
        moveSpeed = 1.5f;
        jumpForce = 1500;
        dashForce = 10;
        m_FallMultiplier = 6.25f;
        m_LowJumpMultiplier = 1.5f;
    }
}
