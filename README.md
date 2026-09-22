# delivery-app-academia-do-programador

## Estado atual

Projeto acadêmico de uma API de delivery em .NET 10. Esta etapa contém a
infraestrutura inicial, o módulo de Clientes e os fluxos públicos de cadastro e
login com ASP.NET Core Identity e JWT.

## Endpoints disponíveis

- `POST /api/clientes/cadastro`: cadastra um cliente e retorna um token JWT.
- `POST /api/clientes/login`: autentica um cliente e retorna um token JWT.

O cadastro valida nome e CPF, exige e-mail único e associa o cliente à identidade
criada. Credenciais inválidas retornam `401 Unauthorized`.

## Arquitetura

- `src/Api`: endpoints HTTP, autenticação, JWT, Swagger e logging.
- `src/Aplicacao`: serviços e tipos compartilhados da aplicação.
- `src/Dominio`: entidades, validações e contratos de repositório.
- `src/Infraestrutura`: Entity Framework Core, PostgreSQL, Identity e migrations.

## Tecnologias e execução

- .NET 10 e ASP.NET Core Web API;
- Entity Framework Core com PostgreSQL via Npgsql;
- ASP.NET Core Identity;
- JWT Bearer Authentication;
- Serilog e New Relic Logs;
- Swagger/OpenAPI.

Para restaurar e compilar:

```bash
dotnet restore DeliveryApp.slnx
dotnet build DeliveryApp.slnx
```

Em desenvolvimento, configure a chave JWT e a conexão PostgreSQL por configuração
local ou pelo Secret Manager antes de executar a API.
