using UnityEngine;

[CreateAssetMenu(fileName = "Create Ability", menuName = "ScriptableObjects/Ability")]
public class AbilityInfo : ScriptableObject {
    public int abilityId;
    public string abilityName;
    public int range;
    public int damage;
    public int cost;
    public bool singleTarget;
    public bool isForAllies;
    public bool isForEnemies;
    public string description;
    public GameObject abilityPrefab;

    public virtual void useAbility(UnitInfo targetUnitInfo) {
        
    }
}