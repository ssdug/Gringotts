using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Extensions
{
    public static class ModelStateExtensions
    {
        public static bool NotValid(this ModelStateDictionary state)
        {
            return !state.IsValid;
        }

        public static void AddModelError<TModel>(this ModelStateDictionary dictionary, Expression<Func<TModel, object>> expression, string errorMessage)
        {
            dictionary.AddModelError(ExpressionHelper.GetExpressionText(expression), errorMessage);
        }

        public static bool HasModelErrors<TModel>(this ModelStateDictionary dictionary,Expression<Func<TModel, object>> expression)
        {
            var fragment = ExpressionHelper.GetExpressionText(expression);

            return dictionary.Keys.Where(k => k.StartsWith(fragment))
                                  .Select(k => dictionary[k].Errors)
                                  .Any(e => e.Any());

        }
    }
}