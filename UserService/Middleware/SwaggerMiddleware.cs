namespace UserService.Middleware;

public static class SwaggerMiddleware
{
    public static void UseSwaggerOptions(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "USER SERVICE SCHEMA");
            options.RoutePrefix = string.Empty;
        });
    }
}