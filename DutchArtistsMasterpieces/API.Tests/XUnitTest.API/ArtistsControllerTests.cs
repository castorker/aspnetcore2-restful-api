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
    [Collection("Artists Controller Tests Collection")]
    public class ArtistsControllerTests
    {
        private Mock<IHostingEnvironment> _mockEnvironment;
        private ArtistsController _cutArtists;

        public ArtistsControllerTests()
        {
            _mockEnvironment = new Mock<IHostingEnvironment>();

            //...Setup the mock as needed
            _mockEnvironment
                .Setup(m => m.EnvironmentName)
                .Returns("Hosting:UnitTestEnvironment");
            //...other setup for mocked IHostingEnvironment...

            _cutArtists = new ArtistsController();
        }

        // In Memory Data Store initialization: Artists = 10; Artworks = 3 per Artist

        // 1. GetArtists method - get all artists
        [Fact]
        public void TestGetArtists()
        {
            // Arrange
            //... done in the constructor

            // Act
            IActionResult actionResult = _cutArtists.GetArtists();

            // Assert
            Assert.NotNull(actionResult);

            OkObjectResult result = actionResult as OkObjectResult;

            Assert.NotNull(result);

            List<ArtistDto> artists = result.Value as List<ArtistDto>;

            Assert.Equal(5, artists.Count);
            Assert.Equal("Rembrandt van Rijn", artists[0].Name);
            Assert.Equal("Vincent van Gogh", artists[1].Name);
            Assert.Equal("Maurits Cornelis Escher", artists[2].Name);
            Assert.Equal("Piet Mondrian", artists[3].Name);
            Assert.Equal("Johannes Vermeer", artists[4].Name);
        }

        // 2. GetArtistById method - get an existing Artist by Id
        [Theory,
            InlineData(new object[] { 2, "Vincent van Gogh", "Zundert", 1853 })]
        public void TestGetAnExistingArtistById(int artistId, string name, string city, int birthYear)
        {
            // Arrange
            //... done in the constructor

            // Act
            IActionResult actionResult = _cutArtists.GetArtistById(artistId);

            // Assert
            Assert.NotNull(actionResult);

            OkObjectResult result = actionResult as OkObjectResult;

            Assert.NotNull(result);

            ArtistDto artist = result.Value as ArtistDto;

            Assert.Equal(artistId, artist.Id);
            Assert.Equal(name, artist.Name);
            Assert.Equal(city, artist.City);
            Assert.Equal(birthYear, artist.BirthYear);
        }

        // 3. GetArtistById method - get a nonexisting Artist by Id
        [Theory,
            InlineData(999)]
        public void TestGetANonExistingArtistById(int artistId)
        {
            // Arrange
            //... done in the constructor

            // Act
            IActionResult actionResult = _cutArtists.GetArtistById(artistId);

            // Assert
            NotFoundResult result = actionResult as NotFoundResult;

            Assert.Equal(404, result.StatusCode);
        }

        // 4. CreateArtist method - create a new Artist
        [Fact]
        public void TestCreateArtist()
        {
            // Arrange
            ArtistForCreationDto artist = new ArtistForCreationDto
            {
                Name = "Jheronimus Bosch",
                City = "Hertogenbosch",
                BirthYear = 1450,
                Birth = new DateTime(1568, 02, 25),
                DeathYear = 1516,
                Death = new DateTime(1890, 07, 29),
                ShortDescription = "Was a Dutch/Netherlandish draughtsman and painter from Brabant.",
                LongDescription = "He is widely considered one of the most notable representatives of Early Netherlandish painting school. His work is known for its fantastic imagery, detailed landscapes, and illustrations of religious concepts and narratives. Within his lifetime his work was collected in the Netherlands, Austria, and Spain, and widely copied, especially his macabre and nightmarish depictions of hell.",
                ImageUrl = "/images/Artists/Jheronimus-Bosch.jpg",
                ImageThumbnailUrl = "/images/Artists/Jheronimus-Bosch_tn.jpg"
            };

            // Act
            IActionResult actionResult = _cutArtists.CreateArtist(artist);

            // Assert
            Assert.NotNull(actionResult);
            CreatedAtRouteResult result = actionResult as CreatedAtRouteResult;

            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
        }

        // 5. UpdateArtist method - fully update an existing Artist
        [Theory, InlineData(1)]
        public void TestFullyUpdateAnExistingArtist(int artistId)
        {
            // Arrange
            ArtistForUpdateDto artist = new ArtistForUpdateDto
            {
                Name = "Updated - Artist",
                City = "Updated - City",
                BirthYear = 1900,
                Birth = new DateTime(1900, 01, 01),
                DeathYear = 2017,
                Death = new DateTime(2017, 12, 31),
                ShortDescription = "Updated - Short Description.",
                LongDescription = "Updated - Long Description.",
                ImageUrl = "/images/Artists/Updated-Artist.jpg",
                ImageThumbnailUrl = "/images/Artists/Updated-Artist_tn.jpg",
            };

            // Act
            IActionResult actionResult = _cutArtists.UpdateArtist(artistId, artist);

            // Assert
            Assert.NotNull(actionResult);
            NoContentResult result = actionResult as NoContentResult;

            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
        }

        // 6. UpdateArtist method - fully update a nonexisting Artist
        [Theory, InlineData(999)]
        public void TestFullyUpdateANonExistingArtist(int artistId)
        {
            // Arrange
            ArtistForUpdateDto artist = new ArtistForUpdateDto
            {
                Name = "Updated - Artist",
                City = "Updated - City",
                BirthYear = 1900,
                Birth = new DateTime(1900, 01, 01),
                DeathYear = 2017,
                Death = new DateTime(2017, 12, 31),
                ShortDescription = "Updated - Short Description.",
                LongDescription = "Updated - Long Description.",
                ImageUrl = "/images/Artists/Updated-Artist.jpg",
                ImageThumbnailUrl = "/images/Artists/Updated-Artist_tn.jpg",
            };

            // Act
            IActionResult actionResult = _cutArtists.UpdateArtist(artistId, artist);

            // Assert
            NotFoundResult result = actionResult as NotFoundResult;

            Assert.Equal(404, result.StatusCode);
        }

        // 7. PartiallyUpdateArtist method - partially update an existing Artist
        [Theory, InlineData(1)]
        public void TestPartiallyUpdateAnExistingArtist(int artistId)
        {
            // Arrange
            JsonPatchDocument<ArtistForUpdateDto> patchDoc = new JsonPatchDocument<ArtistForUpdateDto>();
            patchDoc.Replace(o => o.Name, "Partially Updated through PATCH - Name");    // replace Name Property with value "Partially Updated through PATCH - Name"
            patchDoc.Replace(o => o.BirthYear, 1950);
            patchDoc.Replace(o => o.Birth, new DateTime(1950, 01, 25));
            patchDoc.Replace(o => o.ShortDescription, "Partially Updated through PATCH - Short Description");
            patchDoc.Replace(o => o.LongDescription, "Partially Updated through PATCH - Long Description");

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(
                It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));
            _cutArtists.ObjectValidator = objectValidator.Object;

            // Act
            IActionResult actionResult = _cutArtists.PartiallyUpdateArtist(artistId, patchDoc);

            // Assert
            Assert.NotNull(actionResult);
            NoContentResult result = actionResult as NoContentResult;

            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
        }

        // 8. PartiallyUpdateArtist method - partially update a nonexisting Artist
        [Theory, InlineData(999)]
        public void TestPartiallyUpdateANonExistingArtist(int artistId)
        {
            // Arrange
            JsonPatchDocument<ArtistForUpdateDto> patchDoc = new JsonPatchDocument<ArtistForUpdateDto>();
            patchDoc.Replace(o => o.Name, "Partially Updated through PATCH - Name");    // replace Name Property with value "Partially Updated through PATCH - Name"
            patchDoc.Replace(o => o.BirthYear, 1950);
            patchDoc.Replace(o => o.Birth, new DateTime(1950, 01, 25));
            patchDoc.Replace(o => o.ShortDescription, "Partially Updated through PATCH - Short Description");
            patchDoc.Replace(o => o.LongDescription, "Partially Updated through PATCH - Long Description");

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(
                It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));
            _cutArtists.ObjectValidator = objectValidator.Object;

            // Act
            IActionResult actionResult = _cutArtists.PartiallyUpdateArtist(artistId, patchDoc);

            // Assert
            NotFoundResult result = actionResult as NotFoundResult;

            Assert.Equal(404, result.StatusCode);
        }

        // 9. DeleteArtist method - delete an existing Artist
        [Theory, InlineData(5)]
        public void TestDeleteAnExistingArtist(int artistId)
        {
            // Arrange
            //... done in the constructor

            // Act
            IActionResult actionResult = _cutArtists.DeleteArtist(artistId);

            // Assert
            Assert.NotNull(actionResult);
            NoContentResult result = actionResult as NoContentResult;

            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
        }

        // 10. DeleteArtist method - delete a nonexisting Artist
        [Theory, InlineData(999)]
        public void TestDeleteANonExistingArtist(int artistId)
        {
            // Arrange
            //... done in the constructor

            // Act
            IActionResult actionResult = _cutArtists.DeleteArtist(artistId);

            // Assert
            NotFoundResult result = actionResult as NotFoundResult;

            Assert.Equal(404, result.StatusCode);
        }
    }
}
