using System;
using System.Globalization;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Binders
{
    [ModelBinderType(typeof(decimal))]
    [ModelBinderType(typeof(decimal?))]
    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };

            object actualValue = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(valueResult.AttemptedValue))
                    actualValue = Convert.ToDecimal(valueResult.AttemptedValue, CultureInfo.CurrentCulture);
            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);

            return bindingContext.ModelType.IsNullableDecimal() ? (decimal?) actualValue  : (decimal) actualValue;
        }
    }
}