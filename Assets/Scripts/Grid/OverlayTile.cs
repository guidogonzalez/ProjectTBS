using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class OverlayTile : NetworkBehaviour {
    public int G;
    public int H;

    public int F {
        get { return G + H; }
    }

    [SyncVar]
    public OverlayTile Previous;
    public Vector3Int gridLocation;

    public Vector2Int grid2DLocation {
        get { return new Vector2Int(gridLocation.x, gridLocation.y); }
    }

    public List<Sprite> arrows;

    [SyncVar]
    public Unit currentUnit;
    
    [SyncVar]
    public bool isBlocked = false;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            HideTile();
        }
    }

    public void HideTile() {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }

    public void ShowTile() {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    [Server]
    private void setCurrentUnit(Unit unit) {
        currentUnit = unit;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetCurrentUnit(Unit unit) {
        setCurrentUnit(unit);
    }

    [Server]
    private void setTileIsBlocked(bool isBlocked) {
        this.isBlocked = isBlocked;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetTileIsBlocked(bool isBlocked) {
        setTileIsBlocked(isBlocked);
    }
}