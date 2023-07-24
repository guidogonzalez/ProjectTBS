using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour {
    [SerializeField] public UnitInfo unitInfo;
    [SerializeField] private Health health;
    [SerializeField] public UnitMovement unitMovement;
    [SerializeField] public Targeter targeter;
    
    [SerializeField] public OverlayTile standingOnTile;

    [SerializeField] private UnityEvent onSelected;
    [SerializeField] private UnityEvent onDeselected;

    [SyncVar] public bool isMoving = false;

    public static event Action<Unit> serverOnUnitSpawned;
    public static event Action<Unit> serverOnUnitDespawned;

    public static event Action<Unit> authorityOnUnitSpawned;
    public static event Action<Unit> authorityOnUnitDespawned;

    #region Server

    public override void OnStartServer() {
        serverOnUnitSpawned?.Invoke(this);

        health.setMaxHealth(unitInfo.health);
        health.serverOnDie += serverHandleDie;
    }

    public override void OnStopServer() {
        health.serverOnDie -= serverHandleDie;

        serverOnUnitDespawned?.Invoke(this);
    }

    [Server]
    private void serverHandleDie() {
        NetworkServer.Destroy(gameObject);
    }
    
    [Server]
    private void setUnitMovingState(bool isMoving) {
        this.isMoving = isMoving;
    }

    [Command]
    public void CmdSetUnitMovingState(bool isMoving) {
        setUnitMovingState(isMoving);
    }

    #endregion

    #region Client

    public override void OnStartAuthority() {
        authorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient() {
        if (!isOwned) {
            return;
        }

        authorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void selectUnit() {
        if (!isOwned) {
            return;
        }

        onSelected?.Invoke();
    }

    [Client]
    public void deselectUnit() {
        if (!isOwned) {
            return;
        }

        onDeselected?.Invoke();
    }

    #endregion
}