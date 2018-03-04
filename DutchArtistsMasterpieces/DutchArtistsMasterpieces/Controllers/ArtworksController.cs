using DutchArtistsMasterpieces.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchArtistsMasterpieces.Controllers
{
    [Route("/api/artists")]
    public class ArtworksController : Controller
    {
        // http://localhost:50919/api/artists/1/artworks
        [HttpGet("{artistId}/artworks")]
        public IActionResult GetArtworksForAnArtist(int artistId)
        {
            try
            {
                var artist = InMemoryDataStore.Current.Artists.FirstOrDefault(a => a.Id == artistId);

                if (artist == null)
                {
                    return NotFound();
                }

                return Ok(artist.Artworks);
            }
            catch (Exception e)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        // http://localhost:50919/api/artists/1/artworks/1
        [HttpGet("{artistId}/artworks/{artworkId}", Name = "GetArtworkByIdForAnArtist")]
        public IActionResult GetArtworkByIdForAnArtist(int artistId, int artworkId)
        {
            var artist = InMemoryDataStore.Current.Artists.FirstOrDefault(a => a.Id == artistId);

            if (artist == null)
            {
                return NotFound();
            }

            var artwork = artist.Artworks.FirstOrDefault(a => a.Id == artworkId);

            if (artwork == null)
            {
                return NotFound();
            }

            return Ok(artwork);
        }
    }
}
