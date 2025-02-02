using Unity.Collections;
using Unity.Jobs;

public struct TileProcessorJob : IJob
{
    public NativeQueue<Tile> tilesToProcess;
    public NativeQueue<ProcessedTileData> tileRenderQueue;
    public void Execute()
    {
        //take in tile structs and do something? actually not sure
        //TODO figure this out
        //this could fetch texture ready to apply
        while (tilesToProcess.Count > 0)
        {
            tileRenderQueue.Enqueue(new ProcessedTileData(tilesToProcess.Dequeue()));
        }
    }
}
