[![Build and publish](https://github.com/ShdwKick/Questly/actions/workflows/build-and-publish.yml/badge.svg)](https://github.com/ShdwKick/Questly/actions/workflows/build-and-publish.yml)
[![Questly CI/CD prod](https://github.com/ShdwKick/Questly/actions/workflows/deploy-prod.yml/badge.svg)](https://github.com/ShdwKick/Questly/actions/workflows/deploy-prod.yml)

# Questly — REST API

Серверное приложение на C# / ASP.NET Core 10. REST API на контроллерах,
Entity Framework Core для работы с PostgreSQL, JWT-аутентификация.
Самой базы в репозитории нет — есть схема (миграции EF Core), которую
приложение накатывает само при старте.

> Раньше API был на GraphQL (HotChocolate). Переход на REST завершён,
> GraphQL-зависимостей и схемы в проекте больше нет.

## Структура

| Проект | Назначение |
| --- | --- |
| `Questly` | Web API: контроллеры, сервисы, репозитории, хостед-сервисы |
| `Questly.Domain` | Сущности, `DatabaseContext`, миграции, DTO, расширения-валидаторы |
| `QuestlyTests` | Юнит-тесты (xunit + Moq) |

## Эндпоинты

Все пути — под `/api`. Кроме `auth/*` и `server/*` всё требует
`Authorization: Bearer <access token>`.

### Авторизация — `/api/auth`

| Метод | Путь | Описание |
| --- | --- | --- |
| POST | `/api/auth/session-authenticate` | Логин, возвращает пару токенов |
| POST | `/api/auth/user_create` | Регистрация, возвращает пару токенов |
| POST | `/api/auth/token-refresh` | Новый access-токен по refresh-токену |
| POST | `/api/auth/session-logout` | Отзыв сессии (требует авторизации) |

### Пользователи — `/api/users`

| Метод | Путь | Описание |
| --- | --- | --- |
| GET | `/api/users/me` | Текущий пользователь по токену |
| GET | `/api/users/{userId}` | Пользователь по идентификатору |
| GET | `/api/users?page=1&pageSize=20` | Страница пользователей |

### Достижения — `/api/achievements`

| Метод | Путь | Описание |
| --- | --- | --- |
| GET | `/api/achievements/{achId}` | Достижение по идентификатору |
| GET | `/api/achievements/user/{userId}` | Все достижения пользователя |
| GET | `/api/achievements/user_completed?userId=...` | Завершённые достижения пользователя |
| GET | `/api/achievements/city/{cityId}?page=1&pageSize=10` | Страница достижений города |

### Города — `/api/cities`

| Метод | Путь | Описание |
| --- | --- | --- |
| GET | `/api/cities` | Список городов |
| GET | `/api/cities/{cityId}` | Город по идентификатору |

### Служебные — `/api/server`

| Метод | Путь | Описание |
| --- | --- | --- |
| GET | `/api/server/health` | Состояние сервиса и базы; 503, если база недоступна или есть неприменённые миграции |
| GET | `/api/server/datetime` | Локальное время сервера |
| GET | `/api/server/datetime/utc` | Время сервера в UTC |

Списочные эндпоинты с пагинацией отдают объект
`{ "items": [...], "totalItems": N, "page": N, "pageSize": N }`.
`page` начинается с 1, `pageSize` ограничен диапазоном 1..100.

Ошибки возвращаются как `ProblemDetails` (RFC 7807): `KeyNotFoundException` → 404,
`ArgumentException` → 400, `UnauthorizedAccessException` → 401, остальное → 500.

Swagger UI доступен при `ASPNETCORE_ENVIRONMENT=Development` на `/swagger`.

## Конфигурация

Читается из переменных окружения (им отдаётся приоритет) и `appsettings.json`.

| Переменная | Назначение |
| --- | --- |
| `CONNECTION_STRING` | Строка подключения к PostgreSQL. **Обязательна** — без неё приложение не стартует |
| `SERVER_KEY` | Секрет для подписи JWT |
| `ISSUER`, `AUDIENCE` | Издатель и получатель JWT |
| `HASH_SALT` | Соль для хешей |
| `RABBITMQ_HOSTNAME`, `RABBITMQ_USERNAME`, `RABBITMQ_PASSWORD` | Подключение к RabbitMQ |

## База данных

Схему поднимает `DatabaseInitializerService` — хостед-сервис, который работает
до того, как приложение начнёт принимать запросы:

1. проверяет, существует ли база, и пишет это в лог;
2. вызывает `Database.MigrateAsync()` — он **создаёт базу, если её нет**, и
   накатывает все миграции;
3. засевает стартовый город, если таблица городов пуста.

Если база недоступна (например, контейнер приложения стартовал раньше, чем
Postgres принял соединения), инициализация повторяется — до 12 попыток с
нарастающей задержкой от 2 до 15 секунд. Когда попытки исчерпаны, исключение
доходит до хоста и приложение **не стартует**: контейнер падает и
перезапускается политикой `restart: unless-stopped`. Это намеренно — приложение
без схемы отвечало бы 500 на каждый запрос.

Проверить результат после деплоя:

```bash
docker logs questly-app | grep -i "база данных"
curl -s http://localhost:5000/api/server/health
```

Новая миграция добавляется из корня репозитория:

```bash
dotnet ef migrations add <Name> --project Questly.Domain --startup-project Questly
```

Применять её руками не нужно — она накатится на следующем старте приложения.

## Локальный запуск

```bash
docker compose up -d --build
```

Нужен файл `.env` рядом с `compose.yaml` с переменными из таблицы выше плюс
`POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB`. API поднимется на
`http://localhost:5000`.

Без Docker (нужен .NET SDK 10):

```bash
dotnet run --project Questly
```

## CI/CD

| Workflow | Триггер | Что делает |
| --- | --- | --- |
| `build-and-publish.yml` | push в `main`, вручную | Собирает Docker-образ и пушит `:latest` в Docker Hub. На сервер не ходит |
| `deploy-prod.yml` | вручную | Сборка, пуш образов и деплой на сервер по SSH |

Образ из `build-and-publish.yml` на сервер забирает **Watchtower** — он следит
за реестром и подменяет контейнер сам. Поэтому у этого workflow нет SSH-ключа:
нужен только токен Docker Hub, а он доступа к серверу не даёт.

Watchtower ставится на сервер один раз:

```bash
docker compose -f deploy/watchtower-compose.yml up -d
```

Обновляются только контейнеры с лейблом
`com.centurylinklabs.watchtower.enable=true` — он стоит на `questly-app`.
Postgres под автообновление не попадает намеренно.

Так как этот workflow на сервер не заходит, файл `.env` там больше не
перегенерируется из CI — новые переменные окружения нужно добавлять на сервере
руками.
