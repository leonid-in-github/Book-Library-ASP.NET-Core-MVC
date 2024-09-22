using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace BookLibrary.WebServer.Models.DataTables
{
    public class DataTableParametersBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(DataTableParameters))
            {
                return new BinderTypeModelBinder(typeof(DataTableParametersBinder));
            }
            return null;
        }
    }
}
