﻿namespace StaffManagementSystem.Api.Domain.DTOs
{
    public class UpdateKadrDto
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
