# CRM API - Полная документация эндпоинтов

## Базовая информация

- **Базовый URL**: https://localhost:7098/api
- **Версия API**: v1
- **Аутентификация**: JWT Bearer Token
- **Формат ответов**: JSON

## Аутентификация и авторизация

### POST /operators/login
Вход в систему и получение JWT токена.

**Запрос:**
```json
{
  "login": "admin",
  "password": "admin123"
}
```

**Ответ (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "operator": {
    "id": 1,
    "login": "admin",
    "firstName": "Admin",
    "lastName": "System",
    "role": "Admin",
    "isActive": true,
    "lastLoginAt": "2026-07-06T10:30:00Z"
  }
}
```

**Ошибки:**
- 401 Unauthorized: Неверный логин или пароль
- 400 Bad Request: Ошибка валидации данных

---

### POST /operators/register
Регистрация нового оператора (доступно только администратору).

**Требуется роль:** Admin

**Запрос:**
```json
{
  "login": "operator1",
  "password": "securePassword123",
  "firstName": "Иван",
  "lastName": "Петров",
  "department": "Support",
  "role": "Operator"
}
```

**Ответ (201 Created):**
```json
{
  "id": 2,
  "login": "operator1",
  "firstName": "Иван",
  "lastName": "Петров",
  "role": "Operator",
  "department": "Support",
  "isActive": true,
  "lastLoginAt": null
}
```

---

## Обращения (Tickets)

### GET /tickets
Получить все обращения.

**Требуется роль:** Operator, SeniorOperator, Admin

**Параметры:** Нет

**Ответ (200 OK):**
```json
[
  {
    "id": 1,
    "title": "Проблема с доступом",
    "description": "Не могу войти в приложение",
    "createdAt": "2026-07-06T10:00:00Z",
    "closedAt": null,
    "clientId": 1,
    "clientName": "Иван Петров",
    "operatorId": 2,
    "operatorName": "Оператор 1",
    "statusId": 2,
    "statusName": "InProgress",
    "priorityId": 3,
    "priorityName": "High",
    "channelId": 1,
    "channelName": "CallCenter",
    "messages": []
  }
]
```

---

### GET /tickets/{id}
Получить обращение по ID.

**Параметры:**
- `id` (path): ID обращения

**Ответ (200 OK):** Полная информация об обращении с сообщениями

**Ошибки:**
- 404 Not Found: Обращение не найдено

---

### GET /tickets/client/{clientId}
Получить обращения клиента.

**Параметры:**
- `clientId` (path): ID клиента

**Ответ (200 OK):** Массив обращений клиента

---

### GET /tickets/status/{statusId}
Получить обращения по статусу.

**Параметры:**
- `statusId` (path): ID статуса

**Ответ (200 OK):** Массив обращений с данным статусом

---

### GET /tickets/operator/{operatorId}
Получить обращения оператора.

**Параметры:**
- `operatorId` (path): ID оператора

**Ответ (200 OK):** Массив обращений оператора

---

### POST /tickets
Создать новое обращение.

**Требуется роль:** Operator, SeniorOperator, Admin

**Запрос:**
```json
{
  "title": "Проблема с доступом к аккаунту",
  "description": "Не могу войти в мобильное приложение",
  "clientId": 1,
  "operatorId": null,
  "priorityId": 2,
  "channelId": 1
}
```

**Ответ (201 Created):** Созданное обращение

---

### PUT /tickets/{id}
Обновить обращение.

**Требуется роль:** Operator, SeniorOperator, Admin

**Запрос:**
```json
{
  "title": "Обновленное название",
  "description": "Обновленное описание",
  "statusId": 2,
  "priorityId": 3,
  "operatorId": 2
}
```

**Ответ (200 OK):** Обновленное обращение

---

### PATCH /tickets/{id}/status/{statusId}
Изменить статус обращения.

**Требуется роль:** Operator, SeniorOperator, Admin

**Параметры:**
- `id` (path): ID обращения
- `statusId` (path): Новый ID статуса

**Ответ (200 OK):** Обновленное обращение

**Автоматически:**
- Создается запись в TicketHistory
- Если статус = "Resolved" (4) или "Closed" (5), устанавливается ClosedAt

---

### DELETE /tickets/{id}
Удалить обращение.

**Требуется роль:** SeniorOperator, Admin

**Ответ (204 No Content)**

---

## Клиенты (Clients)

### GET /clients
Получить всех клиентов.

**Ответ (200 OK):**
```json
[
  {
    "id": 1,
    "firstName": "Иван",
    "lastName": "Петров",
    "middleName": "Иванович",
    "iin": "900101300001",
    "phoneNumber": "+77012345678",
    "email": "ivan@mail.com",
    "address": "г. Астана, ул. Республики, 10",
    "isActive": true
  }
]
```

---

### GET /clients/{id}
Получить клиента по ID.

---

### GET /clients/iin/{iin}
Получить клиента по ИИН.

---

### GET /clients/search?term=значение
Поиск клиентов по ФИО, телефону, email или ИИН.

---

### POST /clients
Создать нового клиента.

**Требуется роль:** Operator, SeniorOperator, Admin

**Запрос:**
```json
{
  "firstName": "Петр",
  "lastName": "Сидоров",
  "middleName": "Петрович",
  "iin": "900102300002",
  "phoneNumber": "+77012345679",
  "email": "petr@mail.com",
  "address": "г. Астана, ул. Нурсултана, 20"
}
```

**Ошибки:**
- 400 Bad Request: Клиент с таким ИИН уже существует

---

### PUT /clients/{id}
Обновить клиента.

---

### DELETE /clients/{id}
Удалить клиента.

**Требуется роль:** SeniorOperator, Admin

---

## Операторы (Operators)

### GET /operators
Получить всех операторов.

**Требуется роль:** SeniorOperator, Admin

---

### GET /operators/{id}
Получить оператора по ID.

**Требуется роль:** SeniorOperator, Admin

---

### PATCH /operators/{id}/role
Изменить роль оператора.

**Требуется роль:** Admin

**Запрос:**
```json
"SeniorOperator"
```

**Допустимые роли:** Operator, SeniorOperator, Admin

---

### PATCH /operators/{id}/deactivate
Деактивировать оператора.

**Требуется роль:** Admin

**Ответ (200 OK)**

---

### PATCH /operators/{id}/activate
Активировать оператора.

**Требуется роль:** Admin

**Ответ (200 OK)**

---

## Сообщения (Messages)

### GET /messages/ticket/{ticketId}
Получить все сообщения обращения.

---

### POST /messages
Добавить сообщение к обращению.

**Требуется роль:** Operator, SeniorOperator, Admin

**Запрос:**
```json
{
  "text": "Попробуйте восстановить пароль",
  "isFromClient": false,
  "ticketId": 1,
  "operatorId": 2
}
```

---

### DELETE /messages/{id}
Удалить сообщение.

**Требуется роль:** SeniorOperator, Admin

---

## Статусы (Statuses)

### GET /statuses
Получить все статусы.

---

### POST /statuses
Создать новый статус.

**Требуется роль:** Admin

**Запрос:**
```json
{
  "name": "InReview",
  "description": "На рассмотрении",
  "order": 6
}
```

---

## Приоритеты (Priorities)

### GET /priorities
Получить все приоритеты.

---

### POST /priorities
Создать новый приоритет.

**Требуется роль:** Admin

---

## Каналы (Channels)

### GET /channels
Получить все каналы поступления.

---

### POST /channels
Создать новый канал.

**Требуется роль:** SeniorOperator, Admin

---

## Файлы (Attachments)

### GET /attachments/ticket/{ticketId}
Получить файлы обращения.

---

### POST /attachments
Прикрепить файл к обращению.

**Требуется роль:** Operator, SeniorOperator, Admin

**Запрос:**
```json
{
  "fileName": "contract.pdf",
  "filePath": "/uploads/contract.pdf",
  "fileSize": 245680,
  "fileType": "application/pdf",
  "ticketId": 1
}
```

---

### DELETE /attachments/{id}
Удалить файл.

**Требуется роль:** SeniorOperator, Admin

---

## Инициализация (Init)

### POST /init/setup
Инициализировать систему (создать администратора).

**Требуется роль:** Не требуется (выполняется один раз)

**Ответ (200 OK):**
```json
{
  "message": "Система инициализирована успешно",
  "admin": {
    "id": 1,
    "login": "admin",
    "firstName": "Admin",
    "lastName": "System",
    "role": "Admin"
  },
  "credentials": {
    "login": "admin",
    "password": "***СКРЫТ***",
    "note": "Пожалуйста, смените пароль администратора после первого входа"
  }
}
```

---

### GET /init/status
Проверить статус инициализации.

**Ответ (200 OK):**
```json
{
  "initialized": true,
  "status": "Система готова к работе"
}
```

---

## Коды ошибок

| Код | Описание |
|-----|---------|
| 200 | OK - Успешный запрос |
| 201 | Created - Ресурс успешно создан |
| 204 | No Content - Успешно, нет контента для возврата |
| 400 | Bad Request - Ошибка в запросе или валидации |
| 401 | Unauthorized - Требуется аутентификация |
| 403 | Forbidden - Недостаточно прав доступа |
| 404 | Not Found - Ресурс не найден |
| 500 | Internal Server Error - Ошибка сервера |

---

## Примеры использования (cURL)

### Вход

```bash
curl -X POST https://localhost:7098/api/operators/login \
  -H "Content-Type: application/json" \
  -d '{"login":"admin","password":"admin123"}'
```

### Получить обращения

```bash
curl -X GET https://localhost:7098/api/tickets \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -k
```

### Создать обращение

```bash
curl -X POST https://localhost:7098/api/tickets \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title":"Проблема",
    "description":"Описание",
    "clientId":1,
    "priorityId":2,
    "channelId":1
  }' \
  -k
```

---

## Рекомендации по использованию

1. **Всегда проверяйте роль** перед выполнением защищенных операций
2. **Сохраняйте токен** после входа для последующих запросов
3. **Логируйте ошибки** для отладки и аудита
4. **Используйте HTTPS** в продакшене
5. **Регулярно обновляйте пароль** администратора
6. **Архивируйте данные** периодически

---

**Документация API**: Версия 1.0  
**Последнее обновление**: 2026-07-06
