using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour {
    //public GameObject cursor;
    [SerializeField] public Unit selectedUnit;
    [SerializeField] private LayerMask layerMask;

    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private List<OverlayTile> path;
    private List<OverlayTile> rangeFinderTiles;

    private void Start() {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();

        path = new List<OverlayTile>();
        rangeFinderTiles = new List<OverlayTile>();
    }

    void LateUpdate() {
        RaycastHit2D? hit = GetFocusedOnTile();

        if (!hit.HasValue) {
            return;
        }

        OverlayTile tile = hit.Value.collider.gameObject.GetComponent<OverlayTile>();

        if (rangeFinderTiles.Contains(tile) && selectedUnit != null &&
            !selectedUnit.isMoving) {
            path = pathFinder.FindPath(selectedUnit.standingOnTile, tile, rangeFinderTiles);
        }

        if (Mouse.current.leftButton.wasPressedThisFrame) {
            tile.ShowTile();

            // There's a unit on the Tile we pressed and we don't have a unit selected 
            if (tile.currentUnit != null && tile.currentUnit.isOwned) {
                selectedUnit = tile.currentUnit;
                GetInRangeTiles();
            }

            // Tile we press doesn't have any unit and it's not blocked
            // we also have a unit selected
            // it's not the same tile we're currently in with the selected unit
            // and the unit is not moving
            if (tile.currentUnit == null && !tile.isBlocked && selectedUnit != null &&
                selectedUnit.standingOnTile != tile && !selectedUnit.isMoving) {
                selectedUnit.CmdSetUnitMovingState(false);
                tile.HideTile();
            }
        }

        if (path.Count > 0 && selectedUnit.isMoving) {
            MoveAlongPathServer();
        }
    }

    private void MoveAlongPathServer() {
        var step = selectedUnit.unitInfo.movementSpeed * Time.deltaTime;

        float zIndex = path[0].transform.position.z;

        selectedUnit.unitMovement.CmdUnitMoveTowards(path[0].transform, step, zIndex);

        if (Vector2.Distance(selectedUnit.transform.position, path[0].transform.position) < 0.00001f) {
            selectedUnit.unitMovement.CmdPositionCharacterOnLine(path[0].grid2DLocation);
            path.RemoveAt(0);
        }

        if (path.Count == 0) {
            GetInRangeTiles();
            selectedUnit.CmdSetUnitMovingState(false);
        }
    }

    private void MoveAlongPath() {
        var step = selectedUnit.unitInfo.movementSpeed * Time.deltaTime;

        float zIndex = path[0].transform.position.z;
        selectedUnit.transform.position =
            Vector2.MoveTowards(selectedUnit.transform.position, path[0].transform.position, step);
        selectedUnit.transform.position =
            new Vector3(selectedUnit.transform.position.x, selectedUnit.transform.position.y, zIndex);

        if (Vector2.Distance(selectedUnit.transform.position, path[0].transform.position) < 0.00001f) {
            PositionCharacterOnLine(path[0]);
            path.RemoveAt(0);
        }

        if (path.Count == 0) {
            GetInRangeTiles();
            selectedUnit.isMoving = false;
        }
    }

    private void PositionCharacterOnLine(OverlayTile tile) {
        selectedUnit.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f,
            tile.transform.position.z);
        selectedUnit.standingOnTile = tile;

        tile.isBlocked = true;
        tile.Previous.isBlocked = false;
        tile.Previous.currentUnit = null;
    }

    private RaycastHit2D? GetFocusedOnTile() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero, layerMask);

        if (hits.Length > 0) {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }

    private void GetInRangeTiles() {
        rangeFinderTiles = rangeFinder.GetTilesInRange(
            new Vector2Int(selectedUnit.standingOnTile.gridLocation.x,
                selectedUnit.standingOnTile.gridLocation.y), 2);

        foreach (var item in rangeFinderTiles) {
            if (!item.isBlocked) {
                item.ShowTile();
            }
            else {
                item.HideTile();
            }
        }
    }
}