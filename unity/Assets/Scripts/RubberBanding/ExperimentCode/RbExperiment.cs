// namespace RubberBanding
// {
//     using NUnit.Framework;
//     using System;
//     using System.Diagnostics;
//     using System.Collections.Generic;
//     using UnityEngine;
//     using Debug = UnityEngine.Debug;
//     using Random = System.Random;

//     public class RbExperiment
//     {
//         // Testing mode: random, grid or clustered
//         private readonly string Mode = "arbitrary";
//         private readonly Random Rand = new Random();

//         private readonly int Queries = 500;
//         private readonly int Warmup = 20;
//         private readonly int Count = 1000000;
//         private readonly int Width = 1000;
//         private readonly int Height = 1000;


//         private void RunExperiment()
//         {
//             // Generate points
//             List<rbPoint> points;
//             switch(Mode)
//             {
//                 case "random":
//                     Debug.Log("RUNNING RANDOM EXPERIMENT");
//                     points = GetRandomPoints(Count);
//                     break;
//                 case "grid":
//                     Debug.Log("RUNNING GRIDDED EXPERIMENT");
//                     points = GetGriddedPoints(Count);
//                     break;
//                 case "clustered":
//                     Debug.Log("RUNNING CLUSTERED EXPERIMENT");
//                     points = GetClusteredPoints(Count);
//                     break;
//                 default:
//                     throw new KeyNotFoundException("Mode not available");
//             }

//             // Compute boundingRectangle
//             AARect boundingRect = AARect.GetBoundingRectangle(points);

//             // Create (and time) range queries
//             var qtBuildWatch = Stopwatch.StartNew();
//             RbQuadTree qt = new RbQuadTree(points);
//             qtBuildWatch.Stop();
//             Debug.Log("Building QT: " + qtBuildWatch.ElapsedMilliseconds + "ms");

//             var rtBuildWatch = Stopwatch.StartNew();
//             RbRangeTree rt = new RbRangeTree(points);
//             rtBuildWatch.Stop();
//             Debug.Log("Building RT: " + qtBuildWatch.ElapsedMilliseconds + "ms");

//             // Run queries
//             float rtTotalTime = 0f, qtTotalTime = 0f;
//             for (int i = 0; i < Queries + Warmup; i++)
//             {
//                 AARect queryRect = GetRandomRect(boundingRect);

//                 // Time both range queries
//                 var qtQueryWatch = Stopwatch.StartNew();
//                 List<rbPoint> qtPoints = qt.FindInRange(queryRect);
//                 qtQueryWatch.Stop();

//                 var rtQueryWatch = Stopwatch.StartNew();
//                 List<rbPoint> rtPoints = rt.FindInRange(queryRect);
//                 rtQueryWatch.Stop();

//                 // Verify correctness-ish
//                 Assert.True(qtPoints.TrueForAll(x => boundingRect.Contains(x)));
//                 Assert.True(rtPoints.TrueForAll(x => boundingRect.Contains(x)));
//                 Assert.True(rtPoints.Count == qtPoints.Count);

//                 // Save time
//                 if (i > Warmup)
//                 {
//                     qtTotalTime += qtQueryWatch.ElapsedMilliseconds;
//                     rtTotalTime += rtQueryWatch.ElapsedMilliseconds;
//                 }
//             }
//             float rtAverageTime = rtTotalTime / Queries;
//             float qtAverageTime = qtTotalTime / Queries;

//             // Print results
//             Debug.Log("Average query time QT: " + qtAverageTime + "ms");
//             Debug.Log("Average query time RT: " + rtAverageTime + "ms");
//         }

//         private AARect GetRandomRect(AARect bounds)
//         {
//             float vOne = (float) Rand.NextDouble() * Height;
//             float vTwo = (float) Rand.NextDouble() * Height;
//             float hOne = (float) Rand.NextDouble() * Width;
//             float hTwo = (float)Rand.NextDouble() * Width;

//             float top = Math.Max(vOne, vTwo);
//             float bottom = Math.Min(vOne, vTwo);
//             float right = Math.Max(hOne, hTwo);
//             float left = Math.Min(hOne, hTwo);

//             return new AARect(top, right, bottom, left);
//         }

//         /// <summary>
//         /// Generates `count` random points.
//         /// </summary>
//         private List<rbPoint> GetRandomPoints(int count)
//         {
//             List<rbPoint> points = new List<rbPoint>();
//             for (int i = 0; i < count; i++)
//             {
//                 float x = (float) Rand.NextDouble() * Width;
//                 float y = (float) Rand.NextDouble() * Height;
//                 points.Add(new rbPoint(new Vector2(x, y)));
//             }
//             return points;
//         }

//         /// <summary>
//         /// Generates `count` points that are formed in a grid formation
//         /// </summary>
//         private List<rbPoint> GetGriddedPoints(int count)
//         {
//             List<rbPoint> points = new List<rbPoint>();
//             int pointsPerCol = (int) Math.Ceiling(Math.Sqrt(count));
//             int pointsPerRow = (count + pointsPerCol - 1) / pointsPerCol;

//             float horSpacing = (Width / (pointsPerRow - 1));
//             float vertSpacing = (Height / (pointsPerCol - 1));
//             for (int i = 0; i < count; i++)
//             {
//                 int colIdx = i / pointsPerCol;
//                 int rowIdx = i % pointsPerCol;                
//                 float x = horSpacing * colIdx;
//                 float y = vertSpacing * rowIdx;
//                 points.Add(new rbPoint(new Vector2(x, y)));
//             }
//             return points;
//         }


//         /// <summary>
//         /// Generates `count` points that are formed in sqrt(count) clusters.
//         /// </summary>
//         private List<rbPoint> GetClusteredPoints(int count)
//         {
//             // Take some random core points
//             int nrCorePoints = (int)Math.Sqrt(count);
//             List<rbPoint> corePoints = GetClusteredPoints(nrCorePoints);

//             // Create points as clusters around these core points
//             List<rbPoint> points = new List<rbPoint>();
//             float maxWidthOffset = (float) Math.Sqrt(Width);
//             float maxHeightOffset = (float) Math.Sqrt(Height);
//             for (int i = 0; i < count; i++)
//             {
//                 int idx = Rand.Next(corePoints.Count);
//                 rbPoint corePoint = corePoints[idx];

//                 float xOffset = (float) (2 * Rand.NextDouble() - 1.0f) * maxWidthOffset;
//                 float yOffset = (float)(2 * Rand.NextDouble() - 1.0f) * maxHeightOffset;
//                 Vector2 offsetVector = new Vector2(xOffset, yOffset);
//                 points.Add(new rbPoint(corePoint.Pos + offsetVector));
//             }

//             return points;
//         }
//     }
// }

