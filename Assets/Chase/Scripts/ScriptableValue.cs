using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ScriptableValue", menuName = "StaticValues/ScriptableValue", order = 0)]
public class ScriptableValue : ScriptableObject {
    public int value;
    public void add(int number) {
        value += number;
    }
}