using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DeliveryApp.Dominio.Compartilhado.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DeliveryApp.WebApi.Compartilhado.Auth;

public sealed record AccessTokenResponse(string AccessToken, DateTime DataExpiracaoEmUtc);

public sealed class JwtProvider(IOptions<JwtOptions> jwtOptions)
{
    private readonly JwtOptions options = jwtOptions.Value;

    public AccessTokenResponse CriarToken(
        Guid usuarioId,
        string email,
        TipoUsuario tipoUsuario
    )
    {
        DateTime dataCriacao = DateTime.UtcNow;
        DateTime dataExpiracao = dataCriacao.AddMinutes(options.AccessTokenMinutes);

        List<Claim> claims = [
            new(ClaimTypes.NameIdentifier, usuarioId.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, tipoUsuario.ToString()),
        ];

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(options.Key));
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: dataCriacao,
            expires: dataExpiracao,
            signingCredentials: credentials
        );

        string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessTokenResponse(accessToken, dataExpiracao);
    }
}
