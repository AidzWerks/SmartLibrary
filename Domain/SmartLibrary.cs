using System;

namespace SmartLibraryManagementSystem.Models
{
    public class Book
    {
        // Properties
        public string BookCode { get; private set; }
        public string Title { get; private set; }
        public string Author { get; private set; }
        public int YearPublished { get; private set; }
        public bool IsAvailable { get; private set; }

        // Constructor
        public Book(string bookCode, string title, string author, int yearPublished)
        {
            if (string.IsNullOrWhiteSpace(bookCode))
            {
                throw new ArgumentException("Book code cannot be empty.");
            }

            if (yearPublished > DateTime.Now.Year)
            {
                throw new ArgumentException("Invalid year published.");
            }

            BookCode = bookCode;
            Title = title;
            Author = author;
            YearPublished = yearPublished;
            IsAvailable = true;
        }

        // Methods
        public void MarkAsBorrowed()
        {
            if (!IsAvailable)
            {
                Console.WriteLine($"'{Title}' is already borrowed.");
                return;
            }

            IsAvailable = false;
            Console.WriteLine($"'{Title}' has been borrowed.");
        }

        public void MarkAsReturned()
        {
            if (IsAvailable)
            {
                Console.WriteLine($"'{Title}' is already available.");
                return;
            }

            IsAvailable = true;
            Console.WriteLine($"'{Title}' has been returned.");
        }

        public void CheckAvailability()
        {
            Console.WriteLine(
                $"{Title}: {(IsAvailable ? "Available" : "Borrowed")}"
            );
        }
    }
}