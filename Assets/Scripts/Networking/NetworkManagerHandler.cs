using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NetworkManagerHandler : NetworkManager {
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public List<Player> players = new List<Player>();

    public GameObject overlayPrefab;
    public GameObject overlayContainer;

    public Dictionary<Vector2Int, OverlayTile> map;

    [SerializeField] private List<Vector2Int> player1SpawnPoints;
    [SerializeField] private List<Vector2Int> player2SpawnPoints;

    #region Server

    public override void OnServerConnect(NetworkConnectionToClient conn) {
        //conn.Disconnect();
    }


    public override void OnServerDisconnect(NetworkConnectionToClient conn) {
        Player player = conn.identity.GetComponent<Player>();

        players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer() {
        players.Clear();
    }

    public void StartGame() {
        if (players.Count < 2) {
            return;
        }

        //ServerChangeScene("Game");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        base.OnServerAddPlayer(conn);

        Player player = conn.identity.GetComponent<Player>();

        players.Add(player);

        // Since there's no lobby scene, I'm instantiating the players and units as a workaround at the moment
        if (players.Count == 1) {
            var tileMaps = gameObject.transform.GetComponentsInChildren<Tilemap>()
                                     .OrderByDescending(x => x.GetComponent<TilemapRenderer>().sortingOrder);
            map = new Dictionary<Vector2Int, OverlayTile>();

            foreach (var tm in tileMaps) {
                BoundsInt bounds = tm.cellBounds;

                for (int z = bounds.max.z; z >= bounds.min.z; z--) {
                    for (int y = bounds.min.y; y < bounds.max.y; y++) {
                        for (int x = bounds.min.x; x < bounds.max.x; x++) {
                            if (tm.HasTile(new Vector3Int(x, y, z))) {
                                if (!map.ContainsKey(new Vector2Int(x, y))) {
                                    var overlayTile = Instantiate(overlayPrefab, overlayContainer.transform);
                                    var cellWorldPosition = tm.GetCellCenterWorld(new Vector3Int(x, y, z));
                                    overlayTile.transform.position = new Vector3(cellWorldPosition.x,
                                        cellWorldPosition.y,
                                        cellWorldPosition.z + 1);

                                    overlayTile.gameObject.GetComponent<OverlayTile>().gridLocation =
                                        new Vector3Int(x, y, z);

                                    map.Add(new Vector2Int(x, y), overlayTile.gameObject.GetComponent<OverlayTile>());

                                    NetworkServer.Spawn(overlayTile.gameObject);
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < player1SpawnPoints.Count; i++) {
                GameObject unitInstance = Instantiate(
                    player.playerDeck.deckUnits[i].unitPrefab,
                    map[new Vector2Int(player1SpawnPoints[i].x, player1SpawnPoints[i].y)].transform.position,
                    Quaternion.identity);

                map[new Vector2Int(player1SpawnPoints[i].x, player1SpawnPoints[i].y)].currentUnit =
                    unitInstance.GetComponent<Unit>();

                unitInstance.GetComponent<Unit>().standingOnTile =
                    map[new Vector2Int(player1SpawnPoints[i].x, player1SpawnPoints[i].y)];

                NetworkServer.Spawn(unitInstance, player.connectionToClient);
            }
        }
        else if (players.Count == 2) {
            for (int i = 0; i < player2SpawnPoints.Count; i++) {
                GameObject unitInstance = Instantiate(
                    player.playerDeck.deckUnits[i].unitPrefab,
                    map[new Vector2Int(player2SpawnPoints[i].x, player2SpawnPoints[i].y)].transform.position,
                    Quaternion.identity);

                map[new Vector2Int(player2SpawnPoints[i].x, player2SpawnPoints[i].y)].currentUnit =
                    unitInstance.GetComponent<Unit>();

                unitInstance.GetComponent<Unit>().standingOnTile =
                    map[new Vector2Int(player2SpawnPoints[i].x, player2SpawnPoints[i].y)];

                NetworkServer.Spawn(unitInstance, player.connectionToClient);
            }
        }
        /*foreach (UnitInfo unitInfo in player.playerDeck.deckUnits) {
            GameObject unitInstance = Instantiate(
                unitInfo.unitPrefab,
                GetStartPosition().position,
                Quaternion.identity);
    
            NetworkServer.Spawn(unitInstance, player.connectionToClient);
        }*/
    }

    public override void OnServerSceneChanged(string sceneName) { }

    #endregion

    #region Client

    public override void OnClientConnect() {
        base.OnClientConnect();
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect() {
        base.OnClientDisconnect();
        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient() {
        players.Clear();
    }

    public List<OverlayTile> GetSurroundingTiles(Vector2Int originTile) {
        var surroundingTiles = new List<OverlayTile>();


        Vector2Int TileToCheck = new Vector2Int(originTile.x + 1, originTile.y);
        if (map.ContainsKey(TileToCheck)) {
            if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1)
                surroundingTiles.Add(map[TileToCheck]);
        }

        TileToCheck = new Vector2Int(originTile.x - 1, originTile.y);
        if (map.ContainsKey(TileToCheck)) {
            if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1)
                surroundingTiles.Add(map[TileToCheck]);
        }

        TileToCheck = new Vector2Int(originTile.x, originTile.y + 1);
        if (map.ContainsKey(TileToCheck)) {
            if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1)
                surroundingTiles.Add(map[TileToCheck]);
        }

        TileToCheck = new Vector2Int(originTile.x, originTile.y - 1);
        if (map.ContainsKey(TileToCheck)) {
            if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1)
                surroundingTiles.Add(map[TileToCheck]);
        }

        return surroundingTiles;
    }

    #endregion
}