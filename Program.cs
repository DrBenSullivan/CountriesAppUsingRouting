internal class Program
{
    private static void Main(string[] args)
    {

        #region Hard-coded countries dictionary as per brief
        var countriesDict = new Dictionary<int, string>()
        {
            {1, "United States"},
            {2, "Canada"},
            {3, "United Kingdom" },
            {4, "India"},
            {5, "Japan"}
        };
        #endregion

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRouting(options =>
        {
            options.ConstraintMap.Add("countryId", typeof(CountryIdConstraint));
        });

        var app = builder.Build();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.Map("countries", async context =>
            {
                context.Response.StatusCode = 200;
                foreach (var country in countriesDict)
                {
                    await context.Response.WriteAsync($"{country.Key}, {country.Value}\n");
                }
            });

            endpoints.Map("countries/{id:countryId}", async context =>
            {
                int countryId = Convert.ToInt32(context.Request.RouteValues["id"]);

                if (countryId > 100 || countryId <= 0)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("The CountryID should be between 1 and 100");
                }
                if (countriesDict.ContainsKey(countryId))
                {
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync($"{countriesDict[countryId]}");
                }
                else
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("[No Country]");
                }
            });

            endpoints.Map("countries/**", async context =>
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid CountryID provided. CountryID must be an integer.");
            });
        });


        app.Run();
    }

    public class CountryIdConstraint : IRouteConstraint
    {
        public bool Match(HttpContext? httpContext, 
                          IRouter? route, 
                          string routeKey, 
                          RouteValueDictionary values, 
                          RouteDirection routeDirection)
        {
            // Try to convert route parameter to int, if successful, constraint is met, else false.
            if (values.TryGetValue(routeKey, out var value) && value != null)
            {
                if (int.TryParse(value.ToString(), out var countryId))
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}