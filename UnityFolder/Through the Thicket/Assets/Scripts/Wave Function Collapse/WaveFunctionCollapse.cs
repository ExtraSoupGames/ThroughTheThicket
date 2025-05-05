using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

public class WaveFunctionCollapse : MonoBehaviour
{
    //Up Left Down Right
    private static int2[] NeighbourOffsets()
    {
        return new int2[] { new int2(0, -1), new int2(-1, 0), new int2(0, 1), new int2(1,0)};
    }
    public static void GenerateDungeon(int ID, int size, string filePath, int seed, Color[,] inputImagePixels)
    {
        //Initialize a random number engine for collapsing tiles
        Unity.Mathematics.Random randomEngine = new Unity.Mathematics.Random((uint)seed);
        // the dungeon will be a square with an odd number of tiles width and height, the size parameter represents the apothem
        int sideLength = size * 2 + 1;
        //instantiate a grid of tiles with the specified size
        WFCTile[,] tiles = new WFCTile[sideLength, sideLength];
        //instantiate a grid of floats to represent the entropy of each tile
        int[,] entropies = new int[sideLength, sideLength];
        //Populate the tiles grid with empty tiles and set all entropies to 1
        for (int i = 0; i < sideLength; i++)
        {
            for (int j = 0; j < sideLength; j++)
            {
                //Create a tile with all possibilities
                tiles[i, j] = new WFCTile(i, j);
                //Update the entropy grid accordingly
                entropies[i, j] = tiles[i, j].CalculateEntropy();
            }
        }
        WFCRuleSet rules = new WFCRuleSet(inputImagePixels);

        //set the entrance and exits TODO make these locations random (or atleast the exit)
        int2 entrancePos = new int2(0, 0);
        int2 exitPos = new int2(size, size);
        tiles[entrancePos.x, entrancePos.y] = new WFCTile(WFCTileType.Entrance, entrancePos.x, entrancePos.y);
        //sets a tile a little away to an exit
        tiles[exitPos.x, exitPos.y] = new WFCTile(WFCTileType.Exit, exitPos.x, exitPos.y);
        UpdateAllPossibilities(ref tiles, new int2(entrancePos.x, entrancePos.y), rules);
        UpdateAllPossibilities(ref tiles, new int2(exitPos.x, exitPos.y), rules);
        //Update the entropy grid accordingly
        entropies[entrancePos.x, entrancePos.y] = tiles[entrancePos.x, entrancePos.y].CalculateEntropy();
        entropies[exitPos.x, exitPos.y] = tiles[exitPos.x, exitPos.y].CalculateEntropy();

        while (!AllTilesCollapsed(tiles))
        {
            //Update all entropies and collapse the tile with the smallest entropy
            int2 lowestEntropyPosition = UpdateAllEntropies(tiles, entropies);
            int x = lowestEntropyPosition.x;
            int y = lowestEntropyPosition.y;
            tiles[x, y].Collapse(ref randomEngine);
            //update possible tiles for surrounding tiles repeatedly until all tiles have been updated and any resultant changes have been updated
            UpdateAllPossibilities(ref tiles, new int2(x, y), rules);
        }

        FileHelper.DirectoryCheckChunk(filePath);
        // write dungeon to relevant file
        SerializableChunk chunkToSave = GetFinalDungeon(tiles, sideLength);
        string chunkAsJSON = JsonUtility.ToJson(chunkToSave, true);
        string fileName = Path.Combine(filePath, "chunk" + chunkToSave.X + "," + chunkToSave.Y);
        File.WriteAllText(fileName + ".json", chunkAsJSON);
    }
    private static bool AllTilesCollapsed(WFCTile[,] tiles)
    {

        foreach (WFCTile tile in tiles)
        {
            if (!tile.IsCollapsed() && !tile.IsImpossible()) return false;
        }
        return true;
    }
    private static void UpdateAllPossibilities(ref WFCTile[,] tiles, int2 startPos, WFCRuleSet rules)
    {
        Queue<int2> updateQueue = new Queue<int2>();
        updateQueue.Enqueue(startPos);

        while (updateQueue.Count > 0)
        {
            int2 currentPos = updateQueue.Dequeue();

            // Skip out-of-bounds or null tiles
            if (!PositionIsInBounds(currentPos, tiles.GetLength(0))) continue;
            WFCTile currentTile = tiles[currentPos.x, currentPos.y];
            if (currentTile == null) continue;
            //skip impossible and collapsed tiles
            if (currentTile.IsImpossible()) continue;



            // Get current neighbors
            WFCTile[] neighbours = new WFCTile[4];
            int index = 0;
            foreach (int2 offset in NeighbourOffsets())
            {
                int2 neighborPos = currentPos + offset;
                neighbours[index] = PositionIsInBounds(neighborPos, tiles.GetLength(0))
                                    ? tiles[neighborPos.x, neighborPos.y]
                                    : null;
                index++;
            }

            // Update possibilities and check if changed
            bool changed = currentTile.UpdatePossibilities(rules, neighbours);
            if (changed)
            {
                // Enqueue all neighbors for further updates
                foreach (int2 offset in NeighbourOffsets())
                {
                    int2 neighbourPos = currentPos + offset;
                    if (!PositionIsInBounds(neighbourPos, tiles.GetLength(0)))
                    {
                        continue;
                    }
                    if (tiles[neighbourPos.x, neighbourPos.y].IsImpossible())
                    {
                        continue;
                    }
                    updateQueue.Enqueue(neighbourPos);
                }
            }
            if(updateQueue.Count > 1000)
            {
                Debug.LogError("Maximum queue count reached!!");
                break;
            }
        }
    }
    private static bool PositionIsInBounds(int2 pos, int length)
    {
        if(pos.x < 0 || pos.y < 0 || pos.x >= length || pos.y >= length)
        {
            return false;
        }
        return true;
    }
    private static int2 UpdateAllEntropies(WFCTile[,] tiles, int[,] entropies)
    {
        int2 lowestEntropyPosition = int2.zero;
        int lowestEntropy = 100;
        int sideLength = tiles.GetLength(0);
        //The grids should be square and equal
        Assert.AreEqual(tiles.GetLength(0), tiles.GetLength(1));
        Assert.AreEqual(entropies.GetLength(0), entropies.GetLength(1));
        Assert.AreEqual(tiles.GetLength(0), entropies.GetLength(0));
        //iterate through and update entropies
        for(int i = 0; i < sideLength; i++)
        {
            for(int j = 0;j < sideLength; j++)
            {
                entropies[i,j] = tiles[i,j].CalculateEntropy();
                if (entropies[i, j] < lowestEntropy)
                {
                    lowestEntropy = entropies[i, j];
                    lowestEntropyPosition = new int2(i, j);
                }
            }
        }
        return lowestEntropyPosition;
    }
    private static SerializableChunk GetFinalDungeon(WFCTile[,] tiles, int sideLength)
    {
        SerializableChunk returnDungeon = new SerializableChunk(0, 0, sideLength); // all dungeons are a chunk at 0,0 containing all tile data
        for(int i = 0; i < sideLength; i++)
        {
            for(int j = 0; j < sideLength; j++)
            {
                returnDungeon.tiles[(i + (j * sideLength))] = (tiles[i,j].GetCollapsedTile());
            }
        }
        return returnDungeon;
    }
    private enum WFCTileType
    {
        None,
        Moss,
        Mycelium,
        Entrance,
        Exit
    }
    private static List<WFCTileType> GetRandomTiles()
    {
        return new List<WFCTileType> { WFCTileType.None, WFCTileType.Moss, WFCTileType.Mycelium};
    }
    private class WFCTile
    {
        public int x;
        public int y;
        private List<WFCTileType> collapsePossibilities;
        //Instantiate a tile with all possibilities
        public WFCTile(int tileX, int tileY)
        {
            collapsePossibilities = new List<WFCTileType>();
            foreach(WFCTileType type in GetRandomTiles())
            {
                collapsePossibilities.Add(type);
            }
            x = tileX;
            y = tileY;
        }
        //Used for instantiating definite tiles (tiles decided by the designer not by the algorithm)
        public WFCTile(WFCTileType type, int tileX, int tileY)
        {
            collapsePossibilities = new List<WFCTileType> { type };
            x = tileX;
            y = tileY;
        }
        public int CalculateEntropy()
        {
            if (IsCollapsed())
            {
                //Collapsed tiles should no longer be considered for collapsing
                return 100;
            }
            if (IsImpossible())
            {
                //impossible tiles should also no longer be considered
                return 100;
            }
            return collapsePossibilities.Count;
        }
        public void Collapse(ref Unity.Mathematics.Random randomEngine)
        {
            if (IsCollapsed()) return;
            if (IsImpossible()) return;
            int collapseResultIndex = randomEngine.NextInt(0, collapsePossibilities.Count);
            collapsePossibilities = new List<WFCTileType> { collapsePossibilities[collapseResultIndex] };
            Debug.Log("Collapsing tile at " + x + " " + y + " into " + collapsePossibilities[0].ToString());
        }
        public bool IsCollapsed()
        {
            bool collapsed = collapsePossibilities.Count == 1;
            return collapsed;
        }
        public bool IsImpossible()
        {
            bool impossible = collapsePossibilities.Count == 0;
            return impossible;
        }
        //Update the possible neighbours
        // rules - the ruleset with which to update the possibilities
        // neighbours, the 4 adjacent WFCTiles, in order Up, Left, Down, Right. Null for tiles outside the range
        //returns true if tile was changed from previous
        public bool UpdatePossibilities(WFCRuleSet rules, WFCTile[] neighbours)
        {
            int previousPossibilities = collapsePossibilities.Count;
            collapsePossibilities = rules.GetPossibilities(neighbours, collapsePossibilities);
            bool changed = collapsePossibilities.Count != previousPossibilities;
            if(changed && collapsePossibilities.Count == 0)
            {
                Debug.Log("Impossible Tile detected! backtracking should occur");
                //TODO implement backtracking
            }
            return changed;
        }
        public Tile GetCollapsedTile()
        {
            if (!IsCollapsed())
            {
                return new Tile(x, y, 0, 0, 0, new Stone());
            }
            switch (collapsePossibilities[0])
            {
                case WFCTileType.None:
                    return new Tile(x, y, 0, 0, 0, new Grass());
                //case WFCTileType.Mud:
                    //return new Tile(x, y, 0, 0, 0, new Grass(), new EmptyFoliage(), new EmptyObject());
                case WFCTileType.Moss:
                    return new Tile(x, y, 0, 0, 0, new Grass(), new Moss(), new EmptyObject());
                case WFCTileType.Mycelium:
                    return new Tile(x, y, 0, 0, 0, new MyceliumBase(), new MyceliumTopper(), new EmptyObject());
                case WFCTileType.Exit:
                    return new Tile(x, y, 0, 0, 0, new Grass(), new EmptyFoliage(), new DungeonExit());
                case WFCTileType.Entrance:
                    return new Tile(x, y, 0, 0, 0, new Grass(), new Carrot(), new EmptyObject());
                default:
                    return new Tile(x, y, 0, 0, 0, new Stone());
            }
        }
        public List<WFCTileType> GetPossibilities()
        {
            return collapsePossibilities;
        }
    }
    private class WFCRuleSet
    {
        private class WFCConstraint
        {
            public WFCTileType type1;
            public WFCTileType type2;
            public bool isHorizontal;//If horizontal, type 1 is left and type 2 is right, if vertical, type 1 is top and type 2 is bottom
            public WFCConstraint(Color p1, Color p2, bool horizontal)
            {
                type1 = PixelColorToTileType(p1);
                type2 = PixelColorToTileType(p2);
                isHorizontal = horizontal;
            }
            public bool IsAllowed(WFCTileType centre, WFCTileType neighbour, int neighbourIndex)
            {
                //neighbourIndex: up = 0 left = 1 down = 2 right = 3
                if(isHorizontal) {
                    switch (neighbourIndex)
                    {
                        case 0:
                        case 2:
                            return false;
                        case 1:
                            return centre == type2 && neighbour == type1;
                        case 3:
                            return centre == type1 && neighbour == type2;
                    }
                }
                else
                {
                    switch (neighbourIndex)
                    {
                        case 1:
                        case 3:
                            return false;
                        case 0:
                            return centre == type2 && neighbour == type1;
                        case 2:
                            return centre == type1 && neighbour == type2;
                    }
                }
                return false; // strict adjacency rules
            }
            public override bool Equals(object obj)
            {
                if (obj is not WFCConstraint other) return false;
                return type1 == other.type1 &&
                       type2 == other.type2 &&
                       isHorizontal == other.isHorizontal;
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(type1, type2, isHorizontal);
            }
        }
        List<WFCConstraint> constraints;
        public WFCRuleSet(Color[,] inputImagePixels)
        {
            constraints = new List<WFCConstraint>();
            //For this example we will use a 2d image to represent our floor, each pixel will represent one tile
            ProcessConstraints(inputImagePixels, inputImagePixels.GetLength(0), inputImagePixels.GetLength(1), ref constraints);
            Debug.Log("Constraints constructed, length: " + constraints.Count);
        }
        private void ProcessConstraints(Color[,] inputImagePixels, int inputImageWidth, int inputImageHeight, ref List<WFCConstraint> constraints)
        {
            //Only one directional constraints
            //Horizontal constraints first

            for (int y = 0; y < inputImageHeight; y++)
            {
                for(int x = 0;x < inputImageWidth - 1; x++)
                {
                    WFCConstraint newConstraint = new WFCConstraint(inputImagePixels[x, y], inputImagePixels[x + 1, y], true);
                    if (!constraints.Contains(newConstraint))
                    {
                        constraints.Add(newConstraint);
                        Debug.Log($"Loaded constraint: {newConstraint.type1} can be {(newConstraint.isHorizontal ? "left of" : "above")} {newConstraint.type2}");
                    }
                }
            }
            //Then vertical
            for (int x = 0; x < inputImageWidth; x++)
            {
                for (int y = 0; y < inputImageHeight - 1; y++)
                {
                    WFCConstraint newConstraint = new WFCConstraint(inputImagePixels[x, y], inputImagePixels[x, y + 1], false);
                    if (!constraints.Contains(newConstraint))
                    {
                        constraints.Add(newConstraint);
                        Debug.Log($"Loaded constraint: {newConstraint.type1} can be {(newConstraint.isHorizontal ? "left of" : "above")} {newConstraint.type2}");

                    }
                }
            }
        }
        public List<WFCTileType> GetPossibilities(WFCTile[] neighbours, List<WFCTileType> currentPossibilities)
        {
            List<WFCTileType> possibilities = new List<WFCTileType>();
            //neighbours are in order up left down right
            foreach(WFCTileType tileType in currentPossibilities)
            {
                bool tileIsAllowed = true;
                int neighbourIndex = 0;
                foreach (WFCTile neighbour in neighbours)
                {
                    if(neighbour == null)
                    {
                        neighbourIndex++;
                        continue;
                    }
                    if (neighbour.IsImpossible())
                    {
                        neighbourIndex++;
                        continue;
                    }
                    bool hasCompatibleNeighbour = false;
                    foreach(WFCTileType neighbourType in neighbour.GetPossibilities())
                    {
                        if (IsPairAllowed(tileType, neighbourType, neighbourIndex))
                        {
                            hasCompatibleNeighbour = true;
                            break;
                        }

                    }
                    if (!hasCompatibleNeighbour){
                        tileIsAllowed = false;
                        neighbourIndex++;
                        break;
                    }
                    neighbourIndex++;
                }
                if (tileIsAllowed)
                {
                    possibilities.Add(tileType);
                }
            }
            return possibilities;
        }
        private static WFCTileType PixelColorToTileType(Color pixelColor)
        {
            if (pixelColor == new Color(0, 0, 0))
            {
                return WFCTileType.None;
            }
            if (pixelColor == new Color(1, 1, 1))
            {
                return WFCTileType.Mycelium;
            }
            if(pixelColor == new Color(1, 0, 0))
            {
                return WFCTileType.Entrance;
            }
            if (pixelColor == new Color(0, 1, 0))
            {
                return WFCTileType.Exit;
            }
            return WFCTileType.Moss;
        }
        private bool IsPairAllowed(WFCTileType centre, WFCTileType neighbour, int neighbourIndex)
        {
            //if any one constraint determines the pair allowed then we return true
            //otherwise false
            foreach(WFCConstraint c in constraints)
            {
                if(c.IsAllowed(centre, neighbour, neighbourIndex))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
