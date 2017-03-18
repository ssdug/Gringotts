using System;
using System.Web.Mvc;

namespace Wiz.Gringotts.UIWeb.Binders
{
    public abstract class PolymorphicModelBinder : DefaultModelBinder, IModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var typeValue = bindingContext.ValueProvider.GetValue("ConcreteModelType");
            var type = Type.GetType(
                (string)typeValue.ConvertTo(typeof(string)),
                true
            );
            if (!GetBaseType().IsAssignableFrom(type))
            {
                throw new InvalidOperationException("Bad Type");
            }
            var model = Activator.CreateInstance(type);
            bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, type);

            return base.BindModel(controllerContext, bindingContext);
        }
        protected abstract Type GetBaseType();
    }
}