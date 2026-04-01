Prerequisites .NET 10 SDK

1. Clone the repo
2. Run migrations : dotnet ef database update --project simple-file-system.API
3. Run with F5 (or on some other way dotnet run...)
4. Open API docs at http://localhost:5150/scalar

Production deployment would idealy be made with docker. Best and easiest way is to have a service in combonation with caddy configuration