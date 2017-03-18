using System.ComponentModel.DataAnnotations;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;

namespace Wiz.Gringotts.UIWeb.Models.ExpenseCategories
{
    public class ExpenseCategoryEditorForm
    {
        public int? ExpenseCategoryId { get; set; }

        [Display(Name = "Name"), Required,
         MaxLength(255), MinLength(1)]
        public string Name { get; set; }

        public ExpenseCategory BuildExpenseCategory(ApplicationDbContext context)
        {
            return new ExpenseCategory
            {
                Name = this.Name.TrimmedOrNull()
            };
        }

        public void UpdateExpenseCategory(ExpenseCategory category, ApplicationDbContext context)
        {
            category.Name = this.Name.TrimmedOrNull();
        }

        public static ExpenseCategoryEditorForm FromExpenseCategory(ExpenseCategory category)
        {
            return new ExpenseCategoryEditorForm
            {
                ExpenseCategoryId = category.Id,
                Name = category.Name
            };
        }
    }
}