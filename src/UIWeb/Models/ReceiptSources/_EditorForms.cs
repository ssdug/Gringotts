using System.ComponentModel.DataAnnotations;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;

namespace Wiz.Gringotts.UIWeb.Models.ReceiptSources
{
    public class ReceiptSourceEditorForm
    {
        public int? ReceiptSourceId { get; set; }

        [Display(Name = "Name"), Required,
         MaxLength(255), MinLength(1)]
        public string Name { get; set; }

        public ReceiptSource BuildReceiptSource(ApplicationDbContext context)
        {
            return new ReceiptSource
            {
                Name = this.Name.TrimmedOrNull()
            };
        }

        public void UpdateReceiptSource(ReceiptSource category, ApplicationDbContext context)
        {
            category.Name = this.Name.TrimmedOrNull();
        }

        public static ReceiptSourceEditorForm FromReceiptSource(ReceiptSource source)
        {
            return new ReceiptSourceEditorForm
            {
                ReceiptSourceId = source.Id,
                Name = source.Name
            };
        }
    }
}