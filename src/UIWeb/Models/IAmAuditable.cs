using System;

namespace Wiz.Gringotts.UIWeb.Models
{
    public interface IAmAuditable
    {
        DateTime Created { get; set; }
        string CreatedBy { get; set; }
        DateTime Updated { get; set; }
        string UpdatedBy { get; set; }
    }
}