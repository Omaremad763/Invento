using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using Application.Contracts;
using Application.CQRS;
using Application.DTOS;

using Moq;

using NUnit.Framework;

namespace InventoTestProject
{
    [TestFixture]
    public class CategoryCQRSTest
    {
        //fack copy of the service to test the handlers without hitting
        //the actual database or business logic
        private Mock<IInventoServices> _serviceMock;
        
        // instance of the handler we want to test UUT unit under test
        private CategoryHandlers _handler;

        [SetUp]
        public void Setup()
        {
            // 1. Arrange: تجهيز الموك والـ Handler قبل كل تست
            _serviceMock = new Mock<IInventoServices>();
            _handler = new CategoryHandlers(_serviceMock.Object);
        }

        [Test]
        public async Task AddCategory_ShouldInvokeService_WhenCommandIsValid()
        {
            // Arrange
            var command = new AddCategoryCommand("New Category");
            _serviceMock.Setup(s => s.categoryService
            .AddCategoryAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.True);
            _serviceMock.Verify(s => s.categoryService
            .AddCategoryAsync("New Category"), Times.Once);
        }
        [Test]
        public void AddCategoryValidator_ShouldSuccess_WhenNameNotEmpty()
        {
            // Arrange
            var validator = new AddCateogryValidator();
            var command = new AddCategoryCommand(""); 

            // Act
            var result = validator.Validate(command);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.IsValid, Is.True);
                Assert.That(result.Errors
               .Any(e => e.PropertyName == "CategoryName"),
               Is.False);
            });
        }

        [Test]
        public async Task DeleteCategory_ShouldReturnFalse_WhenServiceFails()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var command = new DeleteCategoryCommand(categoryId);

            _serviceMock.Setup(s => s.categoryService
            .SoftDeleteCategoryAsync(categoryId))
            .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}