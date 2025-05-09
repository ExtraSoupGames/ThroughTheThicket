using NUnit.Framework;
using System.Diagnostics;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.Profiling;
using UnityEngine.TestTools;
using System.Collections;
using System;
public class BenchmarkingTests
{
    [Test]
    public void CellularAutomataChunkTest()
    {
        string testingFile = Path.Combine(Application.persistentDataPath, "TestingChunks");
        FileHelper.DirectoryCheckTesting();
        long[] generationTimes = new long[100];

        for(int i = 0; i < 100; i++)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            CellularAutomataGenerator.GenerateChunkAt(0, 0, testingFile, 1);
            stopwatch.Stop();
            generationTimes[i] = stopwatch.ElapsedMilliseconds;

        }
        double average = generationTimes.Average();

        double variance = generationTimes
            .Select(time => (time - average) * (time - average))
            .Average();

        double stdDev = System.Math.Sqrt(variance);

        UnityEngine.Debug.Log($"Average Time: {average:F2} ms");
        UnityEngine.Debug.Log($"Standard Deviation: {stdDev:F2} ms");
        UnityEngine.Debug.Log($"Quickest Time: {generationTimes.Min()}");
        UnityEngine.Debug.Log($"Slowest Time: {generationTimes.Max()}");
    }
    [Test]
    public void WaveFunctionCollapseTest()
    {

        string testingFile = Path.Combine(Application.persistentDataPath, "TestingChunks");
        FileHelper.DirectoryCheckTesting();

        Texture2D inputTexture = Resources.Load<Texture2D>("WFCRuleInput");
        int width = inputTexture.width;
        int height = inputTexture.height;
        Color[,] WFCRuleImage = new Color[width,height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                WFCRuleImage[x,y] = inputTexture.GetPixel(x, y);
            }
        }
        long[] generationTimes = new long[100];
        for (int i = 0; i < 100; i++)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            WaveFunctionCollapse.GenerateDungeon(0, 20, testingFile, 1, WFCRuleImage);
            stopwatch.Stop();
            generationTimes[i] = stopwatch.ElapsedMilliseconds;
        }

        double average = generationTimes.Average();

        double variance = generationTimes
            .Select(time => (time - average) * (time - average))
            .Average();

        double stdDev = System.Math.Sqrt(variance);

        UnityEngine.Debug.Log($"Average Time: {average:F2} ms");
        UnityEngine.Debug.Log($"Standard Deviation: {stdDev:F2} ms");
        UnityEngine.Debug.Log($"Quickest Time: {generationTimes.Min()}");
        UnityEngine.Debug.Log($"Slowest Time: {generationTimes.Max()}");
    }

    [UnityTest]
    public IEnumerator CellularAutomataMemoryTest()
    {
        // allows play mode to fully initialize
        yield return new WaitForSeconds(1.5f);
        Profiler.enabled = true;
        yield return null;
        string testingFile = Path.Combine(Application.persistentDataPath, "TestingChunks");
        FileHelper.DirectoryCheckTesting();
        // ensure GC is fully done before starting measurements
        for (int i = 0; i < 3; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            yield return null;
        }
        long beforeMemory = Profiler.GetTotalAllocatedMemoryLong();
        yield return null;
        CellularAutomataGenerator.GenerateChunkAt(0, 0, testingFile, 1);
        yield return null; // ensure method is completed
        long afterMemory = Profiler.GetTotalAllocatedMemoryLong();
        yield return null;
        long usedMemory = afterMemory - beforeMemory;
        UnityEngine.Debug.Log($"CA Memory: {usedMemory:F2} B (Before: {beforeMemory}, After: {afterMemory})");
    }
    [UnityTest]
    public IEnumerator WaveFunctionCollapseMemoryTest()
    {
        //allows play mode to fully initialize
        yield return new WaitForSeconds(1.5f);
        Profiler.enabled = true;
        yield return null;
        string testingFile = Path.Combine(Application.persistentDataPath, "TestingChunks");
        FileHelper.DirectoryCheckTesting();
        Texture2D inputTexture = Resources.Load<Texture2D>("WFCRuleInput");
        int width = inputTexture.width;
        int height = inputTexture.height;
        Color[,] WFCRuleImage = new Color[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                WFCRuleImage[x, y] = inputTexture.GetPixel(x, y);
            }
        }
        yield return null;
        // ensure GC is fully done before starting measurements
        for (int i = 0; i < 3; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            yield return null;
        }
        long beforeMemory = Profiler.GetTotalAllocatedMemoryLong();
        yield return null;
        WaveFunctionCollapse.GenerateDungeon(0, 8, testingFile, 1, WFCRuleImage);
        yield return null; // ensure method is completed
        long afterMemory = Profiler.GetTotalAllocatedMemoryLong();
        yield return null;
        long usedMemory = afterMemory - beforeMemory;
        UnityEngine.Debug.Log($"WFC Memory: {usedMemory:F2} B (Before: {beforeMemory}, After: {afterMemory})");
    }
}