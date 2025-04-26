using System;
using Xunit;
using api.Models;
using api.Dtos.WorkOrder;
using api.Mappers;

namespace TaskApi.Tests.Mappers
{
    public class WorkOrderMapperTests
    {
        [Fact]
        public void ToWorkOrderDto_ShouldMapAllFieldsCorrectly()
        {
            // Arrange
            var workOrder = new WorkOrder
            {
                Id = 1,
                Title = "Test Title",
                Description = "Test Description",
                StartDate = new DateTime(2024, 5, 1),
                EndDate = new DateTime(2024, 5, 10),
                IsCompleted = true
            };

            // Act
            var dto = workOrder.ToWorkOrderDto();

            // Assert
            Assert.Equal(workOrder.Id, dto.Id);
            Assert.Equal(workOrder.Title, dto.Title);
            Assert.Equal(workOrder.Description, dto.Description);
            Assert.Equal(workOrder.StartDate, dto.StartDate);
            Assert.Equal(workOrder.EndDate, dto.EndDate);
            Assert.Equal(workOrder.IsCompleted, dto.IsCompleted);
        }

        [Fact]
        public void ToWorkOrderFromCreateDto_ShouldMapAllFieldsCorrectly()
        {
            // Arrange
            var createDto = new WorkOrderCreateRequestDto
            {
                Title = "Create Title",
                Description = "Create Description",
                EndDate = new DateTime(2024, 6, 15),
                IsCompleted = false
            };

            // Act
            var workOrder = createDto.ToWorkOrderFromCreateDto();

            // Assert
            Assert.Equal(createDto.Title, workOrder.Title);
            Assert.Equal(createDto.Description, workOrder.Description);
            Assert.Equal(createDto.EndDate, workOrder.EndDate);
            Assert.Equal(createDto.IsCompleted, workOrder.IsCompleted);
        }
    }
}
