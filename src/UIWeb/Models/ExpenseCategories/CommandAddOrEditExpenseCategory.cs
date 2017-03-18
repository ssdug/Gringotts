using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.ExpenseCategories
{
    public class AddOrEditExpenseCategoryCommand : IAsyncRequest<ICommandResult>
    {
        public ExpenseCategoryEditorForm Editor { get; private set; }
        public ModelStateDictionary ModelState { get; private set; }

        public AddOrEditExpenseCategoryCommand(ExpenseCategoryEditorForm editor, ModelStateDictionary modelState)
        {
            this.Editor = editor;
            this.ModelState = modelState;
        }
    }

    public class ExpenseCategoryEditorFormValidatorHandler : CommandValidator<AddOrEditExpenseCategoryCommand>
    {
        private readonly ISearch<ExpenseCategory> categories;

        public ExpenseCategoryEditorFormValidatorHandler(ISearch<ExpenseCategory> categories)
        {
            this.categories = categories;
            Validators = new Action<AddOrEditExpenseCategoryCommand>[]
            {
                EnsureExpenseCategoryIsDistinct
            };
        }

        private void EnsureExpenseCategoryIsDistinct(AddOrEditExpenseCategoryCommand request)
        {
            Logger.Trace("EnsureExpenseCategoryIsDistinct");

            var duplicateCategories = categories.All()
                .Where(c => c.Id != request.Editor.ExpenseCategoryId)
                .Any(c => c.Name == request.Editor.Name);

            if (!duplicateCategories)
                return;

            request.ModelState.AddModelError("Name", "Duplicate Expense Category not allowed");
        }
    }

    public class AddOrEditExpenseCategoryCommandHandler : IAsyncRequestHandler<AddOrEditExpenseCategoryCommand, ICommandResult>
    {
        private readonly ISearch<ExpenseCategory> categories;
        private readonly ApplicationDbContext context;
        public ILogger Logger { get; set; }

        public AddOrEditExpenseCategoryCommandHandler(ISearch<ExpenseCategory> categories, ApplicationDbContext context)
        {
            this.categories = categories;
            this.context = context;
        }

        public async Task<ICommandResult> Handle(AddOrEditExpenseCategoryCommand message)
        {
            Logger.Trace("Handle");

            if (message.ModelState.NotValid())
                return new FailureResult("Validation Failed, please correct the errors below");

            if (message.Editor.ExpenseCategoryId.HasValue)
                return await Edit(message);

            return await Add(message);
        }

        private async Task<ICommandResult> Add(AddOrEditExpenseCategoryCommand message)
        {
            Logger.Trace("Add");

            var category = message.Editor.BuildExpenseCategory(context);

            context.ExpenseCategories.Add(category);

            await context.SaveChangesAsync();

            Logger.Info("Add::Success Id:{0}", category.Id);

            return new SuccessResult(category.Id);
        }

        private async Task<ICommandResult> Edit(AddOrEditExpenseCategoryCommand message)
        {
            Logger.Trace("Edit::{0}", message.Editor.ExpenseCategoryId);

            var category = await categories.GetById(message.Editor.ExpenseCategoryId.Value)
                .SingleAsync();

            message.Editor.UpdateExpenseCategory(category, context);

            await context.SaveChangesAsync();

            Logger.Info("Edit::Success Id:{0}", category.Id);

            return new SuccessResult(category.Id);
        }
    }
}