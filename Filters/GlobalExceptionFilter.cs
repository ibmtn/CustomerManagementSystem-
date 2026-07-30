using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace KcetasWeb.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            _logger = logger;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
        }

        public void OnException(ExceptionContext context)
        {
            // Hatayı logla
            _logger.LogError(context.Exception, "Sistemde beklenmeyen bir hata yakalandı. Controller: {Controller}, Action: {Action}", 
                context.RouteData.Values["controller"], 
                context.RouteData.Values["action"]);

            var tempData = _tempDataDictionaryFactory.GetTempData(context.HttpContext);
            
            // Kullanıcıya gösterilecek hata mesajını temizle ve kısalt
            string errorMessage = context.Exception.Message;
            if (errorMessage.Contains("API Hatası:"))
            {
                // Uzun ve karmaşık API hatalarını daha sade göster
                var parts = errorMessage.Split("Detay:");
                errorMessage = parts.Length > 1 ? parts[1].Trim() : errorMessage;
                
                // Çok uzunsa kırp
                if (errorMessage.Length > 150)
                {
                    errorMessage = errorMessage.Substring(0, 147) + "...";
                }
            }
            
            tempData["Mesaj"] = $"İşlem sırasında bir hata oluştu: {errorMessage}";
            tempData["MesajTip"] = "danger";

            // Eğer istek AJAX isteği ise (örn: Abone sorgulama arama ekranı)
            if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                context.Result = new JsonResult(new { success = false, message = errorMessage })
                {
                    StatusCode = 500
                };
            }
            else
            {
                // Normal sayfa ise, geldiği sayfaya geri gönder
                var referer = context.HttpContext.Request.Headers["Referer"].ToString();
                
                if (string.IsNullOrEmpty(referer))
                {
                    // Referer yoksa mevcut controller'ın Index sayfasına at
                    var controllerName = context.RouteData.Values["controller"]?.ToString() ?? "Dashboard";
                    context.Result = new RedirectToActionResult("Index", controllerName, null);
                }
                else
                {
                    context.Result = new RedirectResult(referer);
                }
            }

            // Hatanın işlendiğini (handle edildiğini) belirt, ki 500 sayfasına düşmesin
            context.ExceptionHandled = true;
        }
    }
}
