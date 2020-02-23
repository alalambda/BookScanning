using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BookScanning
{
    public class FileWriter
    {
        public string path = @"C:\Users\annus\source\repos\BookScanning\BookScanning\BookScanning\Output\";

        private static readonly char[] TrimNewLineChars = Environment.NewLine.ToCharArray();

        public List<Library> sortedLibraries = null;

        public void OrderLibraries(List<Library> libraries, bool areRatingsSame, int numberOfDaysTotal)
        {
            sortedLibraries = libraries.OrderBy(x => x.Efficiency).ToList();

            //var libraryRatings = new int[sortedLibraries.Count];

            //if (!areRatingsSame)
            //{
            //    for (int i = 0; i < sortedLibraries.Count; i++)
            //    {
            //        var library = libraries[i];
            //        if (library.booksSortedByRatingDictionary != null)
            //        {
            //            var libraryRating = 0;
            //            if (library.BooksInLibrary < library.BooksCanSendPerDay)
            //            {
            //                var booksLibraryCanShip = library.BooksInLibrary;
            //                libraryRating = library.booksSortedByRatingDictionary.Sum(x => x.Value);
            //            }
            //            else
            //            {
            //                var booksLibraryCanShip = (numberOfDaysTotal - library.DaysToSignoff) / library.BooksCanSendPerDay;

            //                foreach (var rating in library.booksSortedByRatingDictionary.Values)
            //                {
            //                    if (booksLibraryCanShip == 0)
            //                    {
            //                        break;
            //                    }

            //                    libraryRating += rating;
            //                    --booksLibraryCanShip;
            //                }
            //            }
            //            libraryRatings[i] = libraryRating;
            //        }
            //    }

            //    Array.Sort(libraryRatings, sortedLibraries.ToArray(), Comparer<int>.Create((x, y) => y.CompareTo(x)));
            //}
        }

        public void SignoffAndShipBooks(List<Library> libraries, Dictionary<int, int> BookToRatingGlobal, int numberOfDaysTotal, string fileName, bool areRatingsSame)
        {
            if (File.Exists(path + fileName))
            {
                File.Delete(path + fileName);
            }

            OrderLibraries(libraries, areRatingsSame, numberOfDaysTotal);

            var libraryAndBooksString = new StringBuilder();

            var librariesProcessed = 0;
            foreach (var library in sortedLibraries)
            {
                if (BookToRatingGlobal.Count < 1)
                {
                    break;
                }

                if (library.DaysToSignoff >= numberOfDaysTotal)
                {
                    continue;
                }

                var booksProcessed = 0;
                var daysPassed = library.DaysToSignoff;
                if (library.booksSortedByRatingDictionary != null)
                {
                    foreach (var bookId in library.booksSortedByRatingDictionary.Keys.ToArray())
                    {
                        if (daysPassed == numberOfDaysTotal)
                        {
                            break;
                        }

                        if (BookToRatingGlobal.ContainsKey(bookId))
                        {
                            BookToRatingGlobal.Remove(bookId);
                            if (library.BooksCanSendPerDay > library.BooksInLibrary)
                            {
                                booksProcessed += library.BooksInLibrary;
                                break;
                            }
                            else
                            {
                                booksProcessed += library.BooksCanSendPerDay;
                            }
                            ++daysPassed;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    foreach (var bookId in library.Books)
                    {
                        if (daysPassed == numberOfDaysTotal)
                        {
                            break;
                        }

                        if (BookToRatingGlobal.ContainsKey(bookId))
                        {
                            BookToRatingGlobal.Remove(bookId);

                            if (library.BooksCanSendPerDay > library.BooksInLibrary)
                            {
                                booksProcessed += library.BooksInLibrary;
                                break;
                            }
                            else
                            {
                                booksProcessed += library.BooksCanSendPerDay;
                            }
                            ++daysPassed;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                if (booksProcessed == 0)
                {
                    break;
                }

                var arrayOfBookIds = new int[library.Books.Length];
                if (library.booksSortedByRatingDictionary != null)
                {
                    arrayOfBookIds = library.booksSortedByRatingDictionary.Keys.ToArray();
                }
                else
                {
                    arrayOfBookIds = library.Books;
                }

                if (arrayOfBookIds.Length > booksProcessed)
                {
                    Array.Resize(ref arrayOfBookIds, booksProcessed);
                }

                libraryAndBooksString.Append(library.Id).Append(" ").Append(arrayOfBookIds.Length).AppendLine();
                libraryAndBooksString.Append(string.Join(" ", arrayOfBookIds)).AppendLine();

                using (StreamWriter file = new StreamWriter(path + fileName, true))
                {
                    file.WriteLine(libraryAndBooksString.ToString().TrimEnd(TrimNewLineChars));
                }

                libraryAndBooksString.Clear();

                ++librariesProcessed;
            }


            CreateEntry(librariesProcessed.ToString(), fileName);
        }

        public void CreateEntry(string lineToAdd, string fileName)
        {
            var txtLines = File.ReadAllLines(path + fileName).ToList();   //Fill a list with the lines from the txt file.
            txtLines.Insert(0, lineToAdd);                                //Insert the line you want to add last under the tag 'item1'.
            File.WriteAllLines(path + fileName, txtLines);                //Add the lines including the new one.
        }
    }
}
