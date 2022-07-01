using UnityEngine;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour {
	
	[SerializeField] Transform ground = default;

    [SerializeField] GameTile tilePrefab = default;
	
	Vector2Int size;

    GameTile[] tiles;  // using array instead of list beacuse we wont need to change the board

	Queue<GameTile> searchFrontier = new Queue<GameTile>();

	public void Initialize (Vector2Int size) {
		this.size = size;
		ground.localScale = new Vector3(size.x, size.y, 1f);

		Vector2 offset = new Vector2(
			(size.x - 1) * 0.5f, (size.y - 1) * 0.5f
		);

		tiles = new GameTile[size.x * size.y];

		for (int i = 0, y = 0; y < size.y; y++) {
			for (int x = 0; x < size.x; x++, i++) {
				GameTile tile = tiles[i] = Instantiate(tilePrefab);

				tile.transform.SetParent(transform, false);
				tile.transform.localPosition = new Vector3(
					x - offset.x, 0f, y - offset.y
				);

				if (x > 0) {
					GameTile.MakeEastWestNeighbors(tile, tiles[i - 1]);
				}
				if (y > 0) {
					GameTile.MakeNorthSouthNeighbors(tile, tiles[i - size.x]);
				}
			}
		}

		FindPaths();
	}

	void FindPaths () {
		// Clear the path of all tiles
		foreach (GameTile tile in tiles)
		{
			tile.ClearPath();
		}

		// Make one tile destination and add it to the frontier
		//tiles[0].BecomeDestination();
		//searchFrontier.Enqueue(tiles[0]);

		tiles[tiles.Length / 2].BecomeDestination();
		searchFrontier.Enqueue(tiles[tiles.Length / 2]);

		// Take the single tile out of the frontier and grow the path to its neighbors, adding them all to the frontier
		while (searchFrontier.Count > 0) {  // Repeat as long as there a tiles
			GameTile tile = searchFrontier.Dequeue();
			if (tile != null) {
				searchFrontier.Enqueue(tile.GrowPathNorth());
				searchFrontier.Enqueue(tile.GrowPathEast());
				searchFrontier.Enqueue(tile.GrowPathSouth());
				searchFrontier.Enqueue(tile.GrowPathWest());
			}
		}
		
		foreach (GameTile tile in tiles) {
			tile.ShowPath();
		}


	}

	
}