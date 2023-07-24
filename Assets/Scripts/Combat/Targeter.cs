using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour {
    [SerializeField] private Targetable target;

    public Targetable getTarget() {
        return target;
    }

    public override void OnStartServer() {
    }

    public override void OnStopServer() {
    }

    [Command]
    public void cmdSetTarget(GameObject targetGameObject) {
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable newTarget)) {
            return;
        }

        target = newTarget;
    }

    [Server]
    public void clearTarget() {
        target = null;
    }

    [Server]
    private void serverHandleGameOver() {
        clearTarget();
    }
}