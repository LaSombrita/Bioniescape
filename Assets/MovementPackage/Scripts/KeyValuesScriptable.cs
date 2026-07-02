using UnityEngine;

[CreateAssetMenu(fileName = "New Key Value", menuName = "ScriptableObjects/KeyTemplate")]
public class KeyValuesScriptable : ScriptableObject
{
    public string KeyStringRegional;
    public string KeyStringRaw;
    public int KeyCoordinateValue;
    public bool RailChangeL;
    public bool RailChangeR;
}
