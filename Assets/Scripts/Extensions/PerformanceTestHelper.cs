using System;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

/// <summary>
/// A framework for quick performance tests. Uses a delegate to generically perform certain tasks often.
/// The time spent is logged and can be compared to previous results.
/// </summary>
public class PerformanceTestHelper
{
    private readonly int testCount;
    private readonly List<float> results;

    public PerformanceTestHelper(int testCount)
    {
        this.testCount = testCount;
        results = new List<float>();
    }

    /// <summary>
    /// Performs the input testCount amount of times. The time till completion is then added to the results list.
    /// </summary>
    /// <param name="input">The action to perform.</param>
    /// <param name="showOutput">Whether to log the output to the console or not.</param>
    public void StartTest(Action input, bool showOutput = true)
    {
        var sw = new Stopwatch();
        sw.Start();

        for (var i = 0; i < testCount; i++)
        {
            input.Invoke();
        }
        sw.Stop();

        results.Add(sw.ElapsedMilliseconds);
        if (showOutput)
        {
            Debug.Log(sw.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Prints the accumulated results.
    /// </summary>
    public void PrintResults()
    {
        // Print individual results:
        var resultsString = "";
        for(var i = 0; i < results.Count; i++)
        {
            resultsString += string.Format("Result {0}: {1} | ", i, results[i]);
        }
        Debug.Log(resultsString);

        // Compare each result:
        var comparisonString = "";
        for (var i = 0; i < results.Count; i++)
        {
            for (var j = i + 1; j < results.Count; j++)
            {
                comparisonString += string.Format("{0} / {1} : {2} | ", i, j, results[i]/results[j]);
            }
        }
        Debug.Log(comparisonString);
    }
}