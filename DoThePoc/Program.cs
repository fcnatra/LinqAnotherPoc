using System.Diagnostics;

internal class Program
{
    private const int SECONDS_TO_BUILD = 5;
    private static void Main(string[] args)
    {
        TestUsingIEnumerables();
        TestUsingIEnumerables();
        TestUsingIEnumerables();

        TestUsingLists();
        TestUsingLists();
        TestUsingLists();
    }

    private static void TestUsingLists()
    {
        Console.WriteLine("Using Lists\nBuilding Definition...");

        List<List<string>> mainList = [];
        List<List<string>> finalList = [];

        long memoryInUse = GC.GetTotalMemory(true);
        var sw = Stopwatch.StartNew();

        for (int i = 0; sw.Elapsed.Seconds < SECONDS_TO_BUILD; i++)
        {
            if (i % 100 == 0)
                mainList.AddRange(MillionsOfStrings.ToList());
            else
                mainList.AddRange([]);
        }

        Console.WriteLine("Uppercasing...");

        foreach (var sublist in mainList)
        {
            if (sublist.Count != 0)
            {
                var uppercasedSublist = new List<string>();
                foreach (var s in sublist)
                    uppercasedSublist.Add(s.ToUpper());
                
                finalList.Add(uppercasedSublist);
            }
        }
        sw.Stop();

        Console.WriteLine($"Size of finalList: {finalList.Count()} - first string chunk: {finalList[0][0].Substring(0, 10)}");
        ShowFinalMetrics(memoryInUse, sw);
    }

    private static void TestUsingIEnumerables()
    {
        Console.WriteLine("Using IEnumerables\nBuilding Definition...");

        IEnumerable<IEnumerable<string>> mainList = Enumerable.Empty<IEnumerable<string>>();
        IEnumerable<IEnumerable<string>> finalList = Enumerable.Empty<IEnumerable<string>>();
        
        long memoryInUse = GC.GetTotalMemory(true);
        var sw = Stopwatch.StartNew();
        
        for (int i = 0; sw.Elapsed.Seconds < SECONDS_TO_BUILD; i++)
        {
            if (i % 100 == 0)
                mainList = mainList.Append(MillionsOfStrings);
            else
                mainList = mainList.Append(Enumerable.Empty<string>());
        }

        Console.WriteLine("Uppercasing...");

        finalList = mainList.Where(x => x.Any()).Select(x => x.Select(s => s.ToUpper()));
        sw.Stop();

        Console.WriteLine($"Size of finalList: {finalList.Count()} - first string chunk: {finalList.First().First().Substring(0, 10)}");
        ShowFinalMetrics(memoryInUse, sw);
    }

    private static IEnumerable<string> MillionsOfStrings => Enumerable.Range(0, 10000000).Select(_ => new string((char)(65+DateTime.Now.Second), 100));

    private static void ShowFinalMetrics(long memoryInUse, Stopwatch sw)
    {
        Console.WriteLine($"Used memory: {UsedMemoryInMb(memoryInUse)}MB");
        Console.WriteLine("Elapsed time: " + sw.Elapsed);
        Console.WriteLine();
    }

    private static long UsedMemoryInMb(long memoryInUse) => (GC.GetTotalMemory(true) - memoryInUse) / 1024 / 1024;
}