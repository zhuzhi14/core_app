using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace WebApplication3.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string BookName { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public string Author { get; set; }
    }
    
    public class BookstoreDatabaseSettings : IBookstoreDatabaseSettings
    {
        public string BooksCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IBookstoreDatabaseSettings
    {
        string BooksCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
    
    
     public class BookService
        {
            private readonly IMongoCollection<Book> _books;
    
            public BookService(IBookstoreDatabaseSettings settings)
            {
                var client = new MongoClient(settings.ConnectionString);
                var database = client.GetDatabase(settings.DatabaseName);
    
                _books = database.GetCollection<Book>(settings.BooksCollectionName);
            }
    
            public List<Book> Get() =>
                _books.Find(book => true).ToList();
    
            public Book Get(string id) =>
                _books.Find<Book>(book => book.Id == id).FirstOrDefault();
    
            public Book Create(Book book)
            {
                _books.InsertOne(book);
                return book;
            }
    
            public void Update(string id, Book bookIn) =>
                _books.ReplaceOne(book => book.Id == id, bookIn);
    
            public void Remove(Book bookIn) =>
                _books.DeleteOne(book => book.Id == bookIn.Id);
    
            public void Remove(string id) => 
                _books.DeleteOne(book => book.Id == id);
        }
}