using DutchArtistsMasterpieces.Controllers;
using DutchArtistsMasterpieces.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XUnitTest.API
{
    [Collection("Artworks Controller Tests Collection")]
    public class ArtworksControllerTests
    {
        private Mock<IHostingEnvironment> _mockEnvironment;
        private ArtworksController _cutArtworks;

        public ArtworksControllerTests()
        {
            _mockEnvironment = new Mock<IHostingEnvironment>();

            //...Setup the mock as needed
            _mockEnvironment
                .Setup(m => m.EnvironmentName)
                .Returns("Hosting:UnitTestEnvironment");
            //...other setup for mocked IHostingEnvironment...

            _cutArtworks = new ArtworksController();
        }

        // 1. GetArtworksForAnArtist method - get all Artworks for an existing Artist - should be 3
        [Theory, InlineData(2)]
        public void TestGetArtworksForAnExistingArtist(int artistId)
        {
            // Arrange
            //... done in the constructor

            // Act
            IActionResult actionResult = _cutArtworks.GetArtworksForAnArtist(artistId);

            // Assert
            Assert.NotNull(actionResult);

            OkObjectResult result = actionResult as OkObjectResult;

            Assert.NotNull(result);

            List<ArtworkDto> artworks = result.Value as List<ArtworkDto>;

            Assert.Equal(3, artworks.Count);
            Assert.Equal("Vase with Fifteen Sunflowers", artworks[0].Title);
            Assert.Equal("Partially Updated through PATCH - Title", artworks[1].Title);
            Assert.Equal("The Starry Night", artworks[2].Title);
        }

        // 2. GetArtworksForAnArtist method - get all Artworks for a nonexisting Artist
        [Theory, InlineData(999)]
        public void TestGetArtworksForANonExistingArtist(int artistId)
        {
            // Arrange
            //... done in the constructor

            // Act
            IActionResult actionResult = _cutArtworks.GetArtworksForAnArtist(artistId);

            // Assert
            NotFoundResult result = actionResult as NotFoundResult;

            Assert.Equal(404, result.StatusCode);
        }

        // 3. GetArtworkByIdForAnArtist method - get an Artwork by Id for an existing Artist
        [Theory,
            InlineData(new object[] { 2, 4, "Vase with Fifteen Sunflowers", 1888, "www.nationalgallery.org.uk" })]
        public void TestGetArtworkByIdForAnArtist(int artistId, int artworkId, string title, int year, string source)
        {
            // Arrange
            //... done in the constructor

            // Act
            IActionResult actionResult = _cutArtworks.GetArtworkByIdForAnArtist(artistId, artworkId);

            // Assert
            Assert.NotNull(actionResult);

            OkObjectResult result = actionResult as OkObjectResult;

            Assert.NotNull(result);
            ArtworkDto artwork = result.Value as ArtworkDto;

            Assert.Equal(title, artwork.Title);
            Assert.Equal(year, artwork.Year);
            Assert.Equal(source, artwork.Source);
        }

        // 4. GetArtworkByIdForAnArtist method - get an Artwork by Id for a nonexisting Artist
        [Theory,
            InlineData(new object[] { 999, 4 })]
        public void TestGetArtworkByIdForANonExistentArtist(int artistId, int artworkId)
        {
            // Arrange
            //... done in the constructor

            // Act
            IActionResult actionResult = _cutArtworks.GetArtworkByIdForAnArtist(artistId, artworkId);

            // Assert
            NotFoundResult result = actionResult as NotFoundResult;

            Assert.Equal(404, result.StatusCode);
        }

        // 5. CreateArtwork method - create an Artwork for an existing Artist
        [Theory,
            InlineData(2)]
        public void TestCreateArtworkForAnExistingArtist(int artistId)
        {
            // Arrange
            ArtworkForCreationDto artwork = new ArtworkForCreationDto
            {
                Title = "Bedroom in Arles",
                Year = 1888,
                ShortDescription = "Van Gogh's own title for this composition was simply The Bedroom (French: La Chambre à coucher). There are three authentic versions described in his letters, easily discernible from one another by the pictures on the wall to the right.",
                LongDescription = "The painting depicts van Gogh's bedroom at 2, Place Lamartine in Arles, Bouches-du-Rhône, France, known as the Yellow House. The door to the right opened on to the upper floor and the staircase; the door to the left was that of the guest room he held prepared for Gauguin; the window in the front wall looked on to Place Lamartine and its public gardens. This room was not rectangular but trapezoid with an obtuse angle in the left hand corner of the front wall and an acute angle at the right.",
                ImageUrl = "/images/Artworks/02-Vincent-van-Gogh/1888-Bedroom-in-Arles.jpg",
                ImageThumbnailUrl = "/images/Artworks/02-Vincent-van-Gogh/1888-Bedroom-in-Arles_tn.jpg",
                Source = "www.wikipedia.org"
            };

            // Act
            IActionResult actionResult = _cutArtworks.CreateArtwork(artistId, artwork);

            // Assert
            Assert.NotNull(actionResult);
            CreatedAtRouteResult result = actionResult as CreatedAtRouteResult;

            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
        }

        // 6. CreateArtwork method - create an Artwork for a nonexisting Artist
        [Theory,
            InlineData(999)]
        public void TestCreateArtworkForANonExistingArtist(int artistId)
        {
            // Arrange
            ArtworkForCreationDto artwork = new ArtworkForCreationDto
            {
                Title = "The Garden of Earthly Delights",
                Year = 1505,
                ShortDescription = "Is the modern title given to a triptych oil painting on oak panel painted by the Early Netherlandish master Hieronymus Bosch, housed in the Museo del Prado in Madrid since 1939. It dates from between 1490 and 1510, when Bosch was between 40 and 60 years old.",
                LongDescription = "As so little is known of Bosch's life or intentions, interpretations of his intent have ranged from an admonition of worldly fleshy indulgence, to a dire warning on the perils of life's temptations, to an evocation of ultimate sexual joy. The intricacy of its symbolism, particularly that of the central panel, has led to a wide range of scholarly interpretations over the centuries. Twentieth-century art historians are divided as to whether the triptych's central panel is a moral warning or a panorama of paradise lost. Peter S. Beagle describes it as an 'erotic derangement that turns us all into voyeurs, a place filled with the intoxicating air of perfect liberty'.",
                ImageUrl = "/images/Artworks/999-Jheronimus-Bosch/1505-The-Garden-of-Earthly-Delights.jpg",
                ImageThumbnailUrl = "/images/Artworks/999-Jheronimus-Bosch/1505-The-Garden-of-Earthly-Delights_tn.jpg",
                Source = "www.wikipedia.org"
            };

            // Act
            IActionResult actionResult = _cutArtworks.CreateArtwork(artistId, artwork);

            // Assert
            NotFoundResult result = actionResult as NotFoundResult;

            Assert.Equal(404, result.StatusCode);
        }

        // 7. UpdateArtwork method - fully update an existing Artwork for an existing Artist
        [Theory,
            InlineData(new object[] { 2, 5 })]
        public void TestFullyUpdateAnExistingArtworkForAnExistingArtist(int artistId, int artworkId)
        {
            // Arrange
            ArtworkForUpdateDto artwork = new ArtworkForUpdateDto
            {
                Title = "Updated - New Title",
                Year = 2017,
                ShortDescription = "Updated - Short Description",
                LongDescription = "Updated - Long Description",
                ImageUrl = "/images/Artworks/02-Vincent-van-Gogh/2017-New-Title.jpg",
                ImageThumbnailUrl = "/images/Artworks/02-Vincent-van-Gogh/2017-New-Title_tn.jpg",
                Source = "www.wikipedia.org"
            };

            // Act
            IActionResult actionResult = _cutArtworks.UpdateArtwork(artistId, artworkId, artwork);

            // Assert
            Assert.NotNull(actionResult);
            NoContentResult result = actionResult as NoContentResult;

            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
        }

        // 8. UpdateArtwork method - fully update a nonexisting Artwork for an existing Artist
        [Theory,
            InlineData(new object[] { 2, 999 })]
        public void TestFullyUpdateANonExistingArtworkForAnExistingArtist(int artistId, int artworkId)
        {
            // Arrange
            ArtworkForUpdateDto artwork = new ArtworkForUpdateDto
            {
                Title = "Updated - New Title",
                Year = 2017,
                ShortDescription = "Updated - Short Description",
                LongDescription = "Updated - Long Description",
                ImageUrl = "/images/Artworks/02-Vincent-van-Gogh/2017-New-Title.jpg",
                ImageThumbnailUrl = "/images/Artworks/02-Vincent-van-Gogh/2017-New-Title_tn.jpg",
                Source = "www.wikipedia.org"
            };

            // Act
            IActionResult actionResult = _cutArtworks.UpdateArtwork(artistId, artworkId, artwork);

            // Assert
            NotFoundResult result = actionResult as NotFoundResult;

            Assert.Equal(404, result.StatusCode);
        }

        // 9. UpdateArtwork method - fully update an Artwork for a nonexisting Artist
        [Theory,
            InlineData(new object[] { 999, 1 })]
        public void TestFullyUpdateAnArtworkForANonExistingArtist(int artistId, int artworkId)
        {
            // Arrange
            ArtworkForUpdateDto artwork = new ArtworkForUpdateDto
            {
                Title = "Updated - New Title",
                Year = 2017,
                ShortDescription = "Updated - Short Description",
                LongDescription = "Updated - Long Description",
                ImageUrl = "/images/Artworks/02-Vincent-van-Gogh/2017-New-Title.jpg",
                ImageThumbnailUrl = "/images/Artworks/02-Vincent-van-Gogh/2017-New-Title_tn.jpg",
                Source = "www.wikipedia.org"
            };

            // Act
            IActionResult actionResult = _cutArtworks.UpdateArtwork(artistId, artworkId, artwork);

            // Assert
            NotFoundResult result = actionResult as NotFoundResult;

            Assert.Equal(404, result.StatusCode);
        }

        // 10. PartiallyUpdateArtwork method - partially update an existing Artwork for an existing Artist
        [Theory,
            InlineData(new object[] { 2, 5 })]
        public void TestPartiallyUpdateAnExistingArtworkForAnExistingArtist(int artistId, int artworkId)
        {
            // Arrange
            JsonPatchDocument<ArtworkForUpdateDto> patchDoc = new JsonPatchDocument<ArtworkForUpdateDto>();
            patchDoc.Replace(o => o.Title, "Partially Updated through PATCH - Title");    // replace Name Property with value "Partially Updated through PATCH - Name"
            patchDoc.Replace(o => o.Year, 2017);
            patchDoc.Replace(o => o.ShortDescription, "Partially Updated through PATCH - Short Description");
            patchDoc.Replace(o => o.LongDescription, "Partially Updated through PATCH - Long Description");

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(
                It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));
            _cutArtworks.ObjectValidator = objectValidator.Object;

            // Act
            IActionResult actionResult = _cutArtworks.PartiallyUpdateArtwork(artistId, artworkId, patchDoc);

            // Assert
            Assert.NotNull(actionResult);
            NoContentResult result = actionResult as NoContentResult;

            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
        }

        // 11. PartiallyUpdateArtwork method - partially update a nonexisting Artwork for an existing Artist
        [Theory,
            InlineData(new object[] { 2, 999 })]
        public void TestPartiallyUpdateANonExistingArtworkForAnExistingArtist(int artistId, int artworkId)
        {
            // Arrange
            JsonPatchDocument<ArtworkForUpdateDto> patchDoc = new JsonPatchDocument<ArtworkForUpdateDto>();
            patchDoc.Replace(o => o.Title, "Partially Updated through PATCH - Title");    // replace Name Property with value "Partially Updated through PATCH - Name"
            patchDoc.Replace(o => o.Year, 2017);
            patchDoc.Replace(o => o.ShortDescription, "Partially Updated through PATCH - Short Description");
            patchDoc.Replace(o => o.LongDescription, "Partially Updated through PATCH - Long Description");

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(
                It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));
            _cutArtworks.ObjectValidator = objectValidator.Object;

            // Act
            IActionResult actionResult = _cutArtworks.PartiallyUpdateArtwork(artistId, artworkId, patchDoc);

            // Assert
            NotFoundResult result = actionResult as NotFoundResult;

            Assert.Equal(404, result.StatusCode);
        }

        // 12. PartiallyUpdateArtwork method - partially update an Artwork for a nonexisting Artist
        [Theory,
            InlineData(new object[] { 999, 1 })]
        public void TestPartiallyUpdateAnArtworkForANonExistingArtist(int artistId, int artworkId)
        {
            // Arrange
            JsonPatchDocument<ArtworkForUpdateDto> patchDoc = new JsonPatchDocument<ArtworkForUpdateDto>();
            patchDoc.Replace(o => o.Title, "Partially Updated through PATCH - Title");    // replace Name Property with value "Partially Updated through PATCH - Name"
            patchDoc.Replace(o => o.Year, 2017);
            patchDoc.Replace(o => o.ShortDescription, "Partially Updated through PATCH - Short Description");
            patchDoc.Replace(o => o.LongDescription, "Partially Updated through PATCH - Long Description");

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(
                It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));
            _cutArtworks.ObjectValidator = objectValidator.Object;

            // Act
            IActionResult actionResult = _cutArtworks.PartiallyUpdateArtwork(artistId, artworkId, patchDoc);

            // Assert
            NotFoundResult result = actionResult as NotFoundResult;

            Assert.Equal(404, result.StatusCode);
        }

        // 13. DeleteArtwork method - delete an existing Artwork for an existing Artist
        [Theory, InlineData(3, 7)]
        public void TestDeleteAnExistingArtworkForAnExistingArtist(int artistId, int artworkId)
        {
            // Arrange
            //... done in the constructor

            // Act
            IActionResult actionResult = _cutArtworks.DeleteArtwork(artistId, artworkId);

            // Assert
            Assert.NotNull(actionResult);
            NoContentResult result = actionResult as NoContentResult;

            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
        }

        // 14. DeleteArtwork method - delete a nonexisting Artwork for an existing Artist
        [Theory, InlineData(3, 1)]
        public void TestDeleteANonExistingArtworkForAnExistingArtist(int artistId, int artworkId)
        {
            // Arrange
            //... done in the constructor

            // Act
            IActionResult actionResult = _cutArtworks.DeleteArtwork(artistId, artworkId);

            // Assert
            NotFoundResult result = actionResult as NotFoundResult;

            Assert.Equal(404, result.StatusCode);
        }

        // 15. DeleteArtwork method - delete an Artwork for a nonexisting Artist
        [Theory, InlineData(999, 1)]
        public void TestDeleteAnArtworkForANonExistingArtist(int artistId, int artworkId)
        {
            // Arrange
            //... done in the constructor

            // Act
            IActionResult actionResult = _cutArtworks.DeleteArtwork(artistId, artworkId);

            // Assert
            NotFoundResult result = actionResult as NotFoundResult;

            Assert.Equal(404, result.StatusCode);
        }
    }
}
