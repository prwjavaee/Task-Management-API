using System;
using System.ComponentModel.DataAnnotations;

namespace api.Dtos.WorkOrder
{
    public class WorkOrderCreateRequestDto
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Title cannot be more than 10 characters")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public bool IsCompleted { get; set; } = false;

    }
}
