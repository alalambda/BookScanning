using System.Collections.Generic;
using System.Linq;

namespace BookScanning
{
    public class FileWriter
    {
        public int[] CalculateOrderOfLibraries(int[] signoffs, int[] booksPerDay, int[] books, int totalNumberOfLibraries)
        {
            // pair of library ID and it's time to deliver
            var order = new Dictionary<int, int>();

            for (int i = 0; i < totalNumberOfLibraries; i++)
            {
                var timeToDeliver = signoffs[i] + books[i] / booksPerDay[i];
                order.Add(i, timeToDeliver);
            }

            var sorted = from entry in order orderby entry.Value descending select entry;

            return sorted.Select(x => x.Key).ToArray();
        }
    }
}
