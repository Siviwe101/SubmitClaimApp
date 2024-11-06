using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SubmitClaims.Controllers;
using SubmitClaims.Data;
using SubmitClaims.Models;
using Xunit;

namespace SubmitClaims.Tests
{
    public class ClaimsControllerTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly Mock<IWebHostEnvironment> _envMock;
        private readonly Mock<ILogger<ClaimsController>> _loggerMock;
        private readonly ClaimsController _controller;

        public ClaimsControllerTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _envMock = new Mock<IWebHostEnvironment>();
            _loggerMock = new Mock<ILogger<ClaimsController>>();

            _controller = new ClaimsController(_contextMock.Object, _envMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithAListOfClaims()
        {
            // Arrange
            var claims = new List<LecturerClaim>
            {
                new LecturerClaim { Id = 1, LecturerId = 1, Status = "Pending" },
                new LecturerClaim { Id = 2, LecturerId = 1, Status = "Approved" }
            }.AsQueryable();

            var dbSetMock = new Mock<DbSet<LecturerClaim>>();
            dbSetMock.As<IQueryable<LecturerClaim>>().Setup(m => m.Provider).Returns(claims.Provider);
            dbSetMock.As<IQueryable<LecturerClaim>>().Setup(m => m.Expression).Returns(claims.Expression);
            dbSetMock.As<IQueryable<LecturerClaim>>().Setup(m => m.ElementType).Returns(claims.ElementType);
            dbSetMock.As<IQueryable<LecturerClaim>>().Setup(m => m.GetEnumerator()).Returns(claims.GetEnumerator());

            _contextMock.Setup(c => c.LecturerClaims).Returns(dbSetMock.Object);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<LecturerClaim>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Create_ValidModel_AddsClaimAndRedirects()
        {
            // Arrange
            var newClaim = new LecturerClaim
            {
                LecturerId = 1,
                HoursWorked = 10,
                HourlyRate = 15,
                AdditionalNotes = "Test note",
                SubmissionDate = DateTime.Today,
                Status = "Pending"
            };

            // Act
            var result = await _controller.Create(newClaim, null);

            // Assert
            _contextMock.Verify(m => m.Add(It.IsAny<LecturerClaim>()), Times.Once);
            _contextMock.Verify(m => m.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Edit_ValidIdAndModel_UpdatesClaimAndRedirects()
        {
            // Arrange
            var claim = new LecturerClaim { Id = 1, LecturerId = 1, Status = "Pending" };
            _contextMock.Setup(c => c.LecturerClaims.FindAsync(1)).ReturnsAsync(claim);

            var updatedClaim = new LecturerClaim
            {
                Id = 1,
                LecturerId = 2,
                HoursWorked = 20,
                HourlyRate = 20,
                AdditionalNotes = "Updated note",
                SubmissionDate = DateTime.Today,
                Status = "Approved"
            };

            // Act
            var result = await _controller.Edit(1, updatedClaim, null);

            // Assert
            _contextMock.Verify(m => m.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Delete_ExistingId_DeletesClaimAndRedirects()
        {
            // Arrange
            var claim = new LecturerClaim { Id = 1, LecturerId = 1, Status = "Pending" };
            _contextMock.Setup(c => c.LecturerClaims.FindAsync(1)).ReturnsAsync(claim);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            _contextMock.Verify(m => m.Remove(It.IsAny<LecturerClaim>()), Times.Once);
            _contextMock.Verify(m => m.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Approve_ValidId_UpdatesClaimStatusAndRedirects()
        {
            // Arrange
            var claim = new LecturerClaim { Id = 1, LecturerId = 1, Status = "Pending" };
            _contextMock.Setup(c => c.LecturerClaims.FindAsync(1)).ReturnsAsync(claim);

            // Act
            var result = await _controller.Approve(1);

            // Assert
            Assert.Equal("Approved", claim.Status);
            _contextMock.Verify(m => m.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageClaims", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Reject_ValidId_UpdatesClaimStatusAndRedirects()
        {
            // Arrange
            var claim = new LecturerClaim { Id = 1, LecturerId = 1, Status = "Pending" };
            _contextMock.Setup(c => c.LecturerClaims.FindAsync(1)).ReturnsAsync(claim);

            // Act
            var result = await _controller.Reject(1);

            // Assert
            Assert.Equal("Rejected", claim.Status);
            _contextMock.Verify(m => m.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageClaims", redirectToActionResult.ActionName);
        }
    }
}
