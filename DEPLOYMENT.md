# Инструкция по развертыванию CRM системы

## Системные требования

### Минимальные требования
- **OS**: Windows 10+, macOS 10.15+, Ubuntu 18.04+
- **CPU**: 2 ядра
- **RAM**: 2 GB
- **HDD**: 1 GB свободного места

### Рекомендуемые требования
- **OS**: Windows 11, macOS 12+, Ubuntu 20.04+
- **CPU**: 4 ядра
- **RAM**: 4 GB
- **HDD**: 5 GB свободного места на SSD

## Установка зависимостей

### Windows

#### 1. Установка .NET 8 SDK
```bash
# Используя Chocolatey
choco install dotnet

# Или скачайте с microsoft.com
# https://dotnet.microsoft.com/en-us/download/dotnet/8.0
```

#### 2. Установка PostgreSQL
```bash
# Используя Chocolatey
choco install postgresql

# Или скачайте с postgresql.org
# Во время установки установите пароль для пользователя postgres
```

#### 3. Git
```bash
choco install git
```

### macOS

```bash
# Установка Homebrew (если не установлен)
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Установка .NET 8
brew install dotnet

# Установка PostgreSQL
brew install postgresql@15

# Запуск PostgreSQL
brew services start postgresql@15

# Git
brew install git
```

### Linux (Ubuntu/Debian)

```bash
# Обновление пакетов
sudo apt update && sudo apt upgrade

# Установка .NET 8
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x ./dotnet-install.sh
./dotnet-install.sh --version latest

# Установка PostgreSQL
sudo apt install postgresql postgresql-contrib

# Запуск PostgreSQL
sudo systemctl start postgresql

# Git
sudo apt install git
```

## Клонирование и подготовка проекта

```bash
# 1. Клонируйте репозиторий
git clone https://github.com/bank/CRM.git
cd CRM

# 2. Проверьте версию .NET
dotnet --version

# 3. Восстановите зависимости
dotnet restore

# 4. Создайте базу данных PostgreSQL
sudo -u postgres createdb CRMDb

# Проверяем доступ
psql -U postgres -d CRMDb -c "SELECT version();"
```

## Конфигурация приложения

### Редактирование appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CRMDb;Username=postgres;Password=ваш_пароль"
  },
  "Jwt": {
    "Key": "CRMSuperSecretKey1234567890123456",
    "Issuer": "CRMAPI",
    "Audience": "CRMClient",
    "ExpireMinutes": 120
  },
  "Init": {
    "AdminLogin": "admin",
    "AdminPassword": "admin123"
  },
  "AllowedHosts": "*"
}
```

### Изменение строки подключения

Измените параметры PostgreSQL в соответствии с вашей установкой:
- `Host`: адрес сервера БД (по умолчанию `localhost`)
- `Port`: порт PostgreSQL (по умолчанию `5432`)
- `Database`: имя БД (по умолчанию `CRMDb`)
- `Username`: пользователь БД (по умолчанию `postgres`)
- `Password`: пароль пользователя (установите свой!)

## Запуск приложения

### Разработка

```bash
# 1. Запустите приложение в режиме разработки
dotnet run

# 2. Приложение будет доступно по адресам:
# HTTPS: https://localhost:7098
# HTTP: http://localhost:5098

# 3. Swagger UI: https://localhost:7098/swagger
```

### Production

```bash
# 1. Создайте publish версию
dotnet publish -c Release

# 2. Перейдите в папку с published кодом
cd bin/Release/net10.0/publish

# 3. Запустите приложение
./CRM  # На Linux/Mac
CRM.exe  # На Windows
```

## Инициализация системы

После первого запуска приложение создаст базу данных и таблицы автоматически.

### Создание администратора

Сделайте POST запрос для инициализации:

```bash
curl -X POST https://localhost:7098/api/init/setup \
  -H "Content-Type: application/json" \
  -k
```

Ответ:
```json
{
  "message": "Система инициализирована успешно",
  "admin": {
    "id": 1,
    "login": "admin",
    "firstName": "Admin",
    "lastName": "System",
    "role": "Admin"
  }
}
```

### Проверка статуса

```bash
curl -X GET https://localhost:7098/api/init/status -k
```

## Использование SSL сертификата (локально)

### Создание самоподписанного сертификата

```bash
# Windows (PowerShell)
$cert = New-SelfSignedCertificate -DnsName "localhost" -CertStoreLocation "cert:\CurrentUser\My"

# Linux/Mac
openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365 -nodes
```

## Управление БД

### Просмотр таблиц

```bash
psql -U postgres -d CRMDb -c "\dt"
```

### Резервная копия БД

```bash
pg_dump -U postgres -d CRMDb -f backup_crm_$(date +%Y%m%d).sql
```

### Восстановление БД

```bash
psql -U postgres -d CRMDb < backup_crm_20260706.sql
```

## Решение проблем

### Ошибка: "Connection refused" при подключении к PostgreSQL

**Причина**: PostgreSQL не запущен

**Решение**:
```bash
# Windows
pg_ctl -D "C:\Program Files\PostgreSQL\15\data" start

# macOS
brew services start postgresql@15

# Linux
sudo systemctl start postgresql
```

### Ошибка: "Fatal: Ident authentication failed for user"

**Причина**: Неправильный пароль или способ аутентификации

**Решение**: Отредактируйте `/etc/postgresql/15/main/pg_hba.conf`:
```
local   all             all                                     md5
```

Перезагрузите PostgreSQL:
```bash
sudo systemctl restart postgresql
```

### Ошибка: "Port 7098 is already in use"

**Причина**: Порт занят другим процессом

**Решение**: 
Отредактируйте `Program.cs`:
```csharp
app.Urls.Add("https://localhost:7099");  // Измените порт
```

### Ошибка миграции БД

**Решение**:
```bash
# Удалите все таблицы и пересоздайте
dotnet ef database drop --force
dotnet ef database update
```

## Мониторинг приложения

### Логи приложения

Логи записываются в консоль. Для сохранения логов в файл, добавьте в `Program.cs`:

```csharp
builder.Logging.AddFile("logs/crm-{Date}.log");
```

### Проверка здоровья приложения

```bash
curl https://localhost:7098/health -k
```

## Резервная копия и восстановление

### Резервная копия всего проекта

```bash
tar -czf crm_backup_$(date +%Y%m%d_%H%M%S).tar.gz .
```

### Восстановление

```bash
tar -xzf crm_backup_20260706_120000.tar.gz
cd CRM
dotnet run
```

## Безопасность

### Рекомендации

1. **Измените пароль администратора** после первого входа
2. **Используйте HTTPS** в продакшене
3. **Защитите appsettings.json** с чувствительными данными
4. **Регулярно обновляйте** .NET и пакеты
5. **Используйте брандмауэр** для ограничения доступа
6. **Создавайте резервные копии** базы данных

### Переменные окружения

Вместо хранения чувствительных данных в appsettings.json, используйте переменные окружения:

```bash
# Linux/Mac
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=CRMDb;Username=postgres;Password=mypassword"
export Jwt__Key="MySecretKey1234567890123456"

# Windows (PowerShell)
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Port=5432;Database=CRMDb;Username=postgres;Password=mypassword"
$env:Jwt__Key = "MySecretKey1234567890123456"
```

## Docker (опционально)

### Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 5098 7098
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "CRM.dll"]
```

### Docker Compose

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: CRMDb
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  crm-api:
    build: .
    ports:
      - "7098:7098"
      - "5098:5098"
    environment:
      ConnectionStrings__DefaultConnection: "Host=postgres;Port=5432;Database=CRMDb;Username=postgres;Password=postgres"
      ASPNETCORE_URLS: "http://+:5098;https://+:7098"
    depends_on:
      - postgres

volumes:
  postgres_data:
```

Запуск:
```bash
docker-compose up --build
```

## Производственное развертывание

### На VPS/Dedicated Server

1. Установите .NET 8 SDK и PostgreSQL
2. Клонируйте репозиторий
3. Создайте systemd сервис (Linux):

```ini
[Unit]
Description=CRM API Service
After=network.target

[Service]
Type=notify
User=www-data
WorkingDirectory=/var/www/crm
ExecStart=/usr/bin/dotnet /var/www/crm/CRM.dll
Restart=always
RestartSec=10

[Install]
WantedBy=multi-user.target
```

4. Запустите сервис:
```bash
sudo systemctl enable crm-api
sudo systemctl start crm-api
```

5. Настройте Nginx как reverse proxy:

```nginx
server {
    listen 80;
    server_name api.crm.bank;

    location / {
        proxy_pass http://localhost:5098;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

---

**Версия документации**: 1.0  
**Дата**: 2026-07-06  
**Автор**: Служба поддержки IT
