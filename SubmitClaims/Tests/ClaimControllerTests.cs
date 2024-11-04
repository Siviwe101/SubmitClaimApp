using Microsoft.AspNetCore.Mvc;
using Moq;
using SubmitClaims.Controllers;
using SubmitClaims.Data;
using SubmitClaims.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace SubmitClaims.Tests
{
    public class ClaimControllerTests
    {
        private readonly Mock<ApplicationDbContext> _mockDbContext;
        private readonly ClaimController _controller;

        public ClaimControllerTests()
        {
            _mockDbContext = new Mock<ApplicationDbContext>();
            Mock<IWebHostEnvironment> mockEnv = new();
            _controller = new ClaimController(_mockDbContext.Object, mockEnv.Object);
        }

        [Fact]
        public void SubmitClaim_Get_ReturnsViewResult_WithNewLecturerClaim()
        {
            // Act
            var result = _controller.SubmitClaim();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LecturerClaim>(viewResult.Model);
            Assert.NotNull(viewResult.ViewData["Claims"]); // Check that claims list is loaded
        }

        [Fact]
        public async Task SubmitClaim_Post_ValidModel_RedirectsToAction()
        {
            // Arrange
            var newClaim = new LecturerClaim
            {
                HoursWorked = 10,
                HourlyRate = 20,
                AdditionalNotes = "Test notes",
                Status = "Pending"
            };

            var mockDbSet = new Mock<DbSet<LecturerClaim>>();
            _mockDbContext.Setup(m => m.LecturerClaims).Returns(mockDbSet.Object);
            _mockDbContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _controller.SubmitClaim(newClaim, null);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SubmitClaim", redirectResult.ActionName);
            _mockDbContext.Verify(m => m.LecturerClaims.Add(newClaim), Times.Once);
            _mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task SubmitClaim_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("HoursWorked", "Required");
            var invalidClaim = new LecturerClaim { HourlyRate = 20, AdditionalNotes = "Test notes" };
            var mockDbSet = new Mock<DbSet<LecturerClaim>>();
            _mockDbContext.Setup(m => m.LecturerClaims).Returns(mockDbSet.Object);

            // Act
            var result = await _controller.SubmitClaim(invalidClaim, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LecturerClaim>(viewResult.Model);
            Assert.Same(invalidClaim, model); // Check the same model is returned
            Assert.NotNull(viewResult.ViewData["Claims"]); // Check that claims list is reloaded on error
        }
    }
}
