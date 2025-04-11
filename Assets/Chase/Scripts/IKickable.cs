using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKickable
{
    public abstract bool OnKicked(Player player);
    public abstract void Dusted(Player player);
}


//Kicking sandcastles
//Fallingdown
//Dying
//Running/ Walking
//Splashing
//Skid