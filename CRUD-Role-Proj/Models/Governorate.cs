using System;
using System.Collections.Generic;

namespace CRUD_Role_Proj.Models;

public partial class Governorate
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}
