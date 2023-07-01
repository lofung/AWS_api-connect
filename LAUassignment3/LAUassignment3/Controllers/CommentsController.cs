using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LAUassignment3.Data;
using LAUassignment3.Models;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using System.Security.Claims;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Authorization;

namespace LAUassignment3.Controllers
{
    public class CommentsController : Controller
    {
        private readonly LAUassignment3Context _context;
        AmazonDynamoDBClient client;
        BasicAWSCredentials credentials;
        string commentTableName = "comment";
        string movieTableName = "movie";

        public CommentsController(LAUassignment3Context context)
        {
            _context = context;
            credentials = new BasicAWSCredentials(

            );
            client = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.CACentral1);
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            // load full table whatever stupid
            Table commentTable = Table.LoadTable(client, commentTableName, DynamoDBEntryConversion.V2); //load the metadata of the table
            ScanFilter scanFilter = new ScanFilter();
            Search search = commentTable.Scan(scanFilter);
            List<Comment> comments = new List<Comment>();
            do
            {
                var docList = search.GetNextSetAsync();
                docList.Result.ForEach(doc => {

                    //add to moives
                    Comment x = Newtonsoft.Json.JsonConvert.DeserializeObject<Comment>(doc.ToJson());
                    //Console.WriteLine(x);
                    //Console.WriteLine(x.fileName);
                    comments.Add(x);
                    //"fileName": "director3",
                    //"director": "director3",
                    //"rating": 9,
                    //"uploadUser": "lovekillua@gmail.com",
                    //"year": "director3",
                    //"id": 573358,
                    //"genre": "genre3",
                    //"title": "title1"
                });
            } while (!search.IsDone);

            return View(comments);
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Comment == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .FirstOrDefaultAsync(m => m.id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,movieTitle,user,rating,lastEditDateTime,comment")] Comment comment1)
        {
            comment1.user = User.FindFirstValue(ClaimTypes.Name);
            Table commentTable = Table.LoadTable(client, commentTableName, DynamoDBEntryConversion.V2); //load the metadata of the table
            Console.WriteLine(comment1.movieTitle);
            if (ModelState.IsValid)
            {
                Document newComment = new Document();
                var random = new Random();
                newComment["id"] = Int32.Parse(random.Next(0, 10000000).ToString());
                newComment["movieTitle"] = comment1.movieTitle;
                newComment["rating"] = comment1.rating;
                newComment["user"] = comment1.user;
                newComment["lastEditDateTime"] = DateTime.Now;
                newComment["comment"] = comment1.comment;



                await commentTable.PutItemAsync(newComment);
            }

            // find original movie id and go bakc to page
            // load full table whatever stupid
            Table movieTable = Table.LoadTable(client, movieTableName, DynamoDBEntryConversion.V2); //load the metadata of the table
            ScanFilter scanFilter2 = new ScanFilter();
            Search search2 = movieTable.Scan(scanFilter2);
            List<Movie> movies = new List<Movie>();
            string title = "";
            do
            {
                var docList = search2.GetNextSetAsync();
                docList.Result.ForEach(doc => {

                    //add to moives
                    Movie x = Newtonsoft.Json.JsonConvert.DeserializeObject<Movie>(doc.ToJson());
                    //Console.WriteLine(x);
                    //Console.WriteLine(x.fileName);
                    movies.Add(x);
                    //"fileName": "director3",
                    //"director": "director3",
                    //"rating": 9,
                    //"uploadUser": "lovekillua@gmail.com",
                    //"year": "director3",
                    //"id": 573358,
                    //"genre": "genre3",
                    //"title": "title1"
                });
            } while (!search2.IsDone);

            foreach (Movie item in movies)
            {
                if (item.title == comment1.movieTitle)
                {
                    return RedirectToAction("Details", "Movies", new { id = item.id }); ;
                }

            }

            return NotFound();
        }

        // GET: Comments/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            // load full table whatever stupid
            Table commentTable = Table.LoadTable(client, commentTableName, DynamoDBEntryConversion.V2); //load the metadata of the table
            ScanFilter scanFilter = new ScanFilter();
            Search search = commentTable.Scan(scanFilter);
            List<Comment> comments = new List<Comment>();
            do
            {
                var docList = search.GetNextSetAsync();
                docList.Result.ForEach(doc => {

                    //add to moives
                    Comment x = Newtonsoft.Json.JsonConvert.DeserializeObject<Comment>(doc.ToJson());
                    //Console.WriteLine(x);
                    //Console.WriteLine(x.fileName);
                    comments.Add(x);
                    //"fileName": "director3",
                    //"director": "director3",
                    //"rating": 9,
                    //"uploadUser": "lovekillua@gmail.com",
                    //"year": "director3",
                    //"id": 573358,
                    //"genre": "genre3",
                    //"title": "title1"
                });
            } while (!search.IsDone);

            foreach (Comment item in comments)
            {
                if (item.id == id)
                {
                    return View(item);
                }

            }

            return NotFound();
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,movieTitle,rating,user,lastEditDateTime,comment")] Comment comment1)
        {
            comment1.user = User.FindFirstValue(ClaimTypes.Name);
            Table commentTable = Table.LoadTable(client, commentTableName, DynamoDBEntryConversion.V2); //load the metadata of the table
            //Document doc = await movieTable.GetItemAsync(movie.title);
            Document newComment = new Document();
            newComment["id"] = comment1.id;
            newComment["movieTitle"] = comment1.movieTitle;
            newComment["rating"] = comment1.rating;
            newComment["user"] = comment1.user;
            newComment["lastEditDateTime"] = comment1.lastEditDateTime;
            newComment["comment"] = comment1.comment;

            if (id != comment1.id)
            {
                return NotFound();
            }
            await commentTable.PutItemAsync(newComment);

            // find original movie id and go bakc to page
            // load full table whatever stupid
            Table movieTable = Table.LoadTable(client, movieTableName, DynamoDBEntryConversion.V2); //load the metadata of the table
            ScanFilter scanFilter = new ScanFilter();
            Search search = movieTable.Scan(scanFilter);
            List<Movie> movies = new List<Movie>();
            string title = "";
            do
            {
                var docList = search.GetNextSetAsync();
                docList.Result.ForEach(doc => {

                    //add to moives
                    Movie x = Newtonsoft.Json.JsonConvert.DeserializeObject<Movie>(doc.ToJson());
                    //Console.WriteLine(x);
                    //Console.WriteLine(x.fileName);
                    movies.Add(x);
                    //"fileName": "director3",
                    //"director": "director3",
                    //"rating": 9,
                    //"uploadUser": "lovekillua@gmail.com",
                    //"year": "director3",
                    //"id": 573358,
                    //"genre": "genre3",
                    //"title": "title1"
                });
            } while (!search.IsDone);

            foreach (Movie item in movies)
            {
                if (item.title == comment1.movieTitle)
                {
                    return RedirectToAction("Details", "Movies", new { id = item.id }); ;
                }

            }

            return NotFound();

            //return View(comment1);
        }

        // GET: Comments/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            // load full table whatever stupid
            Table commentTable = Table.LoadTable(client, commentTableName, DynamoDBEntryConversion.V2); //load the metadata of the table
            ScanFilter scanFilter = new ScanFilter();
            Search search = commentTable.Scan(scanFilter);
            List<Comment> comments = new List<Comment>();
            do
            {
                var docList = search.GetNextSetAsync();
                docList.Result.ForEach(doc => {

                    //add to moives
                    Comment x = Newtonsoft.Json.JsonConvert.DeserializeObject<Comment>(doc.ToJson());
                    //Console.WriteLine(x);
                    //Console.WriteLine(x.fileName);
                    comments.Add(x);
                    //"fileName": "director3",
                    //"director": "director3",
                    //"rating": 9,
                    //"uploadUser": "lovekillua@gmail.com",
                    //"year": "director3",
                    //"id": 573358,
                    //"genre": "genre3",
                    //"title": "title1"
                });
            } while (!search.IsDone);

            foreach (Comment item in comments)
            {
                if (item.id == id)
                {
                    return View(item);
                }

            }
            return NotFound();
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Movie == null)
            {
                return Problem("Entity set 'LAUassignment3Context.Movie'  is null.");
            }

            // load full table whatever stupid
            Table commentTable = Table.LoadTable(client, commentTableName, DynamoDBEntryConversion.V2); //load the metadata of the table
            ScanFilter scanFilter = new ScanFilter();
            Search search = commentTable.Scan(scanFilter);
            List<Comment> comments = new List<Comment>();
            string title = "";
            do
            {
                var docList = search.GetNextSetAsync();
                docList.Result.ForEach(doc => {

                    //add to moives
                    Comment x = Newtonsoft.Json.JsonConvert.DeserializeObject<Comment>(doc.ToJson());
                    comments.Add(x);
                });
            } while (!search.IsDone);

            foreach (Comment item in comments)
            {
                if (item.id == id)
                {
                    title = item.movieTitle;
                }

            }

            var key = new Dictionary<string, AttributeValue>
            {
                ["id"] = new AttributeValue { N = id.ToString() },
                ["movieTitle"] = new AttributeValue { S = title }
            };

            var request = new DeleteItemRequest
            {
                TableName = commentTableName,
                Key = key,
            };

            var response = await client.DeleteItemAsync(request);

            // find original movie id and go bakc to page
            // load full table whatever stupid
            Table movieTable = Table.LoadTable(client, movieTableName, DynamoDBEntryConversion.V2); //load the metadata of the table
            ScanFilter scanFilter2 = new ScanFilter();
            Search search2 = movieTable.Scan(scanFilter2);
            List<Movie> movies = new List<Movie>();
            do
            {
                var docList = search2.GetNextSetAsync();
                docList.Result.ForEach(doc => {

                    //add to moives
                    Movie x = Newtonsoft.Json.JsonConvert.DeserializeObject<Movie>(doc.ToJson());
                    //Console.WriteLine(x);
                    //Console.WriteLine(x.fileName);
                    movies.Add(x);
                    //"fileName": "director3",
                    //"director": "director3",
                    //"rating": 9,
                    //"uploadUser": "lovekillua@gmail.com",
                    //"year": "director3",
                    //"id": 573358,
                    //"genre": "genre3",
                    //"title": "title1"
                });
            } while (!search2.IsDone);

            foreach (Movie item in movies)
            {
                if (item.title == title)
                {
                    return RedirectToAction("Details", "Movies", new { id = item.id }); ;
                }

            }

            return NotFound();
        }

        private bool CommentExists(int id)
        {
          return (_context.Comment?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
