using System.Text;
using System.Net;
using System.Text.Json;
using server.Exceptions; // וודא שה-Namespace הזה נכון

namespace server.Middleware
{
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // 1. קודם כל נתעד את השגיאה בקובץ כפי שעשית
                await LogErrorToFile(ex, context);

                // 2. נטפל בתשובה שנשלחת חזרה ל-Angular
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // כאן אנחנו קובעים את קוד ה-HTTP לפי סוג ה-Exception
            context.Response.StatusCode = exception switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,      // 404
                ConflictException => (int)HttpStatusCode.Conflict,      // 409
                BusinessException => (int)HttpStatusCode.BadRequest,    // 400
                UnauthorizedException => (int)HttpStatusCode.Unauthorized, // 401
                _ => (int)HttpStatusCode.InternalServerError            // 500
            };

            // מחזירים אובייקט JSON עם ההודעה האמיתית מה-Exception
            var response = new
            {
                message = exception.Message
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private async Task LogErrorToFile(Exception ex, HttpContext context)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "logs.txt");
            var logMessage = new StringBuilder();
            logMessage.AppendLine("--------------------------------------------------");
            logMessage.AppendLine($"Date: {DateTime.Now}");
            logMessage.AppendLine($"Status Code: {context.Response.StatusCode}"); // הוספתי תיעוד של הסטטוס
            logMessage.AppendLine($"Request: {context.Request.Method} {context.Request.Path}");
            logMessage.AppendLine($"User: {context.User?.Identity?.Name ?? "Anonymous"}");
            logMessage.AppendLine($"Error Message: {ex.Message}");
            logMessage.AppendLine($"Stack Trace: {ex.StackTrace}");
            logMessage.AppendLine("--------------------------------------------------");

            await File.AppendAllTextAsync(filePath, logMessage.ToString());
        }
    }
}