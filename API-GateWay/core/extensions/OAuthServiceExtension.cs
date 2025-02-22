using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using API_GateWay.core.Configuration.Oauth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace API_GateWay.core.extensions;

public static class OAuthServiceExtension
{
    public static void AddGitHubOAuth2(this IServiceCollection service, IConfiguration configuration)
    {
        var gitHubConfig = configuration.GetSection("GitHub").Get<GitHubConfiguration>();
        service
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GitHub";
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "GitHub_Cookie"; 
                options.SlidingExpiration = true;
            })
            .AddOAuth("GitHub", options =>
            {
                options.ClientId = gitHubConfig?.ClientId ?? string.Empty;
                options.ClientSecret = gitHubConfig?.ClientSecret ?? string.Empty;
                options.CallbackPath = new PathString("/signin-github");

                options.AuthorizationEndpoint = gitHubConfig?.AuthorizationEndpoint ?? string.Empty;
                options.TokenEndpoint = gitHubConfig?.TokenEndpoint ?? string.Empty;
                options.UserInformationEndpoint = gitHubConfig?.UserInformationEndpoint ?? string.Empty;

                options.Scope.Add("read:user");
                options.Scope.Add("user:email");
                options.SaveTokens = true;

                options.Events.OnCreatingTicket = async context =>
                {
                    try
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await context.Backchannel.SendAsync(request);
                        response.EnsureSuccessStatusCode();

                        var user = JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
                        context.RunClaimActions(user);

                        // Fetch the user's email
                        var emailRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
                        emailRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                        emailRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var emailResponse = await context.Backchannel.SendAsync(emailRequest);
                        emailResponse.EnsureSuccessStatusCode();

                        var emails = JsonSerializer.Deserialize<JsonElement>(await emailResponse.Content.ReadAsStringAsync());
                        foreach (var email in emails.EnumerateArray())
                        {
                            if (email.GetProperty("primary").GetBoolean() && email.GetProperty("verified").GetBoolean())
                            {
                                var emailValue = email.GetProperty("email").GetString();
                                if (!string.IsNullOrEmpty(emailValue))
                                {
                                    context.Identity?.AddClaim(new Claim(ClaimTypes.Email, emailValue));
                                }
                                break;
                            }
                        }
                        
                        var id = user.GetProperty("id").GetInt64();
                        var name = user.GetProperty("name").GetString();
                        var login = user.GetProperty("login").GetString();
                        var profileUrl = user.GetProperty("html_url").GetString();
                        var avatarUrl = user.GetProperty("avatar_url").GetString();
                        var bio = user.GetProperty("bio").GetString();
                        
                        context.Identity?.AddClaim(new Claim(ClaimTypes.NameIdentifier, id.ToString()));
                        context.Identity?.AddClaim(new Claim(ClaimTypes.Name, name ?? string.Empty));
                        context.Identity?.AddClaim(new Claim(ClaimTypes.GivenName, login ?? string.Empty));
                        context.Identity?.AddClaim(new Claim(ClaimTypes.Webpage, profileUrl ?? string.Empty));
                        context.Identity?.AddClaim(new Claim("avatar_url", avatarUrl ?? string.Empty));
                        context.Identity?.AddClaim(new Claim("bio", bio ?? string.Empty));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error while fetching user information from GitHub.", ex);
                    }
                };
            });
    }

    public static void AddFacebookOAuth2(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = FacebookDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Cookie.Name = "FB_Cookie"; 
            options.SlidingExpiration = true;
        })
        .AddFacebook(options =>
        {
            options.AppId = configuration["Facebook:AppId"]!;
            options.AppSecret = configuration["Facebook:AppSecret"]!;
            options.CallbackPath = "/signin-facebook";

            options.SaveTokens = true;
            options.Scope.Add("public_profile");
            options.Fields.Add("email");
            options.Fields.Add("name");
            options.Fields.Add("picture");
            options.AccessDeniedPath = "/access-denied";

            options.Events = new OAuthEvents
            {
                OnCreatingTicket = async context =>
                {
                    try
                    {
                        var request = await context.Backchannel.GetAsync("https://graph.facebook.com/me?fields=id,name,email");
                        request.Headers.Add("User-Agent", "OAuth app");

                        var user = await request.Content.ReadAsStringAsync();
                        var userObject = JsonSerializer.Deserialize<JsonElement>(user);
                        context.Identity?.AddClaim(new Claim(ClaimTypes.Name, userObject.GetProperty("name").GetString() ?? string.Empty));
                        context.Identity?.AddClaim(new Claim(ClaimTypes.Email, userObject.GetProperty("email").GetString() ?? string.Empty));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error while fetching user information from Facebook.", ex);
                    }
                }
            };
        });
    }
}
