using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace MoviesWebApi.Helpers
{
  public class TypeBinder<T> : IModelBinder
  {
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
      string propertyName = bindingContext.ModelName;
      ValueProviderResult valuesProvider = bindingContext.ValueProvider.GetValue(propertyName);

      if(valuesProvider == ValueProviderResult.None)
      {
        return Task.CompletedTask;
      }

      try
      {
        T deserializedValue = JsonConvert.DeserializeObject<T>(valuesProvider.FirstValue);
        bindingContext.Result = ModelBindingResult.Success(deserializedValue);
      }
      catch (Exception ex) 
      {
        bindingContext.ModelState.TryAddModelError(propertyName,"Invalid value for type List<T>.");
      }

      return Task.CompletedTask;
    }
  }
}
