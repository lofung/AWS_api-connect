using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LAUassignment3.Data;
using LAUassignment3.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;

namespace LAUassignment3.Controllers
{
    public class MoviesController : Controller
    {
        private readonly LAUassignment3Context _context;
        AmazonDynamoDBClient client;
        BasicAWSCredentials credentials;
        string movieTableName = "movie";
        string commentTableName = "comment";

        public MoviesController(LAUassignment3Context context)
        {
            _context = context;
            credentials = new BasicAWSCredentials(
            );
            client = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.CACentral1);
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            // load full table whatever stupid
            Table movieTable = Table.LoadTable(client, movieTableName, DynamoDBEntryConversion.V2); //load the metadata of the table
            ScanFilter scanFilter = new ScanFilter();
            Search search = movieTable.Scan(scanFilter);
            List<Movie> movies = new List<Movie>();
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

            return View(movies);
        }

        // GET: Movies/ShowSearchForm
        public async Task<IActionResult> ShowSearchForm()
        {
            return View();
        }

        // POST: Movies/ShowSearchResults
        public async Task<IActionResult> ShowSearchResults(string SearchTitle, string SearchRating, string SearchGenre)
        {
            // load full table whatever stupid
            Table movieTable = Table.LoadTable(client, movieTableName, DynamoDBEntryConversion.V2); //load the metadata of the table
            ScanFilter scanFilter = new ScanFilter();
            Search search = movieTable.Scan(scanFilter);
            List<Movie> movies = new List<Movie>();
            if (!string.IsNullOrEmpty(SearchTitle))
            {
                do
                {
                    var docList = search.GetNextSetAsync();
                    docList.Result.ForEach(doc => {

                        //add to moives
                        Movie x = Newtonsoft.Json.JsonConvert.DeserializeObject<Movie>(doc.ToJson());
                        if (x.title.Contains(SearchTitle))
                        {
                            movies.Add(x);
                        }
                    });
                } while (!search.IsDone);

                return View("Index", movies);

                /*return _context.Movie != null ?
                    View("Index", await _context.Movie.Where( j => j.title.Contains(SearchTitle)).ToListAsync()) :
                    Problem("Entity set 'LAUassignment3Context.Movie'  is null.");*/
            }
            if (!string.IsNullOrEmpty(SearchRating) && int.TryParse(SearchRating, out int number))
            {
                do
                {
                    var docList = search.GetNextSetAsync();
                    docList.Result.ForEach(doc => {

                        //add to moives
                        Movie x = Newtonsoft.Json.JsonConvert.DeserializeObject<Movie>(doc.ToJson());
                        if (x.rating >= number)
                        {
                            movies.Add(x);
                        }
                    });
                } while (!search.IsDone);

                return View("Index", movies);

                /* return _context.Movie != null ?
                    View("Index", await _context.Movie.Where(j => j.rating > number).ToListAsync()) :
                    Problem("Entity set 'LAUassignment3Context.Movie'  is null.");*/
            }

            if (!string.IsNullOrEmpty(SearchGenre))
            {
                do
                {
                    var docList = search.GetNextSetAsync();
                    docList.Result.ForEach(doc => {

                        //add to moives
                        Movie x = Newtonsoft.Json.JsonConvert.DeserializeObject<Movie>(doc.ToJson());
                        if (x.genre.Contains(SearchGenre))
                        {
                            movies.Add(x);
                        }
                    });
                } while (!search.IsDone);

                return View("Index", movies);

                /* return _context.Movie != null ?
                    View("Index", await _context.Movie.Where(j => j.rating > number).ToListAsync()) :
                    Problem("Entity set 'LAUassignment3Context.Movie'  is null.");*/
            }

            //else
            do
            {
                var docList = search.GetNextSetAsync();
                docList.Result.ForEach(doc => {

                    //add to moives
                    Movie x = Newtonsoft.Json.JsonConvert.DeserializeObject<Movie>(doc.ToJson());
                    movies.Add(x);
                });
            } while (!search.IsDone);


            return View("Index", movies);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // load full table whatever stupid
            Table movieTable = Table.LoadTable(client, movieTableName, DynamoDBEntryConversion.V2); //load the metadata of the table
            ScanFilter scanFilter = new ScanFilter();
            Search search = movieTable.Scan(scanFilter);
            List<Movie> movies = new List<Movie>();
            Movie movieResult;
            do
            {
                var docList = search.GetNextSetAsync();
                docList.Result.ForEach(doc => {

                    //add to moives
                    Movie x = Newtonsoft.Json.JsonConvert.DeserializeObject<Movie>(doc.ToJson());
                    movies.Add(x);
                });
            } while (!search.IsDone);

            foreach (Movie item in movies)
            {
                if (item.id == id)
                {
                    movieResult = item;

                    // load full table whatever stupid COMMENTS
                    Table commentTable = Table.LoadTable(client, commentTableName, DynamoDBEntryConversion.V2); //load the metadata of the table
                    Search search2 = commentTable.Scan(scanFilter);
                    List<Comment> comments = new List<Comment>();
                    do
                    {
                        var docList = search2.GetNextSetAsync();
                        docList.Result.ForEach(doc =>
                        {

                            //add to moives
                            Comment x = Newtonsoft.Json.JsonConvert.DeserializeObject<Comment>(doc.ToJson());
                            if (x.movieTitle == movieResult.title)
                                comments.Add(x);
                        });
                    } while (!search.IsDone);

                    TwoModel models = new TwoModel();
                    models.movie = movieResult;
                    models.comments = comments;
                    return View(models);
                }

            }
            return NotFound();
            /* var movie = await _context.Movie.FirstOrDefaultAsync(m => m.id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);*/
        }

        // GET: Movies/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("id,title,rating,genre,director,year,fileName,uploadUser")] Movie movie)
        {
            movie.uploadUser = User.FindFirstValue(ClaimTypes.Name);
            Table movieTable = Table.LoadTable(client, movieTableName, DynamoDBEntryConversion.V2); //load the metadata of the table
            if (ModelState.IsValid)
            {
                Document newMovie = new Document();
                var random = new Random();
                newMovie["id"] = Int32.Parse(random.Next(0, 10000000).ToString());
                newMovie["title"] = movie.title;
                newMovie["rating"] = movie.rating;
                newMovie["genre"] = movie.genre;
                newMovie["director"] = movie.director;
                newMovie["year"] = movie.year;
                if (Int32.Parse(DateTime.Now.ToString("mmssff")) % 2 == 1)
                {
                    newMovie["fileName"] = "movie1.mkv";
                } else
                {
                    newMovie["fileName"] = "title2.mkv";
                }
                newMovie["uploadUser"] = movie.uploadUser;

                await movieTable.PutItemAsync(newMovie);


                /* SQL */
                /*_context.Add(movie);
                await _context.SaveChangesAsync();*/
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            // load full table whatever stupid
            Table movieTable = Table.LoadTable(client, movieTableName, DynamoDBEntryConversion.V2); //load the metadata of the table
            ScanFilter scanFilter = new ScanFilter();
            Search search = movieTable.Scan(scanFilter);
            List<Movie> movies = new List<Movie>();
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
                if (item.id == id)
                {
                    return View(item);
                }

            }

            return NotFound();
            /* var movie = await _context.Movie.FirstOrDefaultAsync(m => m.id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);*/
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("id,title,rating,genre,director,year,fileName,uploadUser")] Movie movie)
        {

            movie.uploadUser = User.FindFirstValue(ClaimTypes.Name);
            Table movieTable = Table.LoadTable(client, movieTableName, DynamoDBEntryConversion.V2); //load the metadata of the table
            //Document doc = await movieTable.GetItemAsync(movie.title);
            Document doc = new Document();
            doc["id"] = movie.id;
            doc["title"] = movie.title;
            doc["rating"] = movie.rating;
            doc["genre"] = movie.genre;
            doc["director"] = movie.director;
            doc["year"] = movie.year;
            doc["fileName"] = movie.fileName;
            doc["uploadUser"] = movie.uploadUser;


            if (id != movie.id)
            {
                return NotFound();
            }
                await movieTable.PutItemAsync(doc);
                /*try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }*/
                return RedirectToAction(nameof(Index));

            return View(movie);
        }

        // GET: Movies/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            // load full table whatever stupid
            Table movieTable = Table.LoadTable(client, movieTableName, DynamoDBEntryConversion.V2); //load the metadata of the table
            ScanFilter scanFilter = new ScanFilter();
            Search search = movieTable.Scan(scanFilter);
            List<Movie> movies = new List<Movie>();
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
                if (item.id == id)
                {
                    return View(item);
                }

            }
            return NotFound();
        }

        // POST: Movies/Delete/5
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
                if (item.id == id)
                {
                    title = item.title;
                }

            }

            var key = new Dictionary<string, AttributeValue>
            {
                ["id"] = new AttributeValue { N = id.ToString() },
                ["title"] = new AttributeValue { S = title }
            };

            var request = new DeleteItemRequest
            {
                TableName = movieTableName,
                Key = key,
            };

            var response = await client.DeleteItemAsync(request);

            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
          return (_context.Movie?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
