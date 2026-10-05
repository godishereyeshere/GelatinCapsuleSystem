using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GelatinCapsule.Web.Infrastructure;

/// <summary>
/// ModelBinder سفارشی برای decimal که هم نقطه و هم ویرگول رو قبول می‌کنه
/// و پیام خطای فارسی می‌ده
/// </summary>
public class DecimalModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        if (string.IsNullOrWhiteSpace(value))
        {
            return Task.CompletedTask;
        }

        value = value.Trim();

        // تبدیل ویرگول فارسی (٫) و ویرگول معمولی (,) به نقطه
        value = value.Replace("٫", ".").Replace(",", ".");

        // اگه چند تا نقطه داشت، همه رو حذف کن به جز آخری
        var dotCount = value.Count(c => c == '.');
        if (dotCount > 1)
        {
            var lastDotIndex = value.LastIndexOf('.');
            var before = value.Substring(0, lastDotIndex).Replace(".", "");
            var after = value.Substring(lastDotIndex);
            value = before + after;
        }

        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
        {
            bindingContext.Result = ModelBindingResult.Success(result);
        }
        else
        {
            var label = GetFieldLabel(bindingContext);
            bindingContext.ModelState.TryAddModelError(
                bindingContext.ModelName,
                $"لطفاً برای «{label}» یک عدد معتبر وارد کنید");
        }

        return Task.CompletedTask;
    }

    private string GetFieldLabel(ModelBindingContext context)
    {
        var metadata = context.ModelMetadata;
        return !string.IsNullOrEmpty(metadata.DisplayName)
            ? metadata.DisplayName
            : metadata.Name;
    }
}

public class DecimalModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var modelType = context.Metadata.ModelType;

        if (modelType == typeof(decimal) || modelType == typeof(decimal?))
        {
            return new DecimalModelBinder();
        }

        return null;
    }
}