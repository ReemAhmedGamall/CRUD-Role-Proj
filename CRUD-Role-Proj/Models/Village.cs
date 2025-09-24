using System;
using System.Collections.Generic;

namespace CRUD_Role_Proj.Models;

public partial class Village
{
    public int Id { get; set; }

    public int DistrictId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual District District { get; set; } = null!;
}
