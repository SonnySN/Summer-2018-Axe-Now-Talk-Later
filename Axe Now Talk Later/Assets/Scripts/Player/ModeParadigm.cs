using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// CREATED BY SONNY
/// 
/// </summary>

public class ModeParadigm : MonoBehaviour {

    public enum Paradigm { AttackMode, DefenceMode, NeutralMode };
    
    public class NewMode
    {
        //Everything that a Mode will contain
        public Paradigm paradigm;
        public float armorMultiplier;
        public float attackMultiplier;
        public float moveSpeedMultipler;
        public float jumpMultiplier;

        //Making a Mode
        public NewMode(Paradigm _paradigm, float _attackMultiplier, float _armorMultiplier, float _speedMultiplier, float _jumpMultiplier)
        {
            paradigm = _paradigm;
            armorMultiplier = _armorMultiplier;
            attackMultiplier = _attackMultiplier;
            moveSpeedMultipler = _speedMultiplier;
            jumpMultiplier = _jumpMultiplier;
        }
    }
    //Premade Modes the numbers you see are percentages
    public static NewMode AttackMode = new NewMode(Paradigm.AttackMode, 1.40f, .40f, 1.66f, 1.66f);
    public static NewMode NeutralMode = new NewMode(Paradigm.NeutralMode, 1, 1, 1, 1);
    public static NewMode DefenceMode = new NewMode(Paradigm.DefenceMode, .40f, 1.40f, .5f, .5f);

    public static NewMode currentMode;

    public Text currentModeName;

    // Alter stats to the appropriate Paradigm, keep in mind to also change transition values IE. Changing modes while sprinting. 
    public void Change(NewMode newMode, PlayerMovement stats)
    {
        currentMode = newMode;

        currentModeName.text = currentMode.paradigm.ToString();
        if (currentMode == AttackMode)
            currentModeName.color = Color.red;
        else if (currentMode == NeutralMode)
            currentModeName.color = Color.green;
        else if (currentMode == DefenceMode)
            currentModeName.color = Color.blue;

        // NeutralMod/Beginning values needed to appropriately scale values. 
        stats.InitialValues();

        stats.armorAmount *= newMode.armorMultiplier;
        stats.attackDamage *= newMode.attackMultiplier;
        stats.jumpForce *= newMode.jumpMultiplier;
        stats.moveSpeed *= newMode.moveSpeedMultipler;
    }

}
