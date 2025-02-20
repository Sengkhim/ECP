using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using API_GateWay.core.Configuration.Oauth2;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace API_GateWay.core.extensions;

public static class OAuthServiceExtension
{
    public static void AddGitHubOAuth2(this IServiceCollection service, IConfiguration configuration)
    {
        var gitHubConfig = configuration.GetSection("GitHub").Get<GitHubConfiguration>();
        service.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GitHub";
            })
            .AddCookie()
            .AddOAuth("GitHub", options =>
            {
                options.ClientId = gitHubConfig?.ClientId ?? "";
                options.ClientSecret = gitHubConfig?.ClientSecret ?? "";
                options.CallbackPath = new PathString("/signin-github");

                options.AuthorizationEndpoint = gitHubConfig?.AuthorizationEndpoint ?? "";
                options.TokenEndpoint = gitHubConfig?.TokenEndpoint ?? "";
                options.UserInformationEndpoint = gitHubConfig?.UserInformationEndpoint ?? "";
                
                options.Scope.Add("read:user");
                options.Scope.Add("user:email");
                options.SaveTokens = true;
                
                options.Events.OnCreatingTicket = async context =>
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
                        if (!email.GetProperty("primary").GetBoolean() || !email.GetProperty("verified").GetBoolean())
                            continue;
                        
                        var emailValue = email.GetProperty("email").GetString();
                       
                        if (string.IsNullOrEmpty(emailValue)) continue;
                        context.Identity?.AddClaim(new Claim(ClaimTypes.Email, emailValue));
                        
                        break;
                    }
                    
                    // Map additional GitHub user info (name, username, profile URL, avatar URL)
                    var id = user.GetProperty("id").GetInt64();
                    var name = user.GetProperty("name").GetString();
                    var login = user.GetProperty("login").GetString();
                    var profileUrl = user.GetProperty("html_url").GetString();
                    var avatarUrl = user.GetProperty("avatar_url").GetString();
                    var bio = user.GetProperty("bio").GetString(); 


                    // Add additional claims
                    context.Identity?.AddClaim(new Claim(ClaimTypes.NameIdentifier, id.ToString())); 
                    context.Identity?.AddClaim(new Claim(ClaimTypes.Name, name ?? ""));
                    context.Identity?.AddClaim(new Claim(ClaimTypes.GivenName, login ?? ""));
                    context.Identity?.AddClaim(new Claim(ClaimTypes.Webpage, profileUrl ?? ""));
                    context.Identity?.AddClaim(new Claim("avatar_url", avatarUrl ?? ""));
                    context.Identity?.AddClaim(new Claim("bio", bio ?? "")); 

                };
            });
    }
}