using System;
using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour {
    [SerializeField] private int maxHealth;

    [SyncVar(hook = nameof(handleHealthUpdated))]
    private int currentHealth;

    public event Action serverOnDie;

    public event Action<int, int> clientOnHealthUpdated;

    public void setMaxHealth(int quantity) {
        maxHealth = quantity;
    }

    #region Server

    public override void OnStartServer() {
        currentHealth = maxHealth;

        //UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
    }

    public override void OnStopServer() {
        //UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }

    [Server]
    private void serverHandlePlayerDie(int connectionId) {
        if (connectionToClient.connectionId != connectionId) {
            return;
        }

        dealDamage(currentHealth);
    }

    [Command]
    public void CmdDealDamage(int damageAmount, GameObject unit) {
        unit.GetComponent<Health>().dealDamage(damageAmount);
    }

    [Server]
    public void dealDamage(int damageAmount) {
        if (currentHealth == 0) {
            return;
        }

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        if (currentHealth != 0) {
            return;
        }

        serverOnDie?.Invoke();
    }

    #endregion

    #region Client

    private void handleHealthUpdated(int oldHealth, int newHealth) {
        clientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }

    #endregion
}