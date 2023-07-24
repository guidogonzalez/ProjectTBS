using UnityEngine;

[CreateAssetMenu(fileName = "Create Unit", menuName = "ScriptableObjects/Unit")]
public class UnitInfo : ScriptableObject {
    public int unitId;
    public string unitName;
    public int attackDamage;
    public int health;
    public int movementRange;
    public int movementSpeed;
    public int attackRange;
    public string description;
    public AbilityInfo abilityInfo;
    public bool isHero;
    public GameObject unitPrefab;
}