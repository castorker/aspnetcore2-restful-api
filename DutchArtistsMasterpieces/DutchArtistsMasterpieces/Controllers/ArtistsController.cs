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

        // http://localhost:50919/api/artists
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

        // http://localhost:50919/api/artists/6
        [HttpPut("{artistId}")]
        public IActionResult UpdateArtist(int artistId, [FromBody] ArtistForUpdateDto artist)
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

            var artistToUpdate = InMemoryDataStore.Current.Artists.FirstOrDefault(a => a.Id == artistId);

            if (artistToUpdate == null)
            {
                return NotFound();
            }

            artistToUpdate.Name = artist.Name;
            artistToUpdate.City = artist.City;
            artistToUpdate.BirthYear = artist.BirthYear;
            artistToUpdate.Birth = artist.Birth;
            artistToUpdate.DeathYear = artist.DeathYear;
            artistToUpdate.Death = artist.Death;
            artistToUpdate.ShortDescription = artist.ShortDescription;
            artistToUpdate.LongDescription = artist.LongDescription;
            artistToUpdate.ImageUrl = artist.ImageUrl;
            artistToUpdate.ImageThumbnailUrl = artist.ImageThumbnailUrl;
            artistToUpdate.IsArtistOfTheMonth = (artist.Birth == null) ? false : (artist.Birth.Month == DateTime.Now.Month ? true : false);

            return NoContent();
        }
    }
}
