using DutchArtistsMasterpieces.Models;
using DutchArtistsMasterpieces.Services;
using Microsoft.AspNetCore.JsonPatch;
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
            var maxArtistId = InMemoryDataStore.Current.Artists.Any() ? InMemoryDataStore.Current.Artists.Max(i => i.Id) : 0;
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

        // http://localhost:50919/api/artists/6
        [HttpPatch("{artistId}")]
        public IActionResult PartiallyUpdateArtist(int artistId, [FromBody] JsonPatchDocument<ArtistForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var artistToUpdate = InMemoryDataStore.Current.Artists.FirstOrDefault(a => a.Id == artistId);

            if (artistToUpdate == null)
            {
                return NotFound();
            }

            var artistToPatch = new ArtistForUpdateDto()
            {
                Name = artistToUpdate.Name,
                City = artistToUpdate.City,
                BirthYear = artistToUpdate.BirthYear,
                Birth = artistToUpdate.Birth,
                DeathYear = artistToUpdate.DeathYear,
                Death = artistToUpdate.Death,
                ShortDescription = artistToUpdate.ShortDescription,
                LongDescription = artistToUpdate.LongDescription,
                ImageUrl = artistToUpdate.ImageUrl,
                ImageThumbnailUrl = artistToUpdate.ImageThumbnailUrl,
                IsArtistOfTheMonth = artistToUpdate.IsArtistOfTheMonth
            };

            patchDoc.ApplyTo(artistToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (artistToPatch.ShortDescription == artistToPatch.Name)
            {
                ModelState.AddModelError("Short Description", "The provided short description should be different from the name.");
            }

            if (artistToPatch.LongDescription == artistToPatch.Name)
            {
                ModelState.AddModelError("Long Description", "The provided long description should be different from the name.");
            }

            if (artistToPatch.LongDescription == artistToPatch.ShortDescription)
            {
                ModelState.AddModelError("Long Description", "The provided long description should be different from the short description.");
            }

            TryValidateModel(artistToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            artistToUpdate.Name = artistToPatch.Name;
            artistToUpdate.City = artistToPatch.City;
            artistToUpdate.BirthYear = artistToPatch.BirthYear;
            artistToUpdate.Birth = artistToPatch.Birth;
            artistToUpdate.DeathYear = artistToPatch.DeathYear;
            artistToUpdate.Death = artistToPatch.Death;
            artistToUpdate.ShortDescription = artistToPatch.ShortDescription;
            artistToUpdate.LongDescription = artistToPatch.LongDescription;
            artistToUpdate.ImageUrl = artistToPatch.ImageUrl;
            artistToUpdate.ImageThumbnailUrl = artistToPatch.ImageThumbnailUrl;
            artistToUpdate.IsArtistOfTheMonth = (artistToPatch.Birth == null) ? false : (artistToPatch.Birth.Month == DateTime.Now.Month ? true : false);

            return NoContent();
        }

        // http://localhost:52797/api/artists/6
        [HttpDelete("{artistId}")]
        public IActionResult DeleteArtist(int artistId)
        {
            var artistToDelete = InMemoryDataStore.Current.Artists.FirstOrDefault(a => a.Id == artistId);

            if (artistToDelete == null)
            {
                return NotFound();
            }

            InMemoryDataStore.Current.Artists.Remove(artistToDelete);

            return NoContent();
        }
    }
}
