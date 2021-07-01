using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ESFA.DC.Job.WebApi.ModelBinders
{
    public class DateTimeModelBinderProvider : IModelBinderProvider
    {
        // You could make this a property to allow customization
        internal static readonly DateTimeStyles SupportedStyles = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AllowWhiteSpaces;

        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var modelType = context.Metadata.UnderlyingOrModelType;
            if (modelType == typeof(DateTime))
            {
                return new UtcAwareDateTimeModelBinder(SupportedStyles);
            }

            return null;
        }
    }
}
