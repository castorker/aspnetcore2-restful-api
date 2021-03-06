﻿using DutchArtistsMasterpieces.Models;
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

        // http://localhost:50919/api/artists/1/artworks
        [HttpPost("{artistId}/artworks")]
        public IActionResult CreateArtwork(int artistId, [FromBody] ArtworkForCreationDto artwork)
        {
            if (artwork == null)
            {
                return BadRequest();
            }

            if (artwork.ShortDescription == artwork.Title)
            {
                ModelState.AddModelError("Short Description", "The provided short description should be different from the title.");
            }

            if (artwork.LongDescription == artwork.Title)
            {
                ModelState.AddModelError("Long Description", "The provided long description should be different from the title.");
            }

            if (artwork.LongDescription == artwork.ShortDescription)
            {
                ModelState.AddModelError("Long Description", "The provided long description should be different from the short description.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var artist = InMemoryDataStore.Current.Artists.FirstOrDefault(a => a.Id == artistId);

            if (artist == null)
            {
                return NotFound();
            }

            // get the next artwork Id - to be improved
            var maxArtworkId = InMemoryDataStore.Current.Artists.SelectMany(a => a.Artworks).Any() ? InMemoryDataStore.Current.Artists.SelectMany(a => a.Artworks).Max(i => i.Id) : 0;
            var nextArtworkId = ++maxArtworkId;

            var newArtwork = new ArtworkDto()
            {
                Id = nextArtworkId,
                Title = artwork.Title,
                Year = artwork.Year,
                ShortDescription = artwork.ShortDescription,
                LongDescription = artwork.LongDescription,
                ImageUrl = artwork.ImageUrl,
                ImageThumbnailUrl = artwork.ImageThumbnailUrl,
                Source = artwork.Source,
            };

            artist.Artworks.Add(newArtwork);

            return CreatedAtRoute("GetArtworkByIdForAnArtist", new { artistId, artworkId = newArtwork.Id }, newArtwork);
        }

        // Full Updating a Resource
        // http://localhost:50919/api/artists/6/artworks/1
        [HttpPut("{artistId}/artworks/{artworkId}")]
        public IActionResult UpdateArtwork(int artistId, int artworkId, [FromBody] ArtworkForUpdateDto artwork)
        {
            if (artwork == null)
            {
                return BadRequest();
            }

            if (artwork.ShortDescription == artwork.Title)
            {
                ModelState.AddModelError("Short Description", "The provided short description should be different from the title.");
            }

            if (artwork.LongDescription == artwork.Title)
            {
                ModelState.AddModelError("Long Description", "The provided long description should be different from the title.");
            }

            if (artwork.LongDescription == artwork.ShortDescription)
            {
                ModelState.AddModelError("Long Description", "The provided long description should be different from the short description.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var artist = InMemoryDataStore.Current.Artists.FirstOrDefault(a => a.Id == artistId);

            if (artist == null)
            {
                return NotFound();
            }

            var artworkToUpdate = artist.Artworks.FirstOrDefault(a => a.Id == artworkId);

            if (artworkToUpdate == null)
            {
                return NotFound();
            }

            artworkToUpdate.Title = artwork.Title;
            artworkToUpdate.Year = artwork.Year;
            artworkToUpdate.ShortDescription = artwork.ShortDescription;
            artworkToUpdate.LongDescription = artwork.LongDescription;
            artworkToUpdate.ImageUrl = artwork.ImageUrl;
            artworkToUpdate.ImageThumbnailUrl = artwork.ImageThumbnailUrl;
            artworkToUpdate.Source = artwork.Source;

            return NoContent();
        }

        // Partially Updating a Resource
        // http://localhost:50919/api/artists/6/artworks/16
        [HttpPatch("{artistId}/artworks/{artworkId}")]
        public IActionResult PartiallyUpdateArtwork(int artistId, int artworkId, [FromBody] JsonPatchDocument<ArtworkForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var artist = InMemoryDataStore.Current.Artists.FirstOrDefault(a => a.Id == artistId);

            if (artist == null)
            {
                return NotFound();
            }

            var artworkToUpdate = artist.Artworks.FirstOrDefault(a => a.Id == artworkId);

            if (artworkToUpdate == null)
            {
                return NotFound();
            }

            var artworkToPatch = new ArtworkForUpdateDto()
            {
                Title = artworkToUpdate.Title,
                Year = artworkToUpdate.Year,
                ShortDescription = artworkToUpdate.ShortDescription,
                LongDescription = artworkToUpdate.LongDescription,
                ImageUrl = artworkToUpdate.ImageUrl,
                ImageThumbnailUrl = artworkToUpdate.ImageThumbnailUrl,
                Source = artworkToUpdate.Source
            };

            patchDoc.ApplyTo(artworkToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (artworkToPatch.ShortDescription == artworkToPatch.Title)
            {
                ModelState.AddModelError("Short Description", "The provided short description should be different from the title.");
            }

            if (artworkToPatch.LongDescription == artworkToPatch.Title)
            {
                ModelState.AddModelError("Long Description", "The provided long description should be different from the title.");
            }

            if (artworkToPatch.LongDescription == artworkToPatch.ShortDescription)
            {
                ModelState.AddModelError("Long Description", "The provided long description should be different from the short description.");
            }

            TryValidateModel(artworkToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            artworkToUpdate.Title = artworkToPatch.Title;
            artworkToUpdate.Year = artworkToPatch.Year;
            artworkToUpdate.ShortDescription = artworkToPatch.ShortDescription;
            artworkToUpdate.LongDescription = artworkToPatch.LongDescription;
            artworkToUpdate.ImageUrl = artworkToPatch.ImageUrl;
            artworkToUpdate.ImageThumbnailUrl = artworkToPatch.ImageThumbnailUrl;
            artworkToUpdate.Source = artworkToPatch.Source;

            return NoContent();
        }

        // Delete a Resource
        // http://localhost:50919/api/artists/6/artworks/16
        [HttpDelete("{artistId}/artworks/{artworkId}")]
        public IActionResult DeleteArtwork(int artistId, int artworkId)
        {
            var artist = InMemoryDataStore.Current.Artists.FirstOrDefault(a => a.Id == artistId);

            if (artist == null)
            {
                return NotFound();
            }

            var artworkToDelete = artist.Artworks.FirstOrDefault(a => a.Id == artworkId);

            if (artworkToDelete == null)
            {
                return NotFound();
            }

            artist.Artworks.Remove(artworkToDelete);

            return NoContent();
        }
    }
}
