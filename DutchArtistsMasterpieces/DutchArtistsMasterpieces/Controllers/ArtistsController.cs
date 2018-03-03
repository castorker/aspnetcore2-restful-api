using DutchArtistsMasterpieces.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchArtistsMasterpieces.Controllers
{
    [Route("/api/artists")]
    public class ArtistsController : Controller
    {
        // http://localhost:50919/api/artists
        [HttpGet()]
        public IActionResult GetArtists()
        {
            return Ok(InMemoryDataStore.Current.Artists.OrderBy(a => a.Id));
        }

        // http://localhost:50919/api/artists/1
        [HttpGet("{artistId}", Name = "GetArtistById")]
        public IActionResult GetArtistById(int artistId)
        {
            var artistToReturn = InMemoryDataStore.Current.Artists.FirstOrDefault(a => a.Id == artistId);

            if (artistToReturn == null)
            {
                return NotFound();
            }

            return Ok(artistToReturn);
        }
    }
}
