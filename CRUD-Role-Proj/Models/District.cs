using System;
using System.Collections.Generic;

namespace CRUD_Role_Proj.Models;

public partial class District
{
    public int Id { get; set; }

    public int GovernorateId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual Governorate Governorate { get; set; } = null!;

    public virtual ICollection<Village> Villages { get; set; } = new List<Village>();
}
