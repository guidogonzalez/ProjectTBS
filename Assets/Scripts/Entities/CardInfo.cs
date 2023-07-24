using UnityEngine;

[CreateAssetMenu(fileName = "Create Card", menuName = "ScriptableObjects/Card")]
public class CardInfo : ScriptableObject {
    public int cardId;
    public int cardCost;
    public AbilityInfo abilityInfo;

    public virtual void useCard(UnitInfo targetUnitInfo) {
        
    }
}