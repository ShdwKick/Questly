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

| Переменная | Обязательна | Назначение |
| --- | --- | --- |
| `CONNECTION_STRING` | да | Строка подключения к PostgreSQL. Без неё приложение не стартует. Хост — `postgres`, имя сервиса в compose |
| `SERVER_KEY` | да | Секрет подписи JWT, минимум 32 байта. На пустом значении `JwtSettings.Validate()` валит старт |
| `ISSUER`, `AUDIENCE` | да | Издатель и получатель JWT. Проверяются при валидации токена, поэтому должны совпадать с теми, при которых токен выпущен |
| `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB` | да | Нужны контейнеру Postgres: из них при первой инициализации создаются роль и база |
| `HASH_SALT` | нет | Не используется. Пароли солятся персональной солью из таблицы пользователей; `GetSalt()` вызывается только в тестовой заглушке |
| `RABBITMQ_HOSTNAME`, `RABBITMQ_USERNAME`, `RABBITMQ_PASSWORD` | нет | Не используются. `IRabbitService` и `RabbitHostedService` в DI не зарегистрированы, подключения к брокеру нет. Переменные нужны лишь чтобы compose не ругался |

### Где хранить секреты

`.env` на сервере — единственный источник правды, в git он не попадает. Держите
копию в менеджере паролей или в secrets репозитория; по ценности они разные:

- **`POSTGRES_PASSWORD` — терять нельзя.** Роль в базе создаётся с этим паролем
  один раз, при инициализации тома `pgdata`. Если `.env` потерян, а том жив,
  новый случайный пароль в `.env` базу не откроет: приложение будет получать
  отказ аутентификации. Менять пароль придётся внутри самой базы через `ALTER ROLE`.
- **`SERVER_KEY`, `ISSUER`, `AUDIENCE` — потеря переживаема.** При смене
  протухают выпущенные access-токены (TTL 15 минут) и пользователи получают 401.
  Refresh-сессии не пострадают: refresh-токен — случайная строка, он хранится
  хешом в БД и подписью не проверяется. То есть максимум — всем придётся
  перелогиниться.
- **`HASH_SALT` и `RABBITMQ_*` — значения не важны**, см. таблицу выше.

Если держите их в GitHub, кладите в **Secrets**, а не в Variables: значения
переменных видны в интерфейсе и попадают в логи, для пароля базы это не годится.

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

Сервисы перечисляются явно — иначе поднимется и `watchtower`, а он заменит
только что собранный образ на `:latest` из Docker Hub:

```bash
docker compose up -d --build postgres questly
```

Нужен файл `.env` рядом с `compose.yaml` — возьмите готовый шаблон:

```bash
cp .env.example .env
```

Все обязательные переменные перечислены в [.env.example](.env.example). API
поднимется на `http://localhost:5000`. Отдельно ставить Postgres не нужно: он
описан сервисом в `compose.yaml` и поднимается вместе с приложением.

Без Docker (нужен .NET SDK 10):

```bash
dotnet run --project Questly
```

## Первый деплой на сервер

Postgres устанавливать в систему не нужно — он поднимается как сервис из
`compose.yaml`, в своём контейнере, с именованным томом `pgdata`. Приложению
остаётся только накатить схему, что оно делает само (см. «База данных» выше).

Порядок для чистого сервера, где ещё ничего нет:

```bash
mkdir -p /root/QuestlyCompose && cd /root/QuestlyCompose
```

Положите рядом `compose.yaml` из этого репозитория и `.env`, собранный по
[.env.example](.env.example). Критично: в `CONNECTION_STRING` хост должен быть
`postgres` — имя сервиса в compose, а не `localhost`.

```bash
docker compose pull && docker compose up -d
```

Postgres стартует первым, приложение ждёт его healthcheck'а (`depends_on`),
затем создаёт базу и применяет миграции. Проверка:

```bash
docker compose ps -a && docker logs questly-app --tail 40
```

```bash
curl -s http://localhost:5000/api/server/health
```

Здоровый ответ — `{"status":"healthy","database":"ready"}`. Если вернулось 503 с
`migrations_pending` или `unreachable`, смотрите логи приложения: инициализатор
пишет туда, нашёл он базу или создал её.

Этот же `up` поднимает и `watchtower`: дальше он сам следит за реестром и
подменяет `questly-app`, когда CI публикует новый образ — см. CI/CD ниже.

## CI/CD

| Workflow | Триггер | Что делает |
| --- | --- | --- |
| `build-and-publish.yml` | push в `main`, вручную | Собирает Docker-образ и пушит `:latest` в Docker Hub. На сервер не ходит |
| `deploy-prod.yml` | вручную | Сборка, пуш образов и деплой на сервер по SSH |

Образ из `build-and-publish.yml` на сервер забирает **Watchtower** — он следит
за реестром и подменяет контейнер сам. Поэтому у этого workflow нет SSH-ключа:
нужен только токен Docker Hub, а он доступа к серверу не даёт.

Watchtower описан сервисом в том же `compose.yaml` и поднимается вместе со
всем стеком:

```bash
docker compose up -d
```

Учтите, что он монтирует `/var/run/docker.sock`, то есть имеет полный контроль
над хостом. И не поднимайте весь стек этой командой на машине разработчика:
там он подменит локально собранный образ (см. «Локальный запуск»).

Обновляются только контейнеры с лейблом
`com.centurylinklabs.watchtower.enable=true` — он стоит на `questly-app`.
Postgres под автообновление не попадает намеренно: смена его мажорной версии
требует `pg_upgrade`, а не подмены контейнера.

Так как этот workflow на сервер не заходит, файл `.env` там больше не
перегенерируется из CI — новые переменные окружения нужно добавлять на сервере
руками.
