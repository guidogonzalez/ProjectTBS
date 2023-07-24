using Mirror;
using UnityEngine;

public class UnitMovement : NetworkBehaviour {
    [Command]
    public void CmdUnitMoveTowards(Transform pathTransform, float step, float zIndex) {
        unitMoveTowards(pathTransform, step, zIndex);
    }

    [Command]
    public void CmdPositionCharacterOnLine(Vector2Int tilePosition) {
        PositionCharacterOnLine(tilePosition);
    }

    [Server]
    public void unitMoveTowards(Transform pathTransform, float step, float zIndex) {
        gameObject.transform.position =
            Vector2.MoveTowards(gameObject.transform.position, pathTransform.transform.position, step);
        gameObject.transform.position =
            new Vector3(gameObject.transform.position.x,
                gameObject.transform.position.y, zIndex);
    }

    [Server]
    public void PositionCharacterOnLine(Vector2Int gridLocation) {
        OverlayTile tile = ((NetworkManagerHandler)NetworkManager.singleton).map[gridLocation];

        gameObject.transform.position = new Vector3(tile.transform.position.x,
            tile.transform.position.y + 0.0001f,
            tile.transform.position.z);

        gameObject.GetComponent<Unit>().standingOnTile = tile;

        tile.currentUnit = gameObject.GetComponent<Unit>();
        tile.CmdSetTileIsBlocked(true);
        tile.Previous.CmdSetTileIsBlocked(false);
        tile.Previous.CmdSetCurrentUnit(null);
    }
}