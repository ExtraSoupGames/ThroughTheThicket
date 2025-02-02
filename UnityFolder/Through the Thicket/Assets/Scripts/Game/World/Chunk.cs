using Unity.Collections;
public struct Chunk
{
    public NativeArray<Tile> tiles;
    public int X, Y;
    public static int ChunkSize()
    {
        return 16;
    }
    public Chunk(int X, int Y)
    {
        this.X = X;
        this.Y = Y;
        tiles = new NativeArray<Tile>(ChunkSize() * ChunkSize(), Allocator.Persistent);
        int chunkRad = ChunkSize() / 2;
        for (int i = 0; i < ChunkSize(); i++)
        {
            for(int j = 0; j < ChunkSize(); j++)
            {
                tiles[i * ChunkSize() + j] = new Tile((X * ChunkSize()) - chunkRad + i, (Y * ChunkSize()) - chunkRad + j, 1, X ,Y);
            }
        }
    }
    public Chunk(NativeArray<Tile> pTiles, int pX, int pY)
    {
        tiles = pTiles;
        X = pX;
        Y = pY;
    }
    public ChunkPos GetPos()
    {
        return new ChunkPos(X, Y);
    }
    public SerializableChunk GetChunkForSerialization()
    {
        return new SerializableChunk(tiles, X, Y);
    }
}
[System.Serializable]
public struct SerializableChunk
{
    public int X, Y;
    public Tile[] tiles;
    public SerializableChunk(NativeArray<Tile> pTiles, int pX, int pY)
    {
        X = pX;
        Y = pY;
        tiles = pTiles.ToArray();
    }
    public Chunk ToChunk()
    {
        return new Chunk(new NativeArray<Tile>(tiles, Allocator.TempJob), X, Y);
    }
}