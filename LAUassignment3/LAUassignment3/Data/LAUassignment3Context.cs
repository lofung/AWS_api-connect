using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LAUassignment3.Models;

namespace LAUassignment3.Data
{
    public class LAUassignment3Context : DbContext
    {
        public LAUassignment3Context (DbContextOptions<LAUassignment3Context> options)
            : base(options)
        {
        }

        public DbSet<LAUassignment3.Models.Movie> Movie { get; set; } = default!;
        public DbSet<LAUassignment3.Models.Comment> Comment { get; set; } = default!;
    }
}
