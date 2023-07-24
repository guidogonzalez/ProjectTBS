using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour {
    public enum Faction {
        OrderOfTheShield,
        ShadowSyndicate,
        NaturesEmbrace,
        TechnoUnion,
        ArcaneOrder
    }

    public Faction selectedFaction;
    public List<UnitInfo> deckUnits;
    public List<CardInfo> deckCards;

    public void shuffleDeck() {
        for (int i = 0; i < deckCards.Count; i++) {
            int randomIndex = Random.Range(i, deckCards.Count);
            CardInfo temp = deckCards[i];
            deckCards[i] = deckCards[randomIndex];
            deckCards[randomIndex] = temp;
        }
    }

    public CardInfo drawCard(int cardPosition) {
        return deckCards[cardPosition];
    }
}