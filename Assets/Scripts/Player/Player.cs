using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour {
    [SerializeField] public int playerDrawIndex = 0;
    [SerializeField] public int playerActionPoints;

    [SerializeField] public Deck playerDeck;
    [SerializeField] public List<Card> playerCards;
    [SerializeField] public List<Card> playerHand;
    [SerializeField] public List<Unit> playerUnits;

    #region Server
    
    public override void OnStartServer() {
        Unit.serverOnUnitSpawned += serverHandleUnitSpawned;
        Unit.serverOnUnitDespawned += serverHandleUnitDespawned;

        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopServer() {
        Unit.serverOnUnitSpawned -= serverHandleUnitSpawned;
        Unit.serverOnUnitDespawned -= serverHandleUnitDespawned;
    }

    private void serverHandleUnitSpawned(Unit unit) {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) {
            return;
        }

        playerUnits.Add(unit);
    }

    private void serverHandleUnitDespawned(Unit unit) {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) {
            return;
        }

        playerUnits.Remove(unit);
    }

    #endregion

    #region Client

    public override void OnStartClient() {
        if (NetworkServer.active) {
            return;
        }

        DontDestroyOnLoad(gameObject);

        ((NetworkManagerHandler)NetworkManager.singleton).players.Add(this);
    }

    public override void OnStartAuthority() {
        if (NetworkServer.active) {
            return;
        }

        Unit.authorityOnUnitSpawned += authorityHandleUnitSpawned;
        Unit.authorityOnUnitDespawned += authorityHandleUnitDespawned;
    }

    public override void OnStopClient() {
        if (!isClientOnly) {
            return;
        }

        ((NetworkManagerHandler)NetworkManager.singleton).players.Remove(this);

        if (!isOwned) {
            return;
        }

        Unit.authorityOnUnitSpawned -= authorityHandleUnitSpawned;
        Unit.authorityOnUnitDespawned -= authorityHandleUnitDespawned;
    }

    private void authorityHandleUnitSpawned(Unit unit) {
        playerUnits.Add(unit);
    }

    private void authorityHandleUnitDespawned(Unit unit) {
        playerUnits.Remove(unit);
    }

    #endregion
}