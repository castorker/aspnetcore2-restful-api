using DutchArtistsMasterpieces.Models;
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

        [HttpPost()]
        public IActionResult CreateArtist([FromBody] ArtistForCreationDto artist)
        {
            if (artist == null)
            {
                return BadRequest();
            }

            if (artist.ShortDescription == artist.Name)
            {
                ModelState.AddModelError("Short Description", "The provided short description should be different from the name.");
            }

            if (artist.LongDescription == artist.Name)
            {
                ModelState.AddModelError("Long Description", "The provided long description should be different from the name.");
            }

            if (artist.LongDescription == artist.ShortDescription)
            {
                ModelState.AddModelError("Long Description", "The provided long description should be different from the short description.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // get the next artist Id - to be improved
            var maxArtistId = InMemoryDataStore.Current.Artists.Max(i => i.Id);
            var nextArtistId = ++maxArtistId;

            var newArtist = new ArtistDto()
            {
                Id = nextArtistId,
                Name = artist.Name,
                City = artist.City,
                BirthYear = artist.BirthYear,
                Birth = artist.Birth,
                DeathYear = artist.DeathYear,
                Death = artist.Death,
                ShortDescription = artist.ShortDescription,
                LongDescription = artist.LongDescription,
                ImageUrl = artist.ImageUrl,
                ImageThumbnailUrl = artist.ImageThumbnailUrl,
                IsArtistOfTheMonth = (artist.Birth == null) ? false : (artist.Birth.Month == DateTime.Now.Month ? true : false),
            };

            InMemoryDataStore.Current.Artists.Add(newArtist);

            return CreatedAtRoute("GetArtistById", new { artistId = newArtist.Id }, newArtist);
        }
    }
}
