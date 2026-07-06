# ⚡ Быстрый старт CRM системы

## За 5 минут до первого запуска

### Шаг 1: Подготовка окружения (1 минута)

```bash
# Проверьте установку .NET 8
dotnet --version

# Проверьте PostgreSQL
psql --version
```

### Шаг 2: Восстановление зависимостей (2 минуты)

```bash
cd /path/to/CRM
dotnet restore
```

### Шаг 3: Запуск приложения (2 минуты)

```bash
dotnet run
```

Приложение запустится на:
- 🌐 Swagger UI: https://localhost:7098/swagger
- 📡 API: https://localhost:7098/api

### Шаг 4: Инициализация системы

Откройте браузер и перейдите на:
```
https://localhost:7098/swagger
```

Найдите эндпоинт `POST /api/init/setup` и нажмите "Try it out" → "Execute"

**Ответ:**
```json
{
  "admin": {
    "login": "admin",
    "firstName": "Admin",
    "lastName": "System"
  }
}
```

## Первый вход

В Swagger UI найдите эндпоинт `POST /api/operators/login`:

```json
{
  "login": "admin",
  "password": "admin123"
}
```

Скопируйте полученный `token` и используйте его во всех последующих запросах через кнопку "Authorize" вверху Swagger UI.

## Основные операции

### 1️⃣ Создать клиента

```bash
curl -X POST https://localhost:7098/api/clients \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Иван",
    "lastName": "Петров",
    "iin": "900101300001",
    "phoneNumber": "+77012345678",
    "email": "ivan@mail.com"
  }' -k
```

### 2️⃣ Создать обращение

```bash
curl -X POST https://localhost:7098/api/tickets \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Проблема с доступом",
    "description": "Не могу войти в приложение",
    "clientId": 1,
    "priorityId": 2,
    "channelId": 1
  }' -k
```

### 3️⃣ Добавить сообщение

```bash
curl -X POST https://localhost:7098/api/messages \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "text": "Попробуйте восстановить пароль",
    "isFromClient": false,
    "ticketId": 1,
    "operatorId": 1
  }' -k
```

### 4️⃣ Изменить статус

```bash
curl -X PATCH https://localhost:7098/api/tickets/1/status/2 \
  -H "Authorization: Bearer YOUR_TOKEN" -k
```

### 5️⃣ Получить все обращения

```bash
curl -X GET https://localhost:7098/api/tickets \
  -H "Authorization: Bearer YOUR_TOKEN" -k
```

## Troubleshooting

### ❌ Ошибка: "Connection refused"

**Решение**: Убедитесь, что PostgreSQL запущен:
```bash
# macOS
brew services start postgresql@15

# Linux
sudo systemctl start postgresql

# Windows (PowerShell)
pg_ctl -D "C:\Program Files\PostgreSQL\15\data" start
```

### ❌ Ошибка: "FATAL: Ident authentication failed"

**Решение**: Измените пароль в appsettings.json на правильный пароль для PostgreSQL.

### ❌ Ошибка: "Port 7098 already in use"

**Решение**: Измените порт в Program.cs:
```csharp
app.Urls.Add("https://localhost:7099");  // Измените порт
```

### ❌ Ошибка при запуске: "No 'DefaultConnection' found"

**Решение**: Убедитесь, что appsettings.json содержит:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=CRMDb;Username=postgres;Password=вашпароль"
}
```

## Подсказки Swagger UI

### Как авторизоваться в Swagger

1. Выполните `POST /operators/login`
2. Скопируйте значение `token` из ответа
3. Нажмите кнопку **"Authorize"** вверху страницы
4. Введите: `Bearer YOUR_TOKEN`
5. Нажмите **"Authorize"**
6. Теперь все эндпоинты доступны

### Структура ответов

**Успешные запросы**:
```
GET /api/tickets
200 OK
[
  { "id": 1, "title": "...", ... }
]
```

**Ошибки**:
```
GET /api/tickets/999
404 Not Found
"Обращение с ID 999 не найдено"
```

## Типичные процессы

### Процесс обработки обращения

1. Клиент создает обращение (Статус: New)
2. Оператор получает обращение и меняет статус на InProgress
3. Оператор добавляет сообщения с решением
4. Статус меняется на Resolved
5. История всех изменений автоматически логируется

### Роли и права

- **Operator** (оператор): Может работать со своими обращениями
- **SeniorOperator** (старший оператор): Управляет всеми обращениями
- **Admin** (администратор): Полный доступ + управление пользователями

## Полезные ссылки

- 📚 [README.md](./README.md) - Полная документация
- 📖 [API_DOCUMENTATION.md](./API_DOCUMENTATION.md) - Все эндпоинты
- 🚀 [DEPLOYMENT.md](./DEPLOYMENT.md) - Развертывание в Production
- ✅ [REQUIREMENTS_CHECKLIST.md](./REQUIREMENTS_CHECKLIST.md) - Проверка требований

## Следующие шаги

1. 📖 Прочитайте полную документацию в [README.md](./README.md)
2. 🔍 Изучите все эндпоинты в [API_DOCUMENTATION.md](./API_DOCUMENTATION.md)
3. 🚀 Подготовьте к развертыванию по инструкции [DEPLOYMENT.md](./DEPLOYMENT.md)

---

**Вопросы?** Обратитесь к документации или контактируйте службу поддержки.

**Версия**: 1.0  
**Дата**: 2026-07-06
