## .NET EF Core ile MSSQL yapısını Postgres'e çevirme ve kayıtları aktarma

Youtube için hazırlanan videonun reposu

### <a href="" target="_blank">Video için Tıkla</a>

### Docker CLI
```dash
docker run -d --name eticaret-postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=eticaret -p 5432:5432 postgres:latest
```

### Docker Connection string
```dash
Host=localhost;Port=5432;Database=eticaret;Username=postgres;Password=postgres
```