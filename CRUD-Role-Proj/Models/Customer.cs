using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRUD_Role_Proj.Models;

public partial class Customer
{
    public int Id { get; set; }

    [RegularExpression(@"^[\u0621-\u064Aa-zA-Z\s]+$", ErrorMessage = "Full Name must contain letters only")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "National ID is required.")]
    [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be 14 numbers only.")]
    public string NationalId { get; set; } = null!;

    public int? GovernorateId { get; set; }

    public int? DistrictId { get; set; }

    public int? VillageId { get; set; }

    public int? GenderId { get; set; }

    [Range(5000, 20000, ErrorMessage = "Salary must be from 5000 to 20000")]
    public int? Salary { get; set; }

    public DateTime? BirthDate { get; set; }

    public int? Age { get; set; }

    public virtual District? District { get; set; }

    public virtual Gender? Gender { get; set; }

    public virtual Governorate? Governorate { get; set; }

    public virtual Village? Village { get; set; }
}
